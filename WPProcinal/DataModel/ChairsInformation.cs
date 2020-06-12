using System.Windows.Media;

namespace WPProcinal.Classes
{
    public class ChairsInformation
    {
        public int RelativeColumn { get; set; }
        public string RelativeRow { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        public string Letter { get; set; }

        public string Number { get; set; }

        public string CodTarifa { get; set; }

        public decimal Price { get; set; }

        public bool IsReserved { get; set; }

        public string NumSecuencia { get; set; }

        public int TransactionId { get; set; }
        public ImageSource imageSource { get; set; }
        public bool Available { get; set; }
        public int Row { get; set; }
        public int maxCol { get; set; }
        public int Quantity { get; set; }

    }


}
