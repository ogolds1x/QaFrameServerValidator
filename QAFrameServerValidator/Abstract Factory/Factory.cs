using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator
{
    public class Factory
    {
        private static readonly Factory instance = new Factory();
        private static List<TestBase> lst_TestsToExecute = new List<TestBase>();
        private Factory() { }

        public static Factory Instance
        {
            get
            {
                return instance;
            }
        }

        public void AddTest(TestBase test)
        {
            lst_TestsToExecute.Add(test);
        }

        public static List<TestBase> Lst_TestsToExecute
        {
            get
            {
                return lst_TestsToExecute;
            }
        }

        public  void ExecuteAllTests()
        {
            foreach (TestBase test in lst_TestsToExecute)
            {
                test.ExecuteTest();
                //Logger.AppendMessage($"SUB_AREA_START:{test.startTestTime}");
                //Logger.AppendMessage($"SUB_AREA_FINISH:{test.endTestTime}");
                //Logger.AppendMessage($"SUB_AREA_RESULT:{test.testStatus}");
                //Logger.AppendMessage($"AREA_RESULT:{test.testStatus}"); ///**for now we have one sub area only,therfore the result of area will be equal to sub area 
                //Logger.AppendMessage(Environment.NewLine);
                //Console.WriteLine($"SUB_AREA_START:{test.startTestTime}");
                //Console.WriteLine($"SUB_AREA_FINISH:{test.endTestTime}");
                //Console.WriteLine($"SUB_AREA_RESULT:{test.testStatus}");
                //Console.WriteLine("\n\n\n");
            }
            Logger.AppendInfo("End of Execute All Tests");
        }

        public void ExecuteTest(TestBase test)
        {
            test.ExecuteTest();
        }
    }
}
