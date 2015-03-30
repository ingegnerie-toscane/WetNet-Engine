/****************************************************************************
 * 
 * WetLib - Common library for WetNet applications.
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
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace WetLib
{
    /// <summary>
    /// Classe per il debug
    /// </summary>
    static class WetDebug
    {
        #region Funzioni del modulo

        /// <summary>
        /// Gestore delle eccezioni
        /// </summary>
        /// <param name="ex">Eccezione</param>
        public static void GestException(Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.Print(ex.StackTrace.ToString());
                //Debugger.Break();
            }
            // Stampo nel log eventi
            string log_name = Assembly.GetEntryAssembly().GetName().Name;
            if (!EventLog.SourceExists(log_name))
                EventLog.CreateEventSource(log_name, log_name);
            string message =
                "SOURCE      : " + ex.Source + Environment.NewLine +
                "MESSAGE     : " + ex.Message + Environment.NewLine +
                "STACK TRACE : " + Environment.NewLine + ex.StackTrace;
            EventLog.WriteEntry(log_name, message, EventLogEntryType.Error);
        }

        #endregion
    }
}
