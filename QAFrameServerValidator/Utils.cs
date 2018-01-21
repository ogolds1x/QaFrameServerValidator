using System;
using System.Runtime.InteropServices;
using Windows.Media.Capture.Frames;

namespace QAFrameServerValidator
{
    public static class Utils
    {
        #region Public Methods
        public static byte[] PdoToBuffer<T>(T obj)
        {
            int len = Marshal.SizeOf<T>();
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr<T>(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            if (bytes != null)
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
                return stuff;
            }
            return default(T);
        }

        #endregion Public Methods

        #region Public Structures

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KSCAMERA_EXTENDEDPROP_HEADER
        {
            public UInt32 Version;
            public UInt32 PinId;
            public UInt32 Size;
            public UInt32 Result;
            public UInt64 Flags;
            public UInt64 Capability;
            public UInt64 value;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct REAL_SENSE_SR300_DEPTH_MMETADATA
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
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME
        {
            public UInt32 version;
            public UInt32 flag;
            public UInt32 frameCounter;
            public UInt32 opticalTimestamp;   //In millisecond unit
            public UInt32 readoutTime;        //The readout time in millisecond second unit
            public UInt32 exposureTime;       //The exposure time in millisecond second unit
            public UInt32 frameInterval;     //The frame interval in millisecond second unit
            public UInt32 pipeLatency;        //The latency between start of frame to frame ready in USB buffer
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FRAME_DATA_RS400
        {
            public object hwTimeStamp; //in microseconds
            public double systemRelativeTime; //in milliseconds
            private uint  frameCounter;
            public long   callBackTime;
            private MediaFrameSourceKind sensorType;
            private uint opticalTimestamp;
            public UInt16 trigger;
            public object intelCaptureTime;
            public object intelconfiguration;
            public string subType;
            private REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME frameMetadata;
            private REALSENSE_SAMPLE_RS400_INTEL_CONFIGURATION_METADATA configurationMetadata;


            public MediaFrameSourceKind SensorType
            {
                set { sensorType = value; }
                get
                {
                    if (subType == "YUY2")
                        return MediaFrameSourceKind.Color;
                    else
                        return MediaFrameSourceKind.Depth;
                }
            }

            public REALSENSE_SAMPLE_RS400_INTEL_CONFIGURATION_METADATA ConfigurationMetadata
            {
                get
                {
                    return ByteArrayToStructure<Utils.REALSENSE_SAMPLE_RS400_INTEL_CONFIGURATION_METADATA>((byte[])intelconfiguration);
                }
            }

            public REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME FrameMetadata
            {
                get
                {
                    return ByteArrayToStructure<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME>((byte[])intelCaptureTime);
                }
            }
            public uint FrameCounter
            {
                get
                {
                    return FrameMetadata.frameCounter;
                }
                set
                {
                    frameCounter = value;
                }
            }
            public uint OpticalTimestamp
            {
                get
                {
                    return FrameMetadata.opticalTimestamp;
                }
                set
                {
                    opticalTimestamp = value;
                }
            }         
        }


        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct REALSENSE_SAMPLE_RS400_INTEL_CONFIGURATION_METADATA
        {          
            [FieldOffset(0)]
            public UInt32 version;
            [FieldOffset(4)]
            public UInt32 raw;
            [FlagsAttribute]
            public enum Flags : UInt32
            {
                intel_configuration_hw_type = 1,
                intel_configuration_sku_id = 1,
                intel_configuration_cookie = 1,
                intel_configuration_format = 1,
                intel_configuration_width = 1,
                intel_configuration_height = 1,
                intel_configuration_fps = 1,
                intel_configuration_trigger = 1,
                intel_configuration_cal_count = 1
            }
            [FieldOffset(4)] public Flags flags;
            [FieldOffset(8)] public sbyte HWType;
            [FieldOffset(9)] public sbyte SKUsID;
            [FieldOffset(10)] public UInt32 cookie;
            [FieldOffset(14)] public UInt16 format;
            [FieldOffset(16)] public UInt16 width;
            [FieldOffset(18)] public UInt16 height;
            [FieldOffset(20)] public UInt16 FPS;
            [FieldOffset(22)] public UInt16 trigger;
            [FieldOffset(24)] public UInt16 calibrationCount;

            [FieldOffset(25)] public sbyte Reseinitilarved0;
            [FieldOffset(26)] public sbyte Reseinitilarved1;
            [FieldOffset(27)] public sbyte Reseinitilarved2;
            [FieldOffset(28)] public sbyte Reseinitilarved3;
            [FieldOffset(29)] public sbyte Reseinitilarved4;
            [FieldOffset(30)] public sbyte Reseinitilarved5;
           
        };
        
        #endregion Public Structures




        //public static void CreateTest()
        //{
        //    var types = Assembly.GetEntryAssembly().GetTypes();
        //    var testClasses = types.Where(t => t.IsAbstract != true && t.IsClass && t.GetInterfaces().Contains(typeof(ITest)));
        //    Console.WriteLine("Found Test List: " + testClasses.Count());

        //    foreach (var tClass in testClasses)
        //    {

        //        var test = (TestBase)Activator.CreateInstance(tClass, Factory.Instance);
        //    }

        //    Console.WriteLine("Created Test List: " + Factory.Lst_TestsToExecute.Count);
        //    Console.ReadKey();
        //}
    }
}
