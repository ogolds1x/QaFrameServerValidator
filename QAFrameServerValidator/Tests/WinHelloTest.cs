using System;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator.Tests
{
    public class WinHelloTest : TestBase
    {
        #region Constructor
        public WinHelloTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        #endregion

        #region Override Methods
        public override void ExecuteTest()
        {
            startTestTime = DateTime.Now;           
            CameraUtil cu = new CameraUtil();
            if (controlsList != null && controlsList.Count > 0)
            {
                Logger.AppendMessage(Environment.NewLine);
                Console.WriteLine("\n\n\n");
            }
            else
            {
                Logger.AppendError("CSV of FaceAuth test does not have controls list ");
            }

            endTestTime = DateTime.Now;
            Logger.AppendInfo("End Test Time " + endTestTime);
            Console.WriteLine("End Test Time " + endTestTime);
        }       
        #endregion

    }
}
