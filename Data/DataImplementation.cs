using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 300), random.Next(100, 300));
                Vector initialVelocity = new(random.Next(50, 150), random.Next(50, 150));
                double diameter = random.Next(10, 31);
                double weight = diameter / 2;

                Ball newBall = new(startingPosition, initialVelocity, diameter, weight, logger);

                upperLayerHandler(startingPosition, newBall);

                lock (ballsListLock)
                {
                    BallsList.Add(newBall);
                }
            }
        }

        public override void UpdateBall(Guid id, IVector newPosition, IVector newVelocity)
        {
            Ball? ball;
            lock (ballsListLock)
            {
                ball = BallsList.FirstOrDefault(b => b.Id == id);
            }
            ball?.UpdateFromLogic(newPosition, newVelocity);
        }

        public override void EnableDiagnostics(string filePath)
        {
            logger = new BallLogger(filePath);
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    MoveTimer.Dispose();
                    BallsList.Clear();
                    logger?.Dispose();
                }
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(nameof(DataImplementation));
            }
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private readonly Timer MoveTimer;
        private readonly List<Ball> BallsList = [];
        private readonly object ballsListLock = new();
        private BallLogger? logger;

        private async void Move(object? state)
        {
            const double dt = 0.01; // 10 ms
            List<Ball> copy;

            lock (ballsListLock)
            {
                copy = BallsList.ToList(); // kopia lokalna dla wątków
            }

            var tasks = copy.Select(ball =>
            {
                return Task.Run(() =>
                {
                    Vector delta = new(ball.Velocity.x * dt, ball.Velocity.y * dt);
                    ball.Move(delta);
                });
            });

            await Task.WhenAll(tasks);
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}
