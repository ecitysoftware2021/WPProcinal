namespace WPProcinal.Classes
{
    public class TypeSeat
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
    }


    public class SeatTmp
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string State { get; set; }
    }
}
