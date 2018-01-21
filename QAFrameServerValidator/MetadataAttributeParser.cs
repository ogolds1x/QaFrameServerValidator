using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RealsenseSR300DepthMetaData
    {
        public UInt32 timestamp;
        public Byte version;
        public Byte imageInfoBits;
        public UInt32 frameCounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] reserved0;
        public Byte format;
        public UInt32 resolution;
        public UInt16 fps;
        public Byte reserved1;
        public Byte externalTriggerEnable;
        public Byte externalTriggerDelayMs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Byte[] reserved2;
        public Byte laserPower;
        public Byte accuracy;
        public Byte motionVsRange;
        public Byte filter;
        public Byte confidence;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] reserved3;
        public UInt16 actualExposure;
        public UInt16 frameLatency;
        public Byte actualLaserPower;
        public Int16 syncDelta;
        public UInt16 actualFps;
        public Byte reserved4;
        public Byte mvrMode;
        public Byte syncPairExist;
        public Byte IrPairType;
        public Byte reserved5;
        public Byte lowerValueEnable;
        public Byte TimestampSetCntr;
        public Byte ThermalLoopEnable;
        public Int32 oacVoltage;
        public sbyte oacStability;
        public sbyte irTemperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 73)]
        public Byte[] reserved6;

        //this equals comparator is only relevant for comparing frame CONTROLS. Not things like FPS, timestamps, etc.
        public override bool Equals(object obj)
        {
            if (!(obj is RealsenseSR300DepthMetaData))
                return false;

            RealsenseSR300DepthMetaData mys = (RealsenseSR300DepthMetaData)obj;
            if (laserPower == mys.laserPower && accuracy == mys.accuracy && motionVsRange == mys.motionVsRange && filter == mys.filter && confidence == mys.confidence)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString() 
        {
            return "laser power = " + laserPower + " accuracy = " + accuracy + " motion vs range = " + motionVsRange + " filter = " + filter + " confidence = " + confidence;
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RealsenseSR300ColorMetaData
    {
        public UInt32 timestamp;
        public Byte version;
        public Byte imageInfoBits;
        public UInt32 frameCounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] reserved0;
        public Byte format;
        public UInt32 resolution;
        public UInt16 fps;
        public Byte reserved1;
        public sbyte brightness;
        public Byte contrast;
        public Byte saturation;
        public Byte sharpness;
        public Byte autoExpMode;
        public Byte autoWbTemp;
        public Byte gain;
        public Byte backlightComp;
        public UInt16 gamma;
        public Int16 hue;
        public UInt16 manualExp;
        public UInt16 manualWb;
        public Byte powerLineFrequency;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public Byte[] reserved2;
        public UInt16 actualFps;
        public UInt16 actualTriggerFps;
        public UInt16 actualExposure;
        public UInt16 colorTemperature;
        public UInt16 frameLatency;
        public Byte syncPairExist;
        public Byte TimestampSetCntr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 76)]
        public Byte[] reserved3;

        //this equals comparator is only relevant for comparing frame CONTROLS. Not things like FPS, timestamps, etc.
        public override bool Equals(object obj)
        {
            if (!(obj is RealsenseSR300ColorMetaData))
                return false;

            RealsenseSR300ColorMetaData mys = (RealsenseSR300ColorMetaData)obj;
            if (brightness == mys.brightness && contrast == mys.contrast && /**saturation == mys.saturation && sharpness == mys.sharpness &&*/ autoExpMode == mys.autoExpMode /**&& autoWbTemp == mys.autoWbTemp && gain == mys.gain*/ && backlightComp == mys.backlightComp && /**gamma == mys.gamma && hue == mys.hue &&*/ manualExp == mys.manualExp && manualWb == mys.manualWb && powerLineFrequency == mys.powerLineFrequency)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return "brightness = " + brightness + " contrast = " + contrast + " auto exposure mode = " + autoExpMode + " back light compensation = " + backlightComp + " manual exposure mode = " + (int)Math.Log(manualExp / 10000.0, 2) + " manual white balance = " + manualWb + " power line frequency = " + powerLineFrequency;
        }
    };



    [StructLayout(LayoutKind.Explicit, Size = 48, CharSet = CharSet.Ansi)]
    public struct RealsenseRS400MetaDataIntelDepthControl
    {
        [FieldOffset(0)]
        public UInt32 version;
        [FieldOffset(1)]
        public UInt32 intel_depth_control_gain;
        [FieldOffset(2)]
        public UInt32 intel_depth_control_manual_exposure;
        [FieldOffset(3)]
        public UInt32 intel_depth_control_laser_power;
        [FieldOffset(4)]
        public UInt32 intel_depth_control_ae_mode;
        [FieldOffset(5)]
        public UInt32 intel_depth_control_exposure_priority;
        [FieldOffset(6)]
        public UInt32 intel_depth_control_roi;
        [FieldOffset(7)]
        public UInt32 intel_depth_control_preset;
        [FieldOffset(8)]
        public UInt32 manualGain;         //Manual gain value
        [FieldOffset(12)]
        public UInt32 manualExposure;     //Manual exposure
        [FieldOffset(16)]
        public UInt32 laserPower;        //Laser power value
        [FieldOffset(20)]
        public UInt32 autoExposureMode;   //AE mode
        [FieldOffset(24)]
        public UInt32 exposurePriority;
        [FieldOffset(28)]
        public UInt32 exposureROILeft;
        [FieldOffset(32)]
        public UInt32 exposureROIRight;
        [FieldOffset(36)]
        public UInt32 exposureROITop;
        [FieldOffset(40)]
        public UInt32 exposureROIBottom;
        [FieldOffset(44)]
        public UInt32 preset;

        public override bool Equals(object obj)
        {
            if (!(obj is RealsenseRS400MetaDataIntelDepthControl))
                return false;

            RealsenseRS400MetaDataIntelDepthControl mys = (RealsenseRS400MetaDataIntelDepthControl)obj;
            if (manualGain == mys.manualGain && manualExposure == mys.manualExposure && intel_depth_control_laser_power == mys.intel_depth_control_laser_power)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return "manual gain = " + manualGain + " manual exposure = " + manualExposure + " laser power = " + intel_depth_control_laser_power;
        }
    };


    public static class AttributeParser
        {
            public static T ByteArrayToStructure<T>(byte[] arr)
            {
                T str = default(T);
                int size = Marshal.SizeOf<T>();
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(arr, 0, ptr, size);
                str = Marshal.PtrToStructure<T>(ptr);
                Marshal.FreeHGlobal(ptr);
                return str;
            }

            public static T Parse<T>(IReadOnlyDictionary<Guid, System.Object> Properties, Guid guid, out bool success)
            {
                try
                {
                    object rawData;
                    if (Properties.TryGetValue(guid, out rawData) == true)
                    {
                        success = true;
                        return ByteArrayToStructure<T>((Byte[])rawData);
                    }
                }
                catch (Exception ex)
                {

                }
                success = false;
                return default(T);
            }

            public static T Parse<T>(IReadOnlyDictionary<Guid, System.Object> Properties, Guid guid)
            {
                bool success = false;
                return Parse<T>(Properties, guid, out success);
            }
        }
    }
