using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class TestSetup
    {
        #region members
        //private const string PathToDefaults = @"..\..\..\QAFrameServerValidator\Controls.xml";
        //private const string PathToDefaults = @"C:\TFS\IVCAM\QA\QAFrameServerValidator\QAFrameServerValidator\Controls.xml";
        private const string PathToDefaults = "Controls.xml";
        private const string csvExtention = "csv";
        private const string ExcludeFileName = "Exclude";

        private int m_howManyTimesToRun = 0;
        private string m_folderPath;
        private Dictionary<Types.SubArea, CsvParent> m_csvs = new Dictionary<Types.SubArea, CsvParent>();
        private CsvExclude m_CsvExclude = null;
        #endregion

        #region constructors
        public TestSetup(string folderPath, int howManyTimesToRun)
        {
            this.m_folderPath = folderPath;
            this.m_howManyTimesToRun = howManyTimesToRun;

            CreateCsv();
        }
        #endregion

        #region public methods
        public string GetAreaName(string subAreaName)
        {
            Types.SubArea subArea = Types.ConvertStringToSubArea(subAreaName);
            if (this.m_csvs.ContainsKey(subArea))
                return this.m_csvs[subArea].Area;
            return "N/A";
        }

        public string GetDescription(string subAreaName)
        {
            Types.SubArea subArea = Types.ConvertStringToSubArea(subAreaName);
            if (this.m_csvs.ContainsKey(subArea))
                return this.m_csvs[subArea].Description;
            return "N/A";
        }
        public string GetBelongTo(string subAreaName)
        {
            Types.SubArea subArea = Types.ConvertStringToSubArea(subAreaName);
            if (this.m_csvs.ContainsKey(subArea))
                return this.m_csvs[subArea].BelongTo;
            return "N/A";
        }


        public static void Init()
        {
            Control.Init(PathToDefaults);
        }
        public int HowManyTimesToRun
        {
            get { return this.m_howManyTimesToRun; }
            set { this.m_howManyTimesToRun = value; }
        }

        public List<Profile> GetListOfProfiles(Types.SubArea subArea)
        {
            if (this.m_csvs.ContainsKey(subArea))
                return this.m_csvs[subArea].GetProfiles();
            return null;
        }

        public List<Types.SubArea> GetAllSubAreas()
        {
            return m_csvs.Keys.ToList<Types.SubArea>();
        }

        public List<ControlValues> GetListOfControls(Types.SubArea subArea)
        {
            List<ControlValues> ret = new List<ControlValues>();
            if (!this.m_csvs.ContainsKey(subArea))
                return ret;

            var csv = this.m_csvs[subArea];
            var bcsv = (csv as BasicCsv);
            if (bcsv == null)
                return ret;

            var controls = bcsv.GetControls();
            foreach (var pair in controls)
            {
                ControlValues controlValues = new ControlValues(pair.Key);
                if (pair.Value.Count == 0)
                    continue;

                controlValues.Name = pair.Value[0].Name;
                foreach (var val in pair.Value)
                {
                    controlValues.AddValue(val.Value);
                }
                ret.Add(controlValues);
            }

            return ret;
        }

        //public List<Profile> GetListOfProfiles()
        //{
        //    List<Profile> result = new List<Profile>();
        //    foreach (var csvItem in this.m_csvs)
        //        result.AddRange(csvItem.Value.GetProfiles());
        //    return result;
        //}

        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("CSVs:");
            foreach (var csvItem in this.m_csvs)
            {
                ss.AppendLine("\t" + csvItem.Key.ToString());
                foreach (var profile in csvItem.Value.GetProfiles())
                {
                    ss.AppendFormat(profile.ToString());
                }
            }

            return ss.ToString();
        }
        #endregion

        #region private methods

        private void CreateCsv()
        {
            string[] filePaths = Directory.GetFiles(this.m_folderPath, "*.csv");
            foreach (string file in filePaths)
            {
                string name = Path.GetFileName(file);
                CsvParent csv = null;
                if (name == ExcludeFileName + "." + csvExtention)
                {
                    this.m_CsvExclude = new CsvExclude(file);
                    continue;
                }

                /* Ok. Let's start this big ugly HARDCODED switch-case
                 * I personally do not like this code, but people asked for such a solution */
                switch (name)
                {
                    case "BasicColorTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.BasicColorTest, csv);
                        break;
                    case "BasicDepthTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())                       
                            this.m_csvs.Add(Types.SubArea.BasicDepthTest, csv);                        
                        break;
                    case "WinHelloTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.WinHelloTest, csv);
                        break;
                    case "BasicFishEyeTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.BasicFishEyeTest, csv);
                        break;
                    case "PropertiesTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.PropertiesTest, csv);
                            System.Console.WriteLine("Creating PropertiesTest profile");
                            //System.Console.WriteLine("size of PropertiesTest profile" + this.m_csvs.Count);
                        }
                        else
                        {
                            Logger.AppendInfo("PropertiesTest CSV not ok");
                        }
                        break;

                    case "StabilityTest.csv":
                        csv = new EventCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.StabilityTest, csv);
                        }
                        else
                        {
                            Logger.AppendInfo("StabilityTest CSV not ok");
                        }
                        break;

                    case "PropertiesWhileStreamingTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.PropertiesWhileStreamingTest, csv);
                            System.Console.WriteLine("Creating PropertiesWhileStreamingTest profile");
                            //System.Console.WriteLine("size of PropertiesWhileStreamingTest profile" + this.m_csvs.Count);
                        }
                        else
                        {
                            Logger.AppendInfo("PropertiesWhileStreamingTest CSV not ok");
                        }
                        break;

                    case "SanityParallelStreamTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.SanityParallelStreamTest, csv);
                            System.Console.WriteLine("Creating SanityParallelStreamTest profile");
                        }
                        else
                        {
                            Logger.AppendInfo("SanityParallelStreamTest CSV not ok");
                        }
                        break;
                    case "SensorGroupSanityTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.SensorGroupSanityTest, csv);
                            System.Console.WriteLine("Creating SensorGroupSanityTest profile");
                        }
                        else
                        {
                            Logger.AppendInfo("SensorGroupSanityTest CSV not ok");
                        }
                        break;
                    case "CheckDefaultControlValuesTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                        {
                            this.m_csvs.Add(Types.SubArea.CheckDefaultControlValuesTest, csv);
                            System.Console.WriteLine("Creating CheckDefaultControlValuesTest profile");
                            //System.Console.WriteLine("size of PropertiesTest profile" + this.m_csvs.Count);
                        }
                        else
                        {
                            Logger.AppendInfo("CheckDefaultControlValuesTest CSV not ok");
                        }
                        break;
                    case "IMUTemperatureMeasurementWithoutStreamingTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.IMUTemperatureMeasurementWithoutStreamingTest, csv);
                        break;
                    case "IMUTemperatureMeasurementWhileStreamingTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.IMUTemperatureMeasurementWhileStreamingTest, csv);
                        break;
                    case "ThermalAlarmWhileStreamingTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.ThermalAlarmWhileStreamingTest, csv);
                        break;
                    case "ThermalAlarmWithoutStreamingTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.ThermalAlarmWithoutStreamingTest, csv);
                        break;
                    case "ErrorSimulationTest.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.ErrorSimulationTest, csv);
                        break;
                    case "PNP_Test.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.PNP_Test, csv);
                        break;
                    case "PNP_Test_Multi.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.PNP_Test_Multi, csv);
                        break;
                    case "SyncHW_Test.csv":
                        csv = new BasicCsv(file);
                        if (csv.Ok())
                            this.m_csvs.Add(Types.SubArea.SyncHW_Test, csv);
                        break;
                    //case "StabilityTest.csv":
                    //    csv = new EventCsv(file);
                    //    if (csv.Ok())
                    //        this.m_csvs.Add(Types.SubArea.StabilityTest, csv);
                    //    break;
                    default:
                        Debug.WriteLine("Unknown test: " + name);
                        break;
                }
            }

            if (this.m_CsvExclude == null)
                return;

            foreach (var csvItem in this.m_csvs)
                csvItem.Value.ExcludeProfiles(this.m_CsvExclude[csvItem.Key]);
        }
        #endregion 
        #region ControlValues class
        public class ControlValues
        {
            #region members
            private Types.ControlKey m_key;
            private List<float> m_values = new List<float>();
            #endregion

            #region constructors
            public ControlValues(string name)
            {
                this.m_key = Types.ConvertStringToControlKey(name);
            }

            public ControlValues(Types.ControlKey controlKey)
            {
                this.m_key = controlKey;
            }
            #endregion

            #region public methods
            public string ControlName
            {
                get
                {
                    return Types.ConvertControlKeyToString(this.m_key);
                }
            }

            public Types.ControlKey Control
            {
                get
                {
                    return this.m_key;
                }
            }

            public float[] ArrayValues
            {
                get
                {
                    return this.m_values.ToArray<float>();
                }
            }

            public List<float> ListValues
            {
                get
                {
                    return this.m_values;
                }
            }

            public void AddValue(float val)
            {
                this.m_values.Add(val);
            }

            public string Name
            { get; set; }
            #endregion
        }
        #endregion
    }
}
