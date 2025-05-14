using System;
using System.Diagnostics;

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
                var initialVel = new Vector((random.NextDouble() - 0.5) * 2, (random.NextDouble() - 0.5) * 2);
                //double diameter = random.Next(10, 31);
                //double weight = random.NextDouble() * (20 - 5) + 5;
                double diameter = 20;
                double weight = 10;
                Ball newBall = new(startingPosition, initialVel, diameter, weight);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
        }

        public override void UpdateBall(Guid id, IVector newPosition, IVector newVelocity)
{
    var ball = BallsList.FirstOrDefault(b => b.Id == id);
    ball?.UpdateFromLogic(newPosition, newVelocity);
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
        private void Move(object? x)
        {
            const double dt = 0.01; // 10ms
            foreach (Ball item in BallsList)
            {
                // Przesuwamy wed³ug wektora prêdkoœci
                var delta = new Vector(item.Velocity.x * dt, item.Velocity.y * dt);
                item.Move(delta);
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