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
using System.Data;

namespace WetLib
{
    /// <summary>
    /// Classe statica che incorpora una libreria di funzioni utili
    /// </summary>
    public static class WetUtility
    {
        #region Funzioni del modulo

        /// <summary>
        /// Restituisce una oggetto di tipo "DateTime" con data corrente e ora estratta dalla stringa specificata
        /// </summary>
        /// <param name="time_str">Stringa con l'ora</param>
        /// <returns>Oggetto "DateTime" risultante</returns>
        /// <remarks>
        /// la stringa "time_str" deve essere nel formato HH:mm:ss
        /// </remarks>
        public static DateTime GetDateTimeFromTime(string time_str)
        {
            DateTime ret;

            string[] ss = time_str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 0)
                throw new Exception("Wrong time string format!");
            else
            {
                if (ss.Length == 1)
                    throw new Exception("Minutes in time string must be specified!");
                else
                {
                    int hours, mins, secs;
                    
                    // Imposto i valori delle ore e minuti
                    hours = Convert.ToInt32(ss[0]);
                    if ((hours < 0) && (hours > 23))
                        throw new Exception("Hours value must be between 0 and 23!");
                    mins = Convert.ToInt32(ss[1]);
                    if ((mins < 0) && (mins > 59))
                        throw new Exception("Minutes value must be between 0 and 59!");
                    if (ss.Length == 3)
                    {
                        secs = Convert.ToInt32(ss[2]);
                        if ((secs < 0) && (secs > 59))
                            throw new Exception("Seconds value must be between 0 and 59!");
                    }
                    else if (ss.Length < 3)
                        secs = 0;
                    else
                        throw new Exception("It is allowed max. 3 parameters in this string format -> \"HH:mm:ss\"");

                    // Converto nel valore da ritornare
                    ret = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, mins, secs);
                }
            }

            return ret;
        }

        /// <summary>
        /// Indica se il giorno corrente è festivo
        /// </summary>
        /// <param name="date">Data</param>
        /// <returns>Stato di festivistà</returns>
        public static bool IsHolyday(DateTime date)
        {
            bool holyday = false;

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
                holyday = true;

            return holyday;
        }        

        /// <summary>
        /// Restituisce un vettore di 'doubles' da una colonna di una tabella
        /// </summary>
        /// <param name="dt">Tabella</param>
        /// <param name="column">Nome della colonna</param>
        /// <returns>Vettore restituito</returns>
        public static double[] GetDoubleValuesFromColumn(DataTable dt, string column)
        {
            List<double> values = new List<double>();

            foreach (DataRow dr in dt.Rows)
                values.Add(Convert.ToDouble(dr[column]));

            return values.ToArray();
        }

        #endregion
    }
}
