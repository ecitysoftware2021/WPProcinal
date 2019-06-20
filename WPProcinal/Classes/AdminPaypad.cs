using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Classes
{
    class AdminPaypad
    {
        ApiLocal api = new ApiLocal();
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
    }
}