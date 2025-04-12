using System;
using System.Diagnostics;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
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
                Vector velocity = new(random.Next(-3, 4), random.Next(-3, 4));
                Ball newBall = new(startingPosition, velocity);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
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
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;

        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];
        private const double CanvasWidth = 400;
        private const double CanvasHeight = 420;
        private void Move(object? x)
        {
            foreach (Ball ball in BallsList)
            {
                double radius = ball.Radius;
                Vector velocity = (Vector)ball.Velocity;
                double currentX = ball.CurrentPosition.x;
                double currentY = ball.CurrentPosition.y;

                double newX = currentX + velocity.x;
                double newY = currentY + velocity.y;

                // Odbicie od krawêdzi poziomych
                if (newX  <= 0 || 5 + newX + radius * 2 >= CanvasWidth)
                {
                    velocity = new Vector(-velocity.x, velocity.y);
                }

                // Odbicie od krawêdzi pionowych
                if (newY <= 0 || (newY + radius * 2) + 5 >= CanvasHeight)
                {
                    velocity = new Vector(velocity.x, -velocity.y);
                }

                // Zaktualizuj prêdkoœæ po ewentualnym odbiciu
                ball.Velocity = velocity;

                // Porusz kulkê o nowy wektor prêdkoœci
                ball.Move(velocity);
            }
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