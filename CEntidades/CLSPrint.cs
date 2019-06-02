using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;

namespace CEntidades
{
    public class CLSPrint
    {
        SolidBrush sb = new SolidBrush(Color.Black);
        Font fBody = new Font("Arial", 8, FontStyle.Bold);
        Font fBody1 = new Font("Arial", 8, FontStyle.Regular);
        Font rs = new Font("Stencil", 25, FontStyle.Bold);
        Font fTType = new Font("", 150, FontStyle.Bold);

        public string NumeroRecibo { get; set; }
        public string Movie { get; set; }
        public string Referencia { get; set; }
        public string Tramite { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
        public string Seat { get; set; }
        public string Cinema { get; set; }
        public string Date { get; set; }
        public string Category { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public double ValorPagado { get; set; }
        public double Sobrante { get; set; }
        public int SPACE { get { return 120; } }

        public string Destino { get; set; }

        public string Estado { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorDevuelto { get; set; }

        public Receipt Recibo { get; set; }
        public string Consecutivo { get; set; }

        public void ImprimirComprobante()
        {
            try
            {
                Estado = "Aprobada";
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                if (Recibo != null)
                {
                    pd.PrintPage += new PrintPageEventHandler(PrintPage2);
                }
                else
                {
                    pd.PrintPage += new PrintPageEventHandler(PrintPage);
                }

                pd.Print();
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {

            Graphics g = e.Graphics;
            string RutaIMG = GetConfiguration("LogoComprobante");
            g.DrawImage(Image.FromFile(RutaIMG), 30, 0);
            g.DrawString(Cinema, fBody, sb, 120, SPACE);
            //Solo para los demas
            g.DrawString("Colombia de Cines S.A", fBody, sb, 10, SPACE + 25);
            g.DrawString("NIT:", fBody, sb, 10, SPACE + 40);
            g.DrawString(GetConfiguration("Nit"), fBody1, sb, 120, SPACE + 40);
            g.DrawString("Cra 48 N. 50 sur 128", fBody1, sb, 10, SPACE + 55);
            int increment = SPACE + 55;
            increment += 15;
            g.DrawString("Tel:", fBody1, sb, 10, increment);
            g.DrawString(GetConfiguration("Tel"), fBody1, sb, 120, increment);

            increment += 15;
            //Solo para puerta del norta
            //g.DrawString("Promotora Nacional de Cines S.A.S", fBody, sb, 10, increment);
            //increment += 15;
            //g.DrawString("NIT:", fBody, sb, 10, increment);
            //g.DrawString(GetConfiguration("NitPromotora"), fBody1, sb, 120, increment);
            //increment += 15;
            //g.DrawString("Tel:", fBody1, sb, 10, increment);
            //g.DrawString(GetConfiguration("TelPromotora"), fBody1, sb, 120, increment);
            //increment += 15;
            g.DrawString("ENTRADA A CINE - 87" + Consecutivo, fBody1, sb, 10, increment);
            increment += 15;
            g.DrawString("Audi 051682", fBody1, sb, 10, increment);
            increment += 20;

            g.DrawString("Película:", fBody, sb, 10, increment);
            g.DrawString(Movie, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Censura:", fBody, sb, 10, increment);
            g.DrawString(Category, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Fecha:", fBody, sb, 10, increment);
            g.DrawString(Date, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Hora:", fBody, sb, 10, increment);
            g.DrawString(Time, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Sala:", fBody, sb, 10, increment);
            g.DrawString(Room, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Silla", fBody, sb, 10, increment);
            g.DrawString(Seat, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Estado:", fBody, sb, 10, increment);
            g.DrawString(Estado, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Fecha de venta:", fBody, sb, 10, increment);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, increment);
            g.DrawString("Por: E-city software", fBody, sb, 10, increment);
            increment += 20;
            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa:", fBody, sb, 10, increment);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, increment);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
            }
            increment += 20;
            g.DrawString("SOLO SE PERMITE EL INGRESO A LAS", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("INSTALACIONES DE CINEMAS PROCINAL,", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DE PRODUCTOS QUE HAYAN SIDO COMPRADOS", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("EN NUESTRAS CONFITERIAS,", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("O CAFES DEL CINEMA, GRACIAS.", fBody, sb, 10, increment);

            increment += 30;
            g.DrawString("PELICULAS CON RESTRICCIÓN PARA MAYORES", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DE 15 O 18 AÑOS DEBE PRESENTAR", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DOCUMENTO DE IDENTIDAD. GRACIAS.", fBody, sb, 10, increment);

            increment += 30;
            g.DrawString("E-city software", fBody1, sb, 82, increment);
        }

        private void PrintPage2(object sender, PrintPageEventArgs e)
        {

            Graphics g = e.Graphics;
            string RutaIMG = GetConfiguration("LogoComprobante");
            g.DrawImage(Image.FromFile(RutaIMG), 30, 0);
            g.DrawString(Cinema, fBody, sb, 120, SPACE);

            //TODO: Solo para los demas
            g.DrawString("Colombia de Cines S.A", fBody, sb, 10, SPACE + 25);
            g.DrawString("NIT:", fBody, sb, 10, SPACE + 40);
            g.DrawString(GetConfiguration("Nit"), fBody1, sb, 120, SPACE + 40);
            g.DrawString("Cra 48 N. 50 sur 128", fBody1, sb, 10, SPACE + 55);
            int increment = SPACE + 55;
            increment += 15;
            g.DrawString("Tel:", fBody1, sb, 10, increment);
            g.DrawString(GetConfiguration("Tel"), fBody1, sb, 120, increment);

            increment += 15;
            //TODO: Solo para puerta del norta
            //g.DrawString("Promotora Nacional de Cines S.A.S", fBody, sb, 10, increment);
            //increment += 15;
            //g.DrawString("NIT:", fBody, sb, 10, increment);
            //g.DrawString(GetConfiguration("NitPromotora"), fBody1, sb, 120, increment);
            //increment += 15;
            //g.DrawString("Tel:", fBody1, sb, 10, increment);
            //g.DrawString(GetConfiguration("TelPromotora"), fBody1, sb, 120, increment);
            //increment += 15;
            g.DrawString("ENTRADA A CINE - 87 " + Consecutivo, fBody1, sb, 10, increment);
            increment += 15;
            g.DrawString("Audi 051682", fBody1, sb, 10, increment);
            increment += 20;
            g.DrawString("Película:", fBody, sb, 10, increment);
            g.DrawString(Movie, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Censura:", fBody, sb, 10, increment);
            g.DrawString(Category, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Fecha:", fBody, sb, 10, increment);
            g.DrawString(Date, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Hora:", fBody, sb, 10, increment);
            g.DrawString(Time, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Sala:", fBody, sb, 10, increment);
            g.DrawString(Room, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Silla", fBody, sb, 10, increment);
            g.DrawString(Seat, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Estado:", fBody, sb, 10, increment);
            g.DrawString(Estado, fBody1, sb, 120, increment);
            increment += 20;
            g.DrawString("Fecha de venta:", fBody, sb, 10, increment);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, increment);
            increment += 25;
            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("Por: E-city software", fBody, sb, 10, increment);
            increment += 20;

            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa:", fBody, sb, 10, increment);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);

            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, increment);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
            }
            increment += 20;
            g.DrawString("SOLO SE PERMITE EL INGRESO A LAS", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("INSTALACIONES DE CINEMAS PROCINAL,", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DE PRODUCTOS QUE HAYAN SIDO COMPRADOS", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("EN NUESTRAS CONFITERIAS,", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("O CAFES DEL CINEMA", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("GRACIAS.", fBody, sb, 10, increment);

            increment += 30;
            g.DrawString("PELICULAS CON RESTRICCIÓN PARA MAYORES", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DE 15 O 18 AÑOS DEBE PRESENTAR", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("DOCUMENTO DE IDENTIDAD. GRACIAS.", fBody, sb, 10, increment);

            increment += 30;
            g.DrawString("E-city software", fBody1, sb, 82, increment);
        }

        public void ImprimirComprobanteCancelar()
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                pd.PrintPage += new PrintPageEventHandler(PrintPageCancel);
                pd.Print();
            }
            catch (Exception ex)
            {

            }
        }

        private void PrintPageCancel(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            string RutaIMG = GetConfiguration("LogoComprobante");
            g.DrawImage(Image.FromFile(RutaIMG), 30, 0);
            g.DrawString(Cinema, fBody, sb, 120, SPACE);
            g.DrawString("Colombia de Cines S.A", fBody, sb, 10, SPACE + 25);
            g.DrawString("NIT:", fBody, sb, 10, SPACE + 40);
            g.DrawString(GetConfiguration("Nit"), fBody1, sb, 120, SPACE + 40);
            g.DrawString("Cra 4850 sur 128 of 6005 piso 7", fBody1, sb, 10, SPACE + 55);
            int increment = SPACE + 55;
            increment += 15;
            g.DrawString("Tel:", fBody1, sb, 10, increment);
            g.DrawString(GetConfiguration("Tel"), fBody1, sb, 120, increment);

            increment += 15;
            g.DrawString("Promotora Nacional de Cines S.A.S", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("NIT:", fBody, sb, 10, increment);
            g.DrawString(GetConfiguration("NitPromotora"), fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("Tel:", fBody1, sb, 10, increment);
            g.DrawString(GetConfiguration("TelPromotora"), fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("ENTRADA A CINE - 9 DMC9 127813", fBody1, sb, 10, increment);
            increment += 15;
            g.DrawString("Audi 051682", fBody1, sb, 10, increment);
            increment += 20;
            g.DrawString("Película:", fBody, sb, 10, increment);
            g.DrawString(Movie, fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("Censura:", fBody, sb, 10, increment);
            g.DrawString(Category, fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("Estado:", fBody, sb, 10, increment);
            g.DrawString(Estado, fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("Fecha de venta:", fBody, sb, 10, increment);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, increment);
            increment += 15;
            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, increment);
            increment += 15;
            g.DrawString("Por: E-city software", fBody, sb, 10, increment);
            increment += 15;
            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa a devolver:", fBody, sb, 10, increment);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);

            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, increment);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, increment);
            }
            increment += 30;
            g.DrawString("E-city software", fBody1, sb, 82, increment);
        }
    }
}
