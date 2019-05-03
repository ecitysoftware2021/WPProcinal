using PayPad.Billetero;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.WCFPayPad;

namespace WPProcinal.Classes
{
    class ControlPantalla
    {
        #region Properties
        public static string MailTo { get; set; }

        public static string MaxBaul { get; set; }

        public static int IdCorrespo = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));

        public static List<ClSMinDenomination> ClSMinDenominations { get; set; }

        public static string Sucursal = Utilities.GetConfiguration("Corresponsal");

        public static bool EstadoBaul = true;

        public static bool EstadoMonedas = true;

        public static bool EstadoBilletes = true;
        #endregion

        #region Attributes

        private static ServicePayPadClient WCFPayPadWS = new ServicePayPadClient();        
        static Ingresar objIngresar = new Ingresar(); 

        #endregion



        public static void ConsultarControlPantalla()
        {
            var datos = WCFPayPadWS.GetDataControlScreen(IdCorrespo);
            ClSMinDenominations = datos.ClSMinDenominations.ToList();
            MaxBaul = datos.MaxBaul.ToString();
            MailTo = datos.Email;
            ValidarBaul();
            ValidarBilletes();
            ValidarMonedas();
        }

        public static void ValidarBaul()
        {
            string mensajeBaul = string.Empty;
            int cantidadBaul = WCFPayPadWS.BilletesBaul(IdCorrespo);
            if (cantidadBaul >= int.Parse(MaxBaul))
            {
                EstadoBaul = false;
                mensajeBaul = string.Format("Pronto se llenará el Baul de la sucursal {0}, por favor realizar arqueo.", Sucursal);
                HelperEmails.SendEmail(mensajeBaul);
                mensajeBaul = string.Empty;
            }
        }

        public static void ValidarMonedas()
        {
            string mensajeMonedas = "Faltan monedas de {0} en la sucursal {1}";
            var message = new StringBuilder();
            DataSet ds = WCFPayPadWS.InsertarControlMonedas(1, IdCorrespo, 1, 0);
            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    var moneda = ClSMinDenominations
                                .Where(c => c.Description == row["Denominacion"].ToString())
                                .FirstOrDefault();
                    if (moneda != null)
                    {
                        if (int.Parse(row["Cantidad"].ToString()) < moneda.Quantity)
                        {
                            EstadoMonedas = false;
                            message.Append($"{string.Format("{0:C}", decimal.Parse(row["Denominacion"].ToString()))} actualmente hay {row["Cantidad"].ToString()} monedas, ");                            
                        }
                    }
                }
            }
            
            string messageEmail = string.Format(mensajeMonedas, message.ToString(), Sucursal);
            if (!EstadoMonedas)
            {
                HelperEmails.SendEmail(messageEmail);
            }
        }

        public static void ValidarBilletes()
        {
            string mensajeBilletes = "Faltan billetes de {0} en la sucursal {1}";
            var message = new StringBuilder();
            string messageEmail = string.Empty;
            List<Billetes> billetes = objIngresar.ValidarCantidadXBilletes();
            if (billetes.Count > 0)
            {
                foreach (var item in billetes)
                {
                    var t = Task.Run(() =>
                    {
                    //    int denomination = Utilities.GetDescriptionEnum(item.Denominacion);
                    //    WCFPayPadWS.InsertarControlDispenser(denomination, IdCorrespo, 2, int.Parse(item.Cantidad));
                    });

                    //if (item.Denominacion != "1000" && item.Denominacion != "50000")
                    //{
                    //    var billete = ClSMinDenominations
                    //                    .Where(c => c.Description == item.Denominacion)
                    //                    .FirstOrDefault();
                    //    if (billete != null)
                    //    {
                    //        if (int.Parse(item.Cantidad) < billete.Quantity)
                    //        {
                    //            EstadoBilletes = false;
                    //            message.Append($"{string.Format("{0:C}", decimal.Parse(item.Denominacion))} actualmente hay {item.Cantidad} billetes, ");
                    //        }
                    //    }
                    //}
                }
                
                messageEmail = string.Format(mensajeBilletes, message.ToString(), Sucursal);
            }
            else
            {
                EstadoBilletes = false;
                messageEmail = "Se produjo un error al leer la cantidad de billetes";
            }

            if (!EstadoBilletes)
            {
                HelperEmails.SendEmail(messageEmail);
            }
        }
    }
}
