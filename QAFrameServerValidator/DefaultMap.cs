using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QAFrameServerValidator
{
    public class DefaultMap
    {
        #region constants
        public enum Type { Undef, Depth, Color }

        private const string DepthControlsKey = "DepthControls";
        private const string ColorControlsKey = "ColorControls";
        private const string ControlsKey = "Controls";
        private const string NameKey = "Name";
        private const string ControlNameInDllKey = "CONTROL_NAME_IN_DLL"; // this is the foreign key for me at Controls.xml file && *csv files
        private const string MinKey = "Min";
        private const string MaxKey = "Max";
        private const string StepKey = "Step";
        private const string DefaultKey = "Default";
        private const string IsAutoCapableKey = "IsAutoCapable";
        #endregion

        #region members
        private Dictionary<string, Item> m_colorDefaults = new Dictionary<string, Item>();
        private Dictionary<string, Item> m_depthDefaults = new Dictionary<string, Item>();
        private Dictionary<string, Item> m_commonDefaults = new Dictionary<string, Item>();
        #endregion

        #region constructors
        public DefaultMap(string fullpath)
        {
            parse(fullpath);
        }
        #endregion

        #region public methods
        public Item getItem(Type type, string name)
        {
            if (type == Type.Color && this.m_colorDefaults.ContainsKey(name))
                return this.m_colorDefaults[name];
            if (type == Type.Depth && this.m_depthDefaults.ContainsKey(name))
                return this.m_depthDefaults[name];
            return null;
        }

        public Item getItem(string name)
        {
            try
            {
                if (name != null)
                    if (this.m_commonDefaults.ContainsKey(name))
                        return this.m_commonDefaults[name];
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region private methods
        private void parse(string fullpath)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(fullpath);
                Type type = Type.Undef;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case DepthControlsKey:
                                type = Type.Depth;
                                break;

                            case ColorControlsKey:
                                type = Type.Color;
                                break;

                            case ControlsKey:
                                WorkOnAttributes(reader, type);
                                break;

                            default: break;
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Wrong format of the Default Values in file " + fullpath); //CameraSDKTestsException("Wrong format of the Default Values in file " + fullpath);
            }
            finally
            {
                reader.Close();
            }
        }

        private void WorkOnAttributes(XmlTextReader reader, Type type)
        {
            if (!reader.HasAttributes)
                return;

            Item item = new Item();
            while (reader.MoveToNextAttribute())
            {
                string name = reader.Name;
                string value = reader.Value;

                switch (name)
                {
                    case ControlNameInDllKey:
                        item.ControlNameInDll = value;
                        break;

                    case NameKey:
                        item.Name = value;
                        break;

                    case MinKey:
                        item.Min = getFloat(value);
                        break;

                    case MaxKey:
                        item.Max = getFloat(value);
                        break;

                    case StepKey:
                        item.Step = getFloat(value);
                        break;

                    case DefaultKey:
                        item.Default = getFloat(value);
                        break;

                    case IsAutoCapableKey:
                        item.IsAutoCapable = getBool(value);
                        break;
                }
            }

            switch (type)
            {
                case Type.Color:
                    this.m_colorDefaults.Add(item.ControlNameInDll, item);
                    break;

                case Type.Depth:
                    this.m_depthDefaults.Add(item.ControlNameInDll, item);
                    break;

                case Type.Undef:
                    Debug.WriteLine("Default map warning: unknown control type");
                    break;
            }
            this.m_commonDefaults.Add(item.ControlNameInDll, item);
        }

        private float getFloat(string val)
        {
            float ret = 0;
            try
            {
                ret = float.Parse(val);
            }
            catch
            {
                ret = 0;
            }
            return ret;
        }

        private bool getBool(string val)
        {
            bool ret = false;
            try
            {
                ret = bool.Parse(val);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        #endregion

        #region Item
        public class Item
        {
            public string ControlNameInDll { get; set; }
            public string Name { get; set; }
            public float Min { get; set; }
            public float Max { get; set; }
            public float Step { get; set; }
            public float Default { get; set; }
            public bool IsAutoCapable { get; set; }
        }
        #endregion
    }
}
