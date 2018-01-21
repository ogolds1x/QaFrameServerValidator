using System.Collections.Generic;
using System;
using System.Configuration;

namespace QAFrameServerValidator.Abstract_Factory
{
    public abstract class TestBase : ITest
    {
        protected int numberOfIteration = int.Parse(ConfigurationSettings.AppSettings["iterationsNumber"].ToString());
        protected uint numberOfFramesToCollect = uint.Parse(ConfigurationSettings.AppSettings["numberOfFramesToCollectInSingleIteration"].ToString());
        public Factory factory;
        public abstract void ExecuteTest();

        public List<Profile> profilesList;
        public List<TestSetup.ControlValues> controlsList;
        public DateTime startTestTime;
        public DateTime endTestTime;
    }
}
