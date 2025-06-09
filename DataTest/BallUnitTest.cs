using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testingVector = new Vector(0.0, 0.0);
            Ball newInstance = new(testingVector, testingVector, 20, 10); // średnica i masa testowa
            Assert.IsNotNull(newInstance);
            Assert.AreEqual(testingVector.x, newInstance.CurrentPosition.x);
            Assert.AreEqual(testingVector.y, newInstance.CurrentPosition.y);
        }

        [TestMethod]
        public async Task MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), 20, 10);

            IVector curentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                curentPosition = position;
                numberOfCallBackCalled++;
            };

            newInstance.Move(new Vector(0.0, 0.0));

            await Task.Delay(100);

            Assert.AreEqual(1, numberOfCallBackCalled);
            Assert.AreEqual(initialPosition.x, curentPosition.x);
            Assert.AreEqual(initialPosition.y, curentPosition.y);
        }
    }
}
