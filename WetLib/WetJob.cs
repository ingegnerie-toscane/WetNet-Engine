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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WetLib
{
    /// <summary>
    /// Classe per la definizione di un Job
    /// </summary>
    public abstract class WetJob
    {
        #region Costanti

        /// <summary>
        /// Tempo di default per l'attesa fra l'esecuzione di un job e la successiva (millisecondi)
        /// </summary>
        public const int DEFAULT_JOB_SLEEP_TIME = 1;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Tempo di attesa fra una esecuzione e la successiva del job (millisecondi)
        /// </summary>
        protected int job_sleep_time;

        /// <summary>
        /// Token di cancellazione
        /// </summary>
        protected CancellationTokenSource cancellation_token_source = new CancellationTokenSource();

        #endregion
        
        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="job_sleep_time">Tempo di attesa fra una esecuzione e la successiva del job (millisecondi)</param>
        public WetJob(int job_sleep_time = DEFAULT_JOB_SLEEP_TIME)
        {
            if (job_sleep_time < 1)
                throw new Exception("'job_sleep_time' must be greater than 1 millisecond!");
            this.job_sleep_time = job_sleep_time;
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Funzione di avvio del job
        /// </summary>
        public void Start()
        {
            Task.Run(() =>
                {
                    JobTask();
                }, cancellation_token_source.Token);
        }

        /// <summary>
        /// Funzione di arresto del job
        /// </summary>
        public void Stop()
        {
            cancellation_token_source.Cancel();
        }

        /// <summary>
        /// Funzione per l'inizializzazione del job
        /// </summary>
        protected virtual void Load()
        {
            // ...
        }

        /// <summary>
        /// Thread del job
        /// </summary>
        void JobTask()
        {
            // Carico le impostazioni iniziali
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                ExceptionsManager(ex);
            }
            // Loop del thread
            while (!cancellation_token_source.IsCancellationRequested)
            {
                // Eseguo il job
                try
                {
                    // Avvio un conteggio
                    long start_time = DateTime.Now.Ticks;
                    // Metodo
                    DoJob();
                    // Attesa
                    TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - start_time);
                    TimeSpan sleep_time = new TimeSpan(0, 0, 0, 0, job_sleep_time);
                    double difference = sleep_time.TotalMilliseconds - elapsed.TotalMilliseconds;
                    if (difference > 1.0d)
                        Sleep(Convert.ToInt32(difference));
                    else
                        Sleep();
                }
                catch (Exception ex)
                {
                    ExceptionsManager(ex);
                }
            }
            // Chiudo con le impostazioni finali
            try
            {
                UnLoad();
            }
            catch (Exception ex)
            {
                ExceptionsManager(ex);
            }
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected abstract void DoJob();

        /// <summary>
        /// Funzione per la finalizzazione del job
        /// </summary>
        protected virtual void UnLoad()
        {
            // ...
        }

        /// <summary>
        /// Gestore delle eccezioni
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void ExceptionsManager(Exception ex)
        {
            // ...
        }

        /// <summary>
        /// Attende per un numero specificato di millisecondi
        /// </summary>
        /// <param name="milliseconds">Numero di millisecondi di attesa</param>
        protected void Sleep(int milliseconds)
        {
            Task.Delay(milliseconds, cancellation_token_source.Token).Wait();
        }

        /// <summary>
        /// Passa istantaneamente il controllo al s.o.
        /// </summary>
        protected void Sleep()
        {
            Sleep(1);
        }

        #endregion
    }
}
