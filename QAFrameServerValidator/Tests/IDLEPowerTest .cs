using System;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator.Tests
{
    public class IDLEPowerTest : TestBase
    {
        public IDLEPowerTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {

        }

      
    }
}
