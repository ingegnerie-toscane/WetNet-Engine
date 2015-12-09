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
            for (int ii = 0; ii < values.Length; ii++)
                variance[ii] = Math.Pow(values[ii] - avg, 2.0d);

            // Calcolo la media delle varianze (o varianza)
            for (int ii = 0; ii < variance.Length; ii++)
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

        #endregion
    }
}
