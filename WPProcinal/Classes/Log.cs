﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public class LogErrorApi
    {
        public int ERROR_LOG_ID { get; set; }
        public Nullable<int> LOGIN_API_ID { get; set; }
        public string NAME_CLASS { get; set; }
        public string NAME_FUNCTION { get; set; }
        public string MESSAGE_ERROR { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public int TYPE { get; set; }
        public Nullable<bool> STATE { get; set; }
    }

    public class LogUrlApi
    {
        public int Id { get; set; }
        public string Url { get; set; }
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

    public class LogDispenser
    {
        public string SendMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string TransactionId { get; set; }

        public DateTime DateDispenser { get; set; }
    }

    public class LogService
    {
        public string NamePath { get; set; }

        public string FileName { get; set; }

        public void CreateLogs<T>(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            if (!Directory.Exists(NamePath))
            {
                Directory.CreateDirectory(NamePath);
            }

            var nameFile = Path.Combine(NamePath, FileName);
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
    }
}

