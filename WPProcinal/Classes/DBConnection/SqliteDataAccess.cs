using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.DataModel;

namespace WPProcinal.Classes.DBConnection
{
    class SqliteDataAccess
    {

        public SqliteDataAccess()
        {
        }

        public object SaveLog(object log, ELogType type)
        {
            try
            {
                string query = "";
                object data = null;
                if (type == ELogType.General)
                {
                    data = new PAYPAD_LOG
                    {
                        REFERENCE = ((RequestLog)log).Reference,
                        DESCRIPTION = ((RequestLog)log).Description,
                        STATE = true
                    };

                    query = "INSERT INTO PAYPAD_LOG (" +
                                    "REFERENCE," +
                                    "DESCRIPTION, " +
                                    "STATE) VALUES (" +
                                    "@REFERENCE, " +
                                    "@DESCRIPTION, " +
                                    "@STATE)";
                }
                else if (type == ELogType.Error)
                {
                    data = (ERROR_LOG)log;

                    query = "INSERT INTO ERROR_LOG (" +
                                    "NAME_CLASS, " +
                                    "NAME_FUNCTION," +
                                    "MESSAGE_ERROR, " +
                                    "DESCRIPTION, " +
                                    "DATE, " +
                                    "TYPE," +
                                    "STATE) VALUES (" +
                                    "@NAME_CLASS, " +
                                    "@NAME_FUNCTION, " +
                                    "@MESSAGE_ERROR, " +
                                    "@DESCRIPTION, " +
                                    "@DATE, " +
                                    "@TYPE, " +
                                    "@STATE)";
                }
                else
                {
                    var logDevice = (RequestLogDevice)log;

                    data = new DEVICE_LOG
                    {
                        TRANSACTION_ID = logDevice.TransactionId,
                        CODE = logDevice.Code,
                        DATETIME = logDevice.Date,
                        DESCRIPTION = logDevice.Description
                    };

                    query = "INSERT INTO DEVICE_LOG (" +
                                    "TRANSACTION_ID, " +
                                    "DESCRIPTION," +
                                    "DATETIME, " +
                                    "CODE) VALUES (" +
                                    "@TRANSACTION_ID, " +
                                    "@DESCRIPTION, " +
                                    "@DATETIME, " +
                                    "@CODE)";
                }
                if (!string.IsNullOrEmpty(query) && data != null)
                {
                    return this.Execute<object>(query, data);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public bool InsetConsoleError(PAYPAD_CONSOLE_ERROR error)
        {
            try
            {
                var query = "INSERT INTO PAYPAD_CONSOLE_ERROR (" +
                        "PAYPAD_ID, " +
                        "ERROR_ID," +
                        "ERROR_LEVEL_ID, " +
                        "DEVICE_PAYPAD_ID, " +
                        "DESCRIPTION, " +
                        "DATE, " +
                        "OBSERVATION, " +
                        "STATE) VALUES (" +
                        "@PAYPAD_ID, " +
                        "@ERROR_ID, " +
                        "@ERROR_LEVEL_ID, " +
                        "@DEVICE_PAYPAD_ID, " +
                        "@DESCRIPTION, " +
                        "@DATE, " +
                        "@OBSERVATION, " +
                        "@STATE)";
                this.Execute<PAYPAD_CONSOLE_ERROR>(query, error);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            string conect = ConfigurationManager.ConnectionStrings[id].ConnectionString;
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public List<T> Select<T>(string query)
        {
            List<T> result = default(List<T>);
            try
            {
                using (IDbConnection connection = new SQLiteConnection(@"" + Utilities.GetConfiguration("ConnectionString").ToString()))
                {
                    try
                    {
                        result = connection.Query<T>(query).ToList();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public int Execute<T>(string query, T data)
        {
            object result = 0;
            try
            {
                using (IDbConnection connection = new SQLiteConnection(@"" + Utilities.GetConfiguration("ConnectionString").ToString()))
                {
                    try
                    {
                        if (data == null)
                        {
                            result = (int)connection.Execute(query);
                        }
                        else
                        {
                            result = connection.Execute(query, data);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return (int)result;
        }
    }
}
