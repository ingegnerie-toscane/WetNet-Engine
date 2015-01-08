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
