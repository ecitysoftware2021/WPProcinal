using EncryptAndDecryptDLL;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
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
        public WCFServices41()
        {
        }
        private static void ServerCertificateValidationCallback()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        }

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
                try
                {
                    AdminPaypad.SaveErrorControl(seria,
                    "GetStateRoom Request",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                var client = new RestClient(Utilities.APISCORE + "/scoest/");
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

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);

                var est = JsonConvert.DeserializeObject<List<EstadoSala41>>(decryptData);
                if (est.Count < 2)
                {
                    try
                    {
                        AdminPaypad.SaveErrorControl(decryptData,
                        "GetStateRoom Response",
                        EError.Customer,
                        ELevelError.Mild);
                    }
                    catch { }
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "GetStateRoom catch",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
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
                try
                {
                    AdminPaypad.SaveErrorControl(seria,
                    "GetPrices Request",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                var client = new RestClient(Utilities.APISCORE + "/scopla/");
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

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);
                try
                {
                    AdminPaypad.SaveErrorControl(decryptData,
                    "GetPrices Response",
                    EError.Customer,
                    ELevelError.Mild);
                }
                catch { }
                var est = JsonConvert.DeserializeObject<List<ResponseTarifa>>(decryptData);
                if (est[0].sala == null)
                {
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "GetPrices catch",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
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
                try
                {
                    AdminPaypad.SaveErrorControl(seria,
                    "GetSecuence Request",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                var client = new RestClient(Utilities.APISCORE + "/scosec/");
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

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);
                var est = JsonConvert.DeserializeObject<List<ResponseSec>>(decryptData);
                try
                {
                    AdminPaypad.SaveErrorControl(decryptData,
                    "GetSecuence Response",
                    EError.Customer,
                    ELevelError.Mild);
                }
                catch { }
                if (est[0].Secuencia == 0)
                {
                    return null;
                }
                return est;

            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "GetSecuence catch",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
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

                try
                {
                    AdminPaypad.SaveErrorControl(seria,
                    "PostPreventa Request",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                var client = new RestClient(Utilities.APISCORE + "/scogru/");
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

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);

                try
                {
                    AdminPaypad.SaveErrorControl(decryptData,
                    "PostPreventa Response",
                    EError.Customer,
                    ELevelError.Mild);
                }
                catch { }
                var est = JsonConvert.DeserializeObject<List<ResponseScogru>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "PostPreventa catch",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
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
        public static List<EsponseScoint> PostBuy(SCOINT data)
        {

            string decryptData = string.Empty;
            try
            {
                //Data convertida a formato json
                var seria = JsonConvert.SerializeObject(data);

                //Data encriptada con la llave de score
                var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                var client = new RestClient(Utilities.APISCORE + "/scoint/");
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

                decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);

                try
                {
                    AdminPaypad.SaveErrorControl(decryptData,
                    "PostBuy Response",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                var est = JsonConvert.DeserializeObject<List<EsponseScoint>>(decryptData);
                return est;

            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "PostBuy catch",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                return null;
            }
        }
        public static bool StateImage(string image)
        {
            try
            {
                var client = new RestClient(image);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "www.pantallasprocinal.com");
                request.AddHeader("Postman-Token", "8c312204-ba23-4857-9742-751c11ab942c,2b81a37c-5ec3-429b-8a81-b8dafc41ef38");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
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
        public static void PostDesAssingreserva(List<TypeSeat> typeSeatsCurrent, DipMap dipMapCurrent)
        {
            string decryptData = string.Empty;
            try
            {
                foreach (var item in typeSeatsCurrent)
                {
                    SCOSIL data = new SCOSIL
                    {
                        Columna = item.RelativeColumn,
                        FechaFuncion = dipMapCurrent.Date,
                        Fila = item.RelativeRow,
                        Funcion = dipMapCurrent.IDFuncion,
                        Sala = dipMapCurrent.RoomId,
                        teatro = dipMapCurrent.CinemaId,
                        tercero = "1",
                        Usuario = 777
                    };

                    //Data convertida a formato json
                    var seria = JsonConvert.SerializeObject(data);

                    //Data encriptada con la llave de score
                    var encryptData = dataEncrypt.Encrypt(seria, Utilities.SCOREKEY);


                    var client = new RestClient(Utilities.APISCORE + "/scosil/");
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

                    decryptData = dataEncrypt.Decrypt(dataResponse[0].request, Utilities.SCOREKEY);
                    var est = JsonConvert.DeserializeObject<List<EsponseScoint>>(decryptData);

                }

            }
            catch (Exception ex)
            {
                var est = JsonConvert.DeserializeObject<List<RESPONSEERROR>>(decryptData);
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

    public class Receta2
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
    }

    public class Receta
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public List<Receta2> _Receta { get; set; }
    }

    public class Producto
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int Precio { get; set; }
        public List<Receta> Receta { get; set; }
    }

    public class SCOINT
    {
        public int PuntoVenta { get; set; }
        public int Factura { get; set; }
        public string CorreoCliente { get; set; }
        public int DocIdentidad { get; set; }
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
        public string Cortesia { get; set; }
        public int TipoBono { get; set; }
        public int ClienteFrecuente { get; set; }
        public double TotalVenta { get; set; }
        public int PagoInterno { get; set; }
        public double PagoCredito { get; set; }
        public int PagoEfectivo { get; set; }
        public string Accion { get; set; }
        public int teatro { get; set; }
        public int tercero { get; set; }
    }

    public class EsponseScoint
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
        public int Telefono { get; set; }
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

    public class Request41
    {
        public string details { get; set; }
    }
    public class Response41
    {

        public string request { get; set; }

    }


}
