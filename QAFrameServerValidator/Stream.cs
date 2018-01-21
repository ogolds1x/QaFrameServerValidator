using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Threading;

namespace QAFrameServerValidator
{
    public class ProfileType
    {
        public int width;
        public int height;
        public int numerator;
        public int denominator;
        public string sensor;
        public int fps;
        public string mode;

        public ProfileType(int w, int h,int n, int d, string s,int f, string m)
        {
            width = w;
            height = h;
            numerator = n;
            denominator = d;
            sensor = s;
            fps = f;
            mode = m;
        }
    }
    public class Stream
    {
        public List<Utils.FRAME_DATA_RS400> FramesArrived = new List<Utils.FRAME_DATA_RS400>();
        public System.Diagnostics.Stopwatch watchStartStreaming = new System.Diagnostics.Stopwatch();
        Guid hwTimestampGuid = new Guid("D3C6ABAC-291A-4C75-9F47-D7B284A52619");
        Guid metadataIntelCaptureTimeGuid = new Guid("2BF10C23-BF48-4C54-B1F9-9BB19E70DB05");
        Guid rS400MetaDataIntelConfiguration = new Guid("F93AF74F-3525-4172-BF69-32AD6EF81D7A");

        MediaCapture mediaCapture = new MediaCapture();
        MediaCapture depthMediaCapture = new MediaCapture();
        MediaCapture colorMediaCapture = new MediaCapture();

        MediaFrameSourceGroup depthSourceGroup = null;
        MediaFrameSourceGroup colorSourceGroup = null;
        MediaFrameSourceGroup selectedGroup = null;

        private BasicProfile profile = null;
        public List<ProfileType> lstProfileType = null;
        MediaFrameReader mFR = null;
        List<MediaFrameReader> streamingsToRun = new List<MediaFrameReader>();
        MediaFrameSource mediaSource=null;





        public Stream(BasicProfile _profile)
        {
            profile = _profile;
            lstProfileType = new List<ProfileType>();
            if(profile.ColorModeAsString !=null)
                lstProfileType.Add(new ProfileType(profile.ColorResolution.Width, profile.ColorResolution.Height, GetFPS(profile.ColorFps), GetDenominator(profile.ColorFps), "Color", profile.ColorFps, profile.ColorModeAsString));
            string depthSensor = "Depth";
            Console.WriteLine("==>" + profile.DepthModeAsString);
            if (profile != null && profile.DepthModeAsString.Contains("L8"))
            {
                depthSensor = "IR";
            }
            lstProfileType.Add(new ProfileType(profile.DepthResolution.Width, profile.DepthResolution.Height, GetFPS(profile.DepthFps), GetDenominator(profile.DepthFps), depthSensor, profile.DepthFps, profile.DepthModeAsString));
            if (profile.IRModeAsString != null)
                lstProfileType.Add(new ProfileType(profile.IRResolution.Width, profile.IRResolution.Height, GetFPS(profile.IRFps), GetDenominator(profile.IRFps), "IR", profile.IRFps, profile.IRModeAsString));
        }



        public async void SetSensorGroups(IReadOnlyList<MediaFrameSourceGroup> sourceGroupsList)
        {
            try
            {
                
                string groupName;
                foreach (var s in sourceGroupsList)
                {
                    groupName = s.DisplayName.ToString();

                    switch (groupName)
                    {
                        case "Intel(R) RealSense(TM) 415 RGB":
                        case "Intel(R) RealSense(TM) 430 with RGB Module RGB":
                            colorSourceGroup = s;
                            break;
                        case "Intel(R) RealSense(TM) 430 with RGB Module Depth":
                        case "Intel(R) RealSense(TM) 415 Depth":
                            depthSourceGroup = s;
                            break;
                        case "Intel RS400 Cameras":
                            selectedGroup = s;
                            break;

                    }                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("*** " + e.Message);
            }
        }

        public void FLow()
        {
            Task.Run(async () =>
            {
                IReadOnlyList<MediaFrameSourceGroup> sourceGroupsList = await MediaFrameSourceGroup.FindAllAsync();
                SetSensorGroups(sourceGroupsList);
                

                var mediaCaptureSettings = new MediaCaptureInitializationSettings()
                {
                    SourceGroup = selectedGroup,
                    SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                    StreamingCaptureMode = StreamingCaptureMode.Video
                };

                var depthMediaCaptureSettings = new MediaCaptureInitializationSettings()
                {
                    SourceGroup = depthSourceGroup,
                    SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                    StreamingCaptureMode = StreamingCaptureMode.Video
                };

                var colorMediaCaptureSettings = new MediaCaptureInitializationSettings()
                {
                    SourceGroup = colorSourceGroup,
                    SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                    StreamingCaptureMode = StreamingCaptureMode.Video
                };

                await mediaCapture?.InitializeAsync(mediaCaptureSettings);
                await depthMediaCapture?.InitializeAsync(depthMediaCaptureSettings);
                await colorMediaCapture?.InitializeAsync(colorMediaCaptureSettings);
                
            }).Wait();

            SetContolsValues("Color");
            SetContolsValues("Depth");

            foreach (var p in lstProfileType)
            {
                Task.Run(async () =>
                {
                    MediaFrameSourceKind sourceKind;
                    sourceKind = GetMediaFrameSourceKind(p.sensor);
                    MediaFrameSourceInfo mediaFrameSourceInfo = GetMediaSourceInfoFromGroup(mediaCapture, selectedGroup, sourceKind, out mediaSource);
                    await setFrameFormat(mediaSource, GetFormat(p.mode), (uint)p.width, (uint)p.height, p.fps, (uint)p.numerator, (uint)p.denominator);
                    mFR = await mediaCapture.CreateFrameReaderAsync(mediaSource);
                    mFR.FrameArrived += MFR_FrameArrived;

                    streamingsToRun.Add(mFR);

                }).Wait();
            }
           
            foreach (var stream in streamingsToRun)
            {
                Task.Run(async () =>
                {
                    await stream.StartAsync();
                }).Wait();
            }

            Thread.Sleep(Program.streamTimeInMilliseconds);
            
            foreach (var stream in streamingsToRun)
            {                
                Task.Run(async () =>
                {
                    await stream.StopAsync();
                    stream.FrameArrived -= MFR_FrameArrived;
                    stream.Dispose();
                }).Wait();
            }

            Task.Run(() =>
            {
                //Disopse media capture
                mediaCapture.Dispose();
                mediaCapture = null;
                mediaSource = null;
                
            }).Wait();

            //for pnp test
           Program._waitHandle.Set();

        }
        
        private void MFR_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            watchStartStreaming.Start();
            using (var frame = sender.TryAcquireLatestFrame())
            {
                //Console.WriteLine(frame.SourceKind + " @@@@ "+ frame.SystemRelativeTime.Value.TotalMilliseconds);
                Utils.FRAME_DATA_RS400 tempFrame = new Utils.FRAME_DATA_RS400();
                tempFrame.systemRelativeTime = frame.SystemRelativeTime.Value.TotalMilliseconds;                
                tempFrame.subType = frame.Format.Subtype;
                
                int foundGuid = 0;
                foreach (var fp in frame.Properties)
                {
                    if (fp.Key == hwTimestampGuid)
                    {
                        tempFrame.hwTimeStamp = fp.Value;
                        foundGuid++;
                        if (foundGuid == 3)
                            break;
                    }
                    else if (fp.Key == metadataIntelCaptureTimeGuid)
                    {
                        tempFrame.intelCaptureTime = fp.Value;
                        foundGuid++;
                        if (foundGuid == 3)
                            break;
                    }
                    else if (fp.Key == rS400MetaDataIntelConfiguration)
                    {
                        tempFrame.intelconfiguration = fp.Value;
                        foundGuid++;
                        if (foundGuid == 3)
                            break;
                    }
                }
                watchStartStreaming.Stop();
                tempFrame.callBackTime = watchStartStreaming.ElapsedMilliseconds;
                FramesArrived.Add(tempFrame);
            }
        }
        public void SetContolsValues(string type)
        {
            if (type == "Depth")
            {
                SetControl.setup(null, depthMediaCapture, null);
                foreach (var c in profile.Controls)
                {
                    if (c.KeyAsString.Contains("DEPTH"))
                    {
                        int valueToSet = (int)c.Value;
                        //if (c.KeyAsString.Contains("EXPOSURE"))
                        //{
                        //    valueToSet = GetExposureByFPS((int)this.profile.DepthFps);
                        //}
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

                // for Pnp only
                //int expValueToSet = GetExposureByFPS((int)this.profile.DepthFps);
                //SetControl.setPropertyToSpecificValue(Types.ControlKey.DS5_DEPTH_EXPOSURE, expValueToSet);

            }
            if (type == "Color")
            {
                SetControl.setup(colorMediaCapture, null, null);
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
        private MediaFrameSourceInfo GetMediaSourceInfoFromGroup(MediaCapture mC, MediaFrameSourceGroup group, MediaFrameSourceKind sK, out MediaFrameSource mFS)
        {
            mFS = null;
            var list = group?.SourceInfos.ToList();
            foreach (var sI in group?.SourceInfos)
            {
                mFS = mC?.FrameSources[sI.Id];

                if (sI.SourceKind == MediaFrameSourceKind.Color)
                {
                    if (sK == MediaFrameSourceKind.Infrared)
                    {
                        foreach (var item in mFS.SupportedFormats)
                        {
                            if (item.Subtype.ToLower().Contains("l8"))
                                return sI;
                        }
                    }
                    else if (sK == MediaFrameSourceKind.Color)
                    {
                        foreach (var item in mFS.SupportedFormats)
                        {
                            if (item.Subtype.ToLower().Contains("yuy2"))
                                return sI;
                        }
                    }
                }
                else if (sI.SourceKind == MediaFrameSourceKind.Depth)
                {
                    if (sK == MediaFrameSourceKind.Depth)
                    {
                        foreach (var item in mFS.SupportedFormats)
                        {
                            if (item.Subtype.ToLower().Contains("d16"))
                                return sI;
                        }
                    }
                }
            }
            return null;
        }
        private async Task<bool> setFrameFormat(MediaFrameSource mediaSource, string type,
           uint width, uint height, int fps, uint numerator, uint denominator)
        {
            Logger.AppendDebug("Set format " + type);
            var list = mediaSource.SupportedFormats.Where(format =>
            {
                return format.VideoFormat.Width == width
                && format.VideoFormat.Height == height
                && format.FrameRate.Numerator == numerator
                && format.FrameRate.Denominator == denominator
                && format.Subtype == type;
            }).ToList();

            if (list.Count == 0)
            {
                Logger.AppendError("Format not found!");
                return false;
            }

            await mediaSource.SetFormatAsync(list[0]);
            Console.WriteLine($"Get format : {mediaSource.CurrentFormat.Subtype},{mediaSource.CurrentFormat.VideoFormat.Width}x{mediaSource.CurrentFormat.VideoFormat.Height}");
            Logger.AppendDebug($"Get format : {mediaSource.CurrentFormat.Subtype},{mediaSource.CurrentFormat.VideoFormat.Width}x{mediaSource.CurrentFormat.VideoFormat.Height}");
            Console.WriteLine("******** FPS is :" + mediaSource.CurrentFormat.FrameRate.Numerator);
            return true;
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
        public int GetExposureByFPS(int fps)
        {
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
        }
        
        private MediaFrameSourceKind GetMediaFrameSourceKind(string kindStr)
        {
            switch (kindStr)
            {
                case "Color": return MediaFrameSourceKind.Color;
                case "Depth": return MediaFrameSourceKind.Depth;
                case "IR": return MediaFrameSourceKind.Infrared;
                default: return MediaFrameSourceKind.Custom;
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

    }



}
