using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.Classes;
using WPProcinal.Models.ApiLocal;
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Service
{
    public class ApiLocal
    {
        #region "Referencias"
        private HttpClient client;
        private string basseAddress;
        private HttpResponseMessage response;
        private RequestAuth requestAuth;
        private static RequestApi requestApi;
        private string User4Told;
        private string Password4Told;
        #endregion

        #region "Constructor"
        public ApiLocal()
        {
            basseAddress = Utilities.GetConfiguration("basseAddressLocal");
            client = new HttpClient();
            requestApi = new RequestApi();
            client.BaseAddress = new Uri(basseAddress);
            ReadKeys();
        }
        #endregion


        #region "Métodos"
        public async Task<bool> SecurityToken()
        {
            try
            {
                var request = JsonConvert.SerializeObject(requestAuth);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client = new HttpClient();
                client.BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressLocal"));
                var url = Utilities.GetConfiguration("GetToken");
                var authentication = Encoding.ASCII.GetBytes(User4Told + ":" + Password4Told);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    var requestresponse = JsonConvert.DeserializeObject<ResponseAuth>(result);
                    if (requestresponse != null)
                    {
                        if (requestresponse.CodeError == 200)
                        {
                            Utilities.TOKEN = requestresponse.Token;
                            Utilities.CorrespondentId = Convert.ToInt16(requestresponse.User);
                            Utilities.Session = Convert.ToInt16(requestresponse.Session);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ResponseApi> GetResponse<T>(T model, string controller)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client = new HttpClient();
                client.BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressLocal"));
                var url = Utilities.GetConfiguration(controller);
                var authentication = Encoding.ASCII.GetBytes(Utilities.TOKEN);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Utilities.TOKEN);
                HttpResponseMessage response;

                if (controller == "GetInvoiceData")
                {
                    url = string.Format(url, 21, "&", true);
                    response = await client.GetAsync(url);
                }
                else
                {
                    response = await client.PostAsync(url, content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseApi
                    {
                        CodeError = 500,
                        Message = response.ReasonPhrase
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseApi>(result);
                return responseApi;
            }
            catch (Exception ex)
            {
                return new ResponseApi
                {
                    CodeError = 100,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Método encargado de leer el archivo plano con la información de usuarios y passwords para consumir el API
        /// </summary>
        private void ReadKeys()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string[] text = System.IO.File.ReadAllLines(string.Format(@"{0}\keysProcinal.txt", path));
                if (text.Length > 0)
                {
                    string[] line1 = text[0].Split(';');
                    User4Told = line1[0].Split(':')[1];
                    Password4Told = line1[1].Split(':')[1];

                    string[] line2 = text[1].Split(';');
                    requestAuth = new RequestAuth
                    {
                        UserName = line2[0].Split(':')[1],
                        Password = line2[1].Split(':')[1],
                        Type = int.Parse(line2[2].Split(':')[1])
                    };
                }
            }
            catch (Exception ex)
            {
            }
        }


        public async Task<object> CallApi(string controller, object data = null)
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(Utilities.GetConfiguration("basseAddressLocal"));

                requestApi.Data = data;
                requestApi.Session = Utilities.Session;

                var request = JsonConvert.SerializeObject(requestApi);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration(controller);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Utilities.TOKEN);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {

                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var responseApi = JsonConvert.DeserializeObject<ResponseApi>(result);

                if (responseApi.CodeError == 200)
                {
                    return responseApi.Data;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        #endregion
    }
}
