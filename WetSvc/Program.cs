/****************************************************************************
 * 
 * WetSvc - WetNet Engine Service.
 * Copyright 2013-2015 Ingegnerie Toscane S.r.l.
 * 
 * This file is part of WetNet application.
 * 
 * Licensed under the EUPL, Version 1.1 or – as soon they
 * will be approved by the European Commission - subsequent
 * versions of the EUPL (the "Licence");
 * 
 * You may not use this work except in compliance with the licence.
 * You may obtain a copy of the Licence at:
 * http://ec.europa.eu/idabc/eupl
 * 
 * Unless required by applicable law or agreed to in
 * writing, software distributed under the Licence is
 * distributed on an "AS IS" basis,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied.
 * 
 * See the Licence for the specific language governing
 * permissions and limitations under the Licence.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Configuration.Install;

namespace WetSvc
{
    static class Program
    {
        #region Funzioni del modulo

        /// <summary>
        /// Procedura di autoinstallazione
        /// </summary>
        static void Install()
        {
            try
            {
                string file_name = Assembly.GetExecutingAssembly().Location;
                ManagedInstallerClass.InstallHelper(new string[] { file_name });              
            }
            catch
            { }
        }

        /// <summary>
        /// Procedura di autodisinstallazione
        /// </summary>
        static void UnInstall()
        {
            try
            {
                string file_name = Assembly.GetExecutingAssembly().Location;
                ManagedInstallerClass.InstallHelper(new string[] { @"/u", file_name });
            }
            catch
            { }
        }

        #endregion

        #region Punto di ingresso dell'applicazione

        /// <summary>
        /// Punto di ingresso dell'applicazione
        /// </summary>
        /// <param name="args">parametri da riga di comando</param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new WetSvc()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else if (args.Length == 1)
            {
                string val = args[0].ToLower();

                switch (val)
                {
                    case "/d":
                    case "-d":
                        WetSvc svc = new WetSvc();
                        svc.StartDebug(null);

                        Process prcs = Process.Start("cmd.exe", "/K TITLE WetSvc Debug Window");
                        prcs.WaitForExit();

                        svc.StopDebug();
                        break;

                    case "/i":
                    case "-i":
                        Install();
                        break;

                    case "/u":
                    case "-u":
                        UnInstall();
                        break;
                }
            }
        }

        #endregion
    }
}
