using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Logic;

namespace Logic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {
        [TestMethod]
        public void LogicConstructorTestMethod()
        {
            LogicAbstractAPI instance1 = LogicAbstractAPI.GetLogicLayer();
            LogicAbstractAPI instance2 = LogicAbstractAPI.GetLogicLayer();
            Assert.AreSame(instance1, instance2);
            instance1.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
        }

    }
}
