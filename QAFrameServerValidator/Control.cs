using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class Control
    {
        #region members
        private static DefaultMap ms_defaults = null;
        private Types.ControlKey m_key;
        private string m_name;
        private float m_value = 0;
        private float m_defaultValue = 0;
        private float m_step = 0;
        private float m_minimum = 0;
        private float m_maximum = 0;
        private bool m_isAutoCapable = false; // XXX wtf?
        #endregion


        /*
         * IMPORTANT NOTE:
         *  This class is works with "CONTROL_NAME_IN_DLL" attribute at file of the defaults (mean: Controls.xml)
         *  The "Name" field - is mean NOTHING!!!!!!!!!!
         *  
         *  Please, send the CONTROL_NAME_IN_DLL name 
         *  
         *  OR
         *  
         * Work with constants, defined at Types.ControlKey enum (recommended)
         */

        #region constructors
        public Control(string key, string value)
        {
            this.m_key = Types.ConvertStringToControlKey(key);
            Control.Init(key, ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);
            try
            {
                float val = float.Parse(value);
                if (val >= this.m_minimum && val <= this.m_maximum)
                    this.m_value = val;
                else
                    this.m_value = this.m_defaultValue;
            }
            catch
            {
                this.m_value = this.m_defaultValue;
            }
        }

        public Control(Types.ControlKey key, string value)
        {
            this.m_key = key;
            Control.Init(Types.ConvertControlKeyToString(key), ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);
            try
            {
                float val = float.Parse(value);
                if (val >= this.m_minimum && val <= this.m_maximum)
                    this.m_value = val;
                else
                    this.m_value = this.m_defaultValue;
            }
            catch
            {
                this.m_value = this.m_defaultValue;
            }
        }

        public Control(string key, float value)
        {
            this.m_key = Types.ConvertStringToControlKey(key);
            Control.Init(key, ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);

            if (value >= this.m_minimum && value <= this.m_maximum)
                this.m_value = value;
            else
                this.m_value = this.m_defaultValue;
        }

        public Control(Types.ControlKey key, float value)
        {
            this.m_key = key;
            Control.Init(Types.ConvertControlKeyToString(key), ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);

            if (value >= this.m_minimum && value <= this.m_maximum)
                this.m_value = value;
            else
                this.m_value = this.m_defaultValue;
        }

        public Control(Types.ControlKey key)
        {
            this.m_key = key;
            Control.Init(Types.ConvertControlKeyToString(key), ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);
            this.m_value = this.m_defaultValue;
        }

        public Control(string key)
        {
            this.m_key = Types.ConvertStringToControlKey(key);
            Control.Init(key, ref this.m_name, ref this.m_defaultValue, ref this.m_step, ref this.m_minimum, ref this.m_maximum, ref this.m_isAutoCapable);
            this.m_value = this.m_defaultValue;
        }
        #endregion

        #region public methods
        public static void Init(string pathToDefault)
        {
            if (!File.Exists(pathToDefault))
                throw new Exception("**file not found: " + pathToDefault);//CameraSDKTestsException("Default file " + pathToDefault + " is not exists");
            Control.ms_defaults = new DefaultMap(pathToDefault);
        }
        public bool Equal(Control other)
        {
            return (this.m_key == other.Key) && (this.m_value == other.Value);
        }

        public string Name
        {
            get { return this.m_name; }
        }

        public Types.ControlKey Key
        {
            get { return this.m_key; }
        }

        public string KeyAsString
        {
            get { return Types.ConvertControlKeyToString(this.m_key); }
        }

        public float Value
        {
            get { return m_value; }
        }

        public float Step
        {
            get { return this.m_step; }
        }

        public bool IsAutoCapable
        {
            get { return this.m_isAutoCapable; }
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", this.Name, this.Value);
        }

        public static DefaultMap GetDefaultMap()
        {
            return ms_defaults;
        }
        #endregion

        #region private methods
        private static void Init(string key, ref string name, ref float defaultValue, ref float step, ref float minimum, ref float maximum, ref bool isAutoCapable)
        {
            if (Control.ms_defaults == null)
                throw new Exception("Please, init Default Controls");//CameraSDKTestsException("Please, init Default Controls");

            DefaultMap.Item defaults = Control.ms_defaults.getItem(key);
            if (defaults == null)
                throw new Exception("Did not find a control with name " + key); //CameraSDKTestsException("Please, init Default Controls");

            name = defaults.Name;
            defaultValue = defaults.Default;
            step = defaults.Step;
            minimum = defaults.Min;
            maximum = defaults.Max;
            isAutoCapable = defaults.IsAutoCapable;
        }
        #endregion
    }
}
