using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
namespace Newtech
{
    public class Proxy
    {
        public event Terminated SessionStarted;
        public event Terminated SessionTerminated;
        object OldSettings;
        string host;
        string Serverport;
        string Cientport;
        string username;
        string password;
        bool verbose;
        bool auto_store_sshkey;
        bool NoShell;
        bool _open;
        bool closed;
        SshClient ssh;
        Thread Check;
        /// <summary>
        /// returns whether the ssh connection is live and open
        /// </summary>
        public bool Open { get { return _open; } }
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        void CheckBool()
        {
            while (ssh.IsConnected)
            {
                Thread.Sleep(20);
            }
            this.Proxy_SessionTerminated(this, new ProxyInfo(this.ToString()));
        }
        /// <summary>
        /// Ensures all threads close
        /// </summary>
       public void Dispose()
        {
           if (Check.IsAlive)
            Check.Abort();
           if (ssh.IsConnected)
               ssh.Disconnect();
           ssh.Dispose();

        }
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
            closed = true;
            verbose = false;
            NoShell = false;
            auto_store_sshkey = false;
            
            this.SessionTerminated += Proxy_SessionTerminated;
            this.SessionStarted += Proxy_SessionStarted;
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
            try
            {
                if (Open)
                    Ssh.Kill();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Runtime]" + e.Message);
            }

            _open = false;
            Console.WriteLine("[Runtime]Closing SSH...");
            ChangeLanProxySettings(0, (this.OldSettings == null) ? "" : OldSettings);
            Console.WriteLine("[Runtime]Returned LAN Proxy");
            SessionTerminated(this, new ProxyInfo(String.Format("{0}, {1}, {2}, {3}", host, Serverport, this.Cientport, this.username)));

        }

        /// <summary>
        /// stops ssh and reverts the lan proxy settings (note this will halt untill the proxy and settings have reverted)
        /// </summary>
        public void StopAndWait()
        {
            Stop();
            while (!closed) ;
        }


        void Proxy_SessionStarted(object source, ProxyInfo e)
        {
            Console.WriteLine("[Runtime]Starting SSH...");
            _open = true;
        }

        void Proxy_SessionTerminated(object source, ProxyInfo e)
        {
            Console.WriteLine("[Runtime]Closed SSH...");
            closed = true;
        }
        void Ssh_Exited(object sender, EventArgs e)
        {
            Stop();
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
    public class Proxytst
    {
        public static SshClient ssh;
        public static bool con { get { return ssh.IsConnected; } }
        public static string test(string Pass)
        {
            ssh = new SshClient(new ConnectionInfo("rhys.rklyne.net", 4243, "rhys", new AuthenticationMethod[] { new PasswordAuthenticationMethod("rhys", Pass) }));
            ssh.HostKeyReceived += ssh_HostKeyReceived;
            ssh.Connect();

            ssh.AddForwardedPort(new ForwardedPortDynamic("localhost", 8080));
            foreach (var f in ssh.ForwardedPorts)
            {
                f.RequestReceived += f_RequestReceived;
                f.Start();
                Console.WriteLine(f.IsStarted);
            } var a = ssh.CreateCommand("ifconfig");
            ssh.KeepAliveInterval = new TimeSpan(0, 0, 20);
            Console.ReadLine();
            return a.Execute();
        }

        static void ssh_HostKeyReceived(object sender, Renci.SshNet.Common.HostKeyEventArgs e)
        {
           Console.WriteLine(String.Join(" ,", new object[]{e.CanTrust,e.FingerPrint,e.HostKey,e.HostKeyName,e.KeyLength,e.ToString()}));
        }

        static void f_RequestReceived(object sender, Renci.SshNet.Common.PortForwardEventArgs e)
        {
           Console.WriteLine(String.Join(" ",new object[]{e.OriginatorHost,e.OriginatorPort,e.ToString()}));
        }

    }
}
