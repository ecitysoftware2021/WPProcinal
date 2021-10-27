using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


        public static async Task UpdatePeripherals()
        {
            try
            {
                var response = await api.GetResponse(new RequestApi(), "InitPaypad");
                if (response.CodeError == 200)
                {
                    Utilities.dataPaypad = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());
                    Utilities.dataPaypad.PaypadConfiguration.DeserializarExtraData();
                    Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.DefinirAmbiente(Utilities.dataPaypad.PaypadConfiguration.iS_PRODUCTION);
                }
                else
                {
                    Utilities.dataPaypad = new DataPaypad();
                    Utilities.dataPaypad.State = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.dataPaypad = new DataPaypad();
                Utilities.dataPaypad.State = false;
            }
        }

        public static void SaveLog(object log, ELogType type)
        {
            try
            {
                Task.Run(() =>
                {
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
                        }
                    }
                });
            }
            catch (Exception ex)
            {
               //GRAVE ERROR, Por último debe de guardar el log en el eventviewer
              // at: jmora
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
                //GRAVE ERROR, Por último debe de guardar el log en el eventviewer
                // att: jmora
            }
        }
    }
}