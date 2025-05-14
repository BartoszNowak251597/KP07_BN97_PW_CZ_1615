using Data;

namespace Logic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (LogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixcure dataLayerFixcure = new();
            LogicImplementation newInstance = new(dataLayerFixcure);
            Assert.IsFalse(dataLayerFixcure.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixcure.Disposed);
        }

        #region testing instrumentation

        private class DataLayerConstructorFixcure : DataAbstractAPI
        {
            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void UpdateBall(Guid id, IVector newPosition, IVector newVelocity)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixcure : DataAbstractAPI
        {
            internal bool Disposed = false;

            public override void Dispose()
            {
                Disposed = true;
            }


            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void UpdateBall(Guid id, IVector newPosition, IVector newVelocity)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixcure : DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;
            internal IBall? CreatedBall { get; private set; }

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                var ball = new DataBallFixture();
                CreatedBall = ball;
                upperLayerHandler(new DataVectorFixture(), (Data.IBall)ball);
            }

            public override void UpdateBall(Guid id, IVector newPosition, IVector newVelocity)
            {
                // No-op
            }

            private class DataVectorFixture : IVector, IPosition
            {
                public double x { get; init; } = 0.0;
                public double y { get; init; } = 0.0;
            }

            internal class DataBallFixture : IBall
            {
                public IVector Velocity { get; set; } = new DataVectorFixture();
                public Guid Id { get; } = Guid.NewGuid();
                public double Diameter => 10.0;
                public double Weight => 1.0;
                public double Radius => 5.0;
                public double Mass => 1.0;

                public event EventHandler<IPosition>? NewPositionNotification;

                public void RaiseNewPosition(IPosition pos)
                {
                    NewPositionNotification?.Invoke(this, pos);
                }

                public void UpdateFromLogic(IVector newPosition, IVector newVelocity)
                {
                    // No-op
                }
            }
        }

        private class DataVectorFixture : IPosition
        {
            public int x { get; set; }
            public int y { get; set; }
            double IPosition.x { get => x; init => throw new NotImplementedException(); }
            double IPosition.y { get => y; init => throw new NotImplementedException(); }
        }

        #endregion
    }
}
