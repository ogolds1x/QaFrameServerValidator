using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public abstract class CsvParent
    {
        #region constants
        private const string HEADER_KEY = "TestDetails";
        private const string AREA_KEY = "Area";
        private const string SUBAREA_KEY = "SubArea";
        private const string DESCRIPTION_KEY = "Description";
        private const string BELONGTO_Key = "BelongTo";

        #endregion

        #region members
        private string m_area = null;
        private string m_subArea = null;
        private string m_description = null;
        private string m_belongTo = null;
        #endregion

        #region constructors
        public CsvParent(string fullpath)
        {
            parseCsv(fullpath);
        }
        #endregion

        #region abstract methods
        public abstract List<Profile> GetProfiles();
        public abstract void ExcludeProfiles(List<string> excludes);
        #endregion

        #region public methods
        public string Area
        {
            get { return m_area; }
        }

        public string SubAreaAsString
        {
            get { return m_subArea; }
        }

        public Types.SubArea SubArea
        {
            get { return Types.ConvertStringToSubArea(m_subArea); }
        }

        public string Description
        {
            get { return m_description; }
        }

        public string BelongTo
        {
            get { return m_belongTo; }
        }

        public virtual bool Ok()
        {
            return (this.m_area != null && this.m_subArea != null && this.m_description != null);
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
                    bool effectiveSection = false;
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine().Trim();
                        if (effectiveSection && line.Length == 0)
                            break;

                        char[] delim = { ',' };
                        string[] values = line.Split(delim);

                        string key = null;
                        string value = null;

                        if (values.Length > 0)
                            key = values[0];
                        if (values.Length > 1)
                            value = values[1];

                        if (key == null)
                            continue;

                        switch (key)
                        {
                            case CsvParent.HEADER_KEY:
                                effectiveSection = true;
                                break;

                            case CsvParent.AREA_KEY:
                                this.m_area = value;
                                break;

                            case CsvParent.SUBAREA_KEY:
                                this.m_subArea = value;
                                break;

                            case CsvParent.DESCRIPTION_KEY:
                                this.m_description = value;
                                break;

                            case CsvParent.BELONGTO_Key:
                                this.m_belongTo = value;
                                break;
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
