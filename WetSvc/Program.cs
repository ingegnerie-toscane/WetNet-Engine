using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WetSvc
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
#if DEBUG
            WetSvc svc = new WetSvc();
            svc.StartDebug(null);

            Process prcs = Process.Start("cmd.exe", "/K TITLE WetSvc Debug Window");
            prcs.WaitForExit();

            svc.StopDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new WetSvc() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
