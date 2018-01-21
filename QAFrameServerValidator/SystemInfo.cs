using System;
using System.Management;
using Microsoft.Win32;

namespace QAFrameServerValidator
{
    public class SystemInfo
    {        
        #region private members
        private static SystemInfo instance;
        private string machine_name = Environment.MachineName;
        private string build_OS = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();
        private string version_OS = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
        private string version_BIOS = null;
        private string info_USB     = null;
        private string processor    = Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", "").ToString();
        #endregion

        #region public methods
        public static SystemInfo GetInstance()
        {
            if( instance == null )
            {
                instance = new SystemInfo();
            }
            return instance;
        }

        public string MachineName
        {
            get { return machine_name; }
        }
        public string OSBuild
        {
            get { return build_OS; }
        }
        public string OSVersion
        {
            get { return version_OS; }
        }
        public string BIOsVersion
        {
            get { return version_BIOS; }
        }
        public string USBinfo
        {
            get { return info_USB; }
        }
        public string Processor
        {
            get { return processor; }
        }
        #endregion

        #region private methods
        private SystemInfo()
        {          
            Logger.AppendMessage(Environment.NewLine + "***Machine Details***");
            Logger.AppendMessage("Machine Name: " + machine_name);
            Logger.AppendMessage("Processor: " + processor);
            Logger.AppendMessage("OS Version: " + version_OS);
            Logger.AppendMessage("OS Build: " + build_OS);

            Get_Win32_DisplayControllerConfiguration();
            Get_Win32_BIOS();
            Get_Win32_USBHub();
           
            Logger.NewLine();
        }

        private void Get_Win32_BIOS()
        {
            ManagementClass managementClass = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection instances = managementClass.GetInstances();

            foreach (ManagementBaseObject instance in instances)
            {     
                version_BIOS = instance.Properties["SMBIOSBIOSVersion"].Value.ToString();
                Logger.AppendMessage("BIOS Version: " + version_BIOS);
            }            
        }

        private void Get_Win32_USBHub()
        {
            ManagementClass managementClass = new ManagementClass("Win32_USBHub");
            ManagementObjectCollection instances = managementClass.GetInstances();
            Logger.AppendMessage("USB Hub Info: ");
            foreach (ManagementBaseObject instance in instances)
            {
                Logger.AppendMessage("--");
                Logger.AppendMessage("DeviceID: " + instance.Properties["DeviceID"].Value.ToString());
                Logger.AppendMessage("PNPDeviceID: " + instance.Properties["PNPDeviceID"].Value.ToString());
                Logger.AppendMessage("Description: " + instance.Properties["Description"].Value.ToString());
            }
        }

        private void Get_Win32_IRQResource()
        {
            ManagementClass managementClass = new ManagementClass("Win32_IRQResource");
            ManagementObjectCollection instances = managementClass.GetInstances();
            Logger.NewLine();
            foreach (ManagementBaseObject instance in instances)
            {
                Logger.AppendMessage("CSName: " + instance.Properties["CSName"].Value.ToString());
                Logger.AppendMessage("Description: " + instance.Properties["Description"].Value.ToString());
                Logger.AppendMessage("Hardware: " + instance.Properties["Hardware"].Value.ToString());
                Logger.AppendMessage("Name: " + instance.Properties["Name"].Value.ToString());
            }
        }

        private void Get_Win32_DisplayControllerConfiguration()
        {
            ManagementClass managementClass = new ManagementClass("Win32_DisplayControllerConfiguration");
            ManagementObjectCollection instances = managementClass.GetInstances();

            foreach (ManagementBaseObject instance in instances)
            {
                Logger.AppendMessage("Description: " + instance.Properties["Description"].Value.ToString());
                Logger.AppendMessage("Name: " + instance.Properties["Name"].Value.ToString());
            }
        }

        #endregion
    }
}
