using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;

//using Winusb.Cli;
//using UVC.Cli;

namespace QAFrameServerValidator
{
    public static class SetControl
    {
        public static bool testPassed;
        public static int MILLISECONDS_TO_SLEEP_BETWEEN_SET = 50;

        #region constants
        //Extention unit property IDs:
        const string RS300FaceAuthPropID = "{1CB79112-C0D2-4213-9CA6-CD4FDB927972} 35";

        const string DS5ExposurePropID = "{C9606CCB-594C-4D25-AF47-CCC496435995} 3";
        public const double DS5ExposureMin = 20;// 1;
        public const double DS5ExposureMax = 166000;// 1660;
        public const double DS5ExposureStep = 20;//1;
        public const double DS5ExposureDefaultValue = 20;//330;


        const string DS5LaserPowerPropID = "{C9606CCB-594C-4D25-AF47-CCC496435995} 4";
        public const double DS5LaserPowerMin = 0;
        public const double DS5LaserPowerMax = 360;
        public const double DS5LaserPowerStep = 30;
        public const double DS5LaserPowerDefaultValue = 150;

        const string DS5LaserOnOffPropID = "{C9606CCB-594C-4D25-AF47-CCC496435995} 2";
        public const double DS5LaserOnOffMin = 0;
        public const double DS5LaserOnOffMax = 2;
        public const double DS5LaserOnOffStep = 1;
        public const double DS5LaserOnOffDefaultValue = 2;

        const int DS5GainMin = 16;
        const int DS5GainMax = 248;
        const int DS5GainStep = 1;
        public const int DS5GainDefaultValue = 16;

        const string DS5AutoWhiteBalancePropID = "{C9606CCB-594C-4D25-AF47-CCC496435995} 10";
        const int DS5AutoWhiteBalanceMin = 0;
        const int DS5AutoWhiteBalanceMax = 1;
        const int DS5AutoWhiteBalanceStep = 1;
        public const int DS5AutoWhiteBalanceDefaultValue = 1;

        public const double SR300ColorExposureMin = -8;
        public const double SR300ColorExposureMax = 0;
        public const double SR300ColorExposureStep = 1;
        public const double SR300ColorExposureDefaultValue = -6;


        public const double SR300ColorBrightnessMin = -64;
        public const double SR300ColorBrightnessMax = 64;
        public const double SR300ColorBrightnessStep = 1;
        public const double SR300ColorBrightnessDefaultValue = 64;

        public const double SR300ColorContrastMin = 0;
        public const double SR300ColorContrastMax = 100;
        public const double SR300ColorContrastStep = 1;
        public const double SR300ColorContrastDefaultValue = 45;

        public const double SR300ColorHueMin = -180;
        public const double SR300ColorHueMax = 180;
        public const double SR300ColorHueStep = 1;
        public const double SR300ColorHueDefaultValue = 0;

        public const double SR300ColorWhiteBalanceMin = 2800;
        public const double SR300ColorWhiteBalanceMax = 6500;
        public const double SR300ColorWhiteBalanceStep = 10;
        public const double SR300ColorWhiteBalanceDefaultValue = 4600;

        public const double SR300ColorBacklightCompensationMin = 0;
        public const double SR300ColorBacklightCompensationMax = 1;
        public const double SR300ColorBacklightCompensationStep = 1;
        public const double SR300ColorBacklightCompensationDefaultValue = 0;

        const string SR300ColorExposurePriorityPropID = "{B8EC416E-A3AC-4580-8D5C-0BEE1597E43D} 1";
        public const double SR300ColorExposurePriorityMin = 0;
        public const double SR300ColorExposurePriorityMax = 1;
        public const double SR300ColorExposurePriorityStep = 1;
        public const double SR300ColorExposurePriorityDefaultValue = 0;

        const string SR300ColorPowerLineFrequencyPropID = "{B8EC416E-A3AC-4580-8D5C-0BEE1597E43D} 2";
        public const double SR300ColorPowerLineFrequencyMin = 0;
        public const double SR300ColorPowerLineFrequencyMax = 3;
        public const double SR300ColorPowerLineFrequencyStep = 1;
        public const double SR300ColorPowerLineFrequencyDefaultValue = 3;

        public const int SR300ColorGammaMin = 100;
        public const int SR300ColorGammaMax = 500;
        public const int SR300ColorGammaStep = 1;
        public const int SR300ColorGammaDefaultValue = 300;

        public const int SR300ColorSaturationMin = 0;
        public const int SR300ColorSaturationMax = 100;
        public const int SR300ColorSaturationStep = 1;
        public const int SR300ColorSaturationDefaultValue = 64;

        public const int SR300ColorGainMin = 0;
        public const int SR300ColorGainMax = 128;
        public const int SR300ColorGainStep = 1;
        public const int SR300ColorGainDefaultValue = 64;

        public const int SR300ColorPrivacyMin = 0;
        public const int SR300ColorPrivacyMax = 1;
        public const int SR300ColorPrivacyStep = 1;
        public const int SR300ColorPrivacyDefaultValue = 0;

        public const int SR300DepthPrivacyMin = 0;
        public const int SR300DepthPrivacyMax = 1;
        public const int SR300DepthPrivacyStep = 1;
        public const int SR300DepthPrivacyDefaultValue = 0;

        public const int SR300DepthGainMin = 0;
        public const int SR300DepthGainMax = 1;
        public const int SR300DepthGainStep = 1;
        public const int SR300DepthGainDefaultValue = 0;

        public const int SR300ColorSharpnessMin = 0;
        public const int SR300ColorSharpnessMax = 100;
        public const int SR300ColorSharpnessStep = 1;
        public const int SR300ColorSharpnessDefaultValue = 50;

        const string SR300DepthMotionVSRangeTradeoffPropID = "{A55751A1-F3C5-4A5E-8D5A-6854B8FA2716} 3";
        public const double SR300DepthMotionVSRangeTradeoffMin = 0;
        public const double SR300DepthMotionVSRangeTradeoffMax = 220;
        public const double SR300DepthMotionVSRangeTradeoffStep = 1;
        public const double SR300DepthMotionVSRangeTradeoffDefaultValue = 9;

        const string SR300DepthLaserPowerPropID = "{A55751A1-F3C5-4A5E-8D5A-6854B8FA2716} 1";
        public const double SR300DepthLaserPowerMin = 0;
        public const double SR300DepthLaserPowerMax = 16;
        public const double SR300DepthLaserPowerStep = 1;
        public const double SR300DepthLaserPowerDefaultValue = 16;

        const string SR300DepthAccuracyPropID = "{A55751A1-F3C5-4A5E-8D5A-6854B8FA2716} 2";
        public const double SR300DepthAccuracyMin = 1;
        public const double SR300DepthAccuracyMax = 3;
        public const double SR300DepthAccuracyStep = 1;
        public const double SR300DepthAccuracyDefaultValue = 1;

        const string SR300DepthFilterOptionPropID = "{A55751A1-F3C5-4A5E-8D5A-6854B8FA2716} 5";
        public const double SR300DepthFilterOptionMin = 0;
        public const double SR300DepthFilterOptionMax = 7;
        public const double SR300DepthFilterOptionStep = 1;
        public const double SR300DepthFilterOptionDefaultValue = 5;

        const string SR300DepthConfidenceThresholdPropID = "{A55751A1-F3C5-4A5E-8D5A-6854B8FA2716} 6";
        public const double SR300DepthConfidenceThresholdMin = 0;
        public const double SR300DepthConfidenceThresholdMax = 15;
        public const double SR300DepthConfidenceThresholdStep = 1;
        public const double SR300DepthConfidenceThresholdDefaultValue = 3;

        const string SR300DepthPresetPropID = "{F84AB655-8931-42BD-8C04-8AE2881454FE} 1";
        const string SR300DepthPresetControlHWCommand = "{F84AB655-8931-42BD-8C04-8AE2881454FE} 3";


        const string FishEyeExposurePropID = "{F6C3C3D1-5CDE-4477-ADF0-4133F58DA6F4} 1";
        public const int FishEyeExposureMin = 20;
        public const int FishEyeExposureMax = 32000;
        public const int FishEyeExposureStep = 1;
        public const int FishEyeExposureDefaultValue = 20;
              
        const string FishEyeGainPropID = "{F6C3C3D1-5CDE-4477-ADF0-4133F58DA6F4} 4";
        public const int FishEyeGainMin = 0;
        public const int FishEyeGainMax = 255;
        public const int FishEyeGainStep = 1;
        public const int FishEyeGainDefaultValue = 10;
        
        #endregion

        static Dictionary<Types.ControlKey, double> controls;
        private static Windows.Media.Devices.VideoDeviceController colorVideoDeviceConroller    = null;
        private static Windows.Media.Devices.VideoDeviceController depthVideoDeviceController   = null;
        private static Windows.Media.Devices.VideoDeviceController fishEyeVideoDeviceController = null;

        private static  bool autoDS5DepthExposureSet = false;

        private static bool isPropertiesWhileStreamingTestRunning = false;

        //Call this function before using this class. If you are only using this class to set controls to specific values, and not from min to max, then call the other setup class, which does not require List<TestSetup.ControlValues>
        public static void setup(List<TestSetup.ControlValues> controlsList, Windows.Media.Capture.MediaCapture colorMediaCapture, Windows.Media.Capture.MediaCapture depthMediaCapture, Windows.Media.Capture.MediaCapture fishEyeMediaCapture)
        {
            colorVideoDeviceConroller = colorMediaCapture?.VideoDeviceController;
            depthVideoDeviceController = depthMediaCapture?.VideoDeviceController;
            fishEyeVideoDeviceController = fishEyeMediaCapture?.VideoDeviceController;
            controls = new Dictionary<Types.ControlKey, double>();
            for (int i = 0; i < controlsList.Count(); i++)
            {
                controls.Add(controlsList[i].Control, GetControlValue(controlsList[i].Control));
            }
        }
        //Call this function when running propertieswhilestreaming.
        public static void setup(List<TestSetup.ControlValues> controlsList, Windows.Media.Capture.MediaCapture colorMediaCapture, Windows.Media.Capture.MediaCapture depthMediaCapture, Windows.Media.Capture.MediaCapture fishEyeMediaCapture, uint fps_)
        {
            isPropertiesWhileStreamingTestRunning = true;
            setup(controlsList, colorMediaCapture, depthMediaCapture, fishEyeMediaCapture);
            //Used for calculating sleep between set time. In the propertieswhilestreaming test, the time between sets is dependent on the FPS, allowing for calculation of frames with specific properties.
            MILLISECONDS_TO_SLEEP_BETWEEN_SET = (int)(double)calculateMillisecondsToSleepBetweenSets(fps_); // divide by 2 because every iteration we do a set and a get
        }
        public static int calculateMillisecondsToSleepBetweenSets (uint fps_)
        {
            return (int)((50.0 / (double)fps_) * 1000.0);
        }

        //Call this function before using this class. If you are only using this class to set controls to specific values, and not from min to max, then call the other setup class, which does not require List<TestSetup.ControlValues>
        public static void setup(Windows.Media.Capture.MediaCapture colorMediaCapture, Windows.Media.Capture.MediaCapture depthMediaCapture, Windows.Media.Capture.MediaCapture fishEyeMediaCapture)
        {           
            if (colorMediaCapture != null)
            {
                colorVideoDeviceConroller = colorMediaCapture?.VideoDeviceController;
                depthVideoDeviceController = null;
                fishEyeVideoDeviceController = null;
            }
            else if (depthMediaCapture != null)
            {
                depthVideoDeviceController = depthMediaCapture?.VideoDeviceController;
                colorVideoDeviceConroller = null;
                fishEyeVideoDeviceController = null;
            }
            else if (fishEyeMediaCapture != null)
            {
                fishEyeVideoDeviceController = fishEyeMediaCapture?.VideoDeviceController;
                depthVideoDeviceController = null;
                colorVideoDeviceConroller = null;
            }
        }

        public static void setPropertyToSpecificValue(Types.ControlKey control, int value)
        {
            Console.WriteLine($"Set control: {control}, Value: {value}");
            setPropertyRange(control, true, value);
        }
        //public static void setPropertyRange(CameraTypes cameraType, Types.ControlKey control, double min, double max, double step, double defaultValue, Windows.Media.Devices.VideoDeviceController colorVideoDeviceConroller, Windows.Media.Devices.VideoDeviceController depthVideoDeviceConroller, bool setToSpecificValue)
        public static void setPropertyRange(Types.ControlKey control, bool setToSpecificValue, int value)
        {
            switch (control)
            {
                case Types.ControlKey.DS5_DEPTH_EXPOSURE:
                    {
                        //depthVideoDeviceController.Exposure.TrySetAuto(false);
                        //VideoProcAmp v = new VideoProcAmp((uint)CameraControl_ControlValues.EXPOSURE, depthVideoDeviceController);
                        //if (setToSpecificValue)
                        //{
                        //    v.Set(value);
                        //}
                        //else
                        //{
                        //    v.SetRange((int)DS5ExposureMin, (int)DS5ExposureMax, (int)DS5ExposureStep, (int)DS5ExposureDefaultValue, Types.ControlKey.DS5_DEPTH_EXPOSURE, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        //}
                        //SetValueViaMicrosoftAPI(depthVideoDeviceController.Exposure, Types.ControlKey.DS5_DEPTH_EXPOSURE, DS5ExposureMin, DS5ExposureMax, DS5ExposureStep, DS5ExposureDefaultValue, false);

                        Logger.AppendDebug("Inside DS5 set Exposure");

                        if (!autoDS5DepthExposureSet)
                        {
                            byte[] setValue = BitConverter.GetBytes(0);
                            object val = PropertyValue.CreateUInt8Array(setValue);
                            depthVideoDeviceController.SetDeviceProperty("{C9606CCB-594C-4D25-AF47-CCC496435995} 11", val);
                            autoDS5DepthExposureSet = true;
                        }
                        changeValueViaXU(DS5ExposurePropID, Types.ControlKey.DS5_DEPTH_EXPOSURE, setToSpecificValue ? value : DS5ExposureMin, setToSpecificValue ? value : DS5ExposureMax, DS5ExposureStep, setToSpecificValue ? value : DS5ExposureDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_LASER_ON_OFF:
                    {
                        changeValueViaXU(DS5LaserOnOffPropID, Types.ControlKey.DS5_DEPTH_LASER_ON_OFF, setToSpecificValue ? value : DS5LaserOnOffMin, setToSpecificValue ? value : DS5LaserOnOffMax, DS5LaserOnOffStep, setToSpecificValue ? value : DS5LaserOnOffDefaultValue, true, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_LASER_POWER:
                    {
                        changeValueViaXU(DS5LaserPowerPropID, Types.ControlKey.DS5_DEPTH_LASER_POWER, setToSpecificValue ? value : DS5LaserPowerMin, setToSpecificValue ? value : DS5LaserPowerMax, DS5LaserPowerStep, setToSpecificValue ? value : DS5LaserPowerDefaultValue, true, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_GAIN:
                    {
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, depthVideoDeviceController);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                            v.SetRange(DS5GainMin, DS5GainMax, DS5GainStep, DS5GainDefaultValue, Types.ControlKey.DS5_DEPTH_GAIN, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }

                        ////SetPropertyViaMissingProperties(SR300ColorGainMin, SR300ColorGainMax, SR300ColorGainStep, SR300ColorGainDefaultValue, Types.ControlKey.COLOR_GAIN);
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_AUTO_WHITE_BALANCE:
                    {
                        changeValueViaXU(DS5AutoWhiteBalancePropID, Types.ControlKey.DS5_DEPTH_AUTO_WHITE_BALANCE, setToSpecificValue ? value : DS5AutoWhiteBalanceMin, setToSpecificValue ? value : DS5AutoWhiteBalanceMax, DS5AutoWhiteBalanceStep, setToSpecificValue ? value : DS5AutoWhiteBalanceDefaultValue, true, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_EXPOSURE:
                    {
                        Console.WriteLine("Is color exposure supported: "+ colorVideoDeviceConroller.Exposure.Capabilities.Supported);
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller?.Exposure, Types.ControlKey.COLOR_EXPOSURE, setToSpecificValue ? value : SR300ColorExposureMin, setToSpecificValue ? value : SR300ColorExposureMax, SR300ColorExposureStep, setToSpecificValue ? value : SR300ColorExposureDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_BRIGHTNESS:
                    {
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller.Brightness, Types.ControlKey.COLOR_BRIGHTNESS, setToSpecificValue ? value : SR300ColorBrightnessMin, setToSpecificValue ? value : SR300ColorBrightnessMax, SR300ColorBrightnessStep, setToSpecificValue ? value : SR300ColorBrightnessDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_CONTRAST:
                    {
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller.Contrast, Types.ControlKey.COLOR_CONTRAST, setToSpecificValue ? value : SR300ColorContrastMin, setToSpecificValue ? value : SR300ColorContrastMax, SR300ColorContrastStep, setToSpecificValue ? value : SR300ColorContrastDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_HUE:
                    {
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller.Hue, Types.ControlKey.COLOR_HUE, setToSpecificValue ? value : SR300ColorHueMin, setToSpecificValue ? value : SR300ColorHueMax, SR300ColorHueStep, setToSpecificValue ? value : SR300ColorHueDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_WHITE_BALANCE:
                    {
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller.WhiteBalance, Types.ControlKey.COLOR_WHITE_BALANCE, setToSpecificValue ? value : SR300ColorWhiteBalanceMin, setToSpecificValue ? value : SR300ColorWhiteBalanceMax, SR300ColorWhiteBalanceStep, setToSpecificValue ? value : SR300ColorWhiteBalanceDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION:
                    {
                        SetValueViaMicrosoftAPI(colorVideoDeviceConroller.BacklightCompensation, Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION, setToSpecificValue ? value : SR300ColorBacklightCompensationMin, setToSpecificValue ? value : SR300ColorBacklightCompensationMax, SR300ColorBacklightCompensationStep, setToSpecificValue ? value : SR300ColorBacklightCompensationDefaultValue, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_EXPOSURE_PRIORITY:
                    {

                        CameraControl exposurePriority = new CameraControl(19, colorVideoDeviceConroller);
                        var expPriorityValue = exposurePriority.Get();
                        var newExposurePriorityValue = value;
                        //FW will return an error if you set the same value twice 
                        try
                        {
                            if (expPriorityValue != newExposurePriorityValue)
                                exposurePriority.Set(newExposurePriorityValue);
                        }
                        catch(Exception e)
                        {
                            Logger.AppendError(e.Message);
                            Console.WriteLine(e.Message);
                        }
                        expPriorityValue = exposurePriority.Get();
                        if (expPriorityValue != newExposurePriorityValue)
                        {
                            Logger.AppendError("failed to set exposure priority to :" + newExposurePriorityValue);
                            Console.WriteLine("failed to set exposure priority to :" + newExposurePriorityValue);
                        }
                        else
                        {
                            Logger.AppendDebug("success to set exposure priority to :" + newExposurePriorityValue);
                            Console.WriteLine("success to set exposure priority to :" + newExposurePriorityValue);
                        }
                        //DS5
                        //var pvalue = colorVideoDeviceConroller.ExposurePriorityVideoControl.Supported;
                        //try
                        //{
                        //    colorVideoDeviceConroller.ExposurePriorityVideoControl.Enabled = false;
                        //}
                        //catch(Exception e)
                        //{
                        //    Console.WriteLine(e.Message);

                        //}
                        //if (pvalue)
                        //{
                        //    Console.WriteLine("Success: Get value COLOR_EXPOSURE_PRIORITY ,value: " + pvalue);
                        //}
                        //else
                        //{
                        //    Console.WriteLine("COLOR_EXPOSURE_PRIORITY ,supported: " + pvalue);
                        //}
                        //SR300
                        //changeValueViaXU(SR300ColorExposurePriorityPropID, Types.ControlKey.COLOR_EXPOSURE_PRIORITY, setToSpecificValue ? value : SR300ColorExposurePriorityMin, setToSpecificValue ? value : SR300ColorExposurePriorityMax, SR300ColorExposurePriorityStep, setToSpecificValue ? value : SR300ColorExposurePriorityDefaultValue, false, colorVideoDeviceConroller, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.COLOR_POWER_LINE_FREQUENCY:
                    {
                        //DS5
                        SetPowerlineFrequencyViaMicrosoftAPI(setToSpecificValue ? value : (int)Windows.Media.Capture.PowerlineFrequency.Disabled, setToSpecificValue ? value : (int)Windows.Media.Capture.PowerlineFrequency.Auto, 1, setToSpecificValue ? value : 3, setToSpecificValue);

                        //colorVideoDeviceConroller.TrySetPowerlineFrequency(Windows.Media.Capture.PowerlineFrequency.FiftyHertz);
                        //Windows.Media.Capture.PowerlineFrequency valuef = (Windows.Media.Capture.PowerlineFrequency)1;
                        //colorVideoDeviceConroller.TryGetPowerlineFrequency(out valuef);

                        //SR300
                        //SetValueViaMicrosoftAPI(colorVideoDeviceConroller., Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION, setToSpecificValue ? value : SR300ColorBacklightCompensationMin, setToSpecificValue ? value : SR300ColorBacklightCompensationMax, SR300ColorBacklightCompensationStep, setToSpecificValue ? value : SR300ColorBacklightCompensationDefaultValue, setToSpecificValue);
                        //changeValueViaXU(SR300ColorPowerLineFrequencyPropID, Types.ControlKey.COLOR_POWER_LINE_FREQUENCY, setToSpecificValue ? value : SR300ColorPowerLineFrequencyMin, setToSpecificValue ? value : SR300ColorPowerLineFrequencyMax, SR300ColorPowerLineFrequencyStep, setToSpecificValue ? value : SR300ColorPowerLineFrequencyDefaultValue, false, colorVideoDeviceConroller, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE:
                    {
                        changeValueViaXU(SR300DepthMotionVSRangeTradeoffPropID, Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE, setToSpecificValue ? value : SR300DepthMotionVSRangeTradeoffMin, setToSpecificValue ? value : SR300DepthMotionVSRangeTradeoffMax, SR300DepthMotionVSRangeTradeoffStep, setToSpecificValue ? value : SR300DepthMotionVSRangeTradeoffDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_LASER_POWER:
                    {
                        changeValueViaXU(SR300DepthLaserPowerPropID, Types.ControlKey.DEPTH_LASER_POWER, setToSpecificValue ? value : SR300DepthLaserPowerMin, setToSpecificValue ? value : SR300DepthLaserPowerMax, SR300DepthLaserPowerStep, setToSpecificValue ? value : SR300DepthLaserPowerDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_ACCURACY:
                    {
                        changeValueViaXU(SR300DepthAccuracyPropID, Types.ControlKey.DEPTH_ACCURACY, setToSpecificValue ? value : SR300DepthAccuracyMin, setToSpecificValue ? value : SR300DepthAccuracyMax, SR300DepthAccuracyStep, setToSpecificValue ? value : SR300DepthAccuracyDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_FILTER_OPTION:
                    {
                        changeValueViaXU(SR300DepthFilterOptionPropID, Types.ControlKey.DEPTH_FILTER_OPTION, setToSpecificValue ? value : SR300DepthFilterOptionMin, setToSpecificValue ? value : SR300DepthFilterOptionMax, SR300DepthFilterOptionStep, setToSpecificValue ? value : SR300DepthFilterOptionDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD:
                    {
                        changeValueViaXU(SR300DepthConfidenceThresholdPropID, Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD, setToSpecificValue ? value : SR300DepthConfidenceThresholdMin, setToSpecificValue ? value : SR300DepthConfidenceThresholdMax, SR300DepthConfidenceThresholdStep, setToSpecificValue ? value : SR300DepthConfidenceThresholdDefaultValue, false, depthVideoDeviceController, setToSpecificValue);
                        break;
                    }
                case Types.ControlKey.DEPTH_FACE_AUTH:
                    {
                        SetFaceAuthControl();
                        break;
                    }
                case Types.ControlKey.FISHEYE_EXPOSURE:
                    {                        
                        Console.WriteLine("=====>>>>>>> setting Control/exposure " + value);
                        TimeSpan ts = new TimeSpan((long)value*10);

                        Task.Run(async () =>
                        {
                            await fishEyeVideoDeviceController.ExposureControl.SetValueAsync(ts);
                        }).GetAwaiter().GetResult();
                        break;
                    }
                case Types.ControlKey.FISHEYE_GAIN:
                    {
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, fishEyeVideoDeviceController);
                        if (setToSpecificValue)
                        {
                            Console.WriteLine("========> set ficheye value : "+ value);
                            v.Set(value);
                            Console.WriteLine("========> get ficheye value : " + v.Get());
                        }
                        else
                        {
                            v.SetRange(FishEyeGainMin, FishEyeGainMax, FishEyeGainStep, FishEyeGainDefaultValue, Types.ControlKey.FISHEYE_GAIN, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }
                        break;
                    }
                case Types.ControlKey.DEPTH_PRESET:
                    {
                        //Preset_Test_IVCAM_Depth();
                        break;
                    }
                case Types.ControlKey.COLOR_GAMMA:
                    {
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAMMA, colorVideoDeviceConroller);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                            v.SetRange(SR300ColorGammaMin, SR300ColorGammaMax, SR300ColorGammaStep, SR300ColorGammaDefaultValue, Types.ControlKey.COLOR_GAMMA, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }
                        //SetPropertyViaMissingProperties(SR300ColorGammaMin, SR300ColorGammaMax, SR300ColorGammaStep, SR300ColorGammaDefaultValue, Types.ControlKey.COLOR_GAMMA);
                        //MissingProperties.SetValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Saturation, SR300ColorSaturationMin-1);
                        //MissingProperties.SetValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Saturation, SR300ColorSaturationMax + 1);
                        //int val;
                        //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Saturation, out val);
                        //Console.WriteLine(val);
                        break;
                    }             
                case Types.ControlKey.COLOR_SATURATION:
                    {
                        //Microsoft RS2:
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.SATURATION, colorVideoDeviceConroller);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                            v.SetRange(SR300ColorSaturationMin, SR300ColorSaturationMax, SR300ColorSaturationStep, SR300ColorSaturationDefaultValue, Types.ControlKey.COLOR_SATURATION, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }
                            //Microsoft RS1:
                            //SetPropertyViaMissingProperties(SR300ColorGammaMin, SR300ColorGammaMax, SR300ColorGammaStep, SR300ColorGammaDefaultValue, Types.ControlKey.COLOR_GAMMA);
                            break;
                        }
                case Types.ControlKey.COLOR_SHARPNESS:
                    {
                        //RS2:
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.SHARPNESS, colorVideoDeviceConroller);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                            v.SetRange(SR300ColorSaturationMin, SR300ColorSaturationMax, SR300ColorSaturationStep, SR300ColorSaturationDefaultValue, Types.ControlKey.COLOR_SHARPNESS, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }
                            //RS1:
                            //   SetPropertyViaMissingProperties(SR300ColorSharpnessMin, SR300ColorSharpnessMax, SR300ColorSharpnessStep, SR300ColorSaturationDefaultValue, Types.ControlKey.COLOR_SHARPNESS);
                            break;
                    }
                case Types.ControlKey.COLOR_GAIN:
                    {
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, colorVideoDeviceConroller);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                            v.SetRange(SR300ColorGainMin, SR300ColorGainMax, SR300ColorGainStep, SR300ColorGainDefaultValue, Types.ControlKey.COLOR_GAIN, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                        }
                        //RS1:
                        //SetPropertyViaMissingProperties(SR300ColorGainMin, SR300ColorGainMax, SR300ColorGainStep, SR300ColorGainDefaultValue, Types.ControlKey.COLOR_GAIN);
                        break;
                    }
                //case Types.ControlKey.DS5_DEPTH_GAIN:
                //    {
                //        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, depthVideoDeviceController);
                //        if (setToSpecificValue)
                //        {
                //            v.Set(value);
                //        }
                //        else
                //        {
                //            v.SetRange(DS5GainMin, DS5GainMax, DS5GainStep, DS5GainDefaultValue, Types.ControlKey.DS5_DEPTH_GAIN, ref controls);
                //        }
                //        //SetPropertyViaMissingProperties(SR300ColorGainMin, SR300ColorGainMax, SR300ColorGainStep, SR300ColorGainDefaultValue, Types.ControlKey.COLOR_GAIN);
                //        break;
                //    }
                case Types.ControlKey.COLOR_PRIVACY:
                    {
                        //RS2:
                        CameraControl v = new CameraControl((uint)CameraControl_ControlValues.PRIVACY, colorVideoDeviceConroller);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                    v.SetRange(SR300ColorPrivacyMin, SR300ColorPrivacyMax, SR300ColorPrivacyStep, SR300ColorPrivacyDefaultValue, Types.ControlKey.COLOR_PRIVACY, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                    }
                        //RS1:
                        //SetPropertyViaMissingProperties(SR300ColorPrivacyMin, SR300ColorPrivacyMax, SR300ColorPrivacyStep, SR300ColorPrivacyDefaultValue, Types.ControlKey.COLOR_PRIVACY);
                        break;
                    }
                case Types.ControlKey.DEPTH_PRIVACY:
                    {
                        //RS2:
                        CameraControl v = new CameraControl((uint)CameraControl_ControlValues.PRIVACY, depthVideoDeviceController);
                        if (setToSpecificValue)
                        {
                            v.Set(value);
                        }
                        else
                        {
                    v.SetRange(SR300DepthPrivacyMin, SR300DepthPrivacyMax, SR300DepthPrivacyStep, SR300DepthPrivacyDefaultValue, Types.ControlKey.DEPTH_PRIVACY, ref controls, MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                }
                //RS1:
                //SetPropertyViaMissingProperties(SR300DepthPrivacyMin, SR300DepthPrivacyMax, SR300DepthPrivacyStep, SR300DepthPrivacyDefaultValue, Types.ControlKey.DEPTH_PRIVACY);
                break;
                    }
                default:
                    Logger.AppendInfo("Fail: Control name " + Types.ConvertControlKeyToString(control) + " not found!");
                    break;
            }
        //}
        // else
        //{
        //    Logger.AppendError("ERROR: Invalid camera type!");
        //}
    }

        private static void SetValueViaMicrosoftAPI(Func<PowerlineFrequency, bool> trySetPowerlineFrequency, object tryGetPowerlineFrequency, int v1, int v2, int v3, int v4, bool setToSpecificValue)
        {
            throw new NotImplementedException();
        }

        //private static void SetPropertyViaMissingProperties(int min, int max, int step, int defaultValue, Types.ControlKey controlType)
        //{
        //    MissingProperties.ControlType type;
        //    string cameraType;
        //    switch (controlType)
        //    {
        //        case Types.ControlKey.COLOR_GAMMA:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = MissingProperties.ControlType.ControlType_Gamma;
        //                break;
        //            }
        //        case Types.ControlKey.COLOR_SATURATION:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = MissingProperties.ControlType.ControlType_Saturation;
        //                break;
        //            }
        //        case Types.ControlKey.COLOR_SHARPNESS:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = MissingProperties.ControlType.ControlType_Sharpness;
        //                break;
        //            }
        //        case Types.ControlKey.COLOR_GAIN:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = MissingProperties.ControlType.ControlType_Gain;
        //                break;
        //            }
        //        case Types.ControlKey.COLOR_PRIVACY:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = MissingProperties.ControlType.ControlType_Privacy;
        //                break;
        //            }
        //        case Types.ControlKey.DEPTH_PRIVACY:
        //            {
        //                cameraType = MissingProperties.SR300_DepthCamera;
        //                type = MissingProperties.ControlType.ControlType_Privacy;
        //                break;
        //            }
        //        case Types.ControlKey.FISHEYE_EXPOSURE:
        //            {
        //                cameraType = MissingProperties.DS5_FishEyeCamera;
        //                type = MissingProperties.ControlType.ControlType_Exposure;
        //                break;
        //            }
        //        case Types.ControlKey.FISHEYE_GAIN:
        //            {
        //                cameraType = MissingProperties.DS5_FishEyeCamera;
        //                type = MissingProperties.ControlType.ControlType_Gain;
        //                break;
        //            }

        //        default:
        //            {
        //                cameraType = MissingProperties.SR300_ColorCamera;
        //                type = 0;
        //                break;
        //            }
        //    }
        //    //int minValue;
        //    //int maxValue;
        //    //int stepValue;
        //    //int defaultValue;
        //    //MissingProperties.GetCapabilities(MissingProperties.ColorCamera, type, out minValue, out maxValue, out stepValue, out defaultValue);
        //    //MissingProperties.GetStepValue(cameraName, type, out stepValue);
        //    for (int i = min; i <= max; i += step)
        //    {
        //        MissingProperties.SetValue(cameraType, type, i);
        //        int currentValue;
        //        MissingProperties.GetCurrentValue(cameraType, type, out currentValue);
        //        if (currentValue == i)
        //        {
        //            Logger.AppendInfo("Set passed for value " + i + " for control " + type);
        //        }
        //        else
        //        {
        //            Logger.AppendInfo("Set failed for value " + i + " for control " + type);
        //        }
        //    }
        //}

        public static double GetControlValue(Types.ControlKey control)
        {
            switch (control)
            {
                case Types.ControlKey.DS5_DEPTH_EXPOSURE:
                    {
                        return GetControlValueHelper(DS5ExposurePropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }

                case Types.ControlKey.DS5_DEPTH_LASER_ON_OFF:
                    {
                        return GetControlValueHelper(DS5LaserOnOffPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DS5_DEPTH_LASER_POWER:
                    {
                        return GetControlValueHelper(DS5LaserPowerPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DS5_DEPTH_GAIN:
                    {
                        VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, depthVideoDeviceController);
                        return v.Get();
                    }
                case Types.ControlKey.DS5_DEPTH_AUTO_WHITE_BALANCE:
                    {
                        return GetControlValueHelper(DS5AutoWhiteBalancePropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.COLOR_EXPOSURE:
                    {
                        double value;
                        colorVideoDeviceConroller.Exposure.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_BRIGHTNESS:
                    {
                        double value;
                        colorVideoDeviceConroller.Brightness.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_CONTRAST:
                    {
                        double value;
                        colorVideoDeviceConroller.Contrast.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_HUE:
                    {
                        double value;
                        colorVideoDeviceConroller.Hue.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_WHITE_BALANCE:
                    {
                        double value;
                        colorVideoDeviceConroller.WhiteBalance.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION:
                    {
                        double value;
                        colorVideoDeviceConroller.BacklightCompensation.TryGetValue(out value);
                        return value;
                    }
                case Types.ControlKey.COLOR_EXPOSURE_PRIORITY:
                    {
                        CameraControl exposurePriority = new CameraControl(19, colorVideoDeviceConroller);
                        return exposurePriority.Get();
                        //return GetControlValueHelper(SR300ColorExposurePriorityPropID, Types.ConvertControlKeyToString(control), colorVideoDeviceConroller);
                    }
                case Types.ControlKey.COLOR_POWER_LINE_FREQUENCY:
                    {
                        return GetControlValueHelper(SR300ColorPowerLineFrequencyPropID, Types.ConvertControlKeyToString(control), colorVideoDeviceConroller);
                    }
                case Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE:
                    {
                        return GetControlValueHelper(SR300DepthMotionVSRangeTradeoffPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DEPTH_LASER_POWER:
                    {
                        return GetControlValueHelper(SR300DepthLaserPowerPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DEPTH_ACCURACY:
                    {
                        return GetControlValueHelper(SR300DepthAccuracyPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DEPTH_FILTER_OPTION:
                    {
                        //throw new Exception("DEPTH_FILTER_OPTION");
                        return GetControlValueHelper(SR300DepthFilterOptionPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD:
                    {
                        return GetControlValueHelper(SR300DepthConfidenceThresholdPropID, Types.ConvertControlKeyToString(control), depthVideoDeviceController);
                    }
                case Types.ControlKey.DEPTH_PRESET:
                    {
                        //       Preset_Test_IVCAM_Depth(depthVideoDeviceConroller);
                        break;
                    }
                case Types.ControlKey.FISHEYE_GAIN:
                    {
                        int value;
                        MissingProperties.GetCurrentValue(MissingProperties.DS5_FishEyeCamera, MissingProperties.ControlType.ControlType_Gain, out value);
                        return value;
                    }
                case Types.ControlKey.FISHEYE_EXPOSURE:
                    {
                        int value;
                        MissingProperties.GetCurrentValue(MissingProperties.DS5_FishEyeCamera, MissingProperties.ControlType.ControlType_Exposure, out value);
                        return value;
                    }
        case Types.ControlKey.DEPTH_PRIVACY:
            {
                CameraControl v = new CameraControl((uint)CameraControl_ControlValues.PRIVACY, depthVideoDeviceController);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.DepthCamera, MissingProperties.ControlType.ControlType_Privacy, out value);
                //return value;
            }
        case Types.ControlKey.COLOR_PRIVACY:
            {
                CameraControl v = new CameraControl((uint)CameraControl_ControlValues.PRIVACY, colorVideoDeviceConroller);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Privacy, out value);
                //return value;
            }
        case Types.ControlKey.COLOR_GAIN:
            {
                VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAIN, colorVideoDeviceConroller);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Gain, out value);
                //return value;
            }
        case Types.ControlKey.COLOR_SHARPNESS:
            {
                VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.SHARPNESS, colorVideoDeviceConroller);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Sharpness, out value);
                //return value;
            }
        case Types.ControlKey.COLOR_GAMMA:
            {
                VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.GAMMA, colorVideoDeviceConroller);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Gamma, out value);
                //return value;
            }
        case Types.ControlKey.COLOR_SATURATION:
            {
                VideoProcAmp v = new VideoProcAmp((uint)VideoProcAmp_ControlValues.SATURATION, colorVideoDeviceConroller);
                return v.Get();
                //int value;
                //MissingProperties.GetCurrentValue(MissingProperties.SR300_ColorCamera, MissingProperties.ControlType.ControlType_Saturation, out value);
                //return value;
            }

        default:
                    //Logger.AppendInfo("Fail: Control name " + Types.ConvertControlKeyToString(control) + " not found!");
                    break;
            }
            return 0;
        }
        //Used for retrieving values from XU
        private static double GetControlValueHelper(string propID, string controlName, Windows.Media.Devices.VideoDeviceController videoDeviceConroller)
        {
            byte[] result = null;
            try
            {
                result = (byte[])videoDeviceConroller.GetDeviceProperty(propID);
            }
            catch
            {
                testPassed = false; //test failed
                Logger.AppendMessage("Failed at getting " + controlName + " value");
                Logger.AppendMessage("PROFILE_RESULT: Failed");
            }
            if (result != null)
            {
                byte[] temp = new byte[result.Count()];
                if(result.Count()<4)
                {
                    temp = new byte[4];
                }
                for (int i = 0; i < result.Count(); i++)
                {
                    temp[i] = 0;
                }
                for (int i = 0; i < result.Count(); i++)
                {
                    temp[i] = result[i];
                }
                try
                {
                    return BitConverter.ToInt32(temp, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return 0;
        }


    private static void SetPowerlineFrequencyViaMicrosoftAPI(double min, double max, double step, double defaultValue, bool setToSpecificValue)
    {
        Windows.Media.Capture.PowerlineFrequency currentVal;
        for (double i = min; i <= max; i += step)
        {
            //Console.WriteLine(colorVideoDeviceConroller.TryGetPowerlineFrequency(out currentVal));
            Console.WriteLine("color frequency set status: "+ colorVideoDeviceConroller.TrySetPowerlineFrequency((Windows.Media.Capture.PowerlineFrequency)i));           
            if(colorVideoDeviceConroller.TryGetPowerlineFrequency(out currentVal) && (int)currentVal==i)
            {
                Logger.AppendInfo("Success: set PowerlineFrequency value to :" + i);
                Console.WriteLine("Success: set PowerlineFrequency value to :" + i);
            }
            else
            {
                Logger.AppendInfo("Failed! set PowerlineFrequency value to :" + i);
                Console.WriteLine("Failed! set PowerlineFrequency value to :" + i);
            }
        }

            // try to set out of range values 
            colorVideoDeviceConroller.TrySetPowerlineFrequency((Windows.Media.Capture.PowerlineFrequency)(max + step));
            if (colorVideoDeviceConroller.TryGetPowerlineFrequency(out currentVal) && (int)currentVal == max + step)
            {
                Logger.AppendInfo("Failure! set PowerlineFrequency value up to max :" + currentVal);
                Console.WriteLine("Failure! set PowerlineFrequency value up to max :" + currentVal);
            }
            colorVideoDeviceConroller.TrySetPowerlineFrequency((Windows.Media.Capture.PowerlineFrequency)(min - step));
            if (colorVideoDeviceConroller.TryGetPowerlineFrequency(out currentVal) && (int)currentVal == min - step)
            {
                Logger.AppendInfo("Failure! set PowerlineFrequency value lower than min :" + currentVal);
                Console.WriteLine("Failure! set PowerlineFrequency value lower than min :" + currentVal);
            }
        }
    
    private static void changeValueViaXU(string propId, Types.ControlKey controlKey, double min, double max, double step, double defaultValue, bool isValueBoolean, Windows.Media.Devices.VideoDeviceController videoDeviceConroller, bool setToSpecificValue)
    {      
            string propertyName = Types.ConvertControlKeyToString(controlKey);
            Console.WriteLine("changeValueViaXU => " + propertyName);
            Logger.AppendDebug("changeValueViaXU => " + propertyName);
            if (!setToSpecificValue)
            {
                Logger.AppendInfo("Now setting " + propertyName + " properties within acceptable range");
            }
            int numberOfControlsSet = 0;
            for (double i = min; i <= max /*&& numberOfControlsSet < 10*/; i += step)
            {
                if (!setToSpecificValue)
                {
                    Logger.AppendMessage("PROFILE_NAME: " + propertyName + "," + i);
                }
                byte[] setValue = BitConverter.GetBytes((int)i);
                //Byte order must be little-endian to send command
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(setValue);
                }
                Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                try
                {
                    Console.WriteLine("Call SetDeviceProperty ");
                    videoDeviceConroller?.SetDeviceProperty(propId, setValue);

                    //Console.WriteLine("=====>>>>>>> setting Control/exposure "+i);
                    //TimeSpan ts = new TimeSpan((long)200);

                    //Task.Run(async () =>
                    //{
                    //    await videoDeviceConroller.ExposureControl.SetValueAsync(ts);
                    //}).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("SetDeviceProperty Failed : " + e.Message);
                    testPassed = false; //test failed
                    if (!setToSpecificValue)
                    {
                        Logger.AppendMessage("PROFILE_RESULT: Failed");
                    }
                    Logger.AppendInfo("Failed at setting value within normal range " + e.Message);
                }
                if (!setToSpecificValue)
                {
                controls[controlKey] = i;
                checkAllDeviceValuesAgainstExpectedValues();
                }
                numberOfControlsSet++;
                Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
            }
            if (!setToSpecificValue)
            {
                byte[] setDValue = BitConverter.GetBytes((int)defaultValue);
                //Byte order must be little-endian to send command
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(setDValue);
                }
                Logger.AppendInfo("Now reverting to default value of " + defaultValue);
                try
                {
                    Console.WriteLine("Call SetDeviceProperty ");
                    videoDeviceConroller?.SetDeviceProperty(propId, setDValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine("SetDeviceProperty Failed : " + e.Message);
                    testPassed = false; //test failed
                    Logger.AppendInfo("Failed at setting default value");
                }
                Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                if (!setToSpecificValue)
                {
                    controls[controlKey] = defaultValue;
                    checkAllDeviceValuesAgainstExpectedValues();
                }
                if (!isValueBoolean) //out of range tests are not applicable to properties with true/false values
                {
                    Logger.AppendInfo("Now setting  " + propertyName + " outside of acceptable range");
                    byte[] outOfRangeValue = BitConverter.GetBytes((int)min - 1);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(outOfRangeValue);
                    }
                    //Try setting below minimum value
                    try
                    {
                        videoDeviceConroller?.SetDeviceProperty(propId, outOfRangeValue);//Exception expected after this call
                        Logger.AppendInfo("Fail: Device allows value to be set lower than minimum value");
                        testPassed = false; //test failed
                    }
                    catch //(Exception e)
                    {
                        Logger.AppendInfo("Success: Device does not allow value to be set lower than minimum value");
                    }
                    outOfRangeValue = BitConverter.GetBytes((int)max + 1);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(outOfRangeValue);
                    }
                    Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                    //Try setting above max value
                    try
                    {
                        videoDeviceConroller?.SetDeviceProperty(propId, outOfRangeValue);
                        Logger.AppendInfo("Fail: Device allows value to be set higher than maximum value");
                        testPassed = false; //test failed
                    }
                    catch// (Exception e)
                    {
                        Logger.AppendInfo("Success: Device does not allow value to be set higher than minimum value");
                    }
                }
                Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                }
            
        }

        public static bool checkAllDeviceValuesAgainstExpectedValues()
        {
            bool passed = true;
            foreach (KeyValuePair<Types.ControlKey, double> kvp in controls)
            {
                if (kvp.Value != GetControlValue(kvp.Key))
                {
                    passed = false;
                    Logger.AppendMessage("PROFILE_RESULT: Failed");
                    Console.WriteLine(kvp.Key + "expected value: " + kvp.Value + " actual value: " +GetControlValue(kvp.Key));
                }
            }
            if (passed)
            {
                Logger.AppendMessage("PROFILE_RESULT: Passed");
            }
            return passed;
        }

        private static void SetValueViaMicrosoftAPI(Windows.Media.Devices.MediaDeviceControl type, Types.ControlKey controlKey, double min, double max, double step, double defaultValue, bool setToSpecificValue)
        {
            Logger.AppendDebug(" SetValueViaMicrosoftAPI: "+ setToSpecificValue);
            string testName = Types.ConvertControlKeyToString(controlKey);
            if (type==null || !type.Capabilities.Supported)
            {
                testPassed = false; //test failed
                Console.WriteLine("Control not supported :"+type+"  "+ testName);
                Logger.AppendError("Properties test fail: Unable to set " + testName);
                Logger.AppendError($"Type: {type}, Is Capabilities Supported: {type?.Capabilities.Supported}");
            }
            else
            {
                if (!setToSpecificValue)
                {
                    Logger.AppendInfo("Now setting " + testName + " properties within acceptable range");
                }
                int numberOfControlsSet = 0;
                for (double i = min; i <= max /*&& numberOfControlsSet <10 */; i += step)
                {
                    if (!setToSpecificValue)
                    {
                        Logger.AppendMessage("PROFILE_NAME: " + testName + "," + i);
                    }
                    type.TrySetAuto(false);
                    //type.TrySetValue(i);
                    //Commented out because there is currently an open bug with this. Uncomment when bug is fixed
                    if (!type.TrySetValue(i))
                    {
                        Logger.AppendInfo("TrySetValue did not succeed for " + testName + "test at value " + i);
                        //testPassed = false; //test failed
                    }
                    if (!setToSpecificValue)
                    {
                        controls[controlKey] = i;
                        checkAllDeviceValuesAgainstExpectedValues();
                    }
                    numberOfControlsSet++;
                    Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                }
                if (!setToSpecificValue)
                {
                    Logger.AppendInfo("Finished setting " + testName + " properties within acceptable range");
                    Logger.AppendInfo("Now setting " + testName + " properties outside of acceptable range");
                    Logger.AppendMessage("PROFILE_NAME: " + testName + "," + (min - step));
                    if (!type.TrySetValue(min - step))
                    {
                        Logger.AppendMessage("PROFILE_RESULT: Passed");
                    Logger.AppendInfo("Success: Unable to set value below minimum");
                }
                    else
                    {
                        testPassed = false;
                        Logger.AppendMessage("PROFILE_RESULT: Failed");
                        Logger.AppendInfo("Fail: Error: Camera allows setting of " + testName + " below minimum value");
                    }
                    Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                    Logger.AppendMessage("PROFILE_NAME: " + testName + "," + (max + step));
                    if (!type.TrySetValue(max + step))
                    {
                        Logger.AppendMessage("PROFILE_RESULT: Passed");
                    Logger.AppendInfo("Success: Unable to set value above maximum");
                }
                    else
                    {
                        testPassed = false;
                        Logger.AppendMessage("PROFILE_RESULT: Failed");
                    Logger.AppendInfo("Fail: Error: Camera allows setting of " + testName + " above maximum value");
                }

                    Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                    Logger.AppendInfo("Setting " + testName + " back to default ");
                    if (!type.TrySetValue(defaultValue))
                    {
                        // testPassed = false;
                        //thisTestPassed = false;
                        Logger.AppendInfo("TrySetValue failed for " + testName + " setting back to default value");
                    }
                    controls[controlKey] = defaultValue;
                    double value;
                    if (!type.TryGetValue(out value))
                    {
                        //testPassed = false;
                        //thisTestPassed = false;
                        Logger.AppendInfo("Fail: Cannot get value from" + testName);
                    }
                    if (value == defaultValue)
                    {
                        Logger.AppendInfo("Successfully set back to default value");
                    }
                    else
                    {
                        testPassed = false; //test failed
                        Logger.AppendInfo("Failed at setting back to default value");
                    }
                    Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
                }
            }
        }

        private static void Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(int valueToSet, ref bool presetTestPassed)
        {
            byte[] setValue = BitConverter.GetBytes(valueToSet);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(setValue);
            }
            Object propVal = PropertyValue.CreateUInt8Array(setValue);
            //Byte order must be little-endian to send command

            Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
            try
            {
                depthVideoDeviceController.SetDeviceProperty(SR300DepthPresetPropID, propVal);
            }
            catch (Exception e)
            {
                //testPassed = false; //test failed
                Logger.AppendInfo("Failed at setting preset value: " + valueToSet + " which is within normal range  "+e.Message);
                testPassed = false;
                presetTestPassed = false;
            }
            Thread.Sleep(MILLISECONDS_TO_SLEEP_BETWEEN_SET);
            byte[] result = (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthPresetPropID);
            bool check = true;
            for (int j = 0; j < 1; j++)
            {
                if (result[j] != setValue[j])
                {
                    check = false;
                }
            }
            if (!check)
            {
                Logger.AppendInfo("Failed at setting preset value: " + valueToSet + " which is within normal range");
                testPassed = false;
                presetTestPassed = false;
            }
        }
        private static void Preset_Test_IVCAM_Depth_Helper2_CheckResult(int expectedResultValue, byte[] actualResultValue, string propName, int presetValue, ref bool presetTestPassed)
        {
            byte[] expectedResultValue_ = BitConverter.GetBytes(expectedResultValue);
            bool check = true;
            for (int j = 0; j < 1; j++)
            {
                if (expectedResultValue_[j] != actualResultValue[j])
                {
                    check = false;
                }
            }
            if (!check)
            {
                testPassed = false;
                Logger.AppendInfo("Failed at setting preset number " + presetValue + " because Property " + propName + " was not set properly.");
                Logger.AppendInfo("expected: " + expectedResultValue + ". Actual: " + actualResultValue[0]);
                presetTestPassed = false;
            }
        }


    //public static void Preset_Test_IVCAM_Depth()
    //{
    //    bool presetTestPassed = true;
    //    Logger.AppendInfo("Now setting Preset properties within acceptable range");

    //    //Used for obtaining results via HW Monitor Commands
    //    Guid ivcamGuid = new Guid("175695CD-30D9-4F87-8BE3-5A8270F49A31");
    //    var winUsbDevice = new HWMonitorDevice("8086", "0AA5", ivcamGuid, 1);
    //    var parser = new CommandsXmlParser("Commands1_5.xml");

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,1");//ShortRange
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(1, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Depth accuracy", 2, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(5, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Depth filter option", 2, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Depth Confidence threshold", 2, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 2, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,2");//LongRange
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(2, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Depth accuracy", 3, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(7, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 3, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 3, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 3, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,3");//BackgroundSegmentation
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(3, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 5, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 5, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(22, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthMotionVSRangeTradeoffPropID), "Motion vs range", 5, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(6, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter", 5, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(2, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 5, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,4");//GestureRecognition
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(4, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 7, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(6, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 7, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(3, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 7, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 7, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,5");//ObjectScanning
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(5, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 10, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(9, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthMotionVSRangeTradeoffPropID), "Motion vs range", 10, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(3, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 10, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence", 10, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 10, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,6"); //FaceAnalytics
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(6, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 11, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(22, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthMotionVSRangeTradeoffPropID), "Motion vs range tradeoff", 11, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(5, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 11, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 11, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 11, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,8"); //GRCursor
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(8, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 9, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(6, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter", 9, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 9, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 9, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,10"); //Default
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(10, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult((int)SR300DepthAccuracyDefaultValue, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 14, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult((int)SR300DepthMotionVSRangeTradeoffDefaultValue, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthMotionVSRangeTradeoffPropID), "Motion vs range", 14, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult((int)SR300DepthFilterOptionDefaultValue, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 14, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult((int)SR300DepthConfidenceThresholdDefaultValue, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 14, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult((int)SR300DepthLaserPowerDefaultValue, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 14, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,11"); //Midrange
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(11, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Depth accuracy", 0, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(5, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 0, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 0, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 0, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,12"); //IROnly
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(12, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 13, ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,13"); //BackgroundSegmentation_HDR
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(13, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 6, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(6, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter", 6, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 6, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 6, ref presetTestPassed);
    //    CommandResult result = parser.SendCommand(winUsbDevice, "HDR_CONFIG_GET");
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(50, BitConverter.ToInt16(result.ByteArray, 0), "HDR_NUMBER_OF_FRAMES_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(22, BitConverter.ToInt16(result.ByteArray, 2), "HDR_MVR_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(1, BitConverter.ToInt16(result.ByteArray, 4), "HDR_NUMBER_OF_FRAMES_IN_LONG_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(110, BitConverter.ToInt16(result.ByteArray, 6), "HDR_MVR_IN_LONG_RANGE", ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,14"); //ShortMidRange_HDR
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(14, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Accuracy", 8, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(6, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Accuracy", 8, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(3, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Depth confidence threshold", 8, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser", 8, ref presetTestPassed);
    //    result = parser.SendCommand(winUsbDevice, "HDR_CONFIG_GET");
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(55, BitConverter.ToInt16(result.ByteArray, 0), "HDR_NUMBER_OF_FRAMES_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(9, BitConverter.ToInt16(result.ByteArray, 2), "HDR_MVR_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(1, BitConverter.ToInt16(result.ByteArray, 4), "HDR_NUMBER_OF_FRAMES_IN_LONG_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(55, BitConverter.ToInt16(result.ByteArray, 6), "HDR_MVR_IN_LONG_RANGE", ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,15");//MidLongRange_HDR
    //    //Send preset command to DMFT
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(15, ref presetTestPassed);
    //    //Check values via DMFT
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Depth accuracy", 1, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(7, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Depth filter option", 1, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Depth confidence threshold", 1, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power option", 1, ref presetTestPassed);
    //    //Check values via HW monitor commands
    //    result = parser.SendCommand(winUsbDevice, "HDR_CONFIG_GET");
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(25, BitConverter.ToInt16(result.ByteArray, 0), "HDR_NUMBER_OF_FRAMES_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(22, BitConverter.ToInt16(result.ByteArray, 2), "HDR_MVR_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(1, BitConverter.ToInt16(result.ByteArray, 4), "HDR_NUMBER_OF_FRAMES_IN_LONG_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(110, BitConverter.ToInt16(result.ByteArray, 6), "HDR_MVR_IN_LONG_RANGE", ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }
    //    presetTestPassed = true;

    //    Logger.AppendMessage("PROFILE_NAME: DEPTH_PRESET,16"); //LongLongRange_HDR
    //    Preset_Test_IVCAM_Depth_Helper_Set_Preset_Value(16, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthAccuracyPropID), "Depth accuracy", 4, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(7, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthFilterOptionPropID), "Filter option", 4, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(1, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthConfidenceThresholdPropID), "Confidence threshold", 4, ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper2_CheckResult(16, (byte[])depthVideoDeviceController.GetDeviceProperty(SR300DepthLaserPowerPropID), "Laser power", 4, ref presetTestPassed);
    //    result = parser.SendCommand(winUsbDevice, "HDR_CONFIG_GET");
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(10, BitConverter.ToInt16(result.ByteArray, 0), "HDR_NUMBER_OF_FRAMES_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(55, BitConverter.ToInt16(result.ByteArray, 2), "HDR_MVR_IN_SHORT_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(1, BitConverter.ToInt16(result.ByteArray, 4), "HDR_NUMBER_OF_FRAMES_IN_LONG_RANGE", ref presetTestPassed);
    //    Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(150, BitConverter.ToInt16(result.ByteArray, 6), "HDR_MVR_IN_LONG_RANGE", ref presetTestPassed);
    //    if (presetTestPassed)
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Passed");
    //    }
    //    else
    //    {
    //        Logger.AppendMessage("PROFILE_RESULT: Failed");
    //    }

    //    //Rebuild dictionary, as preset test changed values without updating the dictionary
    //    foreach (var key in controls.Keys.ToList())
    //    {
    //        controls[key] = getControlValue(key);
    //    }
    //}
    private static void Preset_Test_IVCAM_Depth_Helper_CompareExpectedAndActualResultsFromHWMonitorCommand(int expectedValue, int actualValue, string valueName, ref bool presetTestPassed)
    {
        if (expectedValue != actualValue)
        {
            presetTestPassed = false;
            Logger.AppendInfo("Preset failed because " + valueName + "  must equal " + expectedValue + " but instead equals " + actualValue);
        }
    }
    public static void printAllControlValues(bool isSr300) // if it's not sr300, then it's ds5
        {
            try
            {
                if (isSr300)
                {
                    Logger.AppendInfo("COLOR_EXPOSURE: " + GetControlValue(Types.ControlKey.COLOR_EXPOSURE).ToString());
                    Logger.AppendInfo("COLOR_BRIGHTNESS: " + GetControlValue(Types.ControlKey.COLOR_BRIGHTNESS));
                    Logger.AppendInfo("COLOR_CONTRAST: " + GetControlValue(Types.ControlKey.COLOR_CONTRAST));
                    Logger.AppendInfo("COLOR_HUE: " + GetControlValue(Types.ControlKey.COLOR_HUE));
                    Logger.AppendInfo("COLOR_WHITE_BALANCE: " + GetControlValue(Types.ControlKey.COLOR_WHITE_BALANCE));
                    Logger.AppendInfo("COLOR_BACK_LIGHT_COMPENSATION: " + GetControlValue(Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION));
                    Logger.AppendInfo("COLOR_EXPOSURE_PRIORITY: " + GetControlValue(Types.ControlKey.COLOR_EXPOSURE_PRIORITY));
                    Logger.AppendInfo("COLOR_POWER_LINE_FREQUENCY: " + GetControlValue(Types.ControlKey.COLOR_POWER_LINE_FREQUENCY));
                    Logger.AppendInfo("DEPTH_MOTION_VS_RANGE_TRADE: " + GetControlValue(Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE));
                    Logger.AppendInfo("DEPTH_LASER_POWER: " + GetControlValue(Types.ControlKey.DEPTH_LASER_POWER));
                    Logger.AppendInfo("DEPTH_ACCURACY: " + GetControlValue(Types.ControlKey.DEPTH_ACCURACY));
                    Logger.AppendInfo("DEPTH_FILTER_OPTION: " + GetControlValue(Types.ControlKey.DEPTH_FILTER_OPTION));
                    Logger.AppendInfo("DEPTH_CONFIDENCE_THRESHOLD: " + GetControlValue(Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD));
                }
                else
                {
                    Logger.AppendInfo("DS5_DEPTH_EXPOSURE: ", GetControlValue(Types.ControlKey.DS5_DEPTH_EXPOSURE));
                    Logger.AppendInfo("DS5_DEPTH_LASER_ON_OFF: ", GetControlValue(Types.ControlKey.DS5_DEPTH_LASER_ON_OFF));//DS5_DEPTH_LASER_POWER
                    Logger.AppendInfo("DS5_DEPTH_LASER_POWER: ", GetControlValue(Types.ControlKey.DS5_DEPTH_LASER_POWER));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public static void SetFaceAuthControl()
        {
            Console.WriteLine("Debug: SetControl/SetFaceAuthControl ");
            Logger.AppendDebug("Inside  SetControl/SetFaceAuthControl()");
            Utils.KSCAMERA_EXTENDEDPROP_HEADER faceAuthData = new Utils.KSCAMERA_EXTENDEDPROP_HEADER();
            faceAuthData.Version = 1;
            faceAuthData.PinId = 1;
            faceAuthData.Size = 40;
            faceAuthData.Result = 0;
            faceAuthData.Capability = 3;
            faceAuthData.Flags = 2;

            Logger.AppendInfo("Set FaceAuth Control");

            byte[] bytes = Utils.PdoToBuffer<Utils.KSCAMERA_EXTENDEDPROP_HEADER>(faceAuthData);
            try
            {
                if(depthVideoDeviceController !=null)
                {
                    depthVideoDeviceController.SetDeviceProperty(RS300FaceAuthPropID, bytes);
                    var value = depthVideoDeviceController.GetDeviceProperty(RS300FaceAuthPropID);
                }
                Console.WriteLine("Set faceAuth succeed ");
                Logger.AppendInfo("Set faceAuth succeed ");
                //if (_depthMediaCapture != null && depthSourceInfo != null)
                //{
                //    _depthMediaCapture.FrameSources[depthSourceInfo.Id].Controller.VideoDeviceController.SetDeviceProperty(RS300FaceAuthPropID, bytes);
                //    //object value = _depthMediaCapture.FrameSources[depthSourceInfo.Id].Controller.VideoDeviceController.GetDeviceProperty(RS300FaceAuthPropID); 
                //}
                //else
                //{
                //    Logger.AppendError("Cannot set FaceAuth Control ,_depthMediaCapture is null or depthSourceInfo in null");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.AppendException(e.Message);
            }
        }

    }
}