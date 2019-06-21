using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.Classes.DBConnection;
using WPProcinal.DataModel;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Classes
{
    class AdminPaypad
    {
        static ApiLocal api = new ApiLocal();

        public static SqliteDataAccess consults = new SqliteDataAccess();


        public async void UpdatePeripherals()
        {
            try
            {
                var response = await api.GetResponse(new RequestApi(), "InitPaypad");
                if (response.CodeError == 200)
                {
                    DataPaypad data = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());
                    Utilities.dataPaypad = data;

                    if (!string.IsNullOrEmpty(data.Message))
                    {
                        SendEmails.SendEmail(data.Message);
                    }
                }
                else
                {
                    Utilities.dataPaypad.State = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.dataPaypad.State = false;
            }
        }


        public async static void SaveLog(object log, ELogType type)
        {
            try
            {
                Task.Run(async () =>
                {
                    //var data = consults.SaveLog(log, type);
                    object result = "false";

                    if (log != null)
                    {
                        if (type == ELogType.General)
                        {
                            result = await api.CallApi("SaveLog", (RequestLog)log);
                        }
                        else if (type == ELogType.Error)
                        {
                            result = await api.CallApi("SaveLogError", (ERROR_LOG)log);
                        }
                        else
                        {
                            var error = (RequestLogDevice)log;
                            result = await api.CallApi("SaveLogDevice", error);
                            SaveErrorControl(error.Description, "", EError.Device, error.Level);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                //utilities.saveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex.ToString(), "Ocurrio un error");
            }
        }

        public static void SaveErrorControl(string desciption, string observation, EError error, ELevelError level, int device = 0)
        {
            try
            {
                Task.Run(() =>
                {

                    var idPaypad = Utilities.CorrespondentId;
                    if (idPaypad == 0)
                    {
                        idPaypad = int.Parse(Utilities.GetConfiguration("idPaypad"));
                    }

                    if (desciption.Contains("FATAL"))
                    {
                        level = ELevelError.Strong;
                    }

                    PAYPAD_CONSOLE_ERROR consoleError = new PAYPAD_CONSOLE_ERROR
                    {
                        PAYPAD_ID = (int)idPaypad,
                        DATE = DateTime.Now,
                        STATE = 0,
                        DESCRIPTION = desciption,
                        OBSERVATION = observation,
                        ERROR_ID = (int)error,
                        ERROR_LEVEL_ID = (int)level
                    };

                    consults.InsetConsoleError(consoleError);

                });
            }
            catch
            {
            }
        }


    }
}