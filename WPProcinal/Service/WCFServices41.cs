using EncryptAndDecryptDLL;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Service
{
    public class WCFServices41
    {

        static EcityDataEncrypt dataEncrypt = new EcityDataEncrypt();

        private static void ServerCertificateValidationCallback()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        }

        /// <summary>
        /// DESCARGA Y DESERIALIZACIÓN DEL XML DE LAS FUNCIONES
        /// </summary>
        /// <returns></returns>
        #region DESCARGA XML

        public static Response DownloadData()
        {
            ServerCertificateValidationCallback();
            using (var w = new WebClient())
            {
                var data = string.Empty;
                string url = Utilities.GetConfiguration("WebServiceUrl");
                try
                {
                    w.Encoding = Encoding.UTF8;
                    data = w.DownloadString(url);
                    w.Dispose();
                    return new Response
                    {
                        IsSuccess = true,
                        Result = data,
                    };
                }
                catch (Exception ex)
                {
                    w.Dispose();
                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                }
            }
        }
        public static T DeserealizeXML<T>(string outerXml)
        {
            var typeParameterType = typeof(T);
            XmlSerializer serializer = new XmlSerializer(typeParameterType);
            var reader = new StringReader(outerXml);
            var mapaSala = (T)serializer.Deserialize(reader);

            return mapaSala;
        }

        #endregion

        /// <summary>
        /// TRAER LA SALA PARA PINTAR
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOEST
        public static List<EstadoSala41> GetStateRoom(SCOEST data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion sala", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scoest/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);

                var est = JsonConvert.DeserializeObject<List<EstadoSala41>>(decryptData);
                if (est.Count < 2)
                {
                    LogService.SaveRequestResponse("Respuesta consulta sala", decryptData, 1);
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// OBTENER LOS PRECIOS PARA LAS UBICACIONES
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOPLA
        public static List<ResponseTarifa> GetPrices(SCOPLA data)
        {
            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion precios sillas", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scopla/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta Precios Sillas", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseTarifa>>(decryptData);
                if (est[0].sala == null)
                {
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// OBTENER LA SECUENCIA DE COMPRA
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOSEC
        public static List<ResponseSec> GetSecuence(SCOSEC data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion secuencia", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scosec/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta secuencia", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseSec>>(decryptData);

                if (est[0].Secuencia == 0)
                {
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// REALIZAR PREVENTA PARA ASEGURAR LOS PUESTOS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOGRU
        public static List<ResponseScogru> PostPreventa(SCOGRU data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion preventa", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scogru/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta preventa", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseScogru>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// CONFIRMACION DE LA COMPRA
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOINT
        public static List<ResponseScoint> PostBuy(SCOINT data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion Compra", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scoint/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta Compra", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseScoint>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool StateImage(string image)
        {
            try
            {
                var client = new RestClient(image);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusDescription == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// ELIMINAR PREVENTA
        /// </summary>
        /// <param name="typeSeatsCurrent"></param>
        /// <param name="dipMapCurrent"></param>
        #region SCOSIL
        public static List<ResponseScoint> PostDesAssingreserva(ChairsInformation typeSeatsCurrent, FunctionInformation dipMapCurrent)
        {
            string decryptData = string.Empty;
            try
            {

                SCOSIL data = new SCOSIL
                {
                    Columna = typeSeatsCurrent.RelativeColumn,
                    FechaFuncion = dipMapCurrent.Date,
                    Fila = typeSeatsCurrent.RelativeRow,
                    Funcion = dipMapCurrent.IDFuncion,
                    Sala = dipMapCurrent.RoomId,
                    teatro = dipMapCurrent.CinemaId,
                    tercero = "1",
                    Usuario = 777
                };

                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion desreservar", seria, 1);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scosil/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta al desreservar", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseScoint>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// SERVICIO PARA VISUALIZAR LISTA DE COMBOS Y PRECIOS CONFIGURADOS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOPRE
        public static Confiteria GetConfectionery(SCOPRE data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion cnfiteria", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scopre/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);

                var est = JsonConvert.DeserializeObject<Confiteria>(decryptData);
                if (est.ListaProductos.Count < 1)
                {
                    LogService.SaveRequestResponse("Respuesta al consultar los productos", decryptData, 1);
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Servicio para consultar la clave de un usuario
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOCSN
        public static string GetUserKey(SCOCED data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion clave cliente", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scoced/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta clave cliente", decryptData, 1);
                var est = JsonConvert.DeserializeObject<SCOCSNResponse[]>(decryptData);

                return est[0].Valor;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Servicio para consultar un cliente y el estado de la membresia
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        #region SCOLOG
        public static List<SCOLOGResponse> GetClientData(SCOCED data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion data cliente", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);


                var client = new RestClient(DataService41.APISCORE + "/scoced/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);
                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta al consultar la data del cliente", decryptData, 1);
                var clientdata = JsonConvert.DeserializeObject<List<SCOLOGResponse>>(decryptData);
                return clientdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Servicio para consultar la resolución de factura para la confitería
        /// </summary>
        /// <returns></returns>
        #region "SCORES"
        public static List<ResponseScores> ConsultResolution(SCORES data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Petición Consultar Resolución Factura", seria, 1);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scores/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta al consultar la resolución", decryptData, 1);
                var est = JsonConvert.DeserializeObject<List<ResponseScores>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Servicio para consultar los puntos del cliente
        /// </summary>
        /// <returns></returns>
        #region "SCOMOV"
        public static double ConsultPoints(SCOMOV data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion para consultar los puntos", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scomov/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta al consultar los puntos", decryptData, 1);

                var est = JsonConvert.DeserializeObject<List<ResponseScomov>>(decryptData);
                return est[0].puntos_acumulados;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// Servicio para realizar la anulación de una venta
        /// </summary>
        /// <returns></returns>
        #region "SCORET"
        public static List<ResponseScoret> CancelSale(SCORET data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion para cancelar la compra", seria, 1);
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scoret/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);
                LogService.SaveRequestResponse("Respuesta al cancelar la compra", decryptData, 1);

                var est = JsonConvert.DeserializeObject<List<ResponseScoret>>(decryptData);

                return est;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        /// <summary>
        /// Servicio para consultar los códigos de tarjeta
        /// </summary>
        /// <returns></returns>
        #region "SCOTDC"
        public static List<SCOTDC> GetTDCCodes(SCOPRE data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion para obtener los codigos de tarjetas", seria, 1);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scotdc/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);

                LogService.SaveRequestResponse("Respuesta al obtener los codigos de tarjetas", decryptData, 1);

                var est = JsonConvert.DeserializeObject<List<SCOTDC>>(decryptData);

                return est;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        /// <summary>
        /// Servicio para registarar un usuario nuevo
        /// </summary>
        /// <returns></returns>
        #region "SCOCYA"
        public static List<ResponseScoret> PersonRegister(SCOCYA data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion para registrar un cliente", seria, 1);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scocya/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);

                LogService.SaveRequestResponse("Respuesta al registrar un cliente", decryptData, 1);

                var est = JsonConvert.DeserializeObject<List<ResponseScoret>>(decryptData);

                return est;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        /// <summary>
        /// Servicio para consultar el saldo a favor de un cliente
        /// </summary>
        /// <returns></returns>
        #region "SCOSDO"
        public static List<ResponseScosdo> GetPersonBalance(SCOSDO data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);
                LogService.SaveRequestResponse("Peticion para conocer el saldo de un cliente", seria, 1);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, DataService41.SCOREKEY);

                var client = new RestClient(DataService41.APISCORE + "/scosdo/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Length", "66");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "scorecoorp.procinal.com");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"\"{encryptData}\"", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                var dataResponse = JsonConvert.DeserializeObject<List<Response41>>(response.Content);

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, DataService41.SCOREKEY);

                LogService.SaveRequestResponse("Respuesta al consultar el saldo de un cliente", decryptData, 1);

                var est = JsonConvert.DeserializeObject<List<ResponseScosdo>>(decryptData);

                return est;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion
    }

    #region ERROR
    public class RESPONSEERROR
    {
        public string Respuesta { get; set; }
    }
    #endregion

    #region SCORET
    public class SCORET
    {
        public int Punto { get; set; }
        public int Pedido { get; set; }
        public string teatro { get; set; }
        public string tercero { get; set; }
    }

    public class ResponseScoret
    {
        public string Respuesta { get; set; }

        [JsonProperty("Validación")]
        public string Validacion { get; set; }
    }
    #endregion

    #region SCOMOV
    public class SCOMOV
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
        public int tercero { get; set; }
    }

    public class ResponseScomov
    {
        public double puntos_acumulados { get; set; }
        public double puntos_redimidos { get; set; }
        public double puntos_vencidos { get; set; }
        public double puntos_disponibles { get; set; }
    }
    #endregion

    #region SCORES
    public class SCORES
    {
        public int Punto { get; set; }
        public int Secuencial { get; set; }
        public int teatro { get; set; }
        public int tercero { get; set; }
    }

    public class ResponseScores
    {
        public int Factura { get; set; }
        public string Prefijo { get; set; }

        [JsonProperty("Resolución", NullValueHandling = NullValueHandling.Ignore)]
        public double Resolucion { get; set; }
        public double Inicio { get; set; }
        public double Fin { get; set; }
        public DateTime Vencimiento { get; set; }
    }
    #endregion

    #region SCOSIL
    public class SCOSIL
    {
        public string FechaFuncion { get; set; }
        public int Sala { get; set; }
        public int Funcion { get; set; }
        public string Fila { get; set; }
        public int Columna { get; set; }
        public int Usuario { get; set; }
        public int teatro { get; set; }
        public string tercero { get; set; }
    }
    #endregion

    #region SCOINT
    public class UbicacioneSCOINT
    {
        public string Fila { get; set; }
        public int Columna { get; set; }
        public string FilRelativa { get; set; }
        public int ColRelativa { get; set; }
        public int Tarifa { get; set; }
    }

    public class SCOINT
    {
        public int PuntoVenta { get; set; }
        public int Factura { get; set; }
        public string CorreoCliente { get; set; }
        public long DocIdentidad { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Telefono { get; set; }
        public string Direccion { get; set; }
        public int Sala { get; set; }
        public string FechaFun { get; set; }
        public int Funcion { get; set; }
        public int InicioFun { get; set; }
        public int Pelicula { get; set; }
        public List<UbicacioneSCOINT> Ubicaciones { get; set; }
        public List<Producto> Productos { get; set; }
        public string Placa { get; set; }
        public int AudiPrev { get { return 0; } }
        public string TipoEntrega { get { return "T"; } }
        public string Cortesia { get; set; }
        public int TipoBono { get; set; }
        public long ClienteFrecuente { get; set; }
        public int TotalVenta { get; set; }
        public int PagoInterno { get; set; }
        public int PagoCredito { get; set; }
        public int PagoEfectivo { get; set; }
        public int CodMedioPago { get; set; }
        public string Accion { get; set; }
        public int teatro { get; set; }
        public int tercero { get; set; }
        public string Obs1 { get; set; }
        public string Obs2 { get { return ""; } }
        public string Obs3 { get { return ""; } }
        public string Obs4 { get { return ""; } }
    }

    public class ResponseScoint
    {
        public string Respuesta { get; set; }
    }
    #endregion

    #region SCOGRU
    public class Ubicacione
    {
        public string Fila { get; set; }
        public int Columna { get; set; }
        public int Tarifa { get; set; }
    }

    public class SCOGRU
    {
        public string FechaFuncion { get; set; }
        public int Sala { get; set; }
        public int HoraFuncion { get; set; }
        public int Pelicula { get; set; }
        public string Descripcion { get; set; }
        public int InicioFuncion { get; set; }
        public int PuntoVenta { get; set; }
        public int Secuencia { get; set; }
        public long Telefono { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public List<Ubicacione> Ubicaciones { get; set; }
        public int teatro { get; set; }
        public int tercero { get; set; }
    }

    public class ResponseScogru
    {
        public string Respuesta { get; set; }
    }
    #endregion

    #region SCOSEC
    public class SCOSEC
    {
        public int Punto { get; set; }
        public int teatro { get; set; }
        public string tercero { get; set; }
    }
    public class ResponseSec
    {
        public double Secuencia { get; set; }
        public double Recargo_Venta_Internet { get; set; }
    }
    #endregion

    #region SCOPLA
    public class ResponseTarifa
    {
        public double codigo { get; set; }
        public string descripcion { get; set; }
        public double valor { get; set; }
        public string zona { get; set; }
        public string sala { get; set; }
        public string silla { get; set; }
        public double medioPago { get; set; }
        public string documento { get; set; }
        public string ClienteFrecuente { get; set; }
    }
    #endregion

    #region SCOEST
    public class SCOEST
    {
        public int Sala { get; set; }
        public string FechaFuncion { get; set; }
        public int Funcion { get; set; }
        public string Correo { get; set; }
        public int teatro { get; set; }
        public string tercero { get; set; }
    }

    public class DescripcionSilla
    {
        public string TipoSilla { get; set; }
        public string EstadoSilla { get; set; }
        public double Columna { get; set; }
    }

    public class EstadoSala41
    {
        public double maxCol { get; set; }
        public string maxFil { get; set; }
        public string filRel { get; set; }
        public List<DescripcionSilla> DescripcionSilla { get; set; }
    }
    #endregion

    #region SCOMAP
    public class SCOMAP
    {
        public int Sala { get; set; }
        public int teatro { get; set; }
        public string tercero { get; set; }

    }
    public class MapaSala41
    {
        public List<string> FilaTotal { get; set; }
        public List<string> FilaRelativa { get; set; }
        public List<double> ColumnaTotal { get; set; }
        public List<double> ColumnaRelativa { get; set; }
        public List<string> TipoSilla { get; set; }
        public List<string> TipoZona { get; set; }
    }
    #endregion

    #region SCOPLA
    public class SCOPLA
    {
        public string FechaFuncion { get; set; }
        public int Pelicula { get; set; }
        public int Sala { get; set; }
        public int InicioFuncion { get; set; }
        public int teatro { get; set; }
        public string tercero { get; set; }
    }
    #endregion

    #region SCOPRE
    public class SCOPRE
    {
        public string teatro { get; set; }
        public string tercero { get; set; }
    }

    public class Confiteria
    {
        [JsonProperty("Lista_Productos")]
        public List<Producto> ListaProductos { get; set; }
    }

    public class Producto : INotifyPropertyChanged
    {
        public long Codigo { get; set; }
        public string Imagen { get; set; }
        public string Descripcion { get; set; }

        public string Tipo { get; set; }

        public int Precio { get; set; }
        private int value { get; set; }
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                OnPropertyRaised("Value");
            }
        }
        public List<Precio> Precios { get; set; }

        public List<Receta> Receta { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

    }

    public class MasProductos
    {
        public long Codigo { get; set; }

        public string Descripcion { get; set; }


        public int Precio { get; set; }

    }

    public class Precio
    {
        public string General { get; set; }

        public string OtroPago { get; set; }

        public decimal auxGeneral { get; set; }

        public decimal auxOtroPago { get; set; }

        public string PagoInterno { get; set; }

        public string PrecioAtencion { get; set; }

        public string HorasDias { get; set; }
    }

    public class Receta
    {
        public long Codigo { get; set; }

        public string Descripcion { get; set; }

        public string Tipo { get; set; }

        public long Cantidad { get; set; }


        public List<Precio> Precios { get; set; }
        [JsonProperty("Receta", NullValueHandling = NullValueHandling.Ignore)]
        public List<Receta> RecetaReceta { get; set; }
    }


    #endregion

    #region SCOCSN
    public class SCOCED
    {
        public string Documento { get; set; }
        public string tercero { get; set; }
    }
    public class SCOCSNResponse
    {
        public string Valor { get; set; }
    }
    #endregion

    #region SCOLOG
    public class SCOLOG
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string teatro { get; set; }
        public string tercero { get; set; }
    }

    public class SCOLOGResponse
    {
        public string Valor { get; set; }
        [JsonProperty(PropertyName = "No. Tarjeta")]
        public string Tarjeta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Estado { get; set; }
        public string Login { get; set; }
        public string Sexo { get; set; }
        public int Edad { get; set; }
        public string Cinema { get; set; }
        public string Genero { get; set; }
        public string Documento { get; set; }
        public string Reservas { get; set; }
        public string Noticias { get; set; }
        public string Cartelera { get; set; }
        public string Direccion { get; set; }
        public string Celular { get; set; }
        public string Otras_Salas { get; set; }
        public string Barrio { get; set; }
        public string Municipio { get; set; }
        public string Clave { get; set; }
        public string Telefono { get; set; }
        public string Fecha_Nacimiento { get; set; }
        public double Puntos { get; set; }
        public Nullable<decimal> SaldoFavor { get; set; }

    }
    #endregion

    #region SCOTDC
    public class SCOTDC
    {
        public long Codigo { get; set; }

        [JsonProperty("Descripción")]
        public string Descripcion { get; set; }
        public long TipoPago { get; set; }
    }
    #endregion

    #region "SCOCYA"
    public class SCOCYA
    {
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Sexo { get; set; }
        public int Edad { get; set; }
        public string Documento { get; set; }
        public string Celular { get; set; }
        public string Clave { get; set; }
        public string Telefono { get { return Celular; } }
        public string Fecha_Nacimiento { get; set; }
        public string Direccion { get; set; }
        public string Correo { get { return Login; } }
        public string Genero { get; } = "accion";
        public string Cinema { get; } = Utilities.GetConfiguration("CodCinema");
        public string Reservas { get; } = "N";
        public string Noticias { get; } = "N";
        public string Cartelera { get; } = "N";
        public string Otras_Salas { get; } = "N";
        public string Barrio { get; } = "N";
        public string Municipio { get; } = "medellin";
        public string Accion { get; } = "C";
        public string tercero { get; } = "1";
    }
    #endregion

    #region SCOSDO
    public class SCOSDO
    {
        public string Correo { get; set; }
        public string tercero { get; set; }
    }

    public class ResponseScosdo
    {
        [JsonProperty("Validación")]
        public string Validacion { get; set; }
        public Nullable<decimal> Saldo { get; set; }
    }
    #endregion
    public class Combos
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public long Code { get; set; }
        public Producto dataProduct { get; set; }
        public bool isCombo { get; set; }
        public string Visible { get; set; }
    }

    public class Request41
    {
        public string details { get; set; }
    }

    public class Response41
    {
        public string request { get; set; }
    }

    public static class DataService41
    {
        /// <summary>
        /// Lista para recibir la informacion de la resolución de factura de score
        /// </summary>
        public static List<ResponseScores> _DataResolution = new List<ResponseScores>();
        /// <summary>
        /// Lista global para registrar los productos que se seleccionen
        /// en la pantalla de confiteria
        /// </summary>
        public static List<Combos> _Combos = new List<Combos>();

        /// <summary>
        /// Lista global para almacenar los productos que devuelva  el servicio SCOPRE
        /// </summary>
        public static List<Producto> _Productos;

        /// <summary>
        /// Lista para almacenar las peliculas que devuelva el xml
        /// </summary>
        public static List<Pelicula> Movies;

        /// <summary>
        /// Objeto que almacena la informacion del usuario cinefan
        /// </summary>
        public static SCOLOGResponse dataUser;

        /// <summary>
        /// URL de la api de score
        /// </summary>
        public static string APISCORE = Utilities.GetConfiguration("ScoreService");

        /// <summary>
        /// Llave de desencripción de las respuestas de score
        /// </summary>
        public static string SCOREKEY = Utilities.GetConfiguration("ScoreKey");

        /// <summary>
        /// URL de los posters de las películas
        /// </summary>
        public static string UrlImages = Utilities.GetConfiguration("UrlImages");

        /// <summary>
        /// Numero de secuencia de cada compra en score, se utiliza para la notificación del pago
        /// </summary>
        public static string Secuencia { get; set; }

        /// <summary>
        /// Objeto para almacenar la data de la película seleccionada por el usuario
        /// </summary>
        public static Pelicula Movie;

        /// <summary>
        /// Objeto para deserializar el xml y tenerlo en memoria
        /// </summary>
        public static Peliculas Peliculas;
    }

}
