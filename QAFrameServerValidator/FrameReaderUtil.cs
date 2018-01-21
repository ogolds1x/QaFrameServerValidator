using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Threading;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using System.Globalization;

namespace QAFrameServerValidator
{
    public class FrameReaderUtil
    {
        private int frameCount = 0;
        private static MediaFrameSourceGroup colorGroup = null;
        private static MediaFrameSourceGroup depthGroup = null;
        private static MediaFrameSourceGroup fishEyeGroup = null;
        private static MediaFrameSourceInfo colorSourceInfo = null;
        private static MediaFrameSourceInfo depthSourceInfo = null;
        private static MediaFrameSourceInfo fishEyeSourceInfo = null;
        private static MediaCapture _depthMediaCapture = null;
        private static MediaCapture _colorMediaCapture = null;
        private static MediaCapture _fishEyeMediaCapture = null;
        private static MediaFrameSource colorFrameSource = null;
        public static MediaFrameSource depthFrameSource = null;
        public static MediaFrameSource fishEyeFrameSource = null;
        private static MediaFrameReader _colorMediaFrameReader = null;
        private static MediaFrameReader _depthMediaFrameReader = null;
        private static MediaFrameReader _fishEyeMediaFrameReader = null;
        private uint numberOfFramesToCollect = 60;
        private uint videoFormatWidth;
        private uint videoFormatHeight;
        private int FPS;
        private static uint numerator;
        private static uint denominator;
        public string cameratype;
        private List<Control> controlsL;
        private MediaFrameSourceKind mfsKind;
        private string outputFormat;
        private string sensorGroup;
        public bool cameraInitialiatizationPassed;
        public bool ifAllFramesArrived = false;
        public FPS_Analysis analyser;
        public static readonly AutoResetEvent threadWait = new AutoResetEvent(false);
        public readonly AutoResetEvent propertiesWhileStreamingThreadWait = new AutoResetEvent(false);
        private bool isPropertiesWhileStreamingTestRunning;
        private ManualResetEvent finishedSettingControls = new ManualResetEvent(false);
        public bool isSettingControlsFinished = false;
        private ManualResetEvent finishedStreaming = new ManualResetEvent(false);
        public Dictionary<double, string> frameTimeType;
        public string productName;
        public bool isFirstFrameArrivedLate = false;
        
        //a bool that we will use to not notify about the thermal heat twice
        public bool highTempNotified = false;
        
        //a bool we will use after the thermal test to detect if the test was successfull
        public bool isSuccess = false;
        public System.Diagnostics.Stopwatch watchStartStreaming = new System.Diagnostics.Stopwatch();
        public double delay;

        public bool isFishEyeCamera = false;
        public List<object> lst_AllFrames;
        public List<object> lst_HWTimastamp;
        public List<Utils.FRAME_DATA_RS400> lst_Frames;

        //added for Avinoam's test: a list of all the temperatures taken during the callback: FrameReader_FrameArrived_Temperature()
        public List<string> lst_AllTemperature;

        //we will use Yaniv dll for now (until we will learn how to send hw commands using the realsense.api)
        Guid DSGUID = new Guid("08090549-CE78-41DC-A0FB-1BD66694BB0C "); //the guid i got from Yaniv

        public struct SR300DepthFrame
        {
            public double timeStamp;
            public RealsenseSR300DepthMetaData properties;
        }
        public struct SR300ColorFrame
        {
            public double timeStamp;
            public RealsenseSR300ColorMetaData properties;
        }
        public struct DS5DepthFrame
        {
            public double timeStamp;
            public RealsenseRS400MetaDataIntelDepthControl properties;
        }
        public List<SR300DepthFrame> SR300DepthFrameList;
        public List<SR300ColorFrame> SR300ColorFrameList;
        public List<DS5DepthFrame> DS5DepthFrameList;
        public void resetFrameCounter()
        {
            frameCount = 0;
        }
        public MediaCapture GetColorMediaCapture()
        {
            return _colorMediaCapture;
        }
        public MediaCapture GetDepthMediaCapture()
        {
            return _depthMediaCapture;
        }
        public MediaCapture GetFishEyeMediaCapture()
        {
            return _fishEyeMediaCapture;
        }

        public MediaFrameReader GetColorMediaFrameReader()
        {
            return _colorMediaFrameReader;
        }
        public MediaFrameReader GetDepthMediaFrameReader()
        {
            return _depthMediaFrameReader;
        }
        public MediaFrameReader GetFishEyeMediaFrameReader()
        {
            return _fishEyeMediaFrameReader;
        }
        public ManualResetEvent GetFinishedSettingControls()
        {
            return finishedSettingControls;
        }
        public ManualResetEvent GetFinishedStreaming()
        {
            return finishedStreaming;
        }
        public List<double> GetFramesTime()
        {
            return frameTimeType.Where(x => x.Value != "0").Select(x => x.Key).ToList();
        }
        public List<double> GetFramesColorTime()
        {
            return frameTimeType.Where(x => x.Value != "Color").Select(x => x.Key).ToList();
        }
        public List<double> GetFramesDepthTime()
        {
            return frameTimeType.Where(x => x.Value != "Depth").Select(x => x.Key).ToList();
        }

        //this function will run before we start streaming and will intialize the list of Temperature meassurements
        public void IntializeTempTest()
        {
            lst_AllTemperature = new List<string>();
        }

        public void SetSettingsWithPropertiesWhileStreaming(string _cameratype, string format, uint fps, uint x_resolution, uint y_resolution, uint numberOfFramesToCollect_, List<Control> controls)
        {
            SR300DepthFrameList = new List<SR300DepthFrame>();
            SR300ColorFrameList = new List<SR300ColorFrame>();
            DS5DepthFrameList = new List<DS5DepthFrame>();
            SetSettings(_cameratype, format, fps, x_resolution, y_resolution, numberOfFramesToCollect_, controls, true);
        }
        public void SetSettings(string _cameratype, string format, uint fps, uint x_resolution, uint y_resolution, uint numberOfFramesToCollect_, List<Control> controls, bool isPropertiesWhileStreamingTestRunning_)
        {
            FPS = (int)fps;
            videoFormatWidth = x_resolution;
            videoFormatHeight = y_resolution;
            numberOfFramesToCollect = numberOfFramesToCollect_;
            frameTimeType = new Dictionary<double, string>();
            controlsL = controls;
            cameratype = _cameratype;
            outputFormat = format;
            SetMediaFrameSourceKind();
            denominator = (uint)GetDenominator((int)fps);
            numerator = (uint)GetFPS((int)fps);
            analyser = new FPS_Analysis();
            isPropertiesWhileStreamingTestRunning = isPropertiesWhileStreamingTestRunning_;
            isFirstFrameArrivedLate = false;
            lst_AllFrames = new List<object>();
            lst_HWTimastamp = new List<object>();
            lst_Frames = new List<Utils.FRAME_DATA_RS400>();
        }
        public void SetSettingsSensorGroup(BasicProfile profile)
        {
            cameratype = "Cameras";
            if (profile.ProfileType == Types.ModeType.Depth)
            {
                outputFormat = profile.DepthMode.ToString();
                sensorGroup = "Depth";
                if (outputFormat == "L8" || outputFormat == "G49323159" || outputFormat == "G59565955" || outputFormat == "G20493859")
                {
                    mfsKind = GetSource_StrToEnum("Infrared");
                }
                else
                {
                    mfsKind = GetSource_StrToEnum("Depth");
                }
                videoFormatWidth = (uint)profile.DepthResolution.Width;
                videoFormatHeight = (uint)profile.DepthResolution.Height;
                FPS = (int)profile.DepthFps;
            }
            else if (profile.ProfileType == Types.ModeType.Color)
            {
                mfsKind = GetSource_StrToEnum("Color");
                sensorGroup = "Color";
                videoFormatWidth = (uint)profile.ColorResolution.Width;
                videoFormatHeight = (uint)profile.ColorResolution.Height;
                FPS = (int)profile.ColorFps;
                outputFormat = profile.ColorMode.ToString();
            }
            else if (profile.ProfileType == Types.ModeType.FishEye)
            {
                sensorGroup = "FishEye";
                mfsKind = GetSource_StrToEnum("FishEye");
                videoFormatWidth = (uint)profile.FishEyeResolution.Width;
                videoFormatHeight = (uint)profile.FishEyeResolution.Height;
                FPS = (int)profile.FishEyeFps;
                outputFormat = profile.FishEyeMode.ToString();
            }

            denominator = (uint)GetDenominator(FPS);
            numerator = (uint)GetFPS(FPS);

            numberOfFramesToCollect = 70;
            isPropertiesWhileStreamingTestRunning = false;
            isFirstFrameArrivedLate = false;
            frameTimeType = new Dictionary<double, string>();
            lst_AllFrames = new List<object>();
            analyser = new FPS_Analysis();
            controlsL = profile.Controls;

        }
        public void SetSettingsSpecificGroupAndSource(string group, string source)
        {
            cameratype = group;
            mfsKind = GetSource_StrToEnum(source);
            lst_AllFrames = new List<object>();
            frameTimeType = new Dictionary<double, string>();
        }
        public void SetSettingsPropertiesTest(string _cameratype)
        {
            cameratype = _cameratype;
            outputFormat = "D16";
            SetMediaFrameSourceKind();
        }

        public void SetMediaFrameSourceKind()
        {
            if (cameratype == "Color" || cameratype == "RGB")
            {
                mfsKind = MediaFrameSourceKind.Color;
                //isColorCamera = ture;
            }
            else if (cameratype == "Depth")
            {
                mfsKind = MediaFrameSourceKind.Depth;
            }
            else if (cameratype == "FishEye")
            {
                mfsKind = MediaFrameSourceKind.Custom;
                isFishEyeCamera = true;
            }
        }

        public MediaFrameSourceKind GetSource_StrToEnum(string source)
        {
            switch (source)
            {
                case "Infrared":
                    return MediaFrameSourceKind.Infrared;
                case "Depth":
                    return MediaFrameSourceKind.Depth;
                case "Color":
                    return MediaFrameSourceKind.Color;
                default:
                    return MediaFrameSourceKind.Custom;

            }
        }

        public async Task<bool> CheckCameraGroups()
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
            foreach (var f in frameSourceGroups)
            {
                Console.WriteLine(f.DisplayName);
                Logger.AppendError(f.DisplayName);
            }
            if (frameSourceGroups.Count > 1)
            {
                if (frameSourceGroups.Select(x => x.DisplayName.ToLower().Contains("cameras")).Count() < 1)
                {
                    Logger.AppendError("Intel Cameras Not Found");
                    Console.WriteLine("Intel Cameras Not Found");
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> SelectCameraSourceWithoutMFSKind(bool iscolorCamera)
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

            var selectedGroupObjects = frameSourceGroups.Select(group =>
                new
                {
                    sourceGroup = group,
                    sourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                    {
                        //return iscolorCamera ? ((group.DisplayName.Contains("Color")) || (group.DisplayName.Contains("RGB")))
                        //:group.DisplayName.Contains("Depth");

                        if (isFishEyeCamera)
                            return group.DisplayName.ToLower().Contains("fisheye");
                        if (iscolorCamera)
                            return (group.DisplayName.Contains("Color") || group.DisplayName.Contains("RGB"));
                        else
                            return group.DisplayName.Contains("Depth");
                    })
                }).Where(t => t.sourceInfo != null).FirstOrDefault();

            if (iscolorCamera)
            {
                colorGroup = selectedGroupObjects?.sourceGroup;
                colorSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = colorGroup?.DisplayName;
            }
            else if (!isFishEyeCamera)
            {
                depthGroup = selectedGroupObjects?.sourceGroup;
                depthSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = depthGroup?.DisplayName;
            }
            else
            {
                fishEyeGroup = selectedGroupObjects?.sourceGroup;
                fishEyeSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = depthGroup?.DisplayName;
            }
            return true;
        }

        public async Task<bool> SelectCameraSource(bool iscolorCamera)
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
            foreach(var f in frameSourceGroups)
            {
                Console.WriteLine("===>"+f.DisplayName);
                Console.WriteLine("===>" + f.Id);
                foreach (var s in f.SourceInfos)
                {
                    Console.WriteLine("/t==>" + s.SourceKind);
                }
            }
            var selectedGroupObjects = frameSourceGroups.Select(group =>
                new
                {
                    sourceGroup = group,
                    sourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                    {
                        Console.WriteLine(group.DisplayName);
                        if (group.DisplayName.Contains("SR300"))
                        {
                            if (outputFormat == "L8")
                            {
                                mfsKind = MediaFrameSourceKind.Infrared;
                            }
                            return iscolorCamera ? ((group.DisplayName.Contains("Color") && sourceInfo.SourceKind == mfsKind) || (group.DisplayName.Contains("RGB") && sourceInfo.SourceKind == mfsKind))
                            : (group.DisplayName.Contains("Depth") && sourceInfo.SourceKind == mfsKind);
                        }
                        else
                        {
                            if (isFishEyeCamera)
                            {
                                if (group.DisplayName.Contains("FishEye"))
                                {
                                    mfsKind = MediaFrameSourceKind.Custom;
                                    return sourceInfo.SourceKind == mfsKind;
                                }
                            }
                            else if (outputFormat == "L8" || outputFormat == "G49323159" || outputFormat == "G59565955" || outputFormat == "G20493859")
                            {
                                mfsKind = MediaFrameSourceKind.Color;
                            }
                            //return iscolorCamera ? ((group.DisplayName.Contains("Color") && sourceInfo.SourceKind == mfsKind) || (group.DisplayName.Contains("RGB") && sourceInfo.SourceKind == mfsKind))
                            return iscolorCamera ? (( group.DisplayName.Contains("Video Device") || group.DisplayName.Contains("Module RGB") || group.DisplayName.Contains("415 RGB")/*(group.DisplayName.Contains("Module RGB")|| group.DisplayName.Contains("RGB")*/) && sourceInfo.SourceKind == mfsKind) 
                            : (group.DisplayName.Contains("Depth") && sourceInfo.SourceKind == mfsKind);
                        }
                    })
                }).Where(t => t.sourceInfo != null).FirstOrDefault();


            Console.WriteLine("===>mfsKind : " + mfsKind);
            if (iscolorCamera)
            {
                colorGroup = selectedGroupObjects?.sourceGroup;
                colorSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = colorGroup?.DisplayName;
                Logger.AppendInfo("Color Group has been selected: " + colorGroup?.DisplayName);
            }
            else if (!isFishEyeCamera)
            {
                depthGroup = selectedGroupObjects?.sourceGroup;
                depthSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = depthGroup?.DisplayName;
                Logger.AppendInfo("Depth Group has been selected: " + depthGroup?.DisplayName);
            }
            else
            {
                fishEyeGroup = selectedGroupObjects?.sourceGroup;
                fishEyeSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = fishEyeGroup?.DisplayName;
                Logger.AppendInfo("FishEye Group has been selected: " + fishEyeGroup.DisplayName);
            }

            if (colorGroup == null && depthGroup == null && fishEyeGroup == null)
            {
                Logger.AppendInfo("Unable to select source group");
                cameraInitialiatizationPassed = false;
                return false;
            }
            Logger.AppendInfo("Succesfully selected source group");
            return true;
        }

        public async Task<bool> InitializeMediaCapture(bool iscolorCamera)
        {
            Console.WriteLine("initializeMediaCapture start");

            MediaCaptureInitializationSettings settings;
            try
            {
                if (iscolorCamera)
                {
                    if (colorGroup != null)
                    {
                        _colorMediaCapture = new MediaCapture();
                        settings = SetMediaCaptureSettings(colorGroup);
                        await _colorMediaCapture.InitializeAsync(settings);                       
                        Logger.AppendInfo("Color MediaCapture initialization succeeded");
                    }
                }
                else if (!isFishEyeCamera)
                {
                    if (depthGroup != null)
                    {
                        _depthMediaCapture = new MediaCapture();
                        settings = SetMediaCaptureSettings(depthGroup);
                        await _depthMediaCapture.InitializeAsync(settings);
                        depthFrameSource = _depthMediaCapture.FrameSources[depthSourceInfo.Id];
                        Logger.AppendInfo("Depth MediaCapture initialization succeeded");
                    }
                    else
                    {
                        Logger.AppendError("depthGroup is empty");
                        Console.WriteLine("depthGroup is empty");
                    }
                }
                else
                {
                    _fishEyeMediaCapture = new MediaCapture();
                    settings = SetMediaCaptureSettings(fishEyeGroup);
                    await _fishEyeMediaCapture.InitializeAsync(settings);
                    fishEyeFrameSource = _fishEyeMediaCapture.FrameSources[depthSourceInfo.Id];
                    Logger.AppendInfo("FeshEye MediaCapture initialization succeeded");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.AppendError("MediaCapture initialization failed: " + ex.Message);
                threadWait.Set();
                Console.WriteLine("initializeMediaCapture end false");
                return false;
            }
        }
        public IReadOnlyCollection<MediaFrameSourceGroup> GetAllGroups()
        {
            return Task.Run(async () =>
            {
                return await MediaFrameSourceGroup.FindAllAsync();
            }).GetAwaiter().GetResult();
        }
        public Dictionary<string, SortedSet<string>> camerasSortedByUSBConnection = new Dictionary<string, SortedSet<string>>();
        public void SortGroups()
        {
            var allConnectedGroups = GetAllGroups();
            foreach (var g in allConnectedGroups)
            {
                Console.WriteLine("==>" + g.Id);

                if (g.Id.Contains("#6&"))
                {
                    Console.WriteLine(g.Id.IndexOf("#6"));
                    Console.WriteLine(g.Id.LastIndexOf("#6&"));
                    Console.WriteLine(g.Id.IndexOf("&", g.Id.LastIndexOf("#6&") + 2));
                    string _USBId = g.Id.Substring(g.Id.IndexOf("#6&"), g.Id.IndexOf("&", g.Id.IndexOf("#6&") + 3));

                    if (camerasSortedByUSBConnection.ContainsKey(_USBId))
                    {
                        camerasSortedByUSBConnection[_USBId].Add(g.DisplayName);
                    }
                    else
                    {
                        camerasSortedByUSBConnection.Add(_USBId, new SortedSet<string> { g.DisplayName });
                    }
                }
            }
        }
        public async Task<bool> SelectCameraSource()
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
            foreach (var f in frameSourceGroups)
            {
                Console.WriteLine("Id: " + f.Id);
                Console.WriteLine("DisplayName: " + f.DisplayName);
                Console.WriteLine("**");
                foreach (var s in f.SourceInfos)
                {
                    Console.WriteLine("SourceGroup: " + s.SourceGroup.DisplayName);
                    Console.WriteLine("SourceKind: " + s.SourceKind);
                    Console.WriteLine("********");
                }
                Console.WriteLine("***********************************");
            }
            var selectedGroupObjects = frameSourceGroups.Select(group =>
               new
               {
                   sourceGroup = group,
                   sourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                   {
                       return group.DisplayName.Contains(cameratype) && sourceInfo.SourceKind == mfsKind;
                   })
               }).Where(t => t.sourceInfo != null).FirstOrDefault();

            Console.WriteLine(" Defualt Sensor group source is :" + selectedGroupObjects.sourceGroup.Id);
            Console.WriteLine(" Defualt Sensor group source info is :" + selectedGroupObjects.sourceInfo.SourceKind);
            Console.WriteLine(" Defualt Sensor group source name is :" + selectedGroupObjects.sourceGroup.DisplayName);
            if (mfsKind == MediaFrameSourceKind.Color)
            {
                colorGroup = selectedGroupObjects?.sourceGroup;
                colorSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = colorGroup?.DisplayName;
                isFishEyeCamera = false;
            }
            else if (mfsKind == MediaFrameSourceKind.Depth)
            {
                depthGroup = selectedGroupObjects?.sourceGroup;
                depthSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = depthGroup?.DisplayName;
                isFishEyeCamera = false;
            }
            else
            {
                fishEyeGroup = selectedGroupObjects?.sourceGroup;
                fishEyeSourceInfo = selectedGroupObjects?.sourceInfo;
                productName = fishEyeGroup?.DisplayName;
                isFishEyeCamera = true;
            }

            return true;
        }
        private MediaCaptureInitializationSettings SetMediaCaptureSettings(MediaFrameSourceGroup sg)
        {
            return new MediaCaptureInitializationSettings()
            {
                SourceGroup = sg,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
        }
        public void SetControlValue()
        {
            TimeSpan ts = new TimeSpan((long)40 * 10);

            Task.Run(async () =>
            {
                await _fishEyeMediaCapture?.VideoDeviceController.ExposureControl.SetValueAsync(ts);
            }).GetAwaiter().GetResult();

        }

        private async Task<bool> CallSetFormat(bool iscolorCamera, MediaCapture mediaCapture, MediaFrameSource frameSource, MediaFrameSourceInfo sourceInfo)
        {
            Console.WriteLine("*****SetControl*****");
            SetControl.setup(_colorMediaCapture, _depthMediaCapture, _fishEyeMediaCapture);

            frameSource = mediaCapture.FrameSources[sourceInfo.Id];
           
            var preferredFormat = frameSource.SupportedFormats.Where(format =>
            {
                return format.VideoFormat.Width == videoFormatWidth
                && format.VideoFormat.Height == videoFormatHeight
                && format.FrameRate.Numerator == GetFPS(FPS)
                && format.FrameRate.Denominator == GetDenominator(FPS)
                && format.Subtype == GetFormat(outputFormat);
            }).FirstOrDefault();

            Console.WriteLine("Format: " + outputFormat + " videoFormatWidth: " + videoFormatWidth + " videoFormatHeight: " + videoFormatHeight + " FPS: " + FPS + " Numerator: " + numerator + " Denominator: " + denominator + " ");
            Logger.AppendInfo("Format: " + outputFormat + "videoFormatWidth: " + videoFormatWidth + " videoFormatHeight: " + videoFormatHeight + " FPS: " + FPS + " Numerator: " + numerator + " Denominator: " + denominator + " ");

            if (preferredFormat == null)
            {
                Console.WriteLine("Format is not supported");
                Logger.AppendError("Format is not supported: " + outputFormat + " videoFormatWidth: " + videoFormatWidth + " videoFormatHeight: " + videoFormatHeight + " FPS: " + FPS + " Numerator: " + numerator + " Denominator: " + denominator);
                return false;
            }
            try
            {
                if (iscolorCamera)
                {
                    colorFrameSource = mediaCapture.FrameSources[sourceInfo.Id];
                    await colorFrameSource.SetFormatAsync(preferredFormat);
                }
                else if (!isFishEyeCamera)
                {
                    depthFrameSource = mediaCapture.FrameSources[sourceInfo.Id];
                    await depthFrameSource.SetFormatAsync(preferredFormat);
                }
                else
                {
                    fishEyeFrameSource = mediaCapture.FrameSources[sourceInfo.Id];
                    await fishEyeFrameSource.SetFormatAsync(preferredFormat);
                }
                if (!isPropertiesWhileStreamingTestRunning && controlsL != null && controlsL.Count > 0)
                {
                    foreach (var c in controlsL)
                    {
                        SetControl.setPropertyToSpecificValue(c.Key, (int)c.Value);
                        var _value = SetControl.GetControlValue(c.Key);
                        if (exceptionError)
                        {
                            Task.Run(async () =>
                            {
                                await CleanUp();
                            }).GetAwaiter().GetResult();
                            Console.WriteLine("stop Frame");
                            threadWait.Set();
                            return false;
                        }
                        int i = 0;
                        do
                        {
                            Logger.AppendDebug($"==> Control {c.KeyAsString}  value set to {_value}");
                            Thread.Sleep(1000);
                            _value = SetControl.GetControlValue(c.Key);
                            i++;
                        } while ((int)_value != (int)c.Value && i < 5);                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.AppendException(e.Message);
            }
            return true;
        }

        public static bool exceptionError = false;
        public void SetFaceAuthControl(List<TestSetup.ControlValues> controlsL)
        {
            SetControl.setup(_colorMediaCapture, _depthMediaCapture, _fishEyeMediaCapture);
            Console.WriteLine("Set Control Face Auth");
            Logger.AppendInfo("Set Control Face Auth");
           
            if (controlsL != null && controlsL.Select(x => x.ControlName == "DEPTH_FACE_AUTH") != null)
            {
                Console.WriteLine("Found: Control Face Auth");
                Logger.AppendInfo("Found: Control Face Auth");
                SetControl.setPropertyToSpecificValue(controlsL[0].Control, 0); //this control's value will not be used ,0 meanless 
            }
        }

        public void CallSetControl(List<TestSetup.ControlValues> controlsL)
        {
            SetControl.setup(_colorMediaCapture, _depthMediaCapture, _fishEyeMediaCapture);
            Console.WriteLine("Set Control Face Auth");
            Logger.AppendInfo("Set Control Face Auth");
            if (controlsL != null && controlsL.Count > 0)
                foreach (var c in controlsL)
                {
                    if (c.ControlName != "DEPTH_FACE_AUTH")
                    {
                        SetControl.setPropertyToSpecificValue(c.Control, (int)c.ArrayValues[0]);
                    }
                }
        }

        public async Task<bool> SetFrameFormat(bool iscolorCamera)
        {
            if (iscolorCamera && _colorMediaCapture != null)
            {
                return await CallSetFormat(iscolorCamera, _colorMediaCapture, colorFrameSource, colorSourceInfo);
            }
            else if (_depthMediaCapture != null)
            {
                return await CallSetFormat(iscolorCamera, _depthMediaCapture, depthFrameSource, depthSourceInfo);
            }
            else if (isFishEyeCamera && _fishEyeMediaCapture != null)
            {
                return await CallSetFormat(iscolorCamera, _fishEyeMediaCapture, fishEyeFrameSource, fishEyeSourceInfo);
            }
            return false;
        }
        private async Task<bool> CallCreateFrameReader(MediaCapture mediaCapture, MediaFrameReader mediaFrameReader, MediaFrameSource mediaFrameSource)
        {
            try
            {
                mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("Frame format not accepted: " + e.Message);
            }
            if (!isPropertiesWhileStreamingTestRunning)
            {
                mediaFrameReader.FrameArrived += FrameReader_FrameArrived;
            }
            else
            {
                mediaFrameReader.FrameArrived += FrameReader_FrameArrived_PropertiesWhileStreaming;
            }

            Console.WriteLine("StartAsync");
            MediaFrameReaderStartStatus result = await mediaFrameReader.StartAsync();
            if (result != MediaFrameReaderStartStatus.Success)
            {
                Logger.AppendInfo($"Start reader with result: {result}");
                cameraInitialiatizationPassed = false;
                return false;
            }
            return true;
        }

        /*
        * the callback for the thread: StartWhileMeasurementTempStreaming, happens at the end of every running thread
        * we will try not to make the computer work hard here or we will have weird bugs
        */
        private async Task<bool> CallCreateFrameReaderTemperature(MediaCapture mediaCapture, MediaFrameReader mediaFrameReader, MediaFrameSource mediaFrameSource)
        {
            try
            {
                mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("Frame format not accepted: " + e.Message);
            }
            mediaFrameReader.FrameArrived += FrameReader_FrameArrived_Temperature;
            
            //listen to thermal event, if event occures change isSuccess to true
            mediaCapture.ThermalStatusChanged += _mediaCapture_ThermalStatusChanged;
            Thread.Sleep(1000);            
            Console.WriteLine("StartAsync");
            //watchStartStreaming.Start();
            MediaFrameReaderStartStatus result = await mediaFrameReader.StartAsync();
            if (result != MediaFrameReaderStartStatus.Success)
            {
                Logger.AppendInfo($"Start reader with result: {result}");
                cameraInitialiatizationPassed = false;
                return false;
            }
            return true;
        }


        private async Task<bool> CallCreateFrameReader_GetFrameMetadata(MediaCapture mediaCapture, MediaFrameReader mediaFrameReader, MediaFrameSource mediaFrameSource)
        {
            try
            {
                mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("Frame format not accepted: " + e.Message);
            }
            mediaFrameReader.FrameArrived += FrameReader_FrameArrived_GetFrameMetadata;

            Console.WriteLine("StartAsync");
            //watchStartStreaming.Start();
            MediaFrameReaderStartStatus result = await mediaFrameReader.StartAsync();
            if (result != MediaFrameReaderStartStatus.Success)
            {
                Logger.AppendInfo($"Start reader with result: {result}");
                cameraInitialiatizationPassed = false;
                return false;
            }
            return true;
        }
        public async Task<bool> CreateFrameReader_GetFrameMetadata()
        {
            return await CallCreateFrameReader_GetFrameMetadata(_depthMediaCapture, _depthMediaFrameReader, depthFrameSource);
        }
        private async Task<bool> CallCreateFrameReader_DoValidationWhileStreaming(MediaCapture mediaCapture, MediaFrameReader mediaFrameReader, MediaFrameSource mediaFrameSource)
        {
            try
            {
                mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("Frame format not accepted: " + e.Message);
            }
            mediaFrameReader.FrameArrived += FrameReader_FrameArrived_DoValidationWhileStreaming;

            Console.WriteLine("StartAsync");
            //watchStartStreaming.Start();
            MediaFrameReaderStartStatus result = await mediaFrameReader.StartAsync();
            if (result != MediaFrameReaderStartStatus.Success)
            {
                Logger.AppendInfo($"Start reader with result: {result}");
                cameraInitialiatizationPassed = false;
                return false;
            }
            return true;
        }
        public async Task<bool> CreateFrameReader_DoValidationWhileStreaming()
        {
            return await CallCreateFrameReader_DoValidationWhileStreaming(_depthMediaCapture, _depthMediaFrameReader, depthFrameSource);
        }
        public async Task<bool> CreateFrameReader(bool colorCamera)
        {
            if (isFishEyeCamera)
            {
                return await CallCreateFrameReader(_fishEyeMediaCapture, _fishEyeMediaFrameReader, fishEyeFrameSource);
            }
            if (colorCamera)
            {
                return await CallCreateFrameReader(_colorMediaCapture, _colorMediaFrameReader, colorFrameSource);
            }
            else
            {
                return await CallCreateFrameReader(_depthMediaCapture, _depthMediaFrameReader, depthFrameSource);
            }
        }

        /*
         * calls the callback, CallCreateFrameReaderTemperature, of the thread: StartWhileMeasurementTempStreaming
         * will use the right camera according to the boolean it gets.
        */
        public async Task<bool> CreateFrameReaderTemperature(bool colorCamera)
        {
            if (isFishEyeCamera)
            {
                return await CallCreateFrameReaderTemperature(_fishEyeMediaCapture, _fishEyeMediaFrameReader, fishEyeFrameSource);
            }
            if (colorCamera)
            {
                return await CallCreateFrameReaderTemperature(_colorMediaCapture, _colorMediaFrameReader, colorFrameSource);
            }
            else
            {
                return await CallCreateFrameReaderTemperature(_depthMediaCapture, _depthMediaFrameReader, depthFrameSource);
            }
        }

        private void _mediaCapture_ThermalStatusChanged(MediaCapture sender, object args)
        {
            //in order to avoid notifying about the heat more then once
            if (highTempNotified)
            {
                return;
            }

            highTempNotified = true;

            //the notification to indicate that thermal heat changed (by simulation or it's very hot)
            Console.WriteLine("temperature changed!");

            //instead of throwing an exception we will get the result of the test here.
            //throw new Exception("temperature too high!!");

            //result of the test
            isSuccess = true;


            /*
             * Besudo's original code for the user notification, maybe we will want to use in future
             * 
            //if (highTempNotified)
            //{
            //    return;
            //}
            //highTempNotified = true;
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            //async () =>
            //{
            //    MediaCaptureThermalStatus s = sender.ThermalStatus;
            //    var dialog = new Windows.UI.Popups.MessageDialog("Thermal status = " + s.ToString());
            //    await dialog.ShowAsync();
            //    highTempNotified = false;
            //});
            */
        }

        //this function will get a string of a temperature in hexadecimal and return a string of it in decimal
        private string GetTempFromResult(string val)
        {
            int Start;
            if (val.Contains('>'))
            {
                Start = val.IndexOf('>', 0) + 1;
                val = val.Substring(Start);
            }
            else
            {
                return "the line does not have the sign '>' need to modify the GetTempFromResult function";
            }

            Console.WriteLine("found the temperature in hexa: " + val);  //the temperature in hexa

            val = Int32.Parse(val, NumberStyles.HexNumber).ToString(); //converting to decimal
            Console.WriteLine("found the temperature in decimal: " + val);  //the temperature in decimal

            return val;
        }
        
        //the callback used for the imuTest
        private void FrameReader_FrameArrived_Temperature(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            //Console.WriteLine("FRMAE ARRIVED");

            //var winUSBDevice = new HWMonitorDevice("8086", "0AD2", DSGUID, 1); //setting of the command.. like Sam's code
            //var parser = new CommandsXmlParser("Commands.xml"); //the xml file where you find all commands availble
            //CommandResult result = parser.SendCommand(winUSBDevice, "GTEMP"); //insert the HW command, the same as typing in the Terminal
            //string val = result.FormatedString; //the result of the command, returns the the result if the command was succesfull
            //val = GetTempFromResult(val); //leaving only the number, changed from hexadecimal to decimal number- should be between 0 to 70
            //Console.WriteLine("temp = " + val);



            ////simulate high temperature!! will call the function: _mediaCapture_ThermalStatusChanged

            //CommandResult result_Error = parser.SendCommand(winUSBDevice, "DEPTH_EU_ERROR_SET 2");
            //CommandResult result_Error2 = parser.SendCommand(winUSBDevice, "DEPTH_EU_ERROR_SET 1");
            //string val_Error = result_Error.FormatedString;
            //if(val_Error != "")
            //    Console.WriteLine("Error: " + val_Error);


            //watchStartStreaming.Stop();
            //MediaFrameReference currentFrame = null;
            //try
            //{
            //    currentFrame = sender.TryAcquireLatestFrame();
            //}
            //catch (ObjectDisposedException)
            //{
            //    Console.WriteLine("Disposed");
            //    return;
            //}
            //if (currentFrame == null)
            //{
            //    Console.WriteLine("--- No latest Frame--- ");
            //    return;
            //}

            //if (frameCount == 0)
            //{
            //    frameTimeType.Add(currentFrame.SystemRelativeTime.Value.TotalMilliseconds, "0");
            //}
            //else
            //{
            //    frameTimeType.Add(currentFrame.SystemRelativeTime.Value.TotalMilliseconds, currentFrame.SourceKind.ToString());
            //    //Console.WriteLine("===>" + currentFrame.SystemRelativeTime.Value.TotalMilliseconds);
            //}
            //currentFrame.Dispose();
            //frameCount++;

            //if (frameCount == numberOfFramesToCollect)
            //{
            //    delay = watchStartStreaming.ElapsedMilliseconds;
            //    Console.WriteLine("====>ElapsedMilliseconds: " + delay);
            //    Console.WriteLine("Total time between start async and first fram is : " + delay);
            //    if (delay > 500)
            //    {
            //        isFirstFrameArrivedLate = true;
            //        Logger.AppendError("Start Stream Time Until First Frame Arrived is Longer Than 500 Millisecond : " + delay);
            //    }
            //    Task.Run(async () =>
            //    {
            //        await CleanUp();
            //    }).GetAwaiter().GetResult();
            //    Console.WriteLine("stop Frame");
            //    threadWait.Set();
            //}
        }

        public System.Diagnostics.Stopwatch frameArrivedTime = new System.Diagnostics.Stopwatch();
        public List<long> lst_Framarrivedtime = new List<long>();

        Guid hwTimestampGuid = new Guid("D3C6ABAC-291A-4C75-9F47-D7B284A52619");
        Guid metadataIntelCaptureTimeGuid = new Guid("2BF10C23-BF48-4C54-B1F9-9BB19E70DB05");

        int foundGuid = 0;

        private async Task Add_Frame(MediaFrameReference currentFrame)
        {
            if (currentFrame == null)
            {
                Console.WriteLine("--- No latest Frame--- ");
                return;
            }
            try
            {
                Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
                tempFrame.systemRelativeTime = currentFrame.SystemRelativeTime.Value.TotalMilliseconds;
                tempFrame.SensorType = currentFrame.SourceKind;
                foreach (var fp in currentFrame.Properties)
                {
                    if (fp.Key == hwTimestampGuid)
                    {
                        tempFrame.hwTimeStamp = fp.Value;
                        foundGuid++;
                        if (foundGuid == 2)
                            break;
                    }
                    else if (fp.Key == metadataIntelCaptureTimeGuid)
                    {
                        tempFrame.intelCaptureTime = fp.Value;
                        foundGuid++;
                        if (foundGuid == 2)
                            break;
                    }
                }

                currentFrame.Dispose();
                frameCount++;

                watchStartStreaming.Stop();
                tempFrame.callBackTime = watchStartStreaming.ElapsedMilliseconds;
                lst_Frames.Add(tempFrame);

                if (frameCount == numberOfFramesToCollect)
                {
                    Task.Run(async () =>
                    {
                        await CleanUp();
                    }).GetAwaiter().GetResult();
                    Console.WriteLine("stop Frame");
                    threadWait.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failure " + e.Message);
                Logger.AppendException("Failure " + e.Message);
            }
        }

        
        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            //watchStartStreaming.Start();
            MediaFrameReference currentFrame = null;
            try
            {
                currentFrame = sender.TryAcquireLatestFrame();
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Disposed");
                return;
            }


            Task.Run(async () =>
                    {
                        await Add_Frame(currentFrame);
                    });


            // *********

            //if (currentFrame == null)
            //{
            //    Console.WriteLine("--- No latest Frame--- ");
            //    return;
            //}
            //try
            //{
            //    Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
            //    tempFrame.systemRelativeTime = currentFrame.SystemRelativeTime.Value.TotalMilliseconds;
            //    tempFrame.SensorType = currentFrame.SourceKind;
            //    foreach (var fp in currentFrame.Properties)
            //    {
            //        if (fp.Key == hwTimestampGuid)
            //        {
            //            tempFrame.hwTimeStamp = fp.Value;
            //            foundGuid++;
            //            if (foundGuid == 2)
            //                break;
            //        }
            //        else if (fp.Key == metadataIntelCaptureTimeGuid)
            //        {
            //            tempFrame.intelCaptureTime = fp.Value;
            //            foundGuid++;
            //            if (foundGuid == 2)
            //                break;
            //        }
            //    }

            //    currentFrame.Dispose();
            //    frameCount++;

            //    watchStartStreaming.Stop();
            //    tempFrame.callBackTime = watchStartStreaming.ElapsedMilliseconds;
            //    lst_Frames.Add(tempFrame);

            //    if (frameCount == numberOfFramesToCollect)
            //    {
            //        Task.Run(async () =>
            //        {
            //            await CleanUp();
            //        }).GetAwaiter().GetResult();
            //        Console.WriteLine("stop Frame");
            //        threadWait.Set();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Failure " + e.Message);
            //    Logger.AppendException("Failure " + e.Message);
            //}
        }

        private void FrameReader_FrameArrived_GetFrameMetadata(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming.Stop();
            MediaFrameReference currentFrame = null;
            try
            {
                currentFrame = sender.TryAcquireLatestFrame();
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Disposed");
                return;
            }
            if (currentFrame == null)
            {
                Console.WriteLine("--- No latest Frame--- ");
                return;
            }
            if (frameCount == 0)
            {
                lst_AllFrames.Add(currentFrame.Properties.Where(x => x.Key.ToString().ToUpper() == "634C0558-7E40-4546-81AB-50B2B5A73167").First().Value);
            }
            else
            {
                lst_AllFrames.Add(currentFrame.Properties.Where(x => x.Key.ToString().ToUpper() == "634C0558-7E40-4546-81AB-50B2B5A73167").First().Value);
            }
            currentFrame.Dispose();
            frameCount++;
            if (frameCount == numberOfFramesToCollect)
            {
                delay = watchStartStreaming.ElapsedMilliseconds;
                Console.WriteLine("====>ElapsedMilliseconds: " + delay);
                Console.WriteLine("Total time between start async and first fram is : " + delay);
                if (delay > 500)
                {
                    isFirstFrameArrivedLate = true;
                    Logger.AppendError("Start Stream Time Until First Frame Arrived is Longer Than 500 Millisecond : " + delay);
                }

                Task.Run(async () =>
                {
                    await CleanUp();
                }).GetAwaiter().GetResult();

                Console.WriteLine("stop Frame");
                threadWait.Set();
            }
        }
        
        private void FrameReader_FrameArrived_DoValidationWhileStreaming(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming.Stop();
            MediaFrameReference currentFrame = null;
            try
            {
                currentFrame = sender.TryAcquireLatestFrame();
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Disposed");
                return;
            }
            if (currentFrame == null)
            {
                Console.WriteLine("--- No latest Frame--- ");
                return;
            }
            if (frameCount == 0)
            {
                lst_AllFrames.Add(currentFrame.Properties.Where(x => x.Key.ToString().ToUpper() == "634C0558-7E40-4546-81AB-50B2B5A73167").First().Value);
            }
            else
            {
                lst_AllFrames.Add(currentFrame.Properties.Where(x => x.Key.ToString().ToUpper() == "634C0558-7E40-4546-81AB-50B2B5A73167").First().Value);
            }
            currentFrame.Dispose();
            frameCount++;


            if (frameCount == numberOfFramesToCollect / 2)
            {
                Console.WriteLine("FrameReader_FrameArrived_DoValidationWhileStreaming: set FaceAuth");
                List<TestSetup.ControlValues> lst_temp = new List<TestSetup.ControlValues>();
                TestSetup.ControlValues temp = new TestSetup.ControlValues("DEPTH_FACE_AUTH");
                lst_temp.Add(temp);
                SetFaceAuthControl(lst_temp);
            }

            if (frameCount == numberOfFramesToCollect)
            {

                Task.Run(async () =>
                {
                    await CleanUp();
                }).GetAwaiter().GetResult();

                Console.WriteLine("stop Frame");
                threadWait.Set();
            }
        }
        private void FrameReader_FrameArrived_PropertiesWhileStreaming(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming.Stop();
            MediaFrameReference currentFrame;
            try
            {
                currentFrame = sender.TryAcquireLatestFrame();
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Disposed");
                return;
            }
            //object rawData;
            //if (currentFrame.Properties.TryGetValue(new Guid(0x482f9b07, 0x3668, 0x43fe, 0xad, 0x28, 0xe3, 0xdb, 0x34, 0x63, 0xbc, 0xb9), out rawData)) ;
            //Console.WriteLine("SIZE OF FRAME PROPERTIES = " + Marshal.SizeOf((Byte[])rawData));
            bool success;
            if (productName.Contains("DS5") || productName.Contains("400") || productName.Contains("410") || productName.Contains("430"))
            {
                if (cameratype == "Depth")
                {
                    Guid g = new Guid(0x482f9b07, 0x3668, 0x43fe, 0xad, 0x28, 0xe3, 0xdb, 0x34, 0x63, 0xbc, 0xb9);
                    DS5DepthFrame d = new DS5DepthFrame();
                    d.timeStamp = currentFrame.SystemRelativeTime.Value.TotalMilliseconds;
                    d.properties = AttributeParser.Parse<RealsenseRS400MetaDataIntelDepthControl>(currentFrame.Properties, g, out success);
                    if (!success)
                    {
                        Logger.AppendError("Unable to retrieve depth frame metadata");
                    }
                    DS5DepthFrameList.Add(d);
                }
            }
            else if (productName.Contains("SR300"))
            {
                if (cameratype == "Depth")
                {
                    Guid g = new Guid(0x634c0558, 0x7e40, 0x4546, 0x81, 0xab, 0x50, 0xb2, 0xb5, 0xa7, 0x31, 0x67);
                    SR300DepthFrame d = new SR300DepthFrame();
                    d.properties = AttributeParser.Parse<RealsenseSR300DepthMetaData>(currentFrame.Properties, g, out success);
                    if (!success)
                    {
                        Logger.AppendError("Unable to retrieve depth frame metadata");
                    }
                    d.timeStamp = currentFrame.SystemRelativeTime.Value.TotalMilliseconds;
                    SR300DepthFrameList.Add(d);
                }
                else //color camera frame
                {
                    Guid g = new Guid(0x2f3bde21, 0x74d0, 0x4bad, 0xb2, 0xdd, 0x86, 0xa9, 0x98, 0x7e, 0xf, 0xc5);
                    SR300ColorFrame c = new SR300ColorFrame();
                    c.properties = AttributeParser.Parse<RealsenseSR300ColorMetaData>(currentFrame.Properties, g, out success);
                    if (!success)
                    {
                        Logger.AppendError("Unable to retrieve color frame metadata");
                    }
                    c.timeStamp = currentFrame.SystemRelativeTime.Value.TotalMilliseconds;
                    SR300ColorFrameList.Add(c);
                }
            }
            if (frameCount == 0)
            {
                frameTimeType.Add(currentFrame.SystemRelativeTime.Value.TotalMilliseconds, "0");
            }
            else
            {
                frameTimeType.Add(currentFrame.SystemRelativeTime.Value.TotalMilliseconds, currentFrame.SourceKind.ToString());
                //Console.WriteLine("time :" + currentFrame.SystemRelativeTime.Value.TotalMilliseconds +" type: "+ currentFrame.SourceKind.ToString());
                //Console.WriteLine("time :" + currentFrame.SystemRelativeTime.Value.TotalMilliseconds + " Index: " + frameCount + " arr: " + frameTimeType[frameCount]);                
            }
            currentFrame.Dispose();
            frameCount++;
            if (isSettingControlsFinished) //stop streaming once finished setting properties
            {
                Console.WriteLine("isSettingControlsFinished");
                delay = watchStartStreaming.ElapsedMilliseconds;
                Console.WriteLine("====>ElapsedMilliseconds: " + delay);
                Console.WriteLine("Total time between start async and first fram is : " + delay);
                if (delay > 500)
                {
                    isFirstFrameArrivedLate = true;
                    Logger.AppendError("Start Stream Time Until First Frame Arrived is Longer Than 500 Millisecond : " + delay);
                }
                Task.Run(async () =>
                {
                    await CleanUp();
                }).GetAwaiter().GetResult();
                Console.WriteLine("stop Frame");
                threadWait.Set();
            }
        }

        public bool VerifyIfMediaCaptureDisposed()
        {
            return !(_colorMediaCapture != null || _depthMediaCapture != null || _fishEyeMediaCapture != null);
        }
        public async Task CleanUp()
        {
            if (isPropertiesWhileStreamingTestRunning)
            {
                finishedSettingControls.WaitOne();
            }
            Console.WriteLine("****CLEANUP****");

            Logger.AppendInfo("Cleanup started");
            if (colorSourceInfo != null || _colorMediaCapture != null)
            {
                Logger.AppendInfo("Clean Color");
                if (_colorMediaFrameReader != null)
                {
                    Logger.AppendInfo("Color Stop Async");
                    await _colorMediaFrameReader?.StopAsync();
                    _colorMediaFrameReader.FrameArrived -= FrameReader_FrameArrived;
                    _colorMediaFrameReader?.Dispose();
                }
                _colorMediaCapture?.Dispose();
                _colorMediaCapture = null;
                colorGroup = null;
                colorSourceInfo = null;
                colorFrameSource = null;
                Logger.AppendInfo("End Color Cleanup");
            }
            if (depthSourceInfo != null || _depthMediaCapture != null)
            {
                Logger.AppendInfo("Clean Depth");
                if (_depthMediaFrameReader != null)
                {
                    Logger.AppendInfo("Depth Stop Async");
                    await _depthMediaFrameReader?.StopAsync();
                    _depthMediaFrameReader.FrameArrived -= FrameReader_FrameArrived;
                    _depthMediaFrameReader?.Dispose();
                }
                _depthMediaCapture?.Dispose();
                _depthMediaCapture = null;
                depthGroup = null;
                depthSourceInfo = null;
                depthFrameSource = null;
                Logger.AppendInfo("End Depth Cleanup");
            }

            if (fishEyeSourceInfo != null || _fishEyeMediaCapture != null)
            {
                Logger.AppendInfo("Clean FishEye");
                if (_fishEyeMediaFrameReader != null)
                {
                    Logger.AppendInfo("FishEye Stop Async");
                    await _fishEyeMediaFrameReader?.StopAsync();
                    _fishEyeMediaFrameReader.FrameArrived -= FrameReader_FrameArrived;
                    _fishEyeMediaFrameReader?.Dispose();
                }
                _fishEyeMediaCapture?.Dispose();
                _fishEyeMediaCapture = null;
                fishEyeGroup = null;
                fishEyeSourceInfo = null;
                fishEyeFrameSource = null;
                Logger.AppendInfo("End FishEye Cleanup");
            }

            if (isPropertiesWhileStreamingTestRunning)
            {
                finishedStreaming.Set();
            }
            Logger.AppendInfo("End Cleanup");
        }

        public void TerminateTest()
        {
            double bufferTime = numberOfFramesToCollect;
            if (productName != null && productName.Contains("SR300"))
            {
                if (cameratype.Contains("Depth"))
                {
                    bufferTime = analyser.GetExpectedDepthFpsResultInMotionVsRangeTradeoff("", FPS, 3, controlsL.Where(x => x.Name.ToUpper().Contains("MOTION")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First());
                }
                else
                {
                    bufferTime = analyser.GetExpectedColorFpsInExposureControl(FPS, controlsL.Where(x => x.Name.ToUpper().Contains("EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First());
                }
                float exposure = controlsL.Where(x => x.Name.ToUpper().Contains("EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                if (exposure == 0.0)
                {
                    bufferTime = numberOfFramesToCollect + 10;
                }
                if (isPropertiesWhileStreamingTestRunning)
                {
                    bufferTime += 25;
                }
            }
            if (!(bufferTime > 0))
            {
                bufferTime = 30;
            }
            Console.WriteLine("===>bufferTime " + bufferTime);
            int maxNumberOfSecondsToRunTest = (int)((double)numberOfFramesToCollect / FPS + bufferTime);
            if (threadWait.WaitOne((maxNumberOfSecondsToRunTest + 10) * 1000))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Logger.AppendInfo("Signaled\n");
                Console.ForegroundColor = ConsoleColor.White;
                ifAllFramesArrived = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.AppendInfo("Timedout\n");
                Console.ForegroundColor = ConsoleColor.White;
                ifAllFramesArrived = false;
            }
        }

        public void TerminateTestParallel()
        {
            Thread.Sleep(10 * 1000);
            ifAllFramesArrived = true;
            //if (Thread.Sleep((int)numberOfFramesToCollect/2 * 1000))
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Logger.AppendInfo("Signaled\n");
            //    Console.ForegroundColor = ConsoleColor.White;
            //    ifAllFramesArrived = true;
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Logger.AppendInfo("Timedout\n");
            //    Console.ForegroundColor = ConsoleColor.White;
            //    ifAllFramesArrived = false;
            //    Thread.Sleep(1000);
            //}
        }
        private int CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(double min, double max, double step)
        {
            return (int)(((max - min) / step) * SetControl.calculateMillisecondsToSleepBetweenSets((uint)FPS)) + 1000 /**1000 buffer time*/;
        }
        public async Task SetNonSpecificFrameFormat(bool isColorCamera)
        {
            try
            {
                if (isColorCamera)
                {
                    if (_colorMediaCapture != null)
                    {
                        colorFrameSource = _colorMediaCapture.FrameSources[colorSourceInfo.Id];

                        var preferredColorFormat = colorFrameSource.SupportedFormats.Where(format =>
                        {
                            return true;
                        }).FirstOrDefault();
                        if (preferredColorFormat == null)
                        {
                            Logger.AppendInfo("Error setting frame format");
                            cameraInitialiatizationPassed = false;
                            return;
                        }
                        else
                        {
                            Logger.AppendInfo("Successfully chosen color frame format");
                        }
                        await colorFrameSource.SetFormatAsync(preferredColorFormat);
                    }
                }
                else
                {
                    if (_depthMediaCapture != null)
                    {
                        depthFrameSource = _depthMediaCapture.FrameSources[depthSourceInfo.Id];
                        var preferredDepthFormat = depthFrameSource.SupportedFormats.Where(format =>
                        {
                            return true;
                        }).FirstOrDefault();
                        if (preferredDepthFormat == null)
                        {
                            Logger.AppendInfo("Error setting frame format");
                            cameraInitialiatizationPassed = false;
                            return;
                        }
                        else
                        {
                            Logger.AppendInfo("Successfully chosen depth frame format");
                        }
                        await depthFrameSource.SetFormatAsync(preferredDepthFormat);
                    }
                }
            }
            catch
            {
                Logger.AppendInfo("Exception: Unable to set frame format");
            }
        }
        
        public int GetFPS(int fps)
        {
            if (fps == 90)
            { return 10000000; }
            if (fps == 110)
            { return 10000000; }
            else if (fps == 6)
            { return 2000000; }
            else if (fps == 120)
            { return 10000000; }
            else
            { return fps; }
        }
        public int GetDenominator(int fps)
        {
            if (fps == 90)
            { return 111111; }
            if (fps == 110)
            { return 90909; }
            else if (fps == 6)
            { return 333333; }
            else if (fps == 120)
            { return 83333; }
            else
            { return 1; }
        }
        public string GetFormat(string format)
        {
            switch (format)
            {
                case "G59565955":
                    return "{59565955-0000-0010-8000-00AA00389B71}";
                case "G49323159":
                    return "{49323159-0000-0010-8000-00AA00389B71}";
                case "G20493859":
                    return "{20493859-0000-0010-8000-00AA00389B71}";
                case "G2036315A":
                    return "{2036315A-0000-0010-8000-00AA00389B71}";
                case "G38574152":
                    return "{38574152-1A66-A242-9065-D01814A8EF8A}";
                case "G36315752":
                    return "{36315752-1A66-A242-9065-D01814A8EF8A}";
                default:
                    return format;
            }
        }
        
        //public void SetFaceAuthControl()
        //{
        //    const string faceAuthStr = "{1CB79112-C0D2-4213-9CA6-CD4FDB927972} 35";
        //    Utils.KSCAMERA_EXTENDEDPROP_HEADER faceAuthData = new Utils.KSCAMERA_EXTENDEDPROP_HEADER();
        //    faceAuthData.Version = 1;
        //    faceAuthData.PinId = 1;
        //    faceAuthData.Size = 40;
        //    faceAuthData.Result = 0;
        //    faceAuthData.Capability = 3;
        //    faceAuthData.Flags = 2;

        //    Logger.AppendInfo("Set FaceAuth Control");

        //    byte[] bytes = Utils.PdoToBuffer<Utils.KSCAMERA_EXTENDEDPROP_HEADER>(faceAuthData);
        //    try
        //    {
        //        if (_depthMediaCapture != null && depthSourceInfo != null)
        //        {
        //            _depthMediaCapture.FrameSources[depthSourceInfo.Id].Controller.VideoDeviceController.SetDeviceProperty(faceAuthStr, bytes);
        //            //object value = _depthMediaCapture.FrameSources[depthSourceInfo.Id].Controller.VideoDeviceController.GetDeviceProperty(faceAuthStr); 
        //        }
        //        else
        //        {
        //            Logger.AppendError("Cannot set FaceAuth Control ,_depthMediaCapture is null or depthSourceInfo in null");
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        Logger.AppendException(e.Message);
        //    }
        //}
        public bool CheckFrameFormatAfterSettingFaceAuth()
        {
            Logger.AppendInfo("Check If Format is 360x360 , 60fps");
            Console.WriteLine("_depthMediaCapture: " + _depthMediaCapture);
            MediaFrameSource source = _depthMediaCapture.FrameSources[depthSourceInfo.Id];

            Console.WriteLine("source: " + source);
            Console.WriteLine("source.CurrentFormat: " + source.CurrentFormat);
            uint fps = source.CurrentFormat.FrameRate.Numerator / source.CurrentFormat.FrameRate.Denominator;
            uint width = source.CurrentFormat.VideoFormat.Width;
            uint height = source.CurrentFormat.VideoFormat.Height;
            Logger.AppendInfo("==> FPS: " + fps + "   Resulotion: " + width + "x" + height);
            Console.WriteLine("==> FPS: " + fps + "   Resulotion: " + width + "x" + height);
            if (fps != 60 || width != 360 || height != 360)
            {
                Logger.AppendError("FormatAfterSettingFaceAuth - Failed: first/default fromat should be 360x360 60fps");
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.AppendError("FormatAfterSettingFaceAuth - Failed: first/default fromat should be 360x360 60fps");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
            Logger.AppendInfo("FormatAfterSettingFaceAuth - Passed ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("FormatAfterSettingFaceAuth - Passed ");
            Console.ForegroundColor = ConsoleColor.White;
            return true;
        }
        public List<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA> GetFrameMetaData()
        {
            List<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA> metaDataList = new List<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>();
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                for (int i = 0; i < lst_AllFrames.Count; i++)
                {
                    Utils.REAL_SENSE_SR300_DEPTH_MMETADATA md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>((byte[])lst_AllFrames[i]);
                    metaDataList.Add(md);
                }
            }
            return metaDataList;
        }
        public List<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME> GetFrameMetaData_RS400()
        {
            List<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME> metaDataList = new List<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME>();
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                for (int i = 0; i < lst_AllFrames.Count; i++)
                {
                    Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME>((byte[])lst_AllFrames[i]);
                    metaDataList.Add(md);
                }
            }
            return metaDataList;
        }
        public List<double> GetHW_Timestamp()
        {
            List<double> hwTimestampList = new List<double>();
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                for (int i = 1; i < lst_AllFrames.Count - 1; i++)
                {
                    Utils.REAL_SENSE_SR300_DEPTH_MMETADATA md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>((byte[])lst_AllFrames[i]);
                    hwTimestampList.Add((double)md.timestamp);
                }
            }
            return hwTimestampList;
        }
        public List<double> GetHW_Timestamp_RS400()
        {
            List<double> hwTimestampList = new List<double>();
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                for (int i = 1; i < lst_AllFrames.Count - 1; i++)
                {
                    Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME>((byte[])lst_AllFrames[i]);
                    hwTimestampList.Add((double)md.opticalTimestamp);
                }
            }
            return hwTimestampList;
        }
        public bool IsIRPairTypeValueOk_FaceAuthOn()
        {
            bool isTestPass = false;
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                isTestPass = true;
                Utils.REAL_SENSE_SR300_DEPTH_MMETADATA previousMd = Utils.ByteArrayToStructure<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>((byte[])lst_AllFrames[0]);
                for (int i = 1; i < lst_AllFrames.Count - 1; i++)
                {
                    Utils.REAL_SENSE_SR300_DEPTH_MMETADATA md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>((byte[])lst_AllFrames[i]);
                    Logger.AppendInfo("frameCounter :" + md.frameCounter + "   IrPairType :" + md.IrPairType + " HW  Frame Timestamp :" + md.timestamp + "     frame delta time stamp: " + (md.timestamp - previousMd.timestamp));
                    Console.WriteLine("frameCounter :" + md.frameCounter + "   IrPairType :" + md.IrPairType + " HW  Frame Timestamp :" + md.timestamp + "     frame delta time stamp: " + (md.timestamp - previousMd.timestamp));
                    if ((md.IrPairType + previousMd.IrPairType) != 1)
                    {
                        Logger.AppendError($"IRPairType Values Validation Faild : Frame number {md.frameCounter} , IRPairType {md.IrPairType}");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"IRPairType Values Validation Faild : Frame number {md.frameCounter} , IRPairType {md.IrPairType}");
                        Console.ForegroundColor = ConsoleColor.White;
                        isTestPass = false;
                    }
                    previousMd = md;
                }
            }
            return isTestPass;
        }
        public bool IsIRPairTypeValueOk_FaceAuthOff()
        {
            int irPairSum = 0;
            if (lst_AllFrames != null && lst_AllFrames.Count > 0)
            {
                foreach (var mdObj in lst_AllFrames)
                {
                    Utils.REAL_SENSE_SR300_DEPTH_MMETADATA md = Utils.ByteArrayToStructure<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>((byte[])mdObj);
                    Logger.AppendInfo("frameCounter :" + md.frameCounter + "   IrPairType :" + md.IrPairType + " HW  Frame Timestamp :" + md.timestamp);
                    Console.WriteLine("frameCounter :" + md.frameCounter + "   IrPairType :" + md.IrPairType + " HW  Frame Timestamp :" + md.timestamp);
                    if (md.IrPairType != 0)
                    {
                        irPairSum++;
                        Logger.AppendError($"IRPairType Values Validation Faild : Frame number {md.frameCounter} , IRPairType {md.IrPairType}");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"IRPairType Values Validation Faild : Frame number {md.frameCounter} , IRPairType {md.IrPairType}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            if (irPairSum != 0 && irPairSum != lst_AllFrames.Count)
                return false;
            return true;
        }       
        public bool GetIsSuccess()
        {
            return isSuccess;
        }
    }
}