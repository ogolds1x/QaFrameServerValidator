using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class BasicProfile : Profile
    {
        #region constants
        private const int COMMAND_TYPE_INDEX = 0;
        private const int PROFILE_TYPE_INDEX = 1;
        private const int MODE_INDEX_1 = 2;
        private const int RESOLUTION_INDEX_1 = 3;
        private const int FPS_INDEX_1 = 4;
        private const int MODE_INDEX_2 = 5;
        private const int RESOLUTION_INDEX_2 = 6;
        private const int FPS_INDEX_2 = 7;
        private const int MODE_INDEX_3 = 8;
        private const int RESOLUTION_INDEX_3 = 9;
        private const int FPS_INDEX_3 = 10;
        #endregion

        #region members
        private string m_commandType;
        private Types.ModeType m_profileType = Types.ModeType.Undef;
        private Types.ColorMode m_colorMode = Types.ColorMode.Undef;
        private Types.DepthMode m_depthMode = Types.DepthMode.Undef;
        private Types.FishEyeMode m_fishEyeMode = Types.FishEyeMode.Undef;
        private Types.IRMode m_IRMode = Types.IRMode.Undef;
        private Types.Resolution m_colorResolution;
        private Types.Resolution m_depthResolution;
        private Types.Resolution m_fishEyeResolution;
        private Types.Resolution m_IrResolution;
        private int m_colorFps = 0;
        private int m_depthFps = 0;
        private int m_fishEyeFps = 0;
        private int m_IRFps = 0;
        private List<Control> m_controls = new List<Control>();
        public Types.Status isToDo = Types.Status.Do;
        #endregion

        #region constructors
        public BasicProfile(string values)
        {
            parse(values, out this.m_commandType, out this.m_profileType, out this.m_colorMode, out this.m_depthMode, out this.m_fishEyeMode, out this.m_IRMode, out this.m_colorResolution, out this.m_depthResolution, out this.m_fishEyeResolution, out this.m_IrResolution, out this.m_colorFps, out this.m_depthFps, out this.m_fishEyeFps, out this.m_IRFps);
        }
        public BasicProfile(string commandType, Types.ModeType profileType, Types.ColorMode colorMode, Types.DepthMode depthMode, Types.FishEyeMode fishEyeMode, Types.IRMode iRMode, Types.Resolution colorResolution, Types.Resolution depthResolution, Types.Resolution fishEyeResolution, Types.Resolution iRResolution, int colorFps, int depthFps, int fishEyeFps, int iRFps, List<Control> controls, Types.Status status)
        {
            this.m_commandType = commandType;
            this.m_profileType = profileType;
            this.m_colorMode = colorMode;
            this.m_depthMode = depthMode;
            this.m_fishEyeMode = fishEyeMode;
            this.m_IRMode = iRMode;
            this.m_colorResolution = new Types.Resolution(colorResolution);
            this.m_depthResolution = new Types.Resolution(depthResolution);
            this.m_fishEyeResolution = new Types.Resolution(fishEyeResolution);
            this.m_IrResolution = new Types.Resolution(iRResolution);
            this.m_colorFps = colorFps;
            this.m_depthFps = depthFps;
            this.m_fishEyeFps = fishEyeFps;
            this.m_IRFps = iRFps;
            this.m_controls = new List<Control>(controls);
            this.isToDo = status;
        }
        #endregion

        #region public methods
        void Profile.Status(Types.Status status)
        {
            this.isToDo = status;
        }

        Types.Status Profile.Status()
        {
            return this.isToDo;
        }

        bool Profile.Equal(string excludeProfileStr)
        {
            string commandType;
            Types.ModeType profileType;
            Types.ColorMode colorMode;
            Types.DepthMode depthMode;
            Types.FishEyeMode fishEyeMode;
            Types.IRMode iRMode;
            Types.Resolution colorResolution;
            Types.Resolution depthResolution;
            Types.Resolution fishEyeResolution;
            Types.Resolution irResolution;
            int colorFps;
            int depthFps;
            int fishEyeFps;
            int iRFps;

            parse(excludeProfileStr, out commandType, out profileType, out colorMode, out depthMode,out fishEyeMode, out iRMode, out colorResolution, out depthResolution,out fishEyeResolution, out irResolution, out colorFps, out depthFps,out fishEyeFps, out iRFps);

            int index = excludeProfileStr.IndexOf("Properties");
            bool controlsAreEquals = false;
            if (index >= 0)
            {
                string controlsStr = excludeProfileStr.Substring(index + "Properties".Length + 1).Trim();
                controlsAreEquals = controlsEqualsTo(controlsStr);
            }
            else
                controlsAreEquals = (this.m_controls.Count == 0);

            bool ret = (
                            commandType.IndexOf(this.m_commandType) == 0 &&
                            profileType == this.m_profileType &&
                            colorMode == this.m_colorMode &&
                            depthMode == this.m_depthMode &&
                            fishEyeMode == this.m_fishEyeMode &&
                            iRMode == this.m_IRMode &&
                            (colorResolution != null && colorResolution.Equal(this.m_colorResolution)) &&
                            (depthResolution != null && depthResolution.Equal(this.m_depthResolution))&&
                            (fishEyeResolution != null && fishEyeResolution.Equal(this.m_fishEyeResolution)) &&
                            (irResolution != null && irResolution.Equal(this.m_IrResolution)) &&
                            colorFps == this.m_colorFps &&
                            depthFps == this.m_depthFps &&
                            fishEyeFps == this.m_fishEyeFps &&
                            iRFps == this.m_IRFps &&
                            controlsAreEquals
                        );

            return ret;
        }

        public void AddControls(List<Control> controls)
        {
            this.m_controls = new List<Control>(controls);
        }

        public string CommandType
        {
            get { return this.m_commandType; }
        }

        public Types.ModeType ProfileType
        {
            get { return this.m_profileType; }
        }

        public string ProfileTypeAsString
        {
            get { return Types.ConvertModeTypeToString(this.m_profileType); }
        }
        public Types.ColorMode ColorMode
        {
            get { return this.m_colorMode; }
        }
        public string ColorModeAsString
        {
            get { return Types.ConvertColorModeToString(this.m_colorMode); }
        }

        public Types.DepthMode DepthMode
        {
            get { return this.m_depthMode; }
        }

        public string DepthModeAsString
        {
            get { return Types.ConvertDepthModeToString(this.m_depthMode); }
        }

        public Types.FishEyeMode FishEyeMode
        {
            get { return this.m_fishEyeMode; }
        }
        
        public string FishEyeModeAsString
        {
            get { return Types.ConvertFishEyeModeToString(this.m_fishEyeMode); }
        }

        public string IRModeAsString
        {
            get { return Types.ConvertIRModeToString(this.m_IRMode); }
        }

        public Types.IRMode IRMode
        {
            get { return this.m_IRMode; }
        }
        
        public Types.Resolution ColorResolution
        {
            get { return this.m_colorResolution; }
        }

        public string ColorResolutionAsString
        {
            get { return this.m_colorResolution.ToString(); }
        }

        public Types.Resolution DepthResolution
        {
            get { return this.m_depthResolution; }
        }

        public string DepthResolutionAsString
        {
            get { return this.m_depthResolution.ToString(); }
        }

        public Types.Resolution FishEyeResolution
        {
            get { return this.m_fishEyeResolution; }
        }

        public string FishEyeResolutionAsString
        {
            get { return this.m_fishEyeResolution.ToString(); }
        }
        public Types.Resolution IRResolution
        {
            get { return this.m_IrResolution; }
        }

        public string IRResolutionAsString
        {
            get { return this.m_IrResolution.ToString(); }
        }
        public int ColorFps
        {
            get { return this.m_colorFps; }
        }

        public int DepthFps
        {
            get { return this.m_depthFps; }
        }
        public int FishEyeFps
        {
            get { return this.m_fishEyeFps; }
        }
        public int IRFps
        {
            get { return this.m_IRFps; }
        }
        public List<Control> Controls
        {
            get { return this.m_controls; }
        }

        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();

            ss.AppendFormat("{0} ({1}):", this.isToDo, this.m_commandType);
            ss.AppendFormat(" Profile Type = {0}", Types.ConvertModeTypeToString(this.m_profileType));
            ss.AppendFormat(", Depth Mode = {0}", Types.ConvertDepthModeToString(this.m_depthMode));
            ss.AppendFormat(", Color Mode = {0}", Types.ConvertColorModeToString(this.m_colorMode));
            ss.AppendFormat(", FishEye Mode = {0}", Types.ConvertFishEyeModeToString(this.m_fishEyeMode));
            ss.AppendFormat(", IR Mode = {0}", Types.ConvertIRModeToString(this.m_IRMode));
            ss.AppendFormat(", Depth Resolution = {0}", (this.m_depthResolution == null ? "NULL" : this.m_depthResolution.ToString()));
            ss.AppendFormat(", Color Resolution = {0}", (this.m_colorResolution == null ? "NULL" : this.m_colorResolution.ToString()));
            ss.AppendFormat(", FishEye Resolution = {0}", (this.m_fishEyeResolution == null ? "NULL" : this.m_fishEyeResolution.ToString()));
            ss.AppendFormat(", IR Resolution = {0}", (this.m_IrResolution == null ? "NULL" : this.m_IrResolution.ToString()));
            ss.AppendFormat(", Depth FPS = {0}", this.m_depthFps);
            ss.AppendFormat(", Color FPS = {0}", this.m_colorFps);
            ss.AppendFormat(", FishEye FPS = {0}", this.m_fishEyeFps);
            ss.AppendFormat(", IR FPS = {0}", this.m_IRFps);
            foreach (Control control in this.m_controls)
                ss.AppendFormat(", {0}", control);
            ss.AppendLine();
            return ss.ToString();
        }

        Profile Profile.DeepCopy()
        {
            return new BasicProfile(this.m_commandType, this.m_profileType, this.m_colorMode, this.m_depthMode,this.m_fishEyeMode,this.m_IRMode, this.m_colorResolution, this.m_depthResolution,this.m_fishEyeResolution,this.m_IrResolution, this.m_colorFps, this.m_depthFps,this.m_fishEyeFps,this.m_IRFps, this.m_controls, this.isToDo);
        }
        #endregion

        #region private methods
        private void parse(string values, out string commandType, out Types.ModeType profileType, out Types.ColorMode colorMode, out Types.DepthMode depthMode, out Types.FishEyeMode fishEyeMode, out Types.IRMode iRMode, out Types.Resolution colorResolution, out Types.Resolution depthResolution, out Types.Resolution fishEyeResolution, out Types.Resolution iRResolution, out int colorFps, out int depthFps, out int fishEyeFps, out int iRFps)
        {
            commandType = null;
            profileType = Types.ModeType.Undef;
            colorMode = Types.ColorMode.Undef;
            depthMode = Types.DepthMode.Undef;
            fishEyeMode = Types.FishEyeMode.Undef;
            iRMode = Types.IRMode.Undef;
            colorResolution = null;
            depthResolution = null;
            fishEyeResolution = null;
            iRResolution = null;
            colorFps = 0;
            depthFps = 0;
            fishEyeFps = 0;
            iRFps = 0;
            char[] delim = { ',' };
            string[] valuesArray = values.Split(delim);
            if (valuesArray.Length < 3)
                return;

            commandType = valuesArray[BasicProfile.COMMAND_TYPE_INDEX];
            profileType = Types.ConvertStringToModeType(valuesArray[BasicProfile.PROFILE_TYPE_INDEX]);

            if (valuesArray.Length > BasicProfile.MODE_INDEX_1)
            {
                switch (profileType)
                {
                    case Types.ModeType.Color:
                        colorMode = Types.ConvertStringToColorMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        break;

                    case Types.ModeType.Depth:
                        depthMode = Types.ConvertStringToDepthMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        break;
                    case Types.ModeType.FishEye:
                        fishEyeMode = Types.ConvertStringToFishEyeMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        break;
                    case Types.ModeType.IR:
                        iRMode = Types.ConvertStringToIRMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        break;
                    case Types.ModeType.Sync:
                        colorMode = Types.ConvertStringToColorMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        if (valuesArray.Length > BasicProfile.MODE_INDEX_2)
                            depthMode = Types.ConvertStringToDepthMode(valuesArray[BasicProfile.MODE_INDEX_2]);
                        if (valuesArray.Length > BasicProfile.MODE_INDEX_3)
                            iRMode = Types.ConvertStringToIRMode(valuesArray[BasicProfile.MODE_INDEX_3]);
                            break;
                    case Types.ModeType.SyncIR:
                        iRMode = Types.ConvertStringToIRMode(valuesArray[BasicProfile.MODE_INDEX_1]);
                        if (valuesArray.Length > BasicProfile.MODE_INDEX_2)
                            depthMode = Types.ConvertStringToDepthMode(valuesArray[BasicProfile.MODE_INDEX_2]);
                        break;
                }
            }

            if (valuesArray.Length > BasicProfile.RESOLUTION_INDEX_1)
            {
                switch (profileType)
                {
                    case Types.ModeType.Color:
                        colorResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_1]);
                        break;

                    case Types.ModeType.Depth:
                        depthResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_1]);
                        break;
                    case Types.ModeType.FishEye:
                        fishEyeResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_1]);
                        break;
                    case Types.ModeType.Sync:
                        colorResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_1]);
                        if (valuesArray.Length > BasicProfile.RESOLUTION_INDEX_2)
                            depthResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_2]);
                        if (valuesArray.Length > BasicProfile.RESOLUTION_INDEX_3)
                            iRResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_3]);
                        break;
                    case Types.ModeType.SyncIR:
                        iRResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_1]);
                        if (valuesArray.Length > BasicProfile.RESOLUTION_INDEX_2)
                            depthResolution = new Types.Resolution(valuesArray[BasicProfile.RESOLUTION_INDEX_2]);                       
                        break;
                }
            }

            if (valuesArray.Length > BasicProfile.FPS_INDEX_1)
            {
                int fps1 = 0, fps2 = 0, fps3=0;
                try
                {
                    fps1 = int.Parse(GetFpsStr(valuesArray[BasicProfile.FPS_INDEX_1]));
                    if (valuesArray.Length > BasicProfile.FPS_INDEX_2)
                        fps2 = int.Parse(GetFpsStr(valuesArray[BasicProfile.FPS_INDEX_2]));
                    if (valuesArray.Length > BasicProfile.FPS_INDEX_3)
                        fps3 = int.Parse(GetFpsStr(valuesArray[BasicProfile.FPS_INDEX_3]));
                }
                catch
                {
                    fps1 = 0;
                    fps2 = 0;
                    fps3 = 0; 
                }
                switch (profileType)
                {
                    case Types.ModeType.Color:
                        colorFps = fps1;
                        break;
                    case Types.ModeType.Depth:
                        depthFps = fps1;
                        break;
                    case Types.ModeType.IR:
                        iRFps = fps3;
                        break;
                    case Types.ModeType.FishEye:
                        fishEyeFps = fps1;
                        break;
                    case Types.ModeType.Sync:
                        colorFps = fps1;
                        depthFps = fps2;
                        iRFps = fps3;
                        break;
                    case Types.ModeType.SyncIR:
                        iRFps = fps1;
                        depthFps = fps2;
                        break;
                }
            }
        }

        private string GetFpsStr(string input)
        {
            int index = input.IndexOf("Fps_");
            if (index < 0)
                return input;
            return input.Substring("Fps_".Length);
        }

        private bool controlsEqualsTo(string controlsStr)
        {
            char[] delim = { ',' };
            string[] valuesArray = controlsStr.Split(delim);

            if (valuesArray.Length < 2)
                return false;
            List<Control> controls = new List<Control>();
            int i = 1;
            do
            {
                Control control = new Control(valuesArray[i - 1], valuesArray[i]);
                controls.Add(control);
                i += 2;
            }
            while (i < valuesArray.Length);

            if (controls.Count != this.m_controls.Count)
                return false;

            bool ret = true;

            foreach (Control control1 in this.m_controls)
            {
                bool eq = false;
                foreach (Control control2 in controls)
                {
                    if (control2.Equal(control1))
                    {
                        eq = true;
                        break;
                    }
                }
                if (!eq)
                {
                    ret = false;
                    break;
                }
            }

            return (ret && this.m_controls.Count > 0);
        }
        #endregion
    }
}
