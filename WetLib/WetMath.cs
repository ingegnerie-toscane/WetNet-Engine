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
    /// Classe di funzioni matematiche
    /// </summary>
    public static class WetMath
    {
        /// <summary>
        /// Restituisce 0.0 se il double non è un valore numerico finito
        /// </summary>
        /// <param name="value">Valore da validare</param>
        /// <returns>Valore validato</returns>
        public static double ValidateDouble(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 0.0d;
            else
                return value;
        }

        /// <summary>
        /// Esegue l'interpolazione lineare di una serie di valori localizzati nel tempo
        /// </summary>
        /// <param name="interpolation_time">Tempo di interpolazione</param>
        /// <param name="serie">Serie di valori</param>
        /// <param name="measure_type">Tipo della misura</param>
        /// <returns>Serie interpolata</returns>
        public static Dictionary<DateTime, double> LinearInterpolation(TimeSpan interpolation_time, Dictionary<DateTime, double> serie, MeasureTypes measure_type)
        {
            Dictionary<DateTime, double> return_serie = new Dictionary<DateTime, double>();

            // Devono esserci almeno due campioni per interpolare
            if (serie.Count() > 1)
            {
                DateTime start = serie.ElementAt(0).Key;
                DateTime stop = start + interpolation_time;

                // Loop sui campioni
                for (int ii = 0, jj = 0; ii < serie.Count;)
                {
                    if (serie.ElementAt(ii).Key >= stop)
                    {
                        // Interpolo
                        double y0 = ValidateDouble(serie.ElementAt(jj).Value);
                        double y1 = ValidateDouble(serie.ElementAt(ii).Value);
                        double x0 = Convert.ToDouble(serie.ElementAt(jj).Key.Ticks);
                        double x1 = Convert.ToDouble(serie.ElementAt(ii).Key.Ticks);
                        double x = Convert.ToDouble(stop.Ticks);
                        double y = ValidateDouble((((y1 - y0) * (x - x0)) / (x1 - x0)) + y0);
                        if (measure_type == MeasureTypes.DIGITAL_STATE)
                        {
                            if ((y != 0.0d) && (y != 1.0d))
                            {
                                // Controllo di congruenza valore precedente
                                if (y0 < 0.0d)
                                    y0 = 0.0d;
                                else if (y0 > 1.0d)
                                    y0 = 1.0d;
                                else
                                    y0 = Math.Round(y0);
                                
                                // Assegno al valore attuale lo stato precedente
                                y = y0;
                            }
                        }
                        // Aggiungo il valore
                        return_serie.Add(stop, y);
                        // Aggiorno i contatori
                        if ((serie.ElementAt(ii).Key - stop) <= interpolation_time)
                            jj = ii++;
                        start = stop;
                        stop = start + interpolation_time;
                    }
                    else
                        ii++;
                }
            }

            return return_serie;
        }

        /// <summary>
        /// Converte una tabella con una coppia di valori timestamp+valore in un dizionario analogo
        /// </summary>
        /// <param name="serie">Tabella con la serie</param>
        /// <param name="timestamp_column">Nome della colonna con il timestamp</param>
        /// <param name="value_column">Nome della colonna con il valore</param>
        /// <param name="fixed_value">Valore fittizio da addizionare</param>
        /// <param name="multiplication_factor">Fattore moltiplicativo</param>
        /// <param name="mst">Sorgente della misura</param>
        /// <returns>Dizionario restituito</returns>
        public static Dictionary<DateTime, double> DataTable2Dictionary(DataTable serie, string timestamp_column, string value_column, double fixed_value, double multiplication_factor, MeasuresSourcesTypes mst)
        {
            Dictionary<DateTime, double> return_serie = new Dictionary<DateTime, double>();

            try
            {
                for (int ii = 0; ii < serie.Rows.Count; ii++)
                {
                    // Acquisisco i dati formattati
                    DateTime timestamp = serie.Rows[ii][timestamp_column] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(serie.Rows[ii][timestamp_column]);
                    double value = serie.Rows[ii][value_column] == DBNull.Value ? 0.0d : Convert.ToDouble(serie.Rows[ii][value_column]);
                    // Se la chiave esiste già la salto
                    if (return_serie.ContainsKey(timestamp))
                        continue;
                    // Li aggiungo al dizionario
                    if (mst == MeasuresSourcesTypes.REAL)
                        return_serie.Add(timestamp, (ValidateDouble(value) * multiplication_factor) + ValidateDouble(fixed_value));
                    else
                        return_serie.Add(timestamp, ValidateDouble(value) * multiplication_factor);
                }
            }
            catch
            { }

            return return_serie;
        }
    }
}
