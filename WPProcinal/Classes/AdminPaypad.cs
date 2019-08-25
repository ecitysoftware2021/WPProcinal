using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.DataModel;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Classes
{
    class AdminPaypad
    {
        static ApiLocal api = new ApiLocal();


        public async void UpdatePeripherals()
        {
            try
            {
                var response = await api.GetResponse(new RequestApi(), "InitPaypad");
                if (response.CodeError == 200)
                {
                    DataPaypad data = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());
                    Utilities.dataPaypad = data;
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
                Task.Run(() =>
                {
                    object result = "false";

                    if (log != null)
                    {
                        if (type == ELogType.General)
                        {
                            api.CallApi("SaveLog", (RequestLog)log);
                        }
                        else if (type == ELogType.Error)
                        {
                            api.CallApi("SaveLogError", (ERROR_LOG)log);
                        }
                        else
                        {
                            var error = (RequestLogDevice)log;
                            api.CallApi("SaveLogDevice", error);
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
                    if (desciption.Contains("FATAL"))
                    {
                        level = ELevelError.Strong;
                    }

                    List<PAYPAD_CONSOLE_ERROR> consoleError = new List<PAYPAD_CONSOLE_ERROR>()
                    {
                        new PAYPAD_CONSOLE_ERROR
                        {
                            PAYPAD_ID = Utilities.CorrespondentId,
                        DATE = DateTime.Now,
                        STATE = 1,
                        DESCRIPTION = desciption,
                        OBSERVATION = observation,
                        ERROR_ID = (int)error,
                        ERROR_LEVEL_ID = (int)level
                        }
                    };
                    api.CallApi("SaveErrorConsole", consoleError);

                });
            }
            catch
            {
            }
        }
    }
}