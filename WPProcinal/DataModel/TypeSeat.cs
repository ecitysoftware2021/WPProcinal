using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public class TypeSeat
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Letter { get; set; }

        public string Number { get; set; }

        public string CodTarifa { get; set; }

        public decimal Price { get; set; }

        public bool IsReserved { get; set; }

        public string NumSecuencia { get; set; }

        public int TransactionId { get; set; }
    }

    public class SeatTmp
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string State { get; set; }
    }
}
