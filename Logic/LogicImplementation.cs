using System.Diagnostics;
using UnderneathLayerAPI = Data.DataAbstractAPI;

namespace Logic
{
    internal class LogicImplementation : LogicAbstractAPI
    {
        #region ctor

        public LogicImplementation() : this(null)
        { }

        public override void EnableDiagnostics(string filePath)
        {
            layerBellow.EnableDiagnostics(filePath);
        }

        internal LogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region LogicAbstractAPI

        public override void SetLogicParameters(double width, double height)
        {
            this.tableWidth = width;
            this.tableHeight = height;
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            var logicBalls = new List<Ball>();

            layerBellow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                var logicBall = new Ball(dataBall.Id, startingPosition, dataBall.Velocity, layerBellow, tableWidth, tableHeight, dataBall.Diameter, dataBall.Weight);
                lock (logicBallsLock)
                {
                    logicBalls.Add(logicBall);
                }
                dataBall.NewPositionNotification += async (s, pos) =>
                {
                    logicBall.OnNewPosition(pos);
                    await Task.Run(() => CheckCollisions(logicBall, logicBalls));
                };
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;
        private double tableWidth;
        private double tableHeight;
        private readonly object logicBallsLock = new();
        public object BallLock { get; } = new();
        private readonly Dictionary<string, DateTime> recentCollisions = new();
        private readonly TimeSpan collisionCooldown = TimeSpan.FromMilliseconds(100); // dowolny sensowny czas
        private readonly object collisionLogLock = new();

        private void CheckCollisions(Ball currentBall, List<Ball> allBalls)
        {
            foreach (var otherBall in allBalls)
            {
                if (currentBall == otherBall) continue;

                var (firstLock, secondLock) = currentBall.Id.CompareTo(otherBall.Id) < 0
                    ? (currentBall, otherBall)
                    : (otherBall, currentBall);

                lock (firstLock)
                {
                    lock (secondLock)
                    {
                        double dx = otherBall.Position.x - currentBall.Position.x;
                        double dy = otherBall.Position.y - currentBall.Position.y;
                        double distanceSquared = dx * dx + dy * dy;
                        double radiusSum = currentBall.Radius + otherBall.Radius;

                        if (distanceSquared <= radiusSum * radiusSum)
                        {  
                            ResolveElasticCollision(currentBall, otherBall);
                        }
                    }
                }
            }
        }

        private string GenerateCollisionKey(Guid id1, Guid id2)
        {
            return id1.CompareTo(id2) < 0 ? $"{id1}-{id2}" : $"{id2}-{id1}";
        }

        private void ResolveElasticCollision(Ball a, Ball b)
        {
            double dx = b.Position.x - a.Position.x;
            double dy = b.Position.y - a.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance == 0) return;

            double nx = dx / distance;
            double ny = dy / distance;

            // Odsunięcie kul, żeby nie były w sobie
            double overlap = (a.Radius + b.Radius) - distance;
            if (overlap > 0)
            {
                // Przesuwamy po połowie na każdą kulę
                double correctionX = nx * overlap / 2;
                double correctionY = ny * overlap / 2;

                // Korekta pozycji
                a.ForceMove(-correctionX, -correctionY);
                b.ForceMove(correctionX, correctionY);
            }

            // Prędkości względem normalnej
            double va = a.VelocityX * nx + a.VelocityY * ny;
            double vb = b.VelocityX * nx + b.VelocityY * ny;

            double massA = a.Mass;
            double massB = b.Mass;

            // Nowe prędkości względem normalnej
            double newVa = ((massA - massB) * va + 2 * massB * vb) / (massA + massB);
            double newVb = ((massB - massA) * vb + 2 * massA * va) / (massA + massB);

            double changeVa = newVa - va;
            double changeVb = newVb - vb;

            // Aktualizacja prędkości
            double newVelXA = a.VelocityX + changeVa * nx;
            double newVelYA = a.VelocityY + changeVa * ny;
            double newVelXB = b.VelocityX + changeVb * nx;
            double newVelYB = b.VelocityY + changeVb * ny;

            a.UpdateFromCollision(newVelXA, newVelYA);
            b.UpdateFromCollision(newVelXB, newVelYB);
            string key = GenerateCollisionKey(a.Id, b.Id);
            DateTime now = DateTime.UtcNow;

            lock (collisionLogLock)
            {
                if (recentCollisions.TryGetValue(key, out DateTime lastLogged) && (now - lastLogged) < collisionCooldown)
                {
                    return;
                }

                recentCollisions[key] = now;

                layerBellow.LogBallCollision(
                    a.Id, a.Position.x, a.Position.y,
                    b.Id, b.Position.x, b.Position.y
                );
            }
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}
