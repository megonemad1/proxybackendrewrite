using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtech;
namespace Testssh
{
    class Program
    {
        static void Main(string[] args)
        {
            Proxy P = new Proxy().setServerport("4243").setCientport("8080").sethost("rhys.rklyne.net").setlogin("rhys", Console.ReadLine());
            P.SessionStarted += P_SessionStarted;
            P.SessionTerminated += P_SessionTerminated;
            P.Start();
            Console.WriteLine("enter to close");
            Console.ReadLine();
            P.Stop();
            Console.ReadLine();

        }

        static void P_SessionTerminated(object source, ProxyInfo e)
        {
            Console.WriteLine("Term");
        }

        static void P_SessionStarted(object source, ProxyInfo e)
        {
            Console.WriteLine("start");
        }
    }
}
