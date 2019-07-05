using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProxySettingsManager
{
    public class ProxiSettingsMgmt
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
         (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public void Enable(string ip, string port)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            bool settingsReturn, refreshReturn;

            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", ip + ":" + port);

            if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                throw new Exception("Unable to enable the proxy.");

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public void Disable()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            bool settingsReturn, refreshReturn;



            registry.SetValue("ProxyEnable", 0);
            registry.SetValue("ProxyServer", 0);

            if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                throw new Exception("Unable to disable the proxy.");

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public Form1.myProxy GetAvtiveProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            if(registry.GetValue("ProxyEnable").ToString() == "1")
            {
                string[] split = registry.GetValue("ProxyServer").ToString().Split(':');
                if(split.Length == 2)
                {
                    return new Form1.myProxy("", split[0], split[1]);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
    }
}
