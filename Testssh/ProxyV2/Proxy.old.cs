﻿//using FSM.DotNetSSH;
//using Microsoft.Win32;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using FSM.DotNetSSH;
//using Org.Mentalis.Security;

//namespace ProxyV2
//{
    
//        public class Proxyoldr
//        {
//            public event Terminated SessionStarted;
//            public event Terminated SessionTerminated;
//            object OldSettings;
//            string host;
//            string Serverport;
//            string Cientport;
//            string username;
//            string password;
//            Thread Conn;
//            Process Ssh;
//            bool verbose;
//            bool auto_store_sshkey;
//            bool NoShell;
//            bool _open;
//            bool closed;
//            SshExec exec;
//            /// <summary>
//            /// returns whether the ssh connection is live and open
//            /// </summary>
//            public bool Open { get { return _open; } }
//            [DllImport("wininet.dll")]
//            public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
//            public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
//            public const int INTERNET_OPTION_REFRESH = 37;
//            static bool settingsReturn, refreshReturn;
//            /// <summary>
//            /// sets target port and listening port to defaults (22,8080)
//            /// </summary>
//            public Proxyoldr()
//            {
//                host = "";
//                Serverport = "22";
//                Cientport = "8080";
//                username = "";
//                password = "";
//                closed = true;
//                verbose = false;
//                NoShell = false;
//                auto_store_sshkey = false;
//                this.SessionTerminated += Proxy_SessionTerminated;
//                this.SessionStarted += Proxy_SessionStarted;
//                Conn = new Thread(POpen);
                

//            }
//            /// <summary>
//            /// Hides The Shell
//            /// </summary>
//            /// <param name="h">whether the proxy shell should be hidden</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr AutoStoreSshkey(bool h)
//            {
//                auto_store_sshkey = h; return this;
//            }
//            /// <summary>
//            /// Hides The Shell
//            /// </summary>
//            /// <param name="h">whether the proxy shell should be hidden</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr TurnOffShell(bool h)
//            {
//                NoShell = h; return this;
//            }
//            /// <summary>
//            /// Sets the Proxy into verbose mode
//            /// </summary>
//            /// <param name="h">whether the proxy should be verbose</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr Verbose(bool h)
//            {
//                verbose = h; return this;
//            }
//            /// <summary>
//            /// Sets the ssh server you want to conect to
//            /// </summary>
//            /// <param name="h">Host Name</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr sethost(string h)
//            {
//                host = h;
//                return this;
//            }
//            /// <summary>
//            /// Sets the ssh port to listion on
//            /// </summary>
//            /// <param name="h">Port</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr setCientport(string h)
//            {
//                Cientport = h;
//                return this;
//            }
//            /// <summary>
//            /// sets the ssh port to conect to
//            /// </summary>
//            /// <param name="h">port</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr setServerport(string h)
//            {
//                Serverport = h;
//                return this;
//            }
//            /// <summary>
//            /// Sets the ssh usename
//            /// </summary>
//            /// <param name="h">usename</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr setusername(string h)
//            {
//                username = h;
//                return this;
//            }
//            /// <summary>
//            /// sets the ssh password
//            /// </summary>
//            /// <param name="h">password</param>
//            /// <returns>Amended object</returns>
//            public Proxyoldr setpassword(string h)
//            {
//                password = h;
//                return this;
//            }

//            private void POpen()
//            {
//                if (host + Serverport + username + password == "")
//                    return;
//                closed = false;
//                Console.WriteLine("[Runtime]Changing LAN Proxy...");
//                ChangeLanProxySettings(1, String.Format("socks=LOCALHOST:{0}", Cientport));
//                Console.WriteLine("[Runtime]Changed LAN Proxy...");
//                Console.WriteLine("[Runtime]Opening Proxy...");
//                OpenSshConection();
//                if (exec.Connected)
//                {
//                    Console.WriteLine("[Runtime]Opening Proxy...");
//                    SessionStarted(this, new ProxyInfoold(String.Format("{0}, {1}, {2}, {3}", host, Serverport, this.Cientport, this.username)));
//                }
//                idle();
                
//            }

//            private void idle()
//            {
//                while (exec.Connected)
//                {
//                    Thread.Sleep(10);
//                }
//            }

//            private void ChangeLanProxySettings(int on, object proxsettings)
//            {
//                RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
//                if (on == 1)
//                    OldSettings = registry.GetValue("ProxyServer");
//                registry.SetValue("ProxyEnable", on);//turn on off
//                registry.SetValue("ProxyServer", proxsettings);//chane the settings
//                // These lines implement the Interface in the beginning of program 
//                // They cause the OS to refresh the settings, causing IP to really update
//                settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
//                refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

//            }

//            private void OpenSshConection()
//            {

//               exec = new SshExec(this.host, this.username, this.password);
//               int hostport;
//                try {
//                    hostport= Convert.ToInt32( this.Serverport );
//                }
//                catch (Exception e)
//                {
//                    hostport=22;
//                    Debug.WriteLine(e.Message);
//                }
//                try
//                {
                    
//                    exec.Connect(hostport);
//                    string strOutput = exec.RunCommand("ifconfig");
//                    //Go line by line and add into the array lines while removing empty entries 
//                    string[] lines = strOutput.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);


//                    //loop through each string in lines 
//                    foreach (string s in lines)
//                    {
//                        Console.WriteLine(s);
//                    }
//                }
//                catch (FSM.DotNetSSH.jsch.JSchException e)
//                {
//                    Console.WriteLine(e);
//                }
                
//               // //keeping this in as a safeguard (fixing proxygui.exe only being updated)
//               // string path = "klink.exe";
//               //ProcessStartInfo startInfo = new ProcessStartInfo();
//               // startInfo.FileName = path;
//               // startInfo.Arguments = string.Format("-ssh -l {0} -pw {1} -D {2} -P {3} {4} {5} {6} {7}", username, password, this.Cientport, this.Serverport, (verbose) ? "-v" : "", (auto_store_sshkey) ? "-auto_store_sshkey" : "", (NoShell) ? "-N" : "", host);
//               // startInfo.UseShellExecute = false;
//               // startInfo.CreateNoWindow = NoShell;
//               // Ssh = new Process();
//               // Ssh.StartInfo = startInfo;
//               // Ssh.EnableRaisingEvents = true;
//               // Ssh.Exited += Ssh_Exited;
//               // Ssh.Start();
//            }



//            void Ssh_Exited(object sender, EventArgs e)
//            {
//                Stop();
//            }
//            /// <summary>
//            /// stops ssh and reverts the lan proxy settings (note this Wont Halt to wait for changes)
//            /// </summary>
//            public void Stop()
//            {
//                try
//                {
//                    exec.Close();

//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("[Runtime]"+e.Message);
//                }
//                Proxy_SessionTerminated(this, new ProxyInfoold(String.Format("{0}, {1}, {2}, {3}", host, Serverport, this.Cientport, this.username)));

//                try
//                {
//                    Console.WriteLine("[Runtime]Closing Proxy...");
//                    if (exec.Connected)
//                        exec.Close();
//                    Console.WriteLine("[Runtime]Closed Proxy...");
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("[Runtime]" + e.Message);
//                }

//                _open = false;
//                Console.WriteLine("[Runtime]Return LAN Proxy");
//                this.ChangeLanProxySettings(0, (this.OldSettings == null) ? "" : OldSettings);
//                Console.WriteLine("[Runtime]Returned LAN Proxy");
//                this.SessionTerminated(this, new ProxyInfoold(String.Format("{0}, {1}, {2}, {3}", host, Serverport, this.Cientport, this.username)));

//            }

//            void Proxy_SessionStarted(object source, ProxyInfoold e)
//            {
//                Console.WriteLine("[Runtime]Starting SSH...");
//                _open = true;
//            }

//            void Proxy_SessionTerminated(object source, ProxyInfoold e)
//            {
//                Console.WriteLine("[Runtime]Closed SSH...");
//                closed = true;
//            }

//            /// <summary>
//            /// starts the ssh conection and changes lan proxy settings
//            /// </summary>
//            public void Start()
//            {

//                Conn.Start();

//            }
//            /// <summary>
//            /// stops ssh and reverts the lan proxy settings (note this will halt untill the proxy and settings have reverted)
//            /// </summary>
//            public void StopAndWait()
//            {
//                Stop();
//                while (!closed) ;

//            }


//        }
//        public delegate void Started(object source, ProxyInfo e);
//        public delegate void Terminated(object source, ProxyInfo e);
//        public class ProxyInfoold : EventArgs
//        {
//            private string AcountInfo;
//            public ProxyInfoold(string Text)
//            {
//                AcountInfo = Text;
//            }
//            public string GetInfo()
//            {
//                return AcountInfo;
//            }
//        }
    
//}
