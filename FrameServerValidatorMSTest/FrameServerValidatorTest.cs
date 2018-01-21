using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QAFrameServerValidator;
using System.Diagnostics;
using System.IO;

namespace FrameServerValidatorMSTest
{
    [TestClass]
    public class FrameServerValidatorTest
    {
        TestManager testManager = new TestManager();
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Logger.GetStreamWriter();
            Logger.AppendDebug("Call Garbage Collection");
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        [TestMethod]
        [TestCategory("Depth")]
        [DescriptionAttribute("Check_FPS_delta_and_streaming.")]
        [OwnerAttribute("WOS")]
        public void BasicDepthTest()
        {

            Logger.AppendInfo("Starting BasicDepthTest");
            Console.WriteLine("Starting BasicDepthTest");
            testManager.CreateTest(Types.SubArea.BasicDepthTest);
            testManager.testBase?.ExecuteTest();
            if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
            {
                Assert.Fail();
            }
        }

        //[TestMethod]
        //[TestCategory("Color")]
        //[DescriptionAttribute("Check_FPS_delta_and_streaming.")]
        //[OwnerAttribute("WOS")]
        //public void BasicColorTest()
        //{
        //    Logger.AppendInfo("Starting BasicColorTest");
        //    Console.WriteLine("Starting BasicColorTest");
        //    testManager.CreateTest(Types.SubArea.BasicColorTest);
        //    testManager.testBase?.ExecuteTest();
        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //[TestCategory("Properties")]
        //[DescriptionAttribute("Set all properties values")]
        //[OwnerAttribute("WOS")]
        //public void PropertiesTest()
        //{
        //    Logger.AppendInfo("Starting PropertiesTest");
        //    Console.WriteLine("Starting PropertiesTest");
        //    testManager.CreateTest(Types.SubArea.PropertiesTest);
        //    testManager.testBase?.ExecuteTest();
        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}


        //[TestMethod]
        //[TestCategory("SensorGroup")]
        //[DescriptionAttribute("Run SensorGroup Test")]
        //[OwnerAttribute("WOS")]
        //public void SensorGroupSanityTest()
        //{
        //    Logger.AppendInfo("Starting SensorGroupSanityTest");
        //    Console.WriteLine("Starting SensorGroupSanityTest");
        //    testManager.CreateTest(Types.SubArea.SensorGroupSanityTest);
        //    testManager.testBase?.ExecuteTest();
        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //[TestCategory("IR")]
        //[DescriptionAttribute("Run FaceAuth_FaceLogin Test")]
        //[OwnerAttribute("WOS")]
        //public void WinHelloTest()
        //{
        //    Logger.AppendInfo("Starting WinHelloTest");
        //    Console.WriteLine("Starting WinHelloTest");
        //    testManager.CreateTest(Types.SubArea.WinHelloTest);
        //    testManager.testBase?.ExecuteTest();
        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //[TestCategory("Sync")]
        //[DescriptionAttribute("Start Stream with Color and Depth independently.")]
        //[OwnerAttribute("WOS")]
        //public void SanityParallelStreamTest()
        //{
        //    Logger.AppendInfo("Starting SanityParallelStreamTest");
        //    Console.WriteLine("Starting SanityParallelStreamTest");
        //    testManager.CreateTest(Types.SubArea.SanityParallelStreamTest);
        //    testManager.testBase.ExecuteTest();

        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //[TestCategory("FishEye")]
        //[DescriptionAttribute("Run FishEye Fps Test")]
        //[OwnerAttribute("WOS")]
        //public void BasicFishEyeTest()
        //{
        //    Logger.AppendInfo("Starting FishEye ");
        //    Console.WriteLine("Starting FishEye ");
        //    testManager.CreateTest(Types.SubArea.BasicFishEyeTest);
        //    testManager.testBase?.ExecuteTest();
        //    if (testManager.testBase != null && !testManager.testBase.IsTestPassed())
        //    {
        //        Assert.Fail();
        //    }
        //}


        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Logger.CloseFile();
        }



        //}
        //internal static void ProcessExited(object sender, System.EventArgs e)
        //{
        //    Console.WriteLine($"Trigger {pr.ExitCode}");
        //}

    }
}
