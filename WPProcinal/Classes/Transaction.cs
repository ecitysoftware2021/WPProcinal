using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public DateTime DATE_BEGIN { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal INCOME_AMOUNT { get; set; }
        public decimal RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYER_ID { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
        public List<TRANSACTION_DESCRIPTION> TRANSACTION_DESCRIPTION = new List<TRANSACTION_DESCRIPTION>();
    }

    public class TRANSACTION_DESCRIPTION
    {
        public int TRANSACTION_ID { get; set; }
        public int TRANSACTION_DESCRIPTION_ID { get; set; }
        public string REFERENCE { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string OBSERVATION { get; set; }
        public Nullable<bool> STATE { get; set; }
    }
}
