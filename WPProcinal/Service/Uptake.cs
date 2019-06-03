using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Service
{
    
        public class RequestTransactionDetails
        {
            public int TransactionId { get; set; }

            public int Denomination { get; set; }

            public int Operation { get; set; }

            public int Quantity { get; set; }

            public string Code { get; set; }

            public string Description { get; set; }
        }
    
}
