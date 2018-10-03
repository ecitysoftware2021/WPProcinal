using PayPad.Billetero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.WCFPayPad;

namespace WPProcinal.Classes
{
    public class HelperEmails
    {
        private static ServicePayPadClient WCFPayPadWS = new ServicePayPadClient();

        public static void SendEmail(string mensaje)
        {
            try
            {
                WCFPayPadWS.EnviarCorreo(mensaje, ControlPantalla.Sucursal, ControlPantalla.IdCorrespo, "Email 101", ControlPantalla.MailTo);
            }
            catch { }
        }        

        public static void SendEmail(Utilities.ETipoAlerta tipoAlerta, Ingresar objIngresar, int cantBilletes)
        {
            if (tipoAlerta == Utilities.ETipoAlerta.BilleteroVacio)
            {
                string billetes = objIngresar.ContarBilletes();
                bool envioEmail = false;
                if (!string.IsNullOrEmpty(billetes))
                {
                    envioEmail = WCFPayPadWS.EnviarCorreo(billetes, ControlPantalla.Sucursal, ControlPantalla.IdCorrespo, "Email 101", ControlPantalla.MailTo);
                }
            }
            else
            {
                string mensaje = string.Concat("Pronto se llenará el baúl del billetero de la sucursal ",
                    ControlPantalla.Sucursal, ", actualmente tiene ", cantBilletes, " billetes, por favor realizar arqueo.");
                bool envioEmail = false;
                envioEmail = WCFPayPadWS.EnviarCorreo(mensaje, ControlPantalla.Sucursal, ControlPantalla.IdCorrespo, "Email 101", ControlPantalla.MailTo);
            }
        }
    }
}
