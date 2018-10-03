using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Service
{
    public class ApiServices
    {
        public static async Task<Response> GetDataApi(string controller,string value)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("UrlApi"))
                };

                var url = string.Format("{0}{1}", Utilities.GetConfiguration(controller), value);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var requestresponse = JsonConvert.DeserializeObject<Response>(result);
                return requestresponse;
            }
            catch(Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static async Task<Response> SetDataApi<T>(T model, string controller)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Utilities.GetConfiguration("UrlApi"))
                };

                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                var url = Utilities.GetConfiguration(controller);
                var response = await client.PostAsync(url,content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var requestresponse = JsonConvert.DeserializeObject<Response>(result);
                return requestresponse;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
