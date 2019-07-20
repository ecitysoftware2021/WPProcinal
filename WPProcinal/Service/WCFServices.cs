using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPProcinal.ADO;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.WCFPcrmap;

namespace WPProcinal.Service
{
    public class WCFServices
    {
        #region WebService Procinal
        public static T DeserealizeXML<T>(string outerXml)
        {
            var typeParameterType = typeof(T);
            XmlSerializer serializer = new XmlSerializer(typeParameterType);
            var reader = new StringReader(outerXml);
            var mapaSala = (T)serializer.Deserialize(reader);

            return mapaSala;
        }

        #region 6. PCRGRU1
        public static Response PostReserva(DipMap dipMap, TypeSeat typeSeat)
        {
            try
            {
                WCFReserva.ServiceSoapClient serviceSoapClient = new WCFReserva.ServiceSoapClient();
                var data = serviceSoapClient.resgru(tea_ext_l: dipMap.CinemaId,
                    pun_ext_l: double.Parse(ConfigurationManager.AppSettings["Cinema"]),
                    sal_ext_l: dipMap.RoomId,
                    fex_ext_l: dipMap.Date,
                    fun_ext_l: dipMap.Hour,
                    fil_ext_l: typeSeat.Letter,
                    col_ext_l: int.Parse(typeSeat.Number),
                    gru_ext_l: dipMap.Group,
                    sec_ext_l: dipMap.Secuence,
                    pel_ext_l: dipMap.MovieId,
                    tip_ext_l: dipMap.IsCard,
                    cor_ext_l: dipMap.Login,
                    tr1_ext_l: double.Parse(typeSeat.CodTarifa.ToString()));
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 8. PCRSIL1
        public static Response PostDesAssingreserva(DipMap dipMap, TypeSeat typeSeat)
        {
            try
            {
                WCFDesAssing.ServiceSoapClient serviceSoapClient = new WCFDesAssing.ServiceSoapClient();
                var data = serviceSoapClient.dessil(tea_ext_l: dipMap.CinemaId,
                    sal_ext_l: dipMap.RoomId,
                    fex_ext_l: dipMap.Date,
                    fun_ext_l: dipMap.Hour,
                    fil_ext_l: typeSeat.Letter,
                    col_ext_l: int.Parse(typeSeat.Number),
                    pel_ext_l: dipMap.MovieId,
                    log_ext_l: dipMap.Login,
                    gru_ext_l: dipMap.Group);
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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


        #endregion

        #region 5. PCRSEC
        public static Response GetSecuence(DipMap dipMap)
        {
            try
            {
                WCFSecuencia.ServiceSoapClient serviceSoapClient = new WCFSecuencia.ServiceSoapClient();
                var data = serviceSoapClient.secven(dipMap.CinemaId, dipMap.PointOfSale);
                LogService.CreateLogsSecuencia(data);
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 1. PCRMAP
        public static Response GetDipMap(DipMap dipMap)
        {
            try
            {
                WCFPcrmap.ServiceSoapClient serviceSoapClient = new WCFPcrmap.ServiceSoapClient();
                var data = serviceSoapClient.dibmap(dipMap.CinemaId, dipMap.RoomId, dipMap.Date, dipMap.Hour);
                //try
                //{
                //    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: GetDipMap: ", JsonConvert.SerializeObject(data));
                //}
                //catch { }
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 2. PCRFIL1
        public static Response GetStateFill(DipMap dipMap)
        {
            try
            {
                WCFPcrfil2.ServiceSoapClient serviceSoapClient = new WCFPcrfil2.ServiceSoapClient();
                var data = serviceSoapClient.estfil(dipMap.CinemaId,
                    dipMap.RoomId,
                    dipMap.Date,
                    dipMap.Hour,
                    dipMap.Letter,
                    dipMap.Login);
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 4. PCRPLA
        public static Response GetPrices(DipMap dipMap)
        {
            try
            {
                WCFTarifas.tarifasSoapClient serviceSoapClient = new WCFTarifas.tarifasSoapClient();
                var data = serviceSoapClient.commit(
                                        double.Parse(dipMap.CinemaId.ToString()),
                                        dipMap.Date,
                                        double.Parse(dipMap.MovieId.ToString()),
                                        double.Parse(dipMap.RoomId.ToString()),
                                        double.Parse(dipMap.HourFormat.ToString()));
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 3.PCREST
        public static Response GetStateRoom(DipMap dipMap)
        {
            try
            {
                WCFPcrest.ServiceSoapClient serviceSoapClient = new WCFPcrest.ServiceSoapClient();
                var data = serviceSoapClient.estsal(
                    dipMap.CinemaId,
                    dipMap.RoomId,
                    dipMap.Date,
                    dipMap.Hour,
                    dipMap.Letter,
                    dipMap.Login);
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        #region 7. PCRINT

        public static Response PostComprar(TblDipMap dipMap, double total, string paymentMethod, int seatsCount)
        {
            try
            {
                WCFPcrint.ServiceSoapClient serviceSoapClient = new WCFPcrint.ServiceSoapClient();
                var data = serviceSoapClient.comint(
                    teatro: dipMap.CinemaId.Value,
                    secuencia: dipMap.Secuence.Value,
                    can_entradas: seatsCount,
                    tarifa: 0,
                    tiene_tarjeta: dipMap.IsCard,
                    correo: dipMap.Login,
                    autorizacion_visa: "0",
                    referencia: 0,
                    sec_papaya: 0,
                    monedapago: "COP",
                    fecha_funcion: dipMap.Date,
                    total_compra: total,
                    med_pago: paymentMethod,
                    pun_ext_l: double.Parse(ConfigurationManager.AppSettings["Cinema"])
                    );

                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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

        public static Response PostComprar(DipMap dipMap, List<TypeSeat> typeSeats)
        {
            try
            {
                //try
                //{
                //    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: Peticion PostComprar: ", JsonConvert.SerializeObject(dipMap));
                //}
                //catch { }
                WCFPcrint.ServiceSoapClient serviceSoapClient = new WCFPcrint.ServiceSoapClient();
                var data = serviceSoapClient.comint(
                    teatro: dipMap.CinemaId,
                    secuencia: dipMap.Secuence,
                    can_entradas: typeSeats.Count(),
                    tarifa: 0,
                    tiene_tarjeta: dipMap.IsCard,
                    correo: dipMap.Login,
                    autorizacion_visa: "0",
                    referencia: 0,
                    sec_papaya: 0,
                    monedapago: "COP",
                    fecha_funcion: dipMap.Date,
                    total_compra: double.Parse(Utilities.ValorPagarScore.ToString()),
                    med_pago: dipMap.PaymentMethod,
                    pun_ext_l: double.Parse(ConfigurationManager.AppSettings["Cinema"])
                    );
                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: PostComprar: ", JsonConvert.SerializeObject(data));
                }
                catch { }
                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
            }
            catch (Exception ex)
            {
                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: PostComprar Excepcion: ", ex.Message);
                }
                catch { }
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        #endregion

        #region 9. PCRRET
        public static Response PostReembolso(DipMap dipMap)
        {
            try
            {
                WCFReembolso.reembolsoSoapClient reembolsoSoapClient = new WCFReembolso.reembolsoSoapClient();
                var data = reembolsoSoapClient.comint(
                    dipMap.CinemaId,
                    dipMap.PointOfSale,
                    DateTime.Now.ToString("yyyyMMdd"),
                    dipMap.Secuence);

                return new Response
                {
                    IsSuccess = true,
                    Result = data.OuterXml,
                };
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
        #endregion

        public static Response DownloadData()
        {
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
        #endregion

        public static async Task<Response> InsertDBProcinal(TblDipMap dipMap, List<TblTypeSeat> seats)
        {
            try
            {
                WCFPayPad.ServicePayPadClient servicePayPadClient = new WCFPayPad.ServicePayPadClient();

                var modelDipMap = new WCFPayPad.TblDipMap
                {
                    Category = dipMap.Category,
                    CinemaId = dipMap.CinemaId,
                    Date = dipMap.Date,
                    DateFormat = dipMap.DateFormat,
                    Day = dipMap.Day,
                    Duration = dipMap.Duration,
                    Gener = dipMap.Gener,
                    Group = dipMap.Group,
                    Hour = dipMap.Hour,
                    HourFunction = dipMap.HourFunction,
                    HourMillitar = dipMap.HourFormat,
                    IsCard = dipMap.IsCard,
                    Language = dipMap.Language,
                    Letter = dipMap.Letter,
                    Login = dipMap.Login,
                    MovieName = dipMap.MovieName,
                    PointOfSale = dipMap.PointOfSale,
                    RoomId = dipMap.RoomId,
                    RoomName = dipMap.RoomName,
                    Secuence = dipMap.Secuence,
                };

                List<WCFPayPad.TblSeat> tblSeats = new List<WCFPayPad.TblSeat>();
                foreach (var item in seats)
                {
                    tblSeats.Add(new WCFPayPad.TblSeat
                    {
                        CodTarifa = item.CodTarifa,
                        IsReserved = item.IsReserved,
                        Letter = item.Letter,
                        Name = item.Name,
                        Number = item.Number,
                        Price = item.Price,
                        Secuence = item.NumSecuencia,
                        Type = item.Type,
                        IsPay = item.IsPay,
                    });
                }

                WCFPayPad.InfoProcinal infoProcinal = new WCFPayPad.InfoProcinal
                {
                    Seats = tblSeats.ToArray(),
                    DipMap = modelDipMap
                };


                var response = await servicePayPadClient.InsertDataProcinalAsync(infoProcinal);
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result
                };
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

        public static Response GetReceiptProcinal(int correspondentId)
        {
            try
            {
                WCFPayPad.ServicePayPadClient servicePayPadClient = new WCFPayPad.ServicePayPadClient();
                var result = servicePayPadClient.GetDataReceiptProcinal(correspondentId);
                var response = JsonConvert.DeserializeObject<Response>(result);
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result
                };
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
