using System;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Service
{
    public class WCFServices
    {
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
    }
}
