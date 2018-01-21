using QAFrameServerValidator.Abstract_Factory;
using QAFrameServerValidator.Tests;
using System;

namespace QAFrameServerValidator
{
    public class TestManager
    {
        public TestSetup setup;
        public TestBase testBase;
        
        private static string pathToCSV = "CsvInputs";
        public TestManager()
        {
            Console.WriteLine("TestManager");
        }

        public void CreateTest(Types.SubArea testName)
        {
            TestSetup.Init();
            setup = new TestSetup(pathToCSV, 1);
            if (setup!=null && setup.GetAllSubAreas()!=null && setup.GetAllSubAreas().Contains(testName))
            {
                switch (testName)
                {
                    case Types.SubArea.BasicDepthTest:
                        testBase = new BasicDepthTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicDepthTest);
                        break;
                    case Types.SubArea.BasicColorTest:
                        testBase = new BasicColorTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicColorTest);
                        break;
                    case Types.SubArea.WinHelloTest:
                        testBase = new WinHelloTest(Factory.Instance);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.WinHelloTest);
                        break;
                    case Types.SubArea.BasicFishEyeTest:
                        testBase = new BasicFishEyeTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicFishEyeTest);
                        break;
                    case Types.SubArea.PropertiesTest:
                        testBase = new PropertiesTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PropertiesTest);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.PropertiesTest);
                        break;
                    case Types.SubArea.SanityParallelStreamTest:
                        testBase = new SanityParallelStreamTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.SanityParallelStreamTest);
                        break;
                    case Types.SubArea.StabilityTest:
                        //testBase = new StabilityTest(Factory.Instance);
                        //testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.StabilityTest);
                        break;
                    case Types.SubArea.PropertiesWhileStreamingTest:
                        testBase = new PropertiesWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PropertiesWhileStreamingTest);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.PropertiesWhileStreamingTest);
                        break;
                    case Types.SubArea.CheckDefaultControlValuesTest:
                        testBase = new CheckDefaultControlValuesTest(Factory.Instance);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.CheckDefaultControlValuesTest);
                        break;
                    case Types.SubArea.SensorGroupSanityTest:
                        testBase = new SensorGroupSanityTest(Factory.Instance);
                        //testBase.controlsList = setup.GetListOfControls(Types.SubArea.SensorGroupSanityTest);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.SensorGroupSanityTest);
                        break;
                    case Types.SubArea.IMUTemperatureMeasurementWithoutStreamingTest:
                        testBase = new IMUTemperatureMeasurementWithoutStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.IMUTemperatureMeasurementWithoutStreamingTest);
                        break;
                    case Types.SubArea.IMUTemperatureMeasurementWhileStreamingTest:
                        testBase = new IMUTemperatureMeasurementWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.IMUTemperatureMeasurementWhileStreamingTest);
                        break;
                    case Types.SubArea.ThermalAlarmWhileStreamingTest:
                        testBase = new ThermalAlarmWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ThermalAlarmWhileStreamingTest);
                        break;
                    case Types.SubArea.ThermalAlarmWithoutStreamingTest:
                        testBase = new ThermalAlarmWithoutStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ThermalAlarmWithoutStreamingTest);
                        break;
                    case Types.SubArea.ErrorSimulationTest:
                        testBase = new ErrorSimulationTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ErrorSimulationTest);
                        break;
                    case Types.SubArea.PNP_Test:
                        testBase = new PNP_Test(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PNP_Test);
                        break;                  
                    default:
                        break;
                }
            }
        }

        public void CreateTest()
        {
            TestSetup.Init();
            setup = new TestSetup(pathToCSV, 1);
            Console.WriteLine("setup.GetAllSubAreas(): " + setup.GetAllSubAreas().Count);
            foreach (Types.SubArea testName in setup.GetAllSubAreas())
            {
                switch (testName)
                {
                    case Types.SubArea.BasicDepthTest:
                        testBase = new BasicDepthTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicDepthTest);
                        break;
                    case Types.SubArea.BasicColorTest:
                        testBase = new BasicColorTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicColorTest);
                        break;
                    case Types.SubArea.WinHelloTest:
                        testBase = new WinHelloTest(Factory.Instance);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.WinHelloTest);
                        break;
                    case Types.SubArea.BasicFishEyeTest:
                        testBase = new BasicFishEyeTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.BasicFishEyeTest);
                        break;
                    case Types.SubArea.PropertiesTest:
                        testBase = new PropertiesTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PropertiesTest);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.PropertiesTest);
                        break;
                    case Types.SubArea.SanityParallelStreamTest:
                        testBase = new SanityParallelStreamTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.SanityParallelStreamTest);
                        break;
                    case Types.SubArea.StabilityTest:
                        //testBase = new StabilityTest(Factory.Instance);
                        //testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.StabilityTest);
                        break;
                    case Types.SubArea.PropertiesWhileStreamingTest:
                        testBase = new PropertiesWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PropertiesWhileStreamingTest);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.PropertiesWhileStreamingTest);

                        break;
                    case Types.SubArea.CheckDefaultControlValuesTest:
                        testBase = new CheckDefaultControlValuesTest(Factory.Instance);
                        testBase.controlsList = setup.GetListOfControls(Types.SubArea.CheckDefaultControlValuesTest);                        
                        break;
                    case Types.SubArea.SensorGroupSanityTest:
                        testBase = new SensorGroupSanityTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.SensorGroupSanityTest);
                        //testBase.controlsList = setup.GetListOfControls(Types.SubArea.SensorGroupSanityTest);
                        break;
                    case Types.SubArea.IMUTemperatureMeasurementWithoutStreamingTest:
                        testBase = new IMUTemperatureMeasurementWithoutStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.IMUTemperatureMeasurementWithoutStreamingTest);
                        break;
                    case Types.SubArea.IMUTemperatureMeasurementWhileStreamingTest:
                        testBase = new IMUTemperatureMeasurementWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.IMUTemperatureMeasurementWhileStreamingTest);
                        break;
                    case Types.SubArea.ThermalAlarmWhileStreamingTest:
                        testBase = new ThermalAlarmWhileStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ThermalAlarmWhileStreamingTest);
                        break;
                    case Types.SubArea.ThermalAlarmWithoutStreamingTest:
                        testBase = new ThermalAlarmWithoutStreamingTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ThermalAlarmWithoutStreamingTest);
                        break;
                    case Types.SubArea.ErrorSimulationTest:
                        testBase = new ErrorSimulationTest(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.ErrorSimulationTest);
                        break;
                    case Types.SubArea.PNP_Test:
                        testBase = new PNP_Test(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PNP_Test);
                        break;
                    case Types.SubArea.PNP_Test_Multi:
                        testBase = new PNP_Test_Multi(Factory.Instance);
                        testBase.profilesList = setup.GetListOfProfiles(Types.SubArea.PNP_Test_Multi);
                        break;
                    default:
                        break;
                }
            }


        }
    }
}
