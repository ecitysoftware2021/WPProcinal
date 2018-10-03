using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEntidades
{
    public class Receipt
    {
        public string Header { get; set; }

        public string Body { get; set; }

        public string Footer { get; set; }

        public int ReceiptPaymentId { get; set; }

        public int ClientId { get; set; }
    }
}
