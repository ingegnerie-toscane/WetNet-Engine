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
        const int DEFAULT_JOB_SLEEP_TIME = 1;

        /// <summary>
        /// Timeout di attesa di default per l'arresto del thread in millisecondi
        /// </summary>
        const int DEFAULT_JOB_STOP_TIMEOUT = 6000;

        #endregion

        #region Istanze

        /// <summary>
        /// Thread del job
        /// </summary>
        Thread th_job;

        /// <summary>
        /// Evento di reset manuale
        /// </summary>
        ManualResetEvent mre = new ManualResetEvent(false);

        #endregion

        #region Variabili globali

        /// <summary>
        /// Nome del job
        /// </summary>
        readonly string name;

        /// <summary>
        /// Tempo di attesa fra una esecuzione e la successiva del job (millisecondi)
        /// </summary>
        readonly int job_sleep_time;

        /// <summary>
        /// Timeout di attesa per l'arresto del job
        /// </summary>
        readonly int job_stop_timeout;

        /// <summary>
        /// Variabile semaforo per la condizione di run
        /// </summary>
        protected volatile bool run;        

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="name">Nome del job</param>
        /// <param name="job_sleep_time">Tempo di attesa fra una esecuzione e la successiva del job (millisecondi)</param>
        public WetJob(string name, int job_sleep_time = DEFAULT_JOB_SLEEP_TIME, int job_stop_timeout = DEFAULT_JOB_STOP_TIMEOUT)
        {
            this.name = name;
            if (job_sleep_time < 1)
                throw new Exception("'job_sleep_time' must be greater than 1 millisecond!");
            this.job_sleep_time = job_sleep_time;
            this.job_stop_timeout = job_stop_timeout;
            th_job = new Thread(JobThread);
            th_job.Name = name;
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Funzione di avvio del job
        /// </summary>
        public void Start()
        {
            if (th_job.ThreadState != System.Threading.ThreadState.Running)
            {
                run = true;
                mre.Reset();
                th_job = new Thread(JobThread);
                th_job.Name = name;
                th_job.Start();
            }
        }

        /// <summary>
        /// Funzione di arresto del job
        /// </summary>
        public void Stop()
        {
            if (th_job != null)
            {
                run = false;
                mre.Set();
                bool ret = th_job.Join(job_stop_timeout);
                if (!ret)
                    th_job.Abort();
            }
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
        void JobThread()
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
            while (run)
            {
                // Eseguo il job
                try
                {
                    DoJob();
                }
                catch (Exception ex)
                {
                    ExceptionsManager(ex);
                }
                // Attendo una pausa fra un'esecuzione e la successiva
                mre.WaitOne(job_sleep_time);
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
            Thread.Sleep(milliseconds);
        }

        #endregion
    }
}
