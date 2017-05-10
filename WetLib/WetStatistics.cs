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
using System.Data.Odbc;

namespace WetLib
{
    /// <summary>
    /// Classe di funzioni statistiche
    /// </summary>
    static class WetStatistics
    {
        #region Funzioni del modulo

        /// <summary>
        /// Esegue il calcolo della deviazione standard
        /// </summary>
        /// <param name="values">Lista dei valori</param>
        /// <returns>Deviazione standard</returns>
        public static double StandardDeviation(double[] values)
        {
            double[] variance = new double[values.Length];
            double avg_variance = 0.0d;
            
            if (values.Length < 2)
                throw new Exception("At least two values is required!");

            // Calcolo la media
            double avg = GetMean(values);

            // Calcolo le varianze
            for (long ii = 0; ii < values.LongLength; ii++)
                variance[ii] = Math.Pow(values[ii] - avg, 2.0d);

            // Calcolo la media delle varianze (o varianza)
            for (long ii = 0; ii < variance.LongLength; ii++)
                avg_variance += variance[ii];
            avg_variance /= variance.Length - 1;

            // Calcolo e restituisco la deviazione standard
            return Math.Sqrt(avg_variance);
        }

        /// <summary>
        /// Restituisce un massimo valore di un buffer
        /// </summary>
        /// <param name="values">Buffer dei valori</param>
        /// <returns>Valore massimo</returns>
        public static double GetMax(double[] values)
        {
            return values.Max();
        }

        /// <summary>
        /// Restituisce un minimo valore di un buffer
        /// </summary>
        /// <param name="values">Buffer dei valori</param>
        /// <returns>Valore minimo</returns>
        public static double GetMin(double[] values)
        {
            return values.Min();
        }

        /// <summary>
        /// Restituisce la media matematica di un buffer
        /// </summary>
        /// <param name="values">Buffer dei valori</param>
        /// <returns>Media matematica</returns>
        public static double GetMean(double[] values)
        {
            double mean = 0.0d;

            for (long ii = 0; ii < values.LongLength; ii++)
                mean += values[ii];
            mean /= values.LongLength;

            return mean;
        }

        /// <summary>
        /// Effettua una correlazione di Pearson fra 2 misure
        /// </summary>
        /// <param name="id_first_measure">ID prima misura</param>
        /// <param name="id_second_measure">ID seconda misura</param>
        /// <param name="first_day">Primo giorno di analisi</param>
        /// <param name="last_day">Secondo giorno di analisi</param>
        /// <param name="start_hour">Ora di inizio</param>
        /// <param name="stop_hour">Ora di fine</param>
        /// <returns>Indice di correlazione</returns>
        public static double GetPearsonCorrelation(int id_first_measure, int id_second_measure, DateTime first_day, DateTime last_day, int start_hour, int stop_hour)
        {
            WetDBConn db = null;
            double ret = 0.0d;

            try
            {
                // Carico la configurazione del DSN
                WetConfig cfg = new WetConfig();
                // Istanzio la connessione al database
                db = new WetDBConn(cfg.GetWetDBDSN(), null, null, true);
                // Calcolo la correlazione con l'indice di Bravais-Pearson
                DataTable dt = db.ExecCustomQuery(
                "SELECT IFNULL(((SUM(dt.mul) - SUM(dt.v1) * SUM(dt.v2) / COUNT(dt.v1)) / COUNT(dt.v1)) / (STDDEV_POP(dt.v1) * STDDEV_POP(dt.v2)), 0) AS pearson_correlation " +
                    "FROM " +
                    "(" +
                    "   SELECT HOUR(t1.`timestamp`) AS ts, AVG(t1.`value`) AS v1, AVG(t2.`value`) AS v2, AVG(t1.`value` * t2.`value`) AS mul" +
                    "   FROM data_measures t1" +
                    "   INNER JOIN data_measures t2" +
                    "   ON t1.`timestamp` = t2.`timestamp` AND" +
                    "   t1.measures_id_measures = " + id_first_measure.ToString() + " AND" +
                    "   t2.measures_id_measures = " + id_second_measure.ToString() + " AND" +
                    "   t1.`timestamp` >= '" + first_day.Date.AddHours(start_hour).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND" +
                    "   t2.`timestamp` >= '" + first_day.Date.AddHours(start_hour).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND" +
                    "   t1.`timestamp` < '" + last_day.Date.AddHours(stop_hour).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND" +
                    "   t2.`timestamp` < '" + last_day.Date.AddHours(stop_hour).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                    "   GROUP BY ts" +
                    "   ORDER BY ts ASC" +
                    ") AS dt");
                if (dt.Rows.Count == 1)
                    ret = Convert.ToDouble(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ret;
        }

        #endregion
    }
}
