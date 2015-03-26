using FSM.DotNetSSH;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FSM.DotNetSSH;
using Org.Mentalis.Security;

namespace ProxyV2
{

    public class Proxy
    {
        public event Started SessionStarted;
        public event Terminated SessionTerminated;
        object OldSettings;
        string host;
        string Serverport;
        string Cientport;
        string username;
        string password;
        Thread Conn;
        Process Ssh;
        bool verbose;
        bool auto_store_sshkey;
        bool NoShell;
        bool _SshOpen;
        bool _ProxOpen;
        /// <summary>
        /// returns whether the ssh connection is live and open
        /// </summary>
        public bool Open { get { return _SshOpen; } }
        /// <summary>
        /// returns whether the proxy settings connection is live and open
        /// </summary>
        public bool Open { get { return _ProxOpen; } }
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;
        /// <summary>
        /// sets target port and listening port to defaults (22,8080)
        /// </summary>
        public Proxy()
        {
            host = "";
            Serverport = "22";
            Cientport = "8080";
            username = "";
            password = "";
            verbose = false;
            NoShell = false;
            auto_store_sshkey = false;
            this.SessionTerminated += Proxy_SessionTerminated;
            this.SessionStarted += Proxy_SessionStarted;
            Conn = new Thread(BackgroundThread);


        }
        /// <summary>
        /// Hides The Shell
        /// </summary>
        /// <param name="h">whether the proxy shell should be hidden</param>
        /// <returns>Amended object</returns>
        public Proxy AutoStoreSshkey(bool h)
        {
            auto_store_sshkey = h; return this;
        }
        /// <summary>
        /// Hides The Shell
        /// </summary>
        /// <param name="h">whether the proxy shell should be hidden</param>
        /// <returns>Amended object</returns>
        public Proxy TurnOffShell(bool h)
        {
            NoShell = h; return this;
        }
        /// <summary>
        /// Sets the Proxy into verbose mode
        /// </summary>
        /// <param name="h">whether the proxy should be verbose</param>
        /// <returns>Amended object</returns>
        public Proxy Verbose(bool h)
        {
            verbose = h; return this;
        }
        /// <summary>
        /// Sets the ssh server you want to conect to
        /// </summary>
        /// <param name="h">Host Name</param>
        /// <returns>Amended object</returns>
        public Proxy sethost(string h)
        {
            host = h;
            return this;
        }
        /// <summary>
        /// Sets the ssh port to listion on
        /// </summary>
        /// <param name="h">Port</param>
        /// <returns>Amended object</returns>
        public Proxy setCientport(string h)
        {
            Cientport = h;
            return this;
        }
        /// <summary>
        /// sets the ssh port to conect to
        /// </summary>
        /// <param name="h">port</param>
        /// <returns>Amended object</returns>
        public Proxy setServerport(string h)
        {
            Serverport = h;
            return this;
        }
        /// <summary>
        /// Sets the ssh usename
        /// </summary>
        /// <param name="h">usename</param>
        /// <returns>Amended object</returns>
        public Proxy setusername(string h)
        {
            username = h;
            return this;
        }
        /// <summary>
        /// sets the ssh password
        /// </summary>
        /// <param name="h">password</param>
        /// <returns>Amended object</returns>
        public Proxy setpassword(string h)
        {
            password = h;
            return this;
        }

       

        private void ChangeLanProxySettings(int on, object proxsettings)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            if (on == 1)
                OldSettings = registry.GetValue("ProxyServer");
            registry.SetValue("ProxyEnable", on);//turn on off
            registry.SetValue("ProxyServer", proxsettings);//chane the settings
            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to really update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            _ProxOpen = (on == 1);
        }
        private void BackgroundThread()
        {


        }     
        private void OpenSshConection()
        {
            SshExec ssh = new FSM.DotNetSSH.SshExec(this.host, this.username);
            ssh.
        }        
        private void Proxy_SessionStarted(object source, ProxyInfo e)
        {                    
        
        }
        private void Proxy_SessionTerminated(object source, ProxyInfo e)
        {
       
        }

        /// <summary>
        /// starts the ssh conection and changes lan proxy settings
        /// </summary>
        public void Start()
        {

        }
        /// <summary>
        /// stops ssh and reverts the lan proxy settings (note this Wont Halt to wait for changes)
        /// </summary>
        public void Stop()
        {

        }
        /// <summary>
        /// stops ssh and reverts the lan proxy settings (note this will halt untill the proxy and settings have reverted)
        /// </summary>
        public void StopAndWait()
        {

        }


    }
    public delegate void Started(object source, ProxyInfo e);
    public delegate void Terminated(object source, ProxyInfo e);
    public class ProxyInfo : EventArgs
    {
        private string AcountInfo;
        public ProxyInfo(string Text)
        {
            AcountInfo = Text;
        }
        public string GetInfo()
        {
            return AcountInfo;
        }
    }

}
