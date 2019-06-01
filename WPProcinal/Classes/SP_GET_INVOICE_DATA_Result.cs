using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public class SP_GET_INVOICE_DATA_Result
    {
        public string PREFIJO { get; set; }
        public string RESOLUCION { get; set; }
        public DateTime FECHA_RESOLUCION { get; set; }
        public double RANGO_DESDE { get; set; }
        public double RANGO_HASTA { get; set; }
        public double RANGO_ACTUAL { get; set; }
        public Nullable<bool> IS_AVAILABLE { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
    }
}
