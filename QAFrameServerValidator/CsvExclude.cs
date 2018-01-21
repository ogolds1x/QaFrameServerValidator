using System;
using System.Collections.Generic;
using System.IO;


namespace QAFrameServerValidator
{
    public class CsvExclude
    {
        #region members
        private Dictionary<Types.SubArea, List<string>> m_excludeMap = new Dictionary<Types.SubArea, List<string>>();
        #endregion

        #region constructors
        public CsvExclude(string fullpath)
        {
            parse(fullpath);
        }
        #endregion

        #region public methods
        public List<string> this[Types.SubArea subArea]
        {
            get
            {
                List<string> ret = getExcludes(subArea);
                return (ret == null ? new List<string>() : ret);
            }
        }
        #endregion

        #region private methods
        private void parse(string fullpath)
        {
            if (!File.Exists(fullpath))
                return;
            try
            {
                using (StreamReader reader = new StreamReader(fullpath))
                {
                    Types.SubArea subArea = Types.SubArea.Undef;
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine().Trim();
                        if (line.Length == 0)
                        {
                            subArea = Types.SubArea.Undef;
                        }
                        else if (line[0] == '-' && line.Length > 1)
                        {
                            line = line.Substring(1);
                            subArea = Types.ConvertStringToSubArea(line);
                            if (subArea == Types.SubArea.Undef)
                                continue;
                            if (!this.m_excludeMap.ContainsKey(subArea))
                                this.m_excludeMap.Add(subArea, new List<string>());
                        }
                        else
                        {
                            if (subArea == Types.SubArea.Undef)
                                continue;
                            if (!this.m_excludeMap.ContainsKey(subArea))
                                continue;
                            this.m_excludeMap[subArea].Add(line);
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }

        private List<string> getExcludes(Types.SubArea subArea)
        {
            if (!this.m_excludeMap.ContainsKey(subArea))
                return null;
            return this.m_excludeMap[subArea];
        }
        #endregion
    }
}
