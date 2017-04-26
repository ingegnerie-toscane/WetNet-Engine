using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Data;
using System.IO;
using System.Security.Cryptography;

namespace WetLib
{
    /// <summary>
    /// Classe per l'implementazione del servizio web
    /// </summary>
    class WJ_WebService : WetJob
    {
        #region Interfaccie

        /// <summary>
        /// Interfaccia per servizio web
        /// </summary>
        [ServiceContract]
        interface IWService
        {
            #region Funzioni del modulo            

            [OperationContract]
            [WebGet]
            Stream GetGISAllFieldPoints(string start_date, string stop_date);

            [OperationContract]
            [WebGet]
            Stream GetGISAllDistricts(string start_date, string stop_date);

            [OperationContract]
            [WebGet]
            Stream GetGISAllFieldPointsDayStatistics(string start_date, string stop_date);

            [OperationContract]
            [WebGet]
            Stream GetGISAllDistrictsDayStatistics(string start_date, string stop_date);

            [OperationContract]
            [WebGet]
            Stream GetFieldPointsInputTypes();

            [OperationContract]
            [WebGet]
            Stream GetFieldPointsObjectTypes();

            [OperationContract]
            [WebGet]
            Stream ResetDistrictData(string id_district, string date, string check);

            [OperationContract]
            [WebGet]
            Stream GetMonthsAverageOfYear(string id_district, string year);

            [OperationContract]
            [WebGet]
            Stream GetDayTrend(string id_district, string date);

            [OperationContract]
            [WebGet]
            Stream GetDayTrendCSV(string id_district, string date);

            [OperationContract]
            [WebGet]
            Stream GetDayTrendEx(string id_district, string date);

            [OperationContract]
            [WebGet]
            Stream GetDayTrendCSVEx(string id_district, string date);

            [OperationContract]
            [WebGet]
            Stream GetDayTrends();

            #endregion
        }

        #endregion

        #region Classe per servizio web

        /// <summary>
        /// Classe per la definizione del servizio
        /// </summary>
        class WService : IWService
        {
            #region Istanze

            /// <summary>
            /// Connessione al database WetNet
            /// </summary>
            WetDBConn wet_db;

            #endregion

            #region Costruttore

            /// <summary>
            /// Costruttore
            /// </summary>
            public WService()
            {
                // Istanzio la connessione al database wetnet
                WetConfig wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
            }

            #endregion                      

            #region Funzioni GET/POST            

            /// <summary>
            /// Restituisce un array di valori fra due date
            /// </summary>
            /// <param name="start_date">Data di inzio</param>
            /// <param name="stop_date">Data di fine</param>
            /// <returns>Stringa con le serie timestamp/valore</returns>
            /// <remarks>
            /// Questa funzione restituisce tutti i punti di misura in campo che abbiano un
            /// IDGIS associato.
            /// </remarks>
            public Stream GetGISAllFieldPoints(string start_date, string stop_date)
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    DataTable measures;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (measures.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow measure in measures.Rows)
                        {
                            DataTable dt;
                            string id_gis;
                            DateTime start = DateTime.MinValue;
                            DateTime stop = DateTime.Now;

                            int id_measure = Convert.ToInt32(measure["id_measures"]);
                            MeterTypes mtype = (MeterTypes)Convert.ToInt32(measure["strumentation_type"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                id_gis = Convert.ToString(measure["sap_code"]);
                            else
                                id_gis = Convert.ToString(measure["gis_code"]);

                            try
                            {
                                // Converto i valori in data/ora
                                start = Convert.ToDateTime(start_date);
                                stop = Convert.ToDateTime(stop_date);
                                // Leggo i dati
                                dt = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_measures WHERE measures_id_measures = " + id_measure +
                                    " AND `timestamp` >= '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " AND `timestamp` <= '" + stop.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " ORDER BY `timestamp` ASC");
                                if (dt.Rows.Count > 0)
                                {                                    
                                    // Ciclo per tutti i dati letti
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                                        double value = Convert.ToDouble(dr["value"]);
                                        ret += "<Record id_gis=\"" + id_gis.ToString() + "\" id=\"" + ts.Ticks + "\"><InputType>" +
                                            ((int)WetUtility.GetInputTypeFromMeterType(mtype)).ToString() + "</InputType><ObjectType>" + ((int)mtype).ToString() +
                                            "</ObjectType><Timestamp>" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                            "</Timestamp><Value>" + value.ToString().Replace(',', '.') +
                                            "</Value></Record>";
                                    }                                    
                                }
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }                        
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";                    
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un array di valori fra due date con quality
            /// </summary>
            /// <param name="start_date">Data di inzio</param>
            /// <param name="stop_date">Data di fine</param>
            /// <returns>Stringa con le serie timestamp/valore</returns>
            /// <remarks>
            /// Questa funzione restituisce tutti i punti di misura in campo che abbiano un
            /// IDGIS associato.
            /// </remarks>
            public Stream GetGISAllFieldPointsEx(string start_date, string stop_date)
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    DataTable measures;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (measures.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow measure in measures.Rows)
                        {
                            DataTable dt;
                            string id_gis;
                            DateTime start = DateTime.MinValue;
                            DateTime stop = DateTime.Now;

                            int id_measure = Convert.ToInt32(measure["id_measures"]);
                            MeterTypes mtype = (MeterTypes)Convert.ToInt32(measure["strumentation_type"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                id_gis = Convert.ToString(measure["sap_code"]);
                            else
                                id_gis = Convert.ToString(measure["gis_code"]);

                            try
                            {
                                // Converto i valori in data/ora
                                start = Convert.ToDateTime(start_date);
                                stop = Convert.ToDateTime(stop_date);
                                // Leggo i dati
                                dt = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_measures WHERE measures_id_measures = " + id_measure +
                                    " AND `timestamp` >= '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " AND `timestamp` <= '" + stop.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " ORDER BY `timestamp` ASC");
                                if (dt.Rows.Count > 0)
                                {
                                    // Ciclo per tutti i dati letti
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                                        double value = Convert.ToDouble(dr["value"]);
                                        ret += "<Record id_gis=\"" + id_gis.ToString() + "\" id=\"" + ts.Ticks + "\"><InputType>" +
                                            ((int)WetUtility.GetInputTypeFromMeterType(mtype)).ToString() + "</InputType><ObjectType>" + ((int)mtype).ToString() +
                                            "</ObjectType><Timestamp>" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                            "</Timestamp><Value>" + value.ToString().Replace(',', '.') +
                                            "</Value></Record>";
                                    }
                                }
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un array di valori fra due date
            /// </summary>
            /// <param name="start_date">Data di inzio</param>
            /// <param name="stop_date">Data di fine</param>
            /// <returns>Stringa con le serie timestamp/valore</returns>
            /// <remarks>
            /// Questa funzione restituisce tutti i distretti che abbiano un
            /// IDGIS associato.
            /// </remarks>
            public Stream GetGISAllDistricts(string start_date, string stop_date)
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    DataTable districts;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (districts.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow district in districts.Rows)
                        {
                            DataTable dt;
                            string id_gis;
                            DateTime start = DateTime.MinValue;
                            DateTime stop = DateTime.Now;

                            int id_district = Convert.ToInt32(district["id_districts"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            {
                                id_gis = Convert.ToString(district["sap_code"]);
                                if ((id_gis[0] != '$') && (id_gis.Last() != '$'))
                                    continue;
                                else
                                    id_gis = id_gis.Remove(id_gis.Length - 1).Remove(0, 1);
                            }
                            else
                                id_gis = Convert.ToString(district["gis_code"]);                           

                            try
                            {
                                // Converto i valori in data/ora
                                start = Convert.ToDateTime(start_date);
                                stop = Convert.ToDateTime(stop_date);
                                // Leggo i dati
                                dt = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_districts WHERE districts_id_districts = " + id_district +
                                    " AND `timestamp` >= '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " AND `timestamp` <= '" + stop.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'" +
                                    " ORDER BY `timestamp` ASC");
                                if (dt.Rows.Count > 0)
                                {

                                    // Ciclo per tutti i dati letti
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                                        double value = Convert.ToDouble(dr["value"]);
                                        ret += "<Record id_gis=\"" + id_gis.ToString() + "\" id=\"" + ts.Ticks + 
                                            "\"><Timestamp>" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                            "</Timestamp><Value>" + value.ToString().Replace(',', '.') +
                                            "</Value></Record>";
                                    }
                                }
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un array di valori valori statistici fra due date
            /// </summary>
            /// <param name="start_date">Data di inizio</param>
            /// <param name="stop_date">Data di fine</param>
            /// <returns>Stringa con le serie timestamp/valore</returns>
            /// <remarks>
            /// Questa funzione restituisce tutte le misure che abbiano un
            /// IDGIS associato.
            /// </remarks>
            public Stream GetGISAllFieldPointsDayStatistics(string start_date, string stop_date)
            {
                string ret = "<?xml version =\"1.0\"?>";
                
                try
                {
                    DataTable measures;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        measures = wet_db.ExecCustomQuery("SELECT * FROM measures WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (measures.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow measure in measures.Rows)
                        {
                            DataTable dt;
                            string id_gis;
                            DateTime start = DateTime.MinValue;
                            DateTime stop = DateTime.Now;

                            int id_measure = Convert.ToInt32(measure["id_measures"]);
                            MeterTypes mtype = (MeterTypes)Convert.ToInt32(measure["strumentation_type"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                id_gis = Convert.ToString(measure["sap_code"]);
                            else
                                id_gis = Convert.ToString(measure["gis_code"]);

                            try
                            {
                                // Converto i valori in data/ora
                                start = Convert.ToDateTime(start_date);
                                stop = Convert.ToDateTime(stop_date);
                                // Leggo i dati
                                dt = wet_db.ExecCustomQuery("SELECT `day`, `min_night`, `min_day`, `max_day`, `avg_day` FROM measures_day_statistic WHERE measures_id_measures = " + id_measure +
                                    " AND `day` >= '" + start.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'" +
                                    " AND `day` <= '" + stop.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'" +
                                    " ORDER BY `day` ASC");
                                if (dt.Rows.Count > 0)
                                {

                                    // Ciclo per tutti i dati letti
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        DateTime day = Convert.ToDateTime(dr["day"]);
                                        double min_night = WetMath.ValidateDouble(Convert.ToDouble(dr["min_night"] == DBNull.Value ? 0.0d : dr["min_night"]));
                                        double min_day = WetMath.ValidateDouble(Convert.ToDouble(dr["min_day"] == DBNull.Value ? 0.0d : dr["min_day"]));
                                        double max_day = WetMath.ValidateDouble(Convert.ToDouble(dr["max_day"] == DBNull.Value ? 0.0d : dr["max_day"]));
                                        double avg_day = WetMath.ValidateDouble(Convert.ToDouble(dr["avg_day"] == DBNull.Value ? 0.0d : dr["avg_day"]));
                                        ret += "<Record id_gis=\"" + id_gis.ToString() + "\" id=\"" + day.Ticks + "\"><InputType>" +
                                            ((int)WetUtility.GetInputTypeFromMeterType(mtype)).ToString() + "</InputType><ObjectType>" + ((int)mtype).ToString() +
                                            "</ObjectType><Day>" + day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) +
                                            "</Day><MinNight>" + min_night.ToString().Replace(',', '.') +
                                            "</MinNight><MinDay>" + min_day.ToString().Replace(',', '.') +
                                            "</MinDay><MaxDay>" + max_day.ToString().Replace(',', '.') +
                                            "</MaxDay><AvgDay>" + avg_day.ToString().Replace(',', '.') +
                                            "</AvgDay></Record>";
                                    }
                                }
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un array di valori valori statistici fra due date
            /// </summary>
            /// <param name="start_date">Data di inizio</param>
            /// <param name="stop_date">Data di fine</param>
            /// <returns>Stringa con le serie timestamp/valore</returns>
            /// <remarks>
            /// Questa funzione restituisce tutti i distretti che abbiano un
            /// IDGIS associato.
            /// </remarks>
            public Stream GetGISAllDistrictsDayStatistics(string start_date, string stop_date)
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    DataTable districts;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (districts.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow district in districts.Rows)
                        {
                            DataTable dt;
                            string id_gis;
                            DateTime start = DateTime.MinValue;
                            DateTime stop = DateTime.Now;

                            int id_district = Convert.ToInt32(district["id_districts"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            {
                                id_gis = Convert.ToString(district["sap_code"]);
                                if (id_gis[0] != '*')
                                    continue;
                                else
                                    id_gis = id_gis.Remove(0, 1);
                            }
                            else
                                id_gis = Convert.ToString(district["gis_code"]);

                            try
                            {
                                // Converto i valori in data/ora
                                start = Convert.ToDateTime(start_date);
                                stop = Convert.ToDateTime(stop_date);
                                // Leggo i dati
                                dt = wet_db.ExecCustomQuery("SELECT `day`, `min_night`, `min_day`, `max_day`, `avg_day` FROM districts_day_statistic WHERE districts_id_districts = " + id_district +
                                    " AND `day` >= '" + start.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'" +
                                    " AND `day` <= '" + stop.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'" +
                                    " ORDER BY `day` ASC");
                                if (dt.Rows.Count > 0)
                                {

                                    // Ciclo per tutti i dati letti
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        DateTime day = Convert.ToDateTime(dr["day"]);
                                        double min_night = WetMath.ValidateDouble(Convert.ToDouble(dr["min_night"] == DBNull.Value ? 0.0d : dr["min_night"]));
                                        double min_day = WetMath.ValidateDouble(Convert.ToDouble(dr["min_day"] == DBNull.Value ? 0.0d : dr["min_day"]));
                                        double max_day = WetMath.ValidateDouble(Convert.ToDouble(dr["max_day"] == DBNull.Value ? 0.0d : dr["max_day"]));
                                        double avg_day = WetMath.ValidateDouble(Convert.ToDouble(dr["avg_day"] == DBNull.Value ? 0.0d : dr["avg_day"]));
                                        ret += "<Record id_gis=\"" + id_gis.ToString() + "\" id=\"" + day.Ticks + "\"><Day>" + day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) +
                                            "</Day><MinNight>" + min_night.ToString().Replace(',', '.') +
                                            "</MinNight><MinDay>" + min_day.ToString().Replace(',', '.') +
                                            "</MinDay><MaxDay>" + max_day.ToString().Replace(',', '.') +
                                            "</MaxDay><AvgDay>" + avg_day.ToString().Replace(',', '.') +
                                            "</AvgDay></Record>";
                                    }
                                }
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce i tipi di ingresso delle misure
            /// </summary>
            /// <returns>Tipi di ingresso</returns>
            public Stream GetFieldPointsInputTypes()
            {
                string ret = "<?xml version =\"1.0\"?>";

                ret += "<InputsTypes>";
                foreach (InputMeterTypes imt in Enum.GetValues(typeof(InputMeterTypes)))
                {
                    ret += "<InputType id=\"" + ((int)imt).ToString() + "\">";
                    switch (imt)
                    {
                        default:
                        case InputMeterTypes.UNKNOWN:
                            ret += "<Description>Unknown</Description>";
                            break;

                        case InputMeterTypes.ANALOG_INPUT:
                            ret += "<Description>Analog input</Description>";
                            break;

                        case InputMeterTypes.PULSE_INPUT:
                            ret += "<Description>Digital pulse input</Description>";
                            break;

                        case InputMeterTypes.DIGITAL_STATE:
                            ret += "<Description>Digital binary state</Description>";
                            break;
                    }                    
                    ret += "</InputType>";
                }
                ret += "</InputsTypes>";

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce i tipi di misure acquisite dal campo
            /// </summary>
            /// <returns>Restituisce i tipi di misure disponibili</returns>
            public Stream GetFieldPointsObjectTypes()
            {
                string ret = "<?xml version =\"1.0\"?>";

                ret += "<ObjectTypes>";
                foreach (MeterTypes mtype in Enum.GetValues(typeof(MeterTypes)))
                {
                    ret += "<ObjectType id=\"" + ((int)mtype).ToString() + "\">";
                    switch (mtype)
                    {
                        default:
                        case MeterTypes.UNKNOWN:
                            ret += "<Class>Unknown measure</Class><EngineeringUnits></EngineeringUnits>";
                            break;

                        case MeterTypes.LCF_FLOW_METER:
                            ret += "<Class>LCF flow meter</Class><EngineeringUnits>l/s</EngineeringUnits>";
                            break;

                        case MeterTypes.MAGNETIC_FLOW_METER:
                            ret += "<Class>Magnetic flow meter</Class><EngineeringUnits>l/s</EngineeringUnits>";
                            break;

                        case MeterTypes.ULTRASONIC_FLOW_METER:
                            ret += "<Class>Ultrasonic flow meter</Class><EngineeringUnits>l/s</EngineeringUnits>";
                            break;

                        case MeterTypes.PRESSURE_METER:
                            ret += "<Class>Pressure measure</Class><EngineeringUnits>bar</EngineeringUnits>";
                            break;

                        case MeterTypes.VOLUMETRIC_COUNTER:
                            ret += "<Class>Flow conversion from volumetric counter</Class><EngineeringUnits>l/s</EngineeringUnits>";
                            break;

                        case MeterTypes.PUMP:
                            ret += "<Class>Pump status</Class><EngineeringUnits>Digital state</EngineeringUnits>";
                            break;

                        case MeterTypes.VALVE_NO_REGULATION:
                            ret += "<Class>Valve with no regulation</Class><EngineeringUnits>Digital state</EngineeringUnits>";
                            break;

                        case MeterTypes.VALVE_REGULATION:
                            ret += "<Class>Valve with regulation</Class><EngineeringUnits>%</EngineeringUnits>";
                            break;

                        case MeterTypes.TANK:
                            ret += "<Class>Tank level</Class><EngineeringUnits>mt</EngineeringUnits>";
                            break;

                        case MeterTypes.WELL:
                            ret += "<Class>Well level</Class><EngineeringUnits>mt</EngineeringUnits>";
                            break;

                        case MeterTypes.MOTOR_FREQUENCY:
                            ret += "<Class>Motor frequency</Class><EngineeringUnits>Hz</EngineeringUnits>";
                            break;
                    }
                    ret += "</ObjectType>";
                }
                ret += "</ObjectTypes>";

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Resetta tutti i dati storici associati al distretto specificato
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="date">Data di inizio da cui effettuare la cancellazione</param>
            /// <param name="check">Checksum</param>
            /// <returns>Restituisce il buon esito dell'operazione</returns>
            public Stream ResetDistrictData(string id_district, string date, string check)
            {
                string ret = "<?xml version =\"1.0\"?>";
                int ret_value = 0;

                try
                {
                    // Controllo il codice di checksum
                    MD5Cng md5 = new MD5Cng();
                    byte[] hash_vector = md5.ComputeHash(Encoding.ASCII.GetBytes(date));
                    string hash = string.Empty;
                    foreach (byte bt in hash_vector)
                        hash += bt.ToString("X2");
                    if (check.ToLower() == hash.ToLower())
                    {
                        // Acquisisco i parametri
                        int id = Convert.ToInt32(id_district);
                        DateTime start_date = Convert.ToDateTime(date).Date;
                        DateTime start_month = new DateTime(start_date.Year, start_date.Month, DateTime.DaysInMonth(start_date.Year, start_date.Month));
                        DateTime start_year = new DateTime(start_date.Year, 12, 31);
                        try
                        {
                            // Inserisco il blocco su tutte le tabelle da modificare
                            wet_db.ExecCustomCommand("LOCK TABLES districts WRITE, data_districts WRITE, districts_bands_history WRITE, " +
                                "districts_energy_profile WRITE, districts_energy_day_statistic WRITE, districts_statistic_profiles WRITE, " +
                                "districts_events WRITE, districts_day_statistic WRITE, districts_month_statistic WRITE, districts_year_statistic WRITE");
                            // Elimino i profili di consumo
                            wet_db.ExecCustomCommand("DELETE FROM data_districts WHERE districts_id_districts = " + id.ToString() + 
                                " AND `timestamp` >= '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                            // Elimino lo storico delle bande
                            wet_db.ExecCustomCommand("DELETE FROM districts_bands_history WHERE districts_id_districts = " + id.ToString() +
                                " AND `timestamp` >= '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                            // Elimino i profili energetici
                            wet_db.ExecCustomCommand("DELETE FROM districts_energy_profile WHERE districts_id_districts = " + id.ToString() +
                                " AND `timestamp` >= '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                            // Elimino le statistiche energetiche
                            wet_db.ExecCustomCommand("DELETE FROM districts_energy_day_statistic WHERE districts_id_districts = " + id.ToString() +
                                " AND `day` >= '" + start_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            // Elimino i profili statistici
                            wet_db.ExecCustomCommand("DELETE FROM districts_statistic_profiles WHERE districts_id_districts = " + id.ToString() +
                                " AND `timestamp` >= '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                            // Elimino gli eventi
                            wet_db.ExecCustomCommand("DELETE FROM districts_events WHERE districts_id_districts = " + id.ToString() +
                                " AND `day` >= '" + start_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            // Elimino le statistiche giornaliere
                            wet_db.ExecCustomCommand("DELETE FROM districts_day_statistic WHERE districts_id_districts = " + id.ToString() +
                                " AND `day` >= '" + start_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            // Elimino le statistiche mensili
                            wet_db.ExecCustomQuery("DELETE FROM districts_month_statistic WHERE districts_id_districts = " + id.ToString() +
                                " AND `month` >= '" + start_month.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            // Elimino le statistiche annuali
                            wet_db.ExecCustomQuery("DELETE FROM districts_year_statistic WHERE districts_id_districts = " + id.ToString() +
                                " AND `year` >= '" + start_year.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            // Imposto lo stato di successo
                            ret_value = 1;
                        }
                        catch (Exception ex)
                        {
                            WetDebug.GestException(ex);
                        }
                        finally
                        {
                            try
                            {
                                // Tolgo il blocco
                                wet_db.ExecCustomCommand("UNLOCK TABLES");
                                // Aggiorno il campo di reset
                                wet_db.ExecCustomCommand("UPDATE districts SET `reset_all_data` = " + (id + 1).ToString() + " WHERE id_districts = " + id.ToString());
                            }
                            catch (Exception ex)
                            {                                
                                WetDebug.GestException(ex);
                                // Imposto uno stato di fallimento
                                ret_value = 0;
                            }
                        }
                    }
                }
                catch (Exception ex1)
                {
                    WetDebug.GestException(ex1);
                }
                ret += "<Return>" + ret_value.ToString() + "</Return>";

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce la media mensile per ciascun mese dell'anno specificato
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="year">Anno da analizzare</param>
            /// <returns>Restituisce un vettore con 12 valori</returns>
            public Stream GetMonthsAverageOfYear(string id_district, string year)
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    ret += "<Months>";
                    // Designazione finestar temporale
                    DateTime start_date = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime stop_date = start_date.AddYears(1);
                    // Query
                    DataTable dt = wet_db.ExecCustomQuery("SELECT `month`, `avg_month` FROM districts_month_statistic WHERE `districts_id_districts` = " + id_district +
                        " AND `month` >= '" + start_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND `month` < '" + stop_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + 
                        "' ORDER BY `month` ASC");
                    // Compongo la risposta
                    foreach (DataRow dr in dt.Rows)
                    {
                        int month = Convert.ToDateTime(dr["month"]).Month;
                        double avg = Convert.ToDouble(dr["avg_month"]);
                        ret += "<Month id=\"" + month.ToString() + "\">" + "<Average>" + avg.ToString() +
                            "</Average></Month>";
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
                finally
                {
                    ret += "</Months>";
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un trend previsionale per un dato giorno
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="date">Data da analizzare</param>
            /// <returns>Vettore con i campioni giornalieri</returns>
            public Stream GetDayTrend(string id_district, string date)
            {                
                string ret = "<?xml version =\"1.0\"?>";

                ret += "<Samples>";
                try
                {
                    
                    // Imposto i parametri                    
                    int id = Convert.ToInt32(id_district);
                    DateTime dt = Convert.ToDateTime(date);
                    int interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
                    int samples_in_day = (int)(24 * 60 / WetConfig.GetInterpolationTimeMinutes());
                    // Compongo il vettore
                    DayTrendSample[] profile = WetUtility.GetDayTrend(id, dt, samples_in_day, 8);
                    // Compongo la risposta
                    for (int ii = 0; ii < profile.Length; ii++)
                    {
                        ret += "<Sample id=\"" + profile[ii].timestamp.Ticks.ToString() + "\">";
                        ret += "<TimeStamp>" + profile[ii].timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "</TimeStamp>" +
                            "<HiValue>" + profile[ii].hi_value.ToString().Replace(',', '.') + "</HiValue>" +
                            "<AvgValue>" + profile[ii].avg_value.ToString().Replace(',', '.') + "</AvgValue>" +
                            "<LoValue>" + profile[ii].lo_value.ToString().Replace(',', '.') + "</LoValue>";
                        ret += "</Sample>";
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
                ret += "</Samples>";

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un trend previsionale per un dato giorno in formato CSV
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="date">Data da analizzare</param>
            /// <returns>Vettore con i campioni giornalieri</returns>
            public Stream GetDayTrendCSV(string id_district, string date)
            {
                int id = 0;
                DateTime dt = new DateTime();

                string ret = "TIMESTAMP;LO_VALUE;AVERAGE;HI_VALUE;" + Environment.NewLine;
                try
                {

                    // Imposto i parametri                    
                    id = Convert.ToInt32(id_district);
                    dt = Convert.ToDateTime(date);
                    int interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
                    int samples_in_day = (int)(24 * 60 / WetConfig.GetInterpolationTimeMinutes());
                    // Compongo il vettore
                    DayTrendSample[] profile = WetUtility.GetDayTrend(id, dt, samples_in_day, 8);
                    // Compongo la risposta
                    for (int ii = 0; ii < profile.Length; ii++)
                    {
                        ret += profile[ii].timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + ";" +
                            profile[ii].lo_value.ToString().Replace(',', '.') + ";" +
                            profile[ii].avg_value.ToString().Replace(',', '.') + ";" +
                            profile[ii].hi_value.ToString().Replace(',', '.') + ";" + Environment.NewLine;
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers["Content-Disposition"] = "attachment; filename=" + id.ToString() + "_" + dt.ToString("yyyyMMdd") + "_DayTrend.csv";
                context.ContentType = "application/octet-stream";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un trend previsionale per un dato giorno
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="date">Data da analizzare</param>
            /// <returns>Vettore con i campioni giornalieri</returns>
            public Stream GetDayTrendEx(string id_district, string date)
            {
                string ret = "<?xml version =\"1.0\"?>";

                ret += "<Samples>";
                try
                {

                    // Imposto i parametri                    
                    int id = Convert.ToInt32(id_district);
                    DateTime dt = Convert.ToDateTime(date);
                    int interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
                    int samples_in_day = (int)(24 * 60 / WetConfig.GetInterpolationTimeMinutes());
                    // Compongo il vettore
                    DayTrendSample[] profile = WetUtility.GetDayTrendEx(id, dt, samples_in_day, 8);
                    // Compongo la risposta
                    for (int ii = 0; ii < profile.Length; ii++)
                    {
                        ret += "<Sample id=\"" + profile[ii].timestamp.Ticks.ToString() + "\">";
                        ret += "<TimeStamp>" + profile[ii].timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "</TimeStamp>" +
                            "<HiValue>" + profile[ii].hi_value.ToString().Replace(',', '.') + "</HiValue>" +
                            "<AvgValue>" + profile[ii].avg_value.ToString().Replace(',', '.') + "</AvgValue>" +
                            "<LoValue>" + profile[ii].lo_value.ToString().Replace(',', '.') + "</LoValue>";
                        ret += "</Sample>";
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
                ret += "</Samples>";

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce un trend previsionale per un dato giorno in formato CSV
            /// </summary>
            /// <param name="id_district">ID univoco del distretto</param>
            /// <param name="date">Data da analizzare</param>
            /// <returns>Vettore con i campioni giornalieri</returns>
            public Stream GetDayTrendCSVEx(string id_district, string date)
            {
                int id = 0;
                DateTime dt = new DateTime();

                string ret = "TIMESTAMP;LO_VALUE;AVERAGE;HI_VALUE;" + Environment.NewLine;
                try
                {

                    // Imposto i parametri                    
                    id = Convert.ToInt32(id_district);
                    dt = Convert.ToDateTime(date);
                    int interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
                    int samples_in_day = (int)(24 * 60 / WetConfig.GetInterpolationTimeMinutes());
                    // Compongo il vettore
                    DayTrendSample[] profile = WetUtility.GetDayTrendEx(id, dt, samples_in_day, 8);
                    // Compongo la risposta
                    for (int ii = 0; ii < profile.Length; ii++)
                    {
                        ret += profile[ii].timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + ";" +
                            profile[ii].lo_value.ToString().Replace(',', '.') + ";" +
                            profile[ii].avg_value.ToString().Replace(',', '.') + ";" +
                            profile[ii].hi_value.ToString().Replace(',', '.') + ";" + Environment.NewLine;
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.Headers["Content-Disposition"] = "attachment; filename=" + id.ToString() + "_" + dt.ToString("yyyyMMdd") + "_DayTrend.csv";
                context.ContentType = "application/octet-stream";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            /// <summary>
            /// Restituisce le curve caratteristiche di tutti i distretti con ID GIS associato
            /// </summary>
            /// <returns>Vettori</returns>
            public Stream GetDayTrends()
            {
                string ret = "<?xml version =\"1.0\"?>";

                try
                {
                    DataTable districts;

                    // Acquisisco tutte le misure presenti                
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE sap_code IS NOT NULL AND sap_code <> ''");
                    else
                        districts = wet_db.ExecCustomQuery("SELECT * FROM districts WHERE gis_code IS NOT NULL AND gis_code <> ''");

                    if (districts.Rows.Count > 0)
                    {
                        ret += "<Records>";
                        // Ciclo per tutte le entità
                        foreach (DataRow district in districts.Rows)
                        {
                            string id_gis;

                            int id_district = Convert.ToInt32(district["id_districts"]);
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            {
                                id_gis = Convert.ToString(district["sap_code"]);
                                if ((id_gis[0] != '$') && (id_gis.Last() != '$'))
                                    continue;
                                else
                                    id_gis = id_gis.Remove(id_gis.Length - 1).Remove(0, 1);
                            }
                            else
                                id_gis = Convert.ToString(district["gis_code"]);

                            try
                            {
                                int interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
                                int samples_in_day = (int)(24 * 60 / WetConfig.GetInterpolationTimeMinutes());
                                DateTime day = DateTime.Now.Date;
                                ret += "<Record id_gis=\"" + id_gis.ToString() + "\"><Days>";
                                for (int ii = 0; ii < 7; ii++)
                                {
                                    day = day.AddDays(1.0d);
                                    ret += "<Day day=\"" + day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "\" day_of_week=\"" + day.DayOfWeek.ToString() + "\">";
                                    DayTrendSample[] dts = WetUtility.GetDayTrendEx(id_district, day, samples_in_day, 8);
                                    ret += "<Profile>";
                                    foreach (DayTrendSample dt in dts)
                                    {
                                        ret += "<Timestamp>" + dt.timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "</Timestamp>";
                                        ret += "<LowValue>" + dt.lo_value.ToString("F2") + "</LowValue>";
                                        ret += "<AvgValue>" + dt.avg_value.ToString("F2") + "</AvgValue>";
                                        ret += "<HighValue>" + dt.hi_value.ToString("F2") + "</HighValue>";
                                    }
                                    ret += "</Profile>";
                                    ret += "</Day>";
                                }
                                ret += "</Days></Record>";
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                        }
                        ret += "</Records>";
                    }
                    else
                        ret += "<Error>No entities founds!</Error>";
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }

                // Composizione risposta
                OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
                context.ContentType = "text/plain";
                return new MemoryStream(Encoding.Default.GetBytes(ret));
            }

            #endregion
        }

        #endregion

        #region Istanze

        /// <summary>
        /// Servizio host
        /// </summary>
        WebServiceHost host;

        /// <summary>
        /// Endpoint del servizio
        /// </summary>
        ServiceEndpoint sep;

        /// <summary>
        /// Configurazione del job
        /// </summary>
        WetConfig.WJ_WebService_Config_Struct cfg;

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Funzione di caricamento del job
        /// </summary>
        protected override void Load()
        {
            try
            {
                WetConfig wcfg = new WetConfig();
                cfg = wcfg.GetWJ_Webservice_Config();
                host = new WebServiceHost(typeof(WService), new Uri("http://localhost:" + cfg.port.ToString() + "/"));
                sep = host.AddServiceEndpoint(typeof(IWService), new WebHttpBinding(), "");
                host.Open();
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            // Dummy
        }

        /// <summary>
        /// Funzione di scaricamento del job
        /// </summary>
        protected override void UnLoad()
        {
            if (host != null)
            {
                try
                {
                    if (host.State == CommunicationState.Opened)
                        host.Close();
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
            }
        }

        #endregion
    }
}
