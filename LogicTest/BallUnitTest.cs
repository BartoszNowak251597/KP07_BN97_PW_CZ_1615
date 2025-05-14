using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Logic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public async Task MoveTestMethod()
        {
            // Arrange
            DataBallFixture dataBallFixture = new DataBallFixture();
            Guid ballId = Guid.NewGuid();

            Ball newInstance = new Ball(
                ballId,
                new VectorFixture(0.0, 0.0), // Initial position
                new VectorFixture(1.0, 1.0), // Initial velocity
                dataBallFixture,
                100.0, // Table width
                200.0, // Table height
                10.0,  // Diameter
                10.0   // Mass
            );

            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                Assert.IsNotNull(position);
                numberOfCallBackCalled++;
            };

            // Act
            newInstance.OnNewPosition(new VectorFixture(0.0, 0.0));

            // Assert
            Assert.AreEqual(1, numberOfCallBackCalled);
        }

        #region Fixtures

        private class DataBallFixture : DataAbstractAPI
        {
            public override void Dispose()
            {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void UpdateBall(Guid id, IVector position, IVector velocity)
            {
                // No-op mock
            }
        }

        private class VectorFixture : IVector
        {
            public VectorFixture(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion
    }
}
