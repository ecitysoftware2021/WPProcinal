using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.DataModel;

namespace WPProcinal.Classes
{
    public class RequestLog
    {
        public string Reference { get; set; }

        public string Description { get; set; }
        public int STATE { get; set; }

        public int TRANSACTION_ID { get { return Utilities.IDTransactionDB; } }
    }

    public class RequestLogDevice
    {
        public int TransactionId { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Code { get; set; }

        public ELevelError Level { get; set; }
    }


    public class LogErrorGeneral
    {
        public string TypeConsult { get; set; }
        public string Consult { get; set; }
        public string Id_Nit { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int IdTransaction { get; set; }
        public decimal ValuePay { get; set; }
        public string Date { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public int IDCorresponsal { get; set; }
    }

    public class Error
    {
        public string Mensaje { get; set; }
        public string Metodo { get; set; }
    }



    public class LogService
    {
        public string NamePath { get; set; }

        public string FileName { get; set; }

        public void CreateLogsTransactions<T>(T model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                string fullPath = string.Format(@"C:\\LogsProcinal\{0}\", NamePath);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var nameFile = Path.Combine(fullPath, FileName);
                if (!File.Exists(nameFile))
                {
                    var archivo = File.CreateText(nameFile);
                    archivo.Close();
                }

                using (StreamWriter sw = File.AppendText(nameFile))
                {
                    sw.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void SaveRequestResponse(string operacion, string mensaje, int state)
        {
            try
            {
                AdminPaypad.SaveLog(new RequestLog
                {
                    Description = mensaje,
                    Reference = Dictionaries.Cinemas[Utilities.CinemaId] + " - " + operacion,
                    STATE = state
                }, ELogType.General);
            }
            catch { }
        }

        public static void CreateLogsPeticion(string operacion, string mensaje)
        {
            try
            {
                Error objeto = new Error
                {
                    Mensaje = mensaje,
                    Metodo = operacion
                };
                var json = JsonConvert.SerializeObject(objeto);
                string fullPath = string.Format(@"C:\\LogsPeticiones\");
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var nameFile = Path.Combine(fullPath, "Peticiones" + DateTime.Now.ToString("yyyyMMdd"));
                if (!File.Exists(nameFile))
                {
                    var archivo = File.CreateText(nameFile);
                    archivo.Close();
                }

                using (StreamWriter sw = File.AppendText(nameFile))
                {
                    sw.WriteLine(json);
                }
            }
            catch { }
        }
    }
}

