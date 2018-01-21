using System;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator.Tests
{
    public class ROITest : TestBase
    {
        public ROITest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {

            startTestTime = DateTime.Now;
            Logger.AppendInfo("Start Test Time " + startTestTime);

            Logger.AppendMessage($"LOG_PATH: {Logger.path}");


            ///////TO DO

            endTestTime = DateTime.Now;
            Logger.AppendInfo("End Test Time " + endTestTime);
            Console.WriteLine("End Test Time " + endTestTime);
        }

    }
}
