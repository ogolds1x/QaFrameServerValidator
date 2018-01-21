using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QAFrameServerValidator.Abstract_Factory;
using static QAFrameServerValidator.Tests.CheckDefaultControlValuesTest;
using System.Runtime.InteropServices;

//using ResetDS5;
namespace QAFrameServerValidator.Tests
{
    class IMUTemperatureMeasurementWhileStreamingTest : TestBase
    {
        //bool isStreamingPassed;

        //FrameReaderUtil fr;





        public IMUTemperatureMeasurementWhileStreamingTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public IMUTemperatureMeasurementWhileStreamingTest()
        {

        }

        public override void ExecuteTest()
        {
            /*
             2. IMU Temperature Read while streaming
                • Start HID streaming (for 1 minute). -> not now, might be relevant in the future
                • Start depth streaming (for 1 minute).
                • Get temperature of IMU each 5 seconds[via Sending IMU read command / via public API in windows (temp_mm in the Ipdev terminal)
                • Compare each 2 temperatures.
                • Stop streaming (after 1 minute of streaming)
                • Get temperature of IMU again. (after stop)
                • Compare the last IMU temperature read-out while streaming with the read-out after the stop.
                     expected results:
                •	Get command/ API after starting streaming should success.
                •	Check that first temp in acceptable range 0-70.
                •	Difference between each 2 near temperatures should be <= 1
                •	Temperature after stop should return value 
             * 
            */
            throw new NotImplementedException();
        }

    }
}
