using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QAFrameServerValidator.Abstract_Factory;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace QAFrameServerValidator.Tests
{
    class CheckDefaultControlValuesTest : TestBase
    {
        FrameReaderUtil fr;
        public CheckDefaultControlValuesTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {
            Logger.AppendMessage($"LOG_PATH: {Logger.path}");
            Logger.AppendMessage(Environment.NewLine);

            Logger.AppendMessage("AREA_NAME: DefaultControls");
            Logger.AppendMessage("SUB_AREA_NAME: CheckDefaultControlValuesTest");
            Logger.AppendInfo("Now starting CheckDefaultControlValues Test");

            fr = new FrameReaderUtil();
            fr.SetSettingsPropertiesTest("RGB");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(true);
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                await fr.InitializeMediaCapture(true);
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                await fr.SetNonSpecificFrameFormat(true);
            }).GetAwaiter().GetResult();
            fr.SetSettingsPropertiesTest("Depth");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                await fr.SetNonSpecificFrameFormat(false);
            }).GetAwaiter().GetResult();
            //Subtest 1: check the min/max/step/default values against specification NOT WHILE streaming
            for (int i = 0; i < controlsList.Count(); i++)
            {
                //Logger.AppendInfo
                CheckObtainedValueAgainstDocumentSpecification(controlsList[i].Control);
            }
            //Subtest 2: check the min/max/step/default values against specification WHILE streaming
            for (int i = 0; i < controlsList.Count(); i++)
            {
                //Logger.AppendInfo
                CheckObtainedValueAgainstDocumentSpecification(controlsList[i].Control);
            }
            //Console.WriteLine (" CheckDefaultControlValuesTest " +  controlsList.Count());
        }

        #region DataStructuresForGettingMinMaxAndStepFoRS2Properties
        [StructLayout(LayoutKind.Sequential)]
        public struct KSIDENTIFIER
        {
            Guid Set;
            ulong Id;
            ulong Flags;
            long Alignment;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_DESCRIPTION
        {
            ulong AccessFlags;
            ulong DescriptionSize;
            KSIDENTIFIER PropTypeSet;
            ulong MembersListCount;
            ulong Reserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_MEMBERSHEADER
        {
            ulong MembersFlags;
            ulong MembersSize;
            ulong MembersCount;
            ulong Flags;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_BOUNDS_LONG
        {
           public long SignedMinimum;
           public long SignedMaximum;
           public ulong UnsignedMinimum;
           public ulong UnsignedMaximum;
        };
        [StructLayout(LayoutKind.Sequential)]
        private struct CapSourceKsMemberList
        {
            KSPROPERTY_DESCRIPTION desc;
            KSPROPERTY_MEMBERSHEADER hdr;
            public KSPROPERTY_STEPPING_LONG step;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KSPROPERTY_STEPPING_LONG
        {
            public ulong SteppingDelta;
            ulong Reserved;
            public KSPROPERTY_BOUNDS_LONG Bounds;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KsProperty
        {
            public Guid Set;
            public ulong Id;
            public ulong Flags;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct KsProperty_VideoProcAmp
        {
            public KsProperty Property;
            public long Value;
            public ulong Flags;
            public ulong Capabilities;
        }
        #endregion DataStructuresForGettingMinMaxAndStepFoRS2Properties
        #region FunctionsForGettingMinMaxAndStepFoRS2Properties
        internal static class NativeMethods
        {
            public const uint KSPROPERTY_TYPE_GET = 0x00000001;
            public const uint KSPROPERTY_TYPE_SET = 0x00000002;

            public static byte[] GetBytes<T>(T obj)
            {
                IntPtr ptr = IntPtr.Zero;
                int size = Marshal.SizeOf<T>();
                byte[] result = new byte[size];

                try
                {
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(obj, ptr, /* fDeleteOld */ false);
                    Marshal.Copy(ptr, result, 0, size);
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }

                return result;
            }
        }
        private CapSourceKsMemberList ByteArrayToCapSourceKsMemberList(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            CapSourceKsMemberList stuff = (CapSourceKsMemberList)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CapSourceKsMemberList));
            handle.Free();
            return stuff;
        }
        private KsProperty_VideoProcAmp getKsProperty_VideoProcAmp(string guid, ulong id)
        {
            KsProperty_VideoProcAmp getProp = new KsProperty_VideoProcAmp
            {
                Property = new KsProperty
                {
                    Set = Guid.Parse(guid), 
                    Id = id,
                    Flags = 0x00000200 //KSPROPERTY_TYPE_BASICSUPPORT
                },
                Value = 0,
                Flags = 0xF,
                Capabilities = 0
            };
            return getProp;
        }
        #endregion FunctionsForGettingMinMaxAndStepFoRS2Properties


        private bool CheckObtainedValueAgainstDocumentSpecification(Types.ControlKey control)
        {
            switch (control)
            {
                case Types.ControlKey.DS5_DEPTH_EXPOSURE:
                    {
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_LASER_ON_OFF:
                    {
                        break;
                    }
                case Types.ControlKey.DS5_DEPTH_LASER_POWER:
                    {
                        break;
                    }
                case Types.ControlKey.COLOR_EXPOSURE:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.Exposure.Capabilities.Min == SetControl.SR300ColorExposureMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Exposure.Capabilities.Max == SetControl.SR300ColorExposureMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Exposure.Capabilities.Default == SetControl.SR300ColorExposureDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Exposure.Capabilities.Step == SetControl.SR300ColorExposureStep);
                    }
                case Types.ControlKey.COLOR_BRIGHTNESS:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.Brightness.Capabilities.Min == SetControl.SR300ColorBrightnessMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Brightness.Capabilities.Max == SetControl.SR300ColorBrightnessMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Brightness.Capabilities.Default == SetControl.SR300ColorBrightnessDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Brightness.Capabilities.Step == SetControl.SR300ColorBrightnessStep);
                    }
                case Types.ControlKey.COLOR_CONTRAST:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.Contrast.Capabilities.Min == SetControl.SR300ColorContrastMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Contrast.Capabilities.Max == SetControl.SR300ColorContrastMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Contrast.Capabilities.Default == SetControl.SR300ColorContrastDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Contrast.Capabilities.Step == SetControl.SR300ColorContrastStep);
                    }
                case Types.ControlKey.COLOR_HUE:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.Hue.Capabilities.Min == SetControl.SR300ColorHueMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Hue.Capabilities.Max == SetControl.SR300ColorHueMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Hue.Capabilities.Default == SetControl.SR300ColorHueDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.Hue.Capabilities.Step == SetControl.SR300ColorHueStep);
                    }
                case Types.ControlKey.COLOR_WHITE_BALANCE:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Min == SetControl.SR300ColorWhiteBalanceMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Max == SetControl.SR300ColorWhiteBalanceMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Default == SetControl.SR300ColorWhiteBalanceDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Step == SetControl.SR300ColorWhiteBalanceStep);
                    }
                case Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION:
                    {
                        return
                            (fr.GetColorMediaCapture().VideoDeviceController.BacklightCompensation.Capabilities.Min == SetControl.SR300ColorBacklightCompensationMin)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.BacklightCompensation.Capabilities.Max == SetControl.SR300ColorBacklightCompensationMax)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.BacklightCompensation.Capabilities.Default == SetControl.SR300ColorBacklightCompensationDefaultValue)
                            &&
                            (fr.GetColorMediaCapture().VideoDeviceController.BacklightCompensation.Capabilities.Step == SetControl.SR300ColorBacklightCompensationStep);
                    }
                //case Types.ControlKey.COLOR_EXPOSURE_PRIORITY:
                //    {
                //        return
                //            (fr.GetColorMediaCapture().VideoDeviceController.ExposurePriorityVideoControl.Capabilities.Min == SetControl.SR300ColorWhiteBalanceMin)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.ExposurePriorityVideoControl.Capabilities.Max == SetControl.SR300ColorWhiteBalanceMax)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.ExposurePriorityVideoControl.Capabilities.Default == SetControl.SR300ColorWhiteBalanceDefaultValue)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.ExposurePriorityVideoControl.Capabilities.Step == SetControl.SR300ColorWhiteBalanceStep);
                //    }
                //case Types.ControlKey.COLOR_POWER_LINE_FREQUENCY:
                //    {
                //        return
                //            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Min == SetControl.SR300ColorWhiteBalanceMin)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Max == SetControl.SR300ColorWhiteBalanceMax)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Default == SetControl.SR300ColorWhiteBalanceDefaultValue)
                //            &&
                //            (fr.GetColorMediaCapture().VideoDeviceController.WhiteBalance.Capabilities.Step == SetControl.SR300ColorWhiteBalanceStep);
                //    }
                case Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_LASER_POWER:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_ACCURACY:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_FILTER_OPTION:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_FACE_AUTH:
                    {
                        break;
                    }
                case Types.ControlKey.FISHEYE_EXPOSURE:
                    {
                        break;
                    }
                case Types.ControlKey.FISHEYE_GAIN:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_PRESET:
                    {
                        //Preset_Test_IVCAM_Depth();
                        break;
                    }
                case Types.ControlKey.COLOR_GAMMA:
                    {
                        break;
                    }
                case Types.ControlKey.COLOR_SATURATION:
                    {
                        break;
                    }
                case Types.ControlKey.COLOR_SHARPNESS:
                    {
                        break;
                    }
                case Types.ControlKey.COLOR_GAIN:
                    {//CapSourceKsMemberList
                        CapSourceKsMemberList resultValue;
                        var result = fr.GetColorMediaCapture().VideoDeviceController.GetDevicePropertyByExtendedId(NativeMethods.GetBytes(getKsProperty_VideoProcAmp("C6E13360-30AC-11d0-A18C-00A0C9118956", (ulong)VideoProcAmp_ControlValues.BRIGHTNESS)), /*size of your expected returned struct in bytes*/(uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(CapSourceKsMemberList)));
                        Debug.WriteLine("RESULT OF GetDevicePropertyByExtendedId = " + result.Status);
                        byte[] propertyData = result.Value as byte[];
                        Debug.Assert(propertyData != null && propertyData.Length > 0);
                        resultValue = ByteArrayToCapSourceKsMemberList(propertyData);
                        return resultValue.step.Bounds.SignedMinimum == SetControl.SR300ColorGainMin;

                    }
                case Types.ControlKey.DS5_DEPTH_GAIN:
                    {
                        break;
                    }
                case Types.ControlKey.COLOR_PRIVACY:
                    {
                        break;
                    }
                case Types.ControlKey.DEPTH_PRIVACY:
                    {
                        break;
                    }
                default:
                    Logger.AppendInfo("Fail: Control name " + Types.ConvertControlKeyToString(control) + " not found!");
                    return false;
            }
            return false;
        }

    

    }
}
