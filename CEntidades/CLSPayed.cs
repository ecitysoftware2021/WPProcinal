using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEntidades
{
    public class CLSPayed
    {
        public string Referece { get; set; }


        public string RIdentificacion { get; set; }

        public string RProducto { get; set; }

        public string RContrato { get; set; }

        public DateTime RFec_Pago { get; set; }

        public double RMonto_Recibido { get; set; }

        public int CLSPayedId { get; set; }

        

        public string FechaFin { get; set; }        
    }
}
