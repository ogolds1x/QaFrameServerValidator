using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public class Types
    {
        #region ModeType
        public enum CameraType { DS5Color, DS5Depth, SR300Color, SR300Depth }
        public enum ModeType { Undef, Color, Depth, Sync, FishEye, IR, SyncIR }
        public enum DeviceMode { Undef, DebugMode, ReleaseMode }
        public enum PlatformSupportMode { U1U2Enabled, U1U2Disabled, Unknown }
        public static string ConvertModeTypeToString(ModeType type)
        {
            if (type == ModeType.Undef)
                return null;
            return type.ToString();
        }

        public static ModeType ConvertStringToModeType(string typeStr)
        {
            ModeType ret = ModeType.Undef;
            if (typeStr == null)
                return ret;

            switch (typeStr)
            {
                case "Color":
                    ret = ModeType.Color;
                    break;

                case "Depth":
                    ret = ModeType.Depth;
                    break;
                case "Sync":
                    ret = ModeType.Sync;
                    break;
                case "SyncIR":
                    ret = ModeType.SyncIR;
                    break;
                case "FishEye":
                    ret = ModeType.FishEye;
                    break;
                case "IR":
                    ret = ModeType.IR;
                    break;
            }

            return ret;
        }
        #endregion


        #region HDR
        public enum HdrCommandType { SetMode, GetMode, SetConfig, GetConfig, SetBoth, GetBoth, SetAndGetMode, SetAndGetConfig }
        public enum StreamingInHdr { WithStreaming, WithoutStreaming, WithoutStreamingAndTheWith, WithStreamingAndThenWithout }
        public enum HdrErrorType { NoError, SetConfigNotEqualToGet, SetModeNotEqualToGet, SetConfigFailed, GetConfigFailed, SetModeFailed, GetModeFailed }
        #endregion


        #region ColorMode
        public enum ColorMode { Undef, NoColor, RGB24, Raw, YUY2, L8 }

        public static string ConvertColorModeToString(ColorMode mode)
        {
            if (mode == ColorMode.Undef)
                return null;
            return mode.ToString();
        }
        
        public static ColorMode ConvertStringToColorMode(string modeStr)
        {
            ColorMode ret = ColorMode.Undef;
            if (modeStr == null)
                return ret;

            switch (modeStr)
            {
                case "NoColor":
                    ret = ColorMode.NoColor;
                    break;

                case "RGB24":
                    ret = ColorMode.RGB24;
                    break;

                case "Raw":
                    ret = ColorMode.Raw;
                    break;

                case "YUY2":
                    ret = ColorMode.YUY2;
                    break;                    
            }
            return ret;
        }
        #endregion


        #region IRMode
        public enum IRMode { Undef, L8 , G20493859, G49323159, G59565955}

        public static string ConvertIRModeToString(IRMode mode)
        {
            if (mode == IRMode.Undef)
                return null;
            return mode.ToString();
        }

        public static IRMode ConvertStringToIRMode(string modeStr)
        {
            IRMode ret = IRMode.Undef;
            if (modeStr == null)
                return ret;

            switch (modeStr)
            {
                case "L8":
                    ret = IRMode.L8;
                    break;                
                case "G20493859":
                    ret = IRMode.G20493859;
                    break;
                case "G49323159":
                    ret = IRMode.G49323159;
                    break;
                case "G59565955":
                    ret = IRMode.G59565955;
                    break;                     
            }
            return ret;
        }
        #endregion

        #region DepthMode
        public enum DepthMode { Undef, Z, ZY, SyncedZI, SyncedZC, PackedRaw, Calibration, I, L8, G2036315A, D16, G59565955, G49323159, G20493859, YUY2, }
        
        public static string ConvertDepthModeToString(DepthMode mode)
        {
            if (mode == DepthMode.Undef)
                return null;
            return mode.ToString();
        }

        public static DepthMode ConvertStringToDepthMode(string modeStr)
        {
            DepthMode ret = DepthMode.Undef;
            if (modeStr == null)
                return ret;

            switch (modeStr)
            {
                case "Z":
                    ret = DepthMode.Z;
                    break;
                case "ZY":
                    ret = DepthMode.ZY;
                    break;

                case "SyncedZI":
                    ret = DepthMode.SyncedZI;
                    break;

                case "SyncedZC":
                    ret = DepthMode.SyncedZC;
                    break;

                case "PackedRaw":
                    ret = DepthMode.PackedRaw;
                    break;

                case "Calibration":
                    ret = DepthMode.Calibration;
                    break;
                case "I":
                    ret = DepthMode.I;
                    break;
                case "D16":
                    ret = DepthMode.D16;
                    break;
                case "YUY2":
                    ret = DepthMode.YUY2;
                    break;
                case "L8":
                    ret = DepthMode.L8;
                    break;
                case "G2036315A":
                    ret = DepthMode.G2036315A;
                    break;
                case "G20493859":
                    ret = DepthMode.G20493859;
                    break;
                case "G49323159":
                    ret = DepthMode.G49323159;
                    break;
                case "G59565955":
                    ret = DepthMode.G59565955;
                    break;
            }
            return ret;
        }


        #endregion

        #region FishEyeMode
        public enum FishEyeMode { G38574152, Undef };
        public static string ConvertFishEyeModeToString(FishEyeMode mode)
        {
            if (mode == FishEyeMode.Undef)
                return null;
            return mode.ToString();
        }
        public static FishEyeMode ConvertStringToFishEyeMode(string modeStr)
        {
            FishEyeMode ret = FishEyeMode.Undef;
            if (modeStr == null)
                return ret;

            switch (modeStr)
            {
                case "G38574152":
                    ret = FishEyeMode.G38574152;
                    break;
            }
            return ret;
        }
        #endregion

        #region Resolution
        public class Resolution
        {
            private int m_width = 0;
            private int m_height = 0;

            public Resolution() { }

            public Resolution(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public Resolution(Resolution other)
            {
                if (other == null)
                    return;
                this.m_height = other.Height;
                this.m_width = other.Width;
            }

            public Resolution(string width, string height)
            {
                ConvertFromStrings(width, height);
            }

            public Resolution(string resStr)
            {
                if (resStr == null)
                    return;
                resStr = resStr.Trim();
                if (resStr.IndexOf("Res_") == 0)
                    resStr = resStr.Substring(4);
                else if (resStr.IndexOf("ir") == 0)
                    resStr = resStr.Substring(2);

                if (resStr == null || resStr.Length == 0)
                    return;
                char[] delim = { 'x', 'X' };
                string[] values = resStr.Split(delim);
                if (values.Length == 2)
                    ConvertFromStrings(values[0], values[1]);
            }

            public int Width
            {
                get { return this.m_width; }
                set
                {
                    if (value >= 0)
                        this.m_width = value;
                }
            }

            public int Height
            {
                get { return this.m_height; }
                set
                {
                    if (value > 0)
                        this.m_height = value;
                }
            }

            public override string ToString()
            {
                return String.Format("{0}x{1}", this.m_width, this.m_height);
            }

            public bool Equal(Resolution other)
            {
                if (other == null)
                    return false;
                return (this.Width == other.Width) && (this.Height == other.Height);
            }

            private void ConvertFromStrings(string widthStr, string heightStr)
            {
                try
                {
                    Width = int.Parse(widthStr);
                    Height = int.Parse(heightStr);
                }
                catch
                {
                    this.m_height = 0;
                    this.m_width = 0;
                }
            }
        }
        #endregion

        #region Controls
        public enum ControlName
        {
            DepthAccuracy,
            DepthConfidenceThreshold,
            DepthPrivacy,
            DepthFilterOption,
            DepthFaceAuth,
            LaserPower,
            MotionVsRangeTradeoff,
            DepthMotionVsRangeTradeoff,
            Exposure,
            WhiteBalance,
            BacklightCompensation,
            Brightness,
            Contrast,
            Gamma,
            Hue,
            Saturation,
            Sharpness,
            Gain,
            ColorPrivacy,
            PowerlineFrequency,
            PriorityMode,
            DepthPreset,
            DepthExposure,
            DepthLaserPower,
            DS5_Depth_Exposure,
            DS5_Depth_Laser_Power,
            DS5_Depth_Laser_On_Off,
            DS5_Depth_Auto_White_Balance,
            DS5_Depth_Gain,
            FishEye_Exposure,
            FishEye_Gain,
            Undef
        }
        #endregion

        #region ControlKey
        public enum ControlKey
        {
            Undef, COLOR_EXPOSURE_PRIORITY, COLOR_POWER_LINE_FREQUENCY, COLOR_PRIVACY,
            COLOR_GAIN, COLOR_SHARPNESS, COLOR_SATURATION, COLOR_HUE, COLOR_GAMMA, COLOR_BRIGHTNESS, COLOR_CONTRAST,
            COLOR_BACK_LIGHT_COMPENSATION, COLOR_WHITE_BALANCE, COLOR_EXPOSURE, DEPTH_FILTER_OPTION, DEPTH_ACCURACY, DEPTH_PRIVACY,
            DEPTH_CONFIDENCE_THRESHOLD, DEPTH_LASER_POWER, DEPTH_MOTION_VS_RANGE_TRADE, DEPTH_PRESET, 
            DEPTH_EXPOSURE, DS5_DEPTH_LASER_ON_OFF, DS5_DEPTH_LASER_POWER, DS5_DEPTH_EXPOSURE, DS5_DEPTH_GAIN, DS5_DEPTH_AUTO_WHITE_BALANCE, MOTION_VS_RANGE_TRADE, FISHEYE_EXPOSURE, FISHEYE_GAIN, DEPTH_FACE_AUTH
        }
        public static List<ControlKey> GetAllControlKeys()
        {
            var ret = Enum.GetValues(typeof(ControlKey)).Cast<ControlKey>().ToList();
            return ret;
        }
        public static List<ControlName> GetAllControlNames()
        {
            var ret = Enum.GetValues(typeof(ControlName)).Cast<ControlName>().ToList();
            return ret;
        }
        public static ControlKey GetControlKeyByControlName(ControlName controlName)
        {
            switch (controlName)
            {
                case ControlName.DepthAccuracy: return ControlKey.DEPTH_ACCURACY;
                case ControlName.DepthPrivacy: return ControlKey.DEPTH_PRIVACY;
                case ControlName.DepthFilterOption: return ControlKey.DEPTH_FILTER_OPTION;
                case ControlName.MotionVsRangeTradeoff: return ControlKey.MOTION_VS_RANGE_TRADE;
                case ControlName.DepthMotionVsRangeTradeoff: return ControlKey.DEPTH_MOTION_VS_RANGE_TRADE;
                case ControlName.DepthFaceAuth: return ControlKey.DEPTH_FACE_AUTH;
                case ControlName.BacklightCompensation: return ControlKey.COLOR_BACK_LIGHT_COMPENSATION;
                case ControlName.Brightness: return ControlKey.COLOR_BRIGHTNESS;
                case ControlName.ColorPrivacy: return ControlKey.COLOR_PRIVACY;
                case ControlName.DepthConfidenceThreshold: return ControlKey.DEPTH_CONFIDENCE_THRESHOLD;
                case ControlName.Contrast: return ControlKey.COLOR_CONTRAST;
                case ControlName.Exposure: return ControlKey.COLOR_EXPOSURE;
                case ControlName.Gain: return ControlKey.COLOR_GAIN;
                case ControlName.Gamma: return ControlKey.COLOR_GAMMA;
                case ControlName.Hue: return ControlKey.COLOR_HUE;
                case ControlName.PowerlineFrequency: return ControlKey.COLOR_POWER_LINE_FREQUENCY;
                case ControlName.PriorityMode: return ControlKey.COLOR_EXPOSURE_PRIORITY;
                case ControlName.Saturation: return ControlKey.COLOR_SATURATION;
                case ControlName.Sharpness: return ControlKey.COLOR_SHARPNESS;
                case ControlName.WhiteBalance: return ControlKey.COLOR_WHITE_BALANCE;
                case ControlName.DepthPreset: return ControlKey.DEPTH_PRESET;
                case ControlName.DepthExposure: return ControlKey.DEPTH_EXPOSURE;
                case ControlName.DepthLaserPower: return ControlKey.DEPTH_LASER_POWER;
                case ControlName.DS5_Depth_Exposure: return ControlKey.DS5_DEPTH_EXPOSURE;
                case ControlName.DS5_Depth_Laser_On_Off: return ControlKey.DS5_DEPTH_LASER_ON_OFF;
                case ControlName.DS5_Depth_Laser_Power: return ControlKey.DS5_DEPTH_LASER_POWER;
                case ControlName.DS5_Depth_Gain: return ControlKey.DS5_DEPTH_GAIN;
                case ControlName.DS5_Depth_Auto_White_Balance: return ControlKey.DS5_DEPTH_AUTO_WHITE_BALANCE;
                case ControlName.FishEye_Exposure: return ControlKey.FISHEYE_EXPOSURE;
                case ControlName.FishEye_Gain: return ControlKey.FISHEYE_GAIN;

                default: return ControlKey.Undef;
            }
        }
        public static string ConvertControlKeyToString(ControlKey key)
        {
            if (key == ControlKey.Undef)
                return null;
            return key.ToString();
        }
        public static ControlKey ConvertStringToControlKey(string keyStr)
        {
            ControlKey ret = ControlKey.Undef;
            if (keyStr == null)
                return ret;

            switch (keyStr)
            {
                case "COLOR_EXPOSURE_PRIORITY":
                    ret = ControlKey.COLOR_EXPOSURE_PRIORITY;
                    break;

                case "COLOR_POWER_LINE_FREQUENCY":
                    ret = ControlKey.COLOR_POWER_LINE_FREQUENCY;
                    break;

                case "COLOR_PRIVACY":
                    ret = ControlKey.COLOR_PRIVACY;
                    break;

                case "COLOR_GAIN":
                    ret = ControlKey.COLOR_GAIN;
                    break;

                case "COLOR_SHARPNESS":
                    ret = ControlKey.COLOR_SHARPNESS;
                    break;

                case "COLOR_SATURATION":
                    ret = ControlKey.COLOR_SATURATION;
                    break;

                case "COLOR_HUE":
                    ret = ControlKey.COLOR_HUE;
                    break;

                case "COLOR_GAMMA":
                    ret = ControlKey.COLOR_GAMMA;
                    break;

                case "COLOR_CONTRAST":
                    ret = ControlKey.COLOR_CONTRAST;
                    break;

                case "COLOR_BRIGHTNESS":
                    ret = ControlKey.COLOR_BRIGHTNESS;
                    break;

                case "COLOR_BACK_LIGHT_COMPENSATION":
                    ret = ControlKey.COLOR_BACK_LIGHT_COMPENSATION;
                    break;

                case "COLOR_WHITE_BALANCE":
                    ret = ControlKey.COLOR_WHITE_BALANCE;
                    break;

                case "COLOR_EXPOSURE":
                    ret = ControlKey.COLOR_EXPOSURE;
                    break;

                case "DEPTH_FILTER_OPTION":
                    //throw new Exception("DEPTH_FILTER_OPTION types.cs");
                    ret = ControlKey.DEPTH_FILTER_OPTION;
                    break;

                case "DEPTH_PRIVACY":
                    ret = ControlKey.DEPTH_PRIVACY;
                    break;

                case "DEPTH_ACCURACY":
                    ret = ControlKey.DEPTH_ACCURACY;
                    break;

                case "DEPTH_LASER_POWER":
                    ret = ControlKey.DEPTH_LASER_POWER;
                    break;

                case "DEPTH_MOTION_VS_RANGE_TRADE":
                    ret = ControlKey.DEPTH_MOTION_VS_RANGE_TRADE;
                    break;

                case "DEPTH_CONFIDENCE_THRESHOLD":
                    //throw new Exception("DEPTH_CONFIDENCE_THRESHOLD types.cs");
                    ret = ControlKey.DEPTH_CONFIDENCE_THRESHOLD;
                    break;

                case "DEPTH_PRESET":
                    ret = ControlKey.DEPTH_PRESET;
                    break;

                case "DEPTH_EXPOSURE":
                    ret = ControlKey.DEPTH_EXPOSURE;
                    break;

                case "DEPTH_FACE_AUTH":
                    ret = ControlKey.DEPTH_FACE_AUTH;
                    break;

                case "DS5_DEPTH_EXPOSURE":
                    ret = ControlKey.DS5_DEPTH_EXPOSURE;
                    break;
                case "DS5_DEPTH_GAIN":
                    ret = ControlKey.DS5_DEPTH_GAIN;
                    break;
                case "DS5_DEPTH_AUTO_WHITE_BALANCE":
                    ret = ControlKey.DS5_DEPTH_AUTO_WHITE_BALANCE;
                    break;

                case "DS5_DEPTH_LASER_ON_OFF":
                    ret = ControlKey.DS5_DEPTH_LASER_ON_OFF;
                    break;
                case "DS5_DEPTH_LASER_POWER":
                    ret = ControlKey.DS5_DEPTH_LASER_POWER;
                    break;
                case "FISHEYE_EXPOSURE":
                    ret = ControlKey.FISHEYE_EXPOSURE;
                    break;
                case "FISHEYE_GAIN":
                    ret = ControlKey.FISHEYE_GAIN;
                    break;

                default:
                    throw new Exception("Unknown control key: " + keyStr);
            }

            return ret;
        }


        public static ControlName ConvertStringToControlName(string Str)
        {
            ControlName ret = ControlName.Undef;
            if (Str == null)
                return ret;

            switch (Str)
            {
                case "Accuracy":
                    ret = ControlName.DepthAccuracy;
                    break;

                case "ConfidenceThreshold":
                    ret = ControlName.DepthConfidenceThreshold;
                    break;

                case "DepthPrivacy":
                    ret = ControlName.DepthPrivacy;
                    break;

                case "COFilterOptionLOR_GAIN":
                    ret = ControlName.DepthFilterOption;
                    break;

                case "LaserPower":
                    ret = ControlName.LaserPower;
                    break;

                case "MotionVsRangeTradeoff":
                    ret = ControlName.MotionVsRangeTradeoff;
                    break;

                case "DepthMotionVsRangeTradeoff":
                    ret = ControlName.DepthMotionVsRangeTradeoff;
                    break;

                case "Exposure":
                    ret = ControlName.Exposure;
                    break;

                case "WhiteBalance":
                    ret = ControlName.WhiteBalance;
                    break;

                case "BacklightCompensation":
                    ret = ControlName.BacklightCompensation;
                    break;

                case "Brightness":
                    ret = ControlName.Brightness;
                    break;

                case "Contrast":
                    ret = ControlName.Contrast;
                    break;

                case "Gamma":
                    ret = ControlName.Gamma;
                    break;

                case "Hue":
                    ret = ControlName.Hue;
                    break;

                case "Saturation":
                    ret = ControlName.Saturation;
                    break;

                case "Sharpness":
                    ret = ControlName.Sharpness;
                    break;

                case "Gain":
                    ret = ControlName.Gain;
                    break;

                case "ColorPrivacy":
                    ret = ControlName.ColorPrivacy;
                    break;

                case "PowerlineFrequency":
                    ret = ControlName.PowerlineFrequency;
                    break;
                case "PriorityMode":
                    ret = ControlName.PriorityMode;
                    break;
            }

            return ret;
        }


        #endregion
        public enum ExtValue { Min, Max, NotSet }

        #region HDRConfigurationKey
        public enum HDRConfigurationKey { Undef, SR_NumOfFrames, SR_MVR, SR_FilterOption, SR_ConfidenceThreashold, LR_NumOfFrames, LR_MVR, LR_FilterOption, LR_ConfidenceThreashold }
        public enum HDRFrameType { SR_Frame, LR_Frame, Unknown };
        public static List<HDRConfigurationKey> GetAllHDRConfigurationKeys()
        {
            HDRConfigurationKey[] ret = new HDRConfigurationKey[] { HDRConfigurationKey.SR_NumOfFrames, HDRConfigurationKey.SR_MVR, HDRConfigurationKey.SR_FilterOption, HDRConfigurationKey.SR_ConfidenceThreashold, HDRConfigurationKey.LR_NumOfFrames, HDRConfigurationKey.LR_MVR, HDRConfigurationKey.LR_FilterOption, HDRConfigurationKey.LR_ConfidenceThreashold };
            return ret.ToList<HDRConfigurationKey>();
        }

        public static string ConvertHDRConfigurationKeyToString(HDRConfigurationKey key)
        {
            if (key == HDRConfigurationKey.Undef)
                return null;
            return key.ToString();
        }

        public static HDRConfigurationKey ConvertStringToHDRConfigurationKey(string keyStr)
        {
            HDRConfigurationKey ret = HDRConfigurationKey.Undef;
            if (keyStr == null)
                return ret;

            switch (keyStr)
            {
                case "SR_NumOfFrames":
                    ret = HDRConfigurationKey.SR_NumOfFrames;
                    break;

                case "SR_MVR":
                    ret = HDRConfigurationKey.SR_MVR;
                    break;

                case "SR_FilterOption":
                    ret = HDRConfigurationKey.SR_FilterOption;
                    break;

                case "SR_ConfidenceThreashold":
                    ret = HDRConfigurationKey.SR_ConfidenceThreashold;
                    break;

                case "LR_NumOfFrames":
                    ret = HDRConfigurationKey.LR_NumOfFrames;
                    break;

                case "LR_MVR":
                    ret = HDRConfigurationKey.LR_MVR;
                    break;

                case "LR_FilterOption":
                    ret = HDRConfigurationKey.LR_FilterOption;
                    break;

                case "LR_ConfidenceThreashold":
                    ret = HDRConfigurationKey.LR_ConfidenceThreashold;
                    break;
            }

            return ret;
        }
        #endregion

        #region SubArea
        public enum SubArea
        {
            Undef,
            //ResetTest,
            //DepthFpsTest,
            //ColorFpsTest,
            //SyncFpsTest,
            //SequencesDepthTest,
            //SettingControlsTest,
            //PlayStopTest,
            //LongFpsTest,
            BasicColorTest,
            BasicDepthTest,
            BasicFishEyeTest,
            PropertiesTest,
            StabilityTest,
            PropertiesWhileStreamingTest,
            SanityParallelStreamTest,
            WinHelloTest,
            SensorGroupSanityTest,
            CheckDefaultControlValuesTest,
            IMUTemperatureMeasurementWithoutStreamingTest,
            IMUTemperatureMeasurementWhileStreamingTest,
            ThermalAlarmWhileStreamingTest,
            ThermalAlarmWithoutStreamingTest,
            ErrorSimulationTest,
            PNP_Test,
            PNP_Test_Multi,
            SyncHW_Test,
            //ChangingAccuracyWhileStreaming,
            //ChangingConfidenceThresholdWhileStreaming,
            //ChangingDepthPrivacyWhileStreaming,
            //ChangingFilterOptionWhileStreaming,
            //ChangingLaserPowerWhileStreaming,
            //ChangingMotionVsRangeTradeoffWhileStreaming,

            //ChangingExposureWhileStreaming,
            //ChangingWhiteBalanceWhileStreaming,
            //ChangingBacklightCompensationWhileStreaming,
            //ChangingBrightnessWhileStreaming,
            //ChangingContrastWhileStreaming,
            //ChangingGammaWhileStreaming,
            //ChangingHueWhileStreaming,
            //ChangingSaturationWhileStreaming,
            //ChangingSharpnessWhileStreaming,
            //ChangingGainWhileStreaming,
            //ChangingColorPrivacyWhileStreaming,
            //ChangingPowerlineFrequencyWhileStreaming,
            //ChangingPriorityModeWhileStreaming,

            //DepthControlsEffectTest,
            //CheckingAutoCapableProperties,
            //RegistersTest,
            //OemPropertiesValidation,
            //CommandsTest,

            ////HDR
            //HDREffectOnFramesDeltaTest
        }
        public static string ConvertSubAreaToString(SubArea subArea)
        {
            if (subArea == SubArea.Undef)
                return null;
            return subArea.ToString();
        }
        public static SubArea ConvertStringToSubArea(string subAreaStr)
        {
            SubArea ret = SubArea.Undef;
            if (subAreaStr == null)
                return ret;

            switch (subAreaStr)
            {
                case "BasicColorTest": return SubArea.BasicColorTest;
                case "BasicDepthTest": return SubArea.BasicDepthTest;
                case "WinHelloTest": return SubArea.WinHelloTest;
                case "BasicFishEye": return SubArea.BasicFishEyeTest;
                case "PropertiesTest": return SubArea.PropertiesTest;
                case "StabilityTest": return SubArea.StabilityTest;
                case "SanityParallelStreamTest": return SubArea.SanityParallelStreamTest;
                case "SensorGroupSanityTest": return SubArea.SensorGroupSanityTest;
                //case "ColorFpsTest":                                return SubArea.ColorFpsTest;
                //case "DepthFpsTest":                                return SubArea.DepthFpsTest;
                //case "SyncFpsTest":                                 return SubArea.SyncFpsTest;
                //case "SequencesDepthTest":                          return SubArea.SequencesDepthTest;
                //case "SettingControlsTest":                         return SubArea.SettingControlsTest;

                //case "LongFpsTest":                                 return SubArea.LongFpsTest;
                //case "PlayStopTest":                                return SubArea.PlayStopTest;
                //case "ChangingAccuracyWhileStreaming":              return SubArea.ChangingAccuracyWhileStreaming;
                //case "ChangingConfidenceThresholdWhileStreaming":   return SubArea.ChangingConfidenceThresholdWhileStreaming;
                //case "ChangingDepthPrivacyWhileStreaming":          return SubArea.ChangingDepthPrivacyWhileStreaming;
                //case "ChangingFilterOptionWhileStreaming":          return SubArea.ChangingFilterOptionWhileStreaming;
                //case "ChangingLaserPowerWhileStreaming":            return SubArea.ChangingLaserPowerWhileStreaming;
                //case "ChangingMotionVsRangeTradeoffWhileStreaming": return SubArea.ChangingMotionVsRangeTradeoffWhileStreaming;

                //case "ChangingExposureWhileStreaming":              return SubArea.ChangingExposureWhileStreaming;
                //case "ChangingWhiteBalanceWhileStreaming":          return SubArea.ChangingWhiteBalanceWhileStreaming;
                //case "ChangingBacklightCompensationWhileStreaming": return SubArea.ChangingBacklightCompensationWhileStreaming;
                //case "ChangingBrightnessWhileStreaming":            return SubArea.ChangingBrightnessWhileStreaming;
                //case "ChangingContrastWhileStreaming":              return SubArea.ChangingContrastWhileStreaming;
                //case "ChangingGammaWhileStreaming":                 return SubArea.ChangingGammaWhileStreaming;
                //case "ChangingHueWhileStreaming":                   return SubArea.ChangingHueWhileStreaming;
                //case "ChangingSaturationWhileStreaming":            return SubArea.ChangingSaturationWhileStreaming;
                //case "ChangingSharpnessWhileStreaming":             return SubArea.ChangingSharpnessWhileStreaming;
                //case "ChangingGainWhileStreaming":                  return SubArea.ChangingGainWhileStreaming;
                //case "ChangingColorPrivacyWhileStreaming":          return SubArea.ChangingColorPrivacyWhileStreaming;
                //case "ChangingPowerlineFrequencyWhileStreaming":    return SubArea.ChangingPowerlineFrequencyWhileStreaming;
                //case "ChangingPriorityModeWhileStreaming":          return SubArea.ChangingPriorityModeWhileStreaming;
                //case "CheckingAutoCapableProperties":               return SubArea.CheckingAutoCapableProperties;
                //case "DepthControlsEffectTest":                     return SubArea.DepthControlsEffectTest;
                //case "PresetsRegistersVerification":                return SubArea.RegistersTest;
                //case "OemPropertiesValidation":                     return SubArea.OemPropertiesValidation;
                //case "CommandsTest":                                return SubArea.CommandsTest;
                //case "HDREffectOnFramesDeltaTest":                  return SubArea.HDREffectOnFramesDeltaTest;

                default: return SubArea.Undef;
            }
        }
        #endregion

        #region Status
        public enum Status { Do, Skip }
        public enum TestStatus { NoSelectedTests, Passed, Failed, Stopped, UnExpectedStatus, InProgress }

        #endregion

        #region Pair
        public class Pair<K, V>
        {
            private K key;
            private V val;
            public Pair() { }

            public Pair(K k, V v)
            {
                key = k;
                val = v;
            }

            public K Key
            {
                get { return key; }
                set { key = value; }
            }

            public V Value
            {
                get { return val; }
                set { val = value; }
            }

            public override string ToString()
            {
                return String.Format("{0} = {1}", key, val);
            }
        }
        #endregion
    }
}