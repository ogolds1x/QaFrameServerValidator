using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Threading;
using System.Globalization;

namespace QAFrameServerValidator
{
    public class MultiFrameReader
    { 
        #region public  members
        public readonly AutoResetEvent threadWait = new AutoResetEvent(false);
        public bool exceptionError = false;
        public FPS_Analysis analyser;
        public List<Utils.FRAME_DATA_RS400> FramesArrived = new List<Utils.FRAME_DATA_RS400>();
        public List<Utils.FRAME_DATA_RS400> FramesArrived_Color = new List<Utils.FRAME_DATA_RS400>();
        public List<Utils.FRAME_DATA_RS400> FramesArrived_Depth = new List<Utils.FRAME_DATA_RS400>();
        public System.Diagnostics.Stopwatch watchStartStreaming = new System.Diagnostics.Stopwatch();
        public System.Diagnostics.Stopwatch watchStartStreaming_Color = new System.Diagnostics.Stopwatch();
        public System.Diagnostics.Stopwatch watchStartStreaming_Depth = new System.Diagnostics.Stopwatch();
        //public MediaCapture mediaCapture;//for debug needs 
        #endregion

        #region private members
        private ManualResetEvent finishedStreaming = new ManualResetEvent(false);
        private ManualResetEvent mediaFrameReaderReady = new ManualResetEvent(false);
        private ManualResetEvent finishedCleaning = new ManualResetEvent(false);
        private Dictionary<MediaFrameSourceInfo, MediaFrameSourceGroup> dic_FrameInfoToFrameGroup = new Dictionary<MediaFrameSourceInfo, MediaFrameSourceGroup>();
        private Dictionary<string,MediaFrameReader> lst_MediaFrameReader = new Dictionary<string,MediaFrameReader>();
        private MediaCapture mediaCapture;
        private string cameraType;
        private string format;
        private uint fps;
        private uint videoFormatWidth;
        private uint videoFormatHeight;
        private List<Control> controls;
        private double exposure;
        private uint numerator;
        private uint denominator;
        private BasicProfile profile = null;
        private string depthVideoDeviceId;
        private string colorVideoDeviceId;
        private MediaFrameSourceKind kind;
        #endregion

        public MultiFrameReader()
        {
            mediaCapture = new MediaCapture();
            analyser = new FPS_Analysis();
        }

        public double Exposure
        {
            get { return exposure; }
            set { exposure = value; }
        }

        public async Task<bool> SelectCameraSource()
        {
            try
            {
                IReadOnlyList<MediaFrameSourceGroup> frameSourceGroups = null;               
                Console.WriteLine("Try Find All Groups");
                frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
                
                foreach (var fsg in frameSourceGroups)
                {                   
                    if (fsg.DisplayName.Contains("Cameras"))
                    {
                        foreach (var s in fsg.SourceInfos)
                        {
                            //Console.WriteLine("Id " + s.Id);
                            //Console.WriteLine("SourceKind " + s.SourceKind);
                            //Console.WriteLine("DeviceInformation " + s.DeviceInformation.Id);
                            //Console.WriteLine("DeviceInformation " + s.DeviceInformation.Name);

                            if (s.Id.Contains("#1@"))
                            {
                                depthVideoDeviceId = s.Id.Substring(s.Id.IndexOf("USB"));
                                depthVideoDeviceId = "\\\\?\\"+ depthVideoDeviceId;
                                dic_FrameInfoToFrameGroup.Add(s, s.SourceGroup);
                            }
                            else if (s.Id.Contains("#0@"))
                            {
                                colorVideoDeviceId = s.Id.Substring(s.Id.IndexOf("USB"));
                                colorVideoDeviceId = "\\\\?\\" + colorVideoDeviceId;
                                dic_FrameInfoToFrameGroup.Add(s, s.SourceGroup);
                            }
                        }
                    }                  
                }
                Console.WriteLine("Source Groups found:" + dic_FrameInfoToFrameGroup.Count);
                Logger.AppendDebug("Source Groups found:" + dic_FrameInfoToFrameGroup.Count);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("exception : " + e.Message);
                return false;
            }         
        }
  
        private MediaCaptureInitializationSettings SetMediaCaptureSettings(MediaFrameSourceGroup sg,string videoId)
        {
            return new MediaCaptureInitializationSettings()
            {
                VideoDeviceId = videoId,
                SourceGroup = sg,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
        }

        private MediaCaptureInitializationSettings _SetMediaCaptureSettings(MediaFrameSourceGroup sg)
        {
            return new MediaCaptureInitializationSettings()
            {
                SourceGroup = sg,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video

            };
        }

        public async Task CreateFrameReader()
        {
            //Set media Capture and Modify Controls                      
            Task.Run(async () =>
            {                
                try
                {
                    MediaCaptureInitializationSettings settings = SetMediaCaptureSettings(dic_FrameInfoToFrameGroup.First().Value, colorVideoDeviceId);
                    await mediaCapture?.InitializeAsync(settings);
                    SetContolsValues("Color");

                    mediaCapture.Dispose();
                    mediaCapture = new MediaCapture();
                   
                    settings = SetMediaCaptureSettings(dic_FrameInfoToFrameGroup.First().Value, depthVideoDeviceId);
                    await mediaCapture?.InitializeAsync(settings);
                    SetContolsValues("Depth");

                    mediaCapture.Dispose();
                    mediaCapture = new MediaCapture();

                    settings = _SetMediaCaptureSettings(dic_FrameInfoToFrameGroup.First().Value);
                    await mediaCapture?.InitializeAsync(settings);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception "+e.Message);
                }
            }).Wait();

            // Set support format + start stream
            foreach (var f in dic_FrameInfoToFrameGroup/*.Reverse()*/)
            {
                Task.Run(async () =>
                {
                    MediaFrameSourceInfo info = f.Key;
                    if (info != null)
                    {
                        Console.WriteLine("INFO group"+info.SourceGroup.DisplayName);
                        Console.WriteLine("INFO source"+info.SourceKind);
                        
                        //foreach (var m in mediaCapture.FrameSources)
                        //{
                        //    Console.WriteLine("k-" + m.Key);
                        //}

                        MediaFrameSource frameSource = null;
                        if (mediaCapture.FrameSources.TryGetValue(info.Id, out frameSource))
                        {
                            GetConfigByKind(info);
                            try
                            {
                                var preferredFormat = frameSource.SupportedFormats.Where(format =>
                                {
                                    return format.VideoFormat.Width == videoFormatWidth
                                    && format.VideoFormat.Height == videoFormatHeight &&
                                    format.FrameRate.Numerator == numerator
                                    && format.FrameRate.Denominator == denominator;
                                }).FirstOrDefault();
                                await frameSource.SetFormatAsync(preferredFormat);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            Console.WriteLine("=>frameSource " + frameSource.Info.SourceKind);
                            MediaFrameReader frameReader = await mediaCapture.CreateFrameReaderAsync(frameSource);
                            if (info.SourceKind == MediaFrameSourceKind.Color)
                            {
                                frameReader.FrameArrived += FrameReader_ColorArrived;
                                lst_MediaFrameReader.Add("Color",frameReader);
                            }
                            else if (info.SourceKind == MediaFrameSourceKind.Depth)
                            {
                                frameReader.FrameArrived += FrameReader_DepthArrived;
                                lst_MediaFrameReader.Add("Depth", frameReader);
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine($"Unable to start {info.SourceKind} reader. Frame source not found");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"MediaFrameSourceInfo {f.Key} is null.");
                    }                                
                }).Wait();
            }
            foreach(var fr in lst_MediaFrameReader)
            {
                Task.Run(async () => 
                {
                    mediaFrameReaderReady.Set();
                    MediaFrameReaderStartStatus status = await fr.Value.StartAsync();
                    Console.WriteLine("Start Async status" + status.ToString());
                }).Wait();
            }

            Thread.Sleep(Program.streamTimeInMilliseconds);
            foreach (var mf in lst_MediaFrameReader)
            {
                Task.Run(async () =>
                {  
                    await mf.Value.StopAsync();
                    if (mf.Key =="Color")
                    {
                        mf.Value.FrameArrived -= FrameReader_ColorArrived;
                    }
                    else if (mf.Key == "Depth")
                    { 
                        mf.Value.FrameArrived -= FrameReader_DepthArrived;
                    }
                    mf.Value.Dispose();
                    
                    Console.WriteLine("Stop Stream");
                }).Wait();
            }
            finishedStreaming.Set();
            CleanupMedia();
        }
        Guid hwTimestampGuid = new Guid("D3C6ABAC-291A-4C75-9F47-D7B284A52619");
        Guid metadataIntelCaptureTimeGuid = new Guid("2BF10C23-BF48-4C54-B1F9-9BB19E70DB05");
        

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming.Start();
            using (var frame = sender.TryAcquireLatestFrame())
            {
                //To Do..
                //Console.WriteLine(counter + " Frame Arrived!");
                Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
                tempFrame.systemRelativeTime = frame.SystemRelativeTime.Value.TotalMilliseconds;
                tempFrame.SensorType = frame.SourceKind;
                //int foundGuid = 0;
                //foreach (var fp in frame.Properties)
                //{
                //    if (fp.Key == hwTimestampGuid)
                //    {
                //        tempFrame.hwTimeStamp = fp.Value;
                //        foundGuid++;
                //        if (foundGuid == 2)
                //            break;
                //    }
                //    else if(fp.Key == metadataIntelCaptureTimeGuid)
                //    {
                //        tempFrame.intelCaptureTime = fp.Value;
                //        foundGuid++;
                //        if (foundGuid == 2)
                //            break;
                //    }
                //}

                tempFrame.hwTimeStamp = frame.Properties.Where(x => x.Key == hwTimestampGuid).First().Value;
                tempFrame.intelCaptureTime = frame.Properties.Where(x => x.Key == metadataIntelCaptureTimeGuid).First().Value;
                //(UInt32)frame.Properties.Where(x => x.Key.ToString().ToUpper() == HW_TimeStampGuid).First().Value;
                watchStartStreaming.Stop();
                tempFrame.callBackTime = watchStartStreaming.ElapsedMilliseconds;
                FramesArrived.Add(tempFrame);
                frame.Dispose();
                //Console.WriteLine("=== " + watchStartStreaming.ElapsedMilliseconds);
            }
        }

        private void FrameReader_ColorArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming_Color.Start();
            using (var frame = sender.TryAcquireLatestFrame())
            {
                //Logger.AppendDebug(frame.SourceKind + " ===> " + frame.SystemRelativeTime.Value.TotalMilliseconds);
                //Console.WriteLine("Color @@@@@@@ "+frame.Format.FrameRate.Numerator);
                //Console.WriteLine("      @@@@@@@ " + frame.SystemRelativeTime.Value.TotalMilliseconds);
                Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
                tempFrame.systemRelativeTime = frame.SystemRelativeTime.Value.TotalMilliseconds;
                tempFrame.SensorType = frame.SourceKind;
                int foundGuid = 0;
                foreach (var fp in frame.Properties)
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
                watchStartStreaming_Color.Stop();
                tempFrame.callBackTime = watchStartStreaming_Color.ElapsedMilliseconds;
                FramesArrived_Color.Add(tempFrame);
                frame.Dispose();
            }
        }

        private void FrameReader_DepthArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming_Depth.Start();
            using (var frame = sender.TryAcquireLatestFrame())
            {
                //Logger.AppendDebug(frame.SourceKind + " ===> " + frame.SystemRelativeTime.Value.TotalMilliseconds);
                //Console.WriteLine("Depth @@@@@@@ " + frame.Format.FrameRate.Numerator);
                //Console.WriteLine("      @@@@@@@ " + (double)frame.SystemRelativeTime.Value.TotalMilliseconds);
                Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
                tempFrame.systemRelativeTime = frame.SystemRelativeTime.Value.TotalMilliseconds;
                tempFrame.SensorType = frame.SourceKind;
                int foundGuid = 0;
                foreach (var fp in frame.Properties)
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
                watchStartStreaming_Depth.Stop();
                tempFrame.callBackTime = watchStartStreaming_Depth.ElapsedMilliseconds;
                FramesArrived_Depth.Add(tempFrame);
                frame.Dispose();
            }
        }


        public void SetConfig(string _cameraType, string _format, uint _fps, uint _videoFormatWidth, uint _videoFormatHeight, List<Control> _controls)
        {
            Console.WriteLine("Set Configuration");
            fps = _fps;
            videoFormatWidth = _videoFormatWidth;
            videoFormatHeight = _videoFormatHeight;
            controls = _controls;
            cameraType = _cameraType;
            format = GetFormat(_format);
            SetMediaFrameSourceKind();
            denominator = (uint)GetDenominator((int)fps);
            numerator = (uint)GetFPS((int)fps);
            exposure = controls.Where(x => x.Name.ToUpper().Contains("EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
            SetMediaFrameSourceKind();          
        }


        public void SetConfig(BasicProfile _profile)
        {
            this.profile = _profile;
        }
        
        public void GetConfigByKind(MediaFrameSourceInfo info)
        {
            if(profile != null)
            {
                this.controls = profile.Controls;
                this.cameraType = profile.ProfileType.ToString();
                if (info.Id.ToString().Contains("#0@"))
                {
                    this.fps = (uint)profile.ColorFps;
                    this.videoFormatWidth =(uint)profile.ColorResolution.Width;
                    this.videoFormatHeight = (uint)profile.ColorResolution.Height;                   
                    this.format = GetFormat(profile.ColorMode.ToString());
                    this.denominator = (uint)GetDenominator((int)this.fps);
                    this.numerator = (uint)GetFPS((int)this.fps);
                    this.exposure = controls.Where(x => x.Name.ToUpper().Contains("COLOR_EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                    SetMediaFrameSourceKind();
                }
                else /*if(info.SourceKind.ToString().Contains("Depth") )*/
                {
                    if (info.Id.ToString().Contains("#1@"))
                    {
                        this.fps = (uint)profile.DepthFps;
                        this.videoFormatWidth = (uint)profile.DepthResolution.Width;
                        this.videoFormatHeight = (uint)profile.DepthResolution.Height;
                        this.format = GetFormat(profile.DepthMode.ToString());
                        this.denominator = (uint)GetDenominator((int)this.fps);
                        this.numerator = (uint)GetFPS((int)this.fps);
                        this.exposure = controls.Where(x => x.Name.ToUpper().Contains("DS5_DEPTH_EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                        SetMediaFrameSourceKind();
                    }
                    else if(info.Id.ToString().Contains("#2@"))
                    {
                        this.fps = (uint)profile.IRFps;
                        this.videoFormatWidth = (uint)profile.IRResolution.Width;
                        this.videoFormatHeight = (uint)profile.IRResolution.Height;
                        this.format = GetFormat(profile.IRMode.ToString());
                        this.denominator = (uint)GetDenominator((int)this.fps);
                        this.numerator = (uint)GetFPS((int)this.fps);

                        this.exposure = controls.Where(x => x.Name.ToUpper().Contains("DS5_DEPTH_EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                        //in case we are configure IR stream: the source group/type is depth and the source kind is color 
                        this.cameraType = "Color";
                        SetMediaFrameSourceKind();
                        this.cameraType = profile.ProfileType.ToString();
                    }
                    Console.WriteLine($"{this.format},{this.fps},{this.videoFormatHeight}x{this.videoFormatWidth}");
                    Logger.AppendDebug($"{this.format},{this.fps},{this.videoFormatHeight}x{this.videoFormatWidth}");
                }
            }
        }
        
        public void SetContolsValues(string type)
        {           
            if (type == "Depth")
            {                
                SetControl.setup(null, mediaCapture, null);
                foreach (var c in profile.Controls)
                {
                    if (c.KeyAsString.Contains("DEPTH"))
                    {
                        int valueToSet = (int)c.Value;
                        if (c.KeyAsString.Contains("EXPOSURE"))
                        {
                            valueToSet = GetExposureByFPS((int)this.fps);
                        }
                        SetControl.setPropertyToSpecificValue(c.Key, valueToSet);
                        var _value = SetControl.GetControlValue(c.Key);
                        int i = 0;
                        do
                        {
                            Logger.AppendDebug($"==> Control {c.KeyAsString}  value set to {_value}");
                            Thread.Sleep(1000);
                            _value = SetControl.GetControlValue(c.Key);
                            i++;
                        } while ((int)_value != (int)c.Value && i < 6);
                    }
                }
            }
            if (type =="Color")
            {               
                SetControl.setup(mediaCapture, null, null);
                foreach (var c in profile.Controls)
                {
                    if (!c.KeyAsString.Contains("DEPTH"))
                    {                        
                        SetControl.setPropertyToSpecificValue(c.Key, (int)c.Value);
                        var _value = SetControl.GetControlValue(c.Key);
                        int i = 0;
                        do
                        {
                            Logger.AppendDebug($"==> Control {c.KeyAsString}  value set to {_value}");
                            Thread.Sleep(1000);
                            _value = SetControl.GetControlValue(c.Key);
                            i++;
                        } while ((int)_value != (int)c.Value && i < 6);
                    }
                }
            }
            Console.WriteLine("Set Controls");      
        }

        private void CleanupMedia()
        {
            if (mediaCapture != null)
            {
                mediaCapture?.Dispose();
                mediaCapture = null;
                Console.WriteLine("Clean Media Capture");
            }
            else
            {
                Console.WriteLine("No media capture found");
            }
            finishedCleaning.Set();
        }      

        public ManualResetEvent GetFinishedStreaming()
        {
            return finishedStreaming;
        }
        public ManualResetEvent GetMediaFrameReaderReady()
        {
            return mediaFrameReaderReady;
        }
        public ManualResetEvent GetFinishedCleaning()
        {
            return finishedCleaning;
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

        public void SetMediaFrameSourceKind()
        {
            if (cameraType == "Color" || cameraType == "RGB")
            {
                kind = MediaFrameSourceKind.Color;
            }
            else if (cameraType == "Depth")
            {
                kind = MediaFrameSourceKind.Depth;
            }
            else if (cameraType == "FishEye")
            {
                kind = MediaFrameSourceKind.Custom;
            }
        }
        public int GetExposureByFPS(int fps)
        {
            Console.WriteLine("44444");
            switch (fps)
            {
                case 6:
                    return 140000;
                case 15:
                    return 64000;
                case 30:
                    return 32000;
                case 60:
                    return 15000;
                case 90:
                    return 10000;
                default:
                    return 32000;
            }
            return 32000;
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

    }
}
