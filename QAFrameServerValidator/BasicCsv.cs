using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class BasicCsv : CsvParent
    {
        #region constants
        private const string CONTROLS_AREA_KEY = "Properties";
        private const string PROFILES_AREA_KEY = "Profiles";
        #endregion

        #region members
        private List<Profile> m_profiles = new List<Profile>();
        private Dictionary<Types.ControlKey, List<Control>> m_controls = new Dictionary<Types.ControlKey, List<Control>>();
        #endregion

        #region constructors
        public BasicCsv(string fullpath)
            : base(fullpath)
        {
            parseCsv(fullpath);
        }
        #endregion

        #region public methods
        public List<Control> GetControlsByName(Types.ControlKey name)
        {
            if (this.m_controls.ContainsKey(name))
                return this.m_controls[name];
            return null;
        }

        public List<Control> GetControlsByName(string name)
        {
            return GetControlsByName(Types.ConvertStringToControlKey(name));
        }

        public override List<Profile> GetProfiles()
        {
            return this.m_profiles;
        }

        public Dictionary<Types.ControlKey, List<Control>> GetControls()
        {
            return this.m_controls;
        }

        public override void ExcludeProfiles(List<string> excludes)
        {
            foreach (string exclude in excludes)
            {
                foreach (Profile profile in this.m_profiles)
                {
                    if (profile.Equal(exclude))
                        profile.Status(Types.Status.Skip);
                }
            }
        }

        public override bool Ok()
        {
            Logger.AppendInfo("If base.ok: " + base.Ok());
            Logger.AppendInfo("this.m_profiles.Count size:  " + this.m_profiles.Count);
            Logger.AppendInfo("this.m_controls.Count size: " + this.m_controls.Count);
            return (base.Ok() && (this.m_profiles.Count > 0 || this.m_controls.Count > 0));
        }

        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("Basics:");
            foreach (Profile profile in this.m_profiles)
                ss.AppendFormat("{0}\n", profile.ToString());
            ss.AppendLine();
            return ss.ToString();
        }
        #endregion

        #region private methods
        private void parseCsv(string fullpath)
        {
            if (!File.Exists(fullpath))
                return;
            try
            {
                using (StreamReader reader = new StreamReader(fullpath))
                {
                    bool controlEffectiveArea = false;
                    bool profilesEffectiveArea = false;

                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine().Trim();
                        switch (line)
                        {
                            case BasicCsv.CONTROLS_AREA_KEY:
                                controlEffectiveArea = true;
                                profilesEffectiveArea = false;
                                break;

                            case BasicCsv.PROFILES_AREA_KEY:
                                controlEffectiveArea = false;
                                profilesEffectiveArea = true;
                                break;

                            default:
                                if (controlEffectiveArea)
                                {
                                    if (line.Length > 0)
                                        CreateControls(line);
                                    else
                                        controlEffectiveArea = false;
                                }
                                else if (profilesEffectiveArea)
                                {
                                    if (line.Length > 0)
                                        CreateProfile(line);
                                    else
                                        profilesEffectiveArea = false;
                                }
                                break;
                        }
                    }
                }
                //Dictionary<Types.ControlKey, List<Control>> defaultControls = CreateDefaultControls();
                //MergeDefaultAndEffectiveControls(defaultControls);
                MergeProfilesControls();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION!: " + e.Message);
            }
        }

        private Dictionary<Types.ControlKey, List<Control>> CreateDefaultControls()
        {
            var allControlKeys = Types.GetAllControlKeys();
            var defaultMap = Control.GetDefaultMap();
            Dictionary<Types.ControlKey, List<Control>> ret = new Dictionary<Types.ControlKey, List<Control>>();

            foreach (var controlKey in allControlKeys)
            {
                DefaultMap.Item item = defaultMap.getItem(controlKey.ToString());
                Control control = new Control(controlKey, item.Default);
                List<Control> lst = new List<Control>();
                lst.Add(control);
                ret.Add(controlKey, lst);
            }

            return ret;
        }

        private void MergeDefaultAndEffectiveControls(Dictionary<Types.ControlKey, List<Control>> defaultControls)
        {
            var allControlKeys = Types.GetAllControlKeys();
            foreach (var controlKey in allControlKeys)
            {
                if (!defaultControls.ContainsKey(controlKey))
                    continue;
                if (!this.m_controls.ContainsKey(controlKey))
                    this.m_controls.Add(controlKey, defaultControls[controlKey]);
            }
        }

        private void MergeProfilesControls()
        {
            if (this.m_profiles.Count == 0)
                return;

            Control[][] controlsArray = new Control[this.m_controls.Count][];
            int i = 0;
            foreach (var pair in this.m_controls)
            {
                Control[] controlArray = pair.Value.ToArray<Control>();
                controlsArray[i] = controlArray;
                i++;
            }

            var results = GetCartesianProduct(controlsArray);

            List<Profile> newProfiles = new List<Profile>();
            foreach (Profile p1 in this.m_profiles)
            {
                foreach (var itemList in results)
                {
                    List<Control> toProfile = itemList.ToList<Control>();
                    Profile newProfile = p1.DeepCopy();
                    BasicProfile newBasicProfile = (newProfile as BasicProfile);
                    newBasicProfile.AddControls(toProfile);
                    newProfiles.Add(newBasicProfile);
                }
            }
            this.m_profiles = new List<Profile>(newProfiles);
        }

        private void CreateProfile(string line)
        {
            Profile profile = new BasicProfile(line);
            this.m_profiles.Add(profile);
        }

        private void CreateControls(string line)
        {
            char[] delim = { ',' };
            string[] values = line.Split(delim);
            string controlKeyStr = values[0].Trim();
            Types.ControlKey controlKey = Types.ConvertStringToControlKey(controlKeyStr);
            if (values.Length < 2)
            {
                Control control = new Control(controlKey);
                addToControlMap(controlKey, ref control);
                return;
            }
            
            for (int i = 1; i < values.Length; i++)
            {
                Control control = new Control(controlKey, values[i].Trim());
                addToControlMap(controlKey, ref control);
            }
        }

        private void addToControlMap(Types.ControlKey controlKey, ref Control control)
        {
            if (control == null)
                return;

            if (this.m_controls.ContainsKey(controlKey))
                this.m_controls[controlKey].Add(control);
            else
            {
                List<Control> l = new List<Control>();
                l.Add(control);
                this.m_controls.Add(controlKey, l);
            }
        }

        private Control[][] GetCartesianProduct(Control[][] sets)
        {
            var resultSets = new List<Control[]>();
            var indices = new int[sets.Length];
            CreateProductSet(sets, indices, 0, resultSets);
            return resultSets.ToArray();
        }

        private void CreateProductSet(Control[][] inputSets, int[] indices, int currentSet, List<Control[]> resultSets)
        {
            for (int i = 0; i < inputSets[currentSet].Length; i++)
            {
                indices[currentSet] = i;
                if (currentSet == (inputSets.Length - 1))
                {
                    var cartesianSet = new Control[inputSets.Length];
                    for (int j = 0; j < inputSets.Length; j++)
                    {
                        cartesianSet[j] = (inputSets[j][indices[j]]);
                    }
                    resultSets.Add(cartesianSet);
                }
                else
                {
                    CreateProductSet(inputSets, indices, currentSet + 1, resultSets);
                }
            }
        }
        #endregion

        #region debug
        private void printList(List<Control[]> resultSets)
        {
            StreamWriter sw = File.AppendText(@"C:\tmp\New folder\gdb.txt");
            sw.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            sw.WriteLine("Size of List: {0}", resultSets.Count);
            foreach (var i in resultSets)
            {
                sw.WriteLine("Size of Array: {0}", i.Length);
                foreach (var j in i)
                {
                    sw.WriteLine(j);
                }
            }
            sw.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            sw.Close();
        }

        private void printControlMap(Dictionary<Types.ControlKey, List<Control>> dict)
        {
            Debug.WriteLine("======== CONTROLS =============");
            foreach (var pair in dict)
            {
                Debug.WriteLine("Control: {0}", pair.Key.ToString());
                foreach (var item in pair.Value)
                {
                    Debug.WriteLine("\t" + item);
                }
                Debug.WriteLine("-----------------------------");
            }
            Debug.WriteLine("===============================");
        }
        #endregion
    }
}
