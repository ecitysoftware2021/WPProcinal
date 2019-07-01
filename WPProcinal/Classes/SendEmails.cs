using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.WCFPayPad;

namespace WPProcinal.Classes
{
    public class SendEmails
    {
        private static ServicePayPadClient WCFPayPadWS = new ServicePayPadClient();

        public static void SendEmail(string mensaje)
        {
            if (Utilities.GetConfiguration("EnviarCorreo").Equals("si"))
            {
                try
                {
                    int IdCorresponsal = Convert.ToInt32(Utilities.GetConfiguration("IDCorresponsal"));
                    string Sucursal = Utilities.GetConfiguration("Sucursal");
                    string Email = Utilities.GetConfiguration("Email");

                    if (!string.IsNullOrEmpty(Email))
                    {
                        var emails = Email.Split(';');

                        foreach (var item in emails)
                        {
                            WCFPayPadWS.EnviarCorreo(mensaje, Sucursal, IdCorresponsal, "Email 101", item);
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}