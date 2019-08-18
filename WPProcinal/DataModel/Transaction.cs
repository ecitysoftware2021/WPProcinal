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
        public int PAYMENT_TYPE_ID { get; set; }
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

    public class RequestCardInformation
    {
        public string Last_number { get; set; }

        public int Quotas { get; set; }

        public string Franchise { get; set; }

        public int Card_Type { get; set; }

        public int Transaction_id { get; set; }
        public string Autorization_code { get; set; }
        public string Receipt_number { get; set; }
        public string RRN { get; set; }
    }
}
