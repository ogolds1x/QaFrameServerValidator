using System.Runtime.InteropServices;
namespace QAFrameServerValidator
{
    public class MissingProperties
    {
        public enum ControlType
        {
            ControlType_Gain, ControlType_Saturation, ControlType_Sharpness, ControlType_Gamma, ControlType_Privacy, ControlType_Exposure/*, ControlType_FE_Gain, ControlType_FE_Exposure*/
        };
        public static string SR300_ColorCamera = "Intel(R) RealSense(TM) Camera SR300 RGB";
        public static string SR300_DepthCamera = "Intel(R) RealSense(TM) Camera SR300 Depth";
        public static string DS5_FishEyeCamera = "Intel(R) RealSense(TM) 430 with Tracking Module FishEye";

        [DllImport("MissingProp.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, PreserveSig = false, CharSet = CharSet.Unicode)]
        public static extern void GetCapabilities(string devName, ControlType control, out int min, out int max, out int step, out int def);

        [DllImport("MissingProp.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, PreserveSig = false, CharSet = CharSet.Unicode)]
        public static extern void GetCurrentValue(string name, ControlType control, out int value);

        [DllImport("MissingProp.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, PreserveSig = false, CharSet = CharSet.Unicode)]
        public static extern void SetValue(string name, ControlType control, int value);
    }
}
