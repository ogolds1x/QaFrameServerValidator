using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class EventCsv : CsvParent
    {
        #region constants
        private const string TEST_PROFILES_AREA = "TestProfiles";
        #endregion

        #region members
        private List<Profile> m_profiles = new List<Profile>();
        #endregion

        #region constructors
        public EventCsv(string fullpath)
            : base(fullpath)
        {
            parseCsv(fullpath);
        }
        #endregion

        #region public methods
        public override List<Profile> GetProfiles()
        {
            return this.m_profiles;
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
            return (base.Ok() && this.m_profiles.Count > 0);
        }

        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("Events:");
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
                    bool testProfilesEffectiveArea = false;

                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine().Trim();
                        if (testProfilesEffectiveArea && line.Length == 0)
                            break;

                        if (line == EventCsv.TEST_PROFILES_AREA)
                            testProfilesEffectiveArea = true;

                        else if (testProfilesEffectiveArea && line.Length > 0)
                        {
                            Profile profile = new TestProfile(line);
                            this.m_profiles.Add(profile);
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
        #endregion
    }
}
