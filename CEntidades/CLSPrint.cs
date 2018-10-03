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
            g.DrawString("Colombia de Cines S.A", fBody, sb, 10, SPACE + 25);
            g.DrawString("NIT:", fBody, sb, 10, SPACE + 40);
            g.DrawString(GetConfiguration("Nit"), fBody1, sb, 120, SPACE + 40);
            g.DrawString("ENTRADA A CINE - 9 DMC9 127813", fBody1, sb, 10, SPACE + 55);
            g.DrawString("Audi 051682", fBody1, sb, 10, SPACE + 70);

            g.DrawString("Película:", fBody, sb, 10, SPACE + 100);
            g.DrawString(Movie, fBody1, sb, 120, SPACE + 100);
            g.DrawString("Censura:", fBody, sb, 10, SPACE + 120);
            g.DrawString(Category, fBody1, sb, 120, SPACE + 120);
            g.DrawString("Fecha:", fBody, sb, 10, SPACE + 140);
            g.DrawString(Date, fBody1, sb, 120, SPACE + 140);
            g.DrawString("Hora:", fBody, sb, 10, SPACE + 160);
            g.DrawString(Time, fBody1, sb, 120, SPACE + 160);            
            g.DrawString("Sala:", fBody, sb, 10, SPACE + 180);
            g.DrawString(Room, fBody1, sb, 120, SPACE + 180);
            g.DrawString("Silla", fBody, sb, 10, SPACE + 200);
            g.DrawString(Seat, fBody1, sb, 120, SPACE + 200);
            g.DrawString("Estado:", fBody, sb, 10, SPACE + 220);
            g.DrawString(Estado, fBody1, sb, 120, SPACE + 220);
            g.DrawString("Fecha de venta:", fBody, sb, 10, SPACE + 240);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, SPACE +240);

            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, SPACE + 280);
            g.DrawString("Por: E-city software", fBody, sb, 10, SPACE + 295);
            int count = 315;

            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa:", fBody, sb, 10, SPACE + count);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);

            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, SPACE + count);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);
            }

            g.DrawString("SOLO SE PERMITE EL INGRESO A LAS", fBody, sb, 10, SPACE + 335);
            g.DrawString("INSTALACIONES DE CINEMAS PROCINAL,", fBody, sb, 10, SPACE + 350);
            g.DrawString("DE PRODUCTOS QUE HAYAN SIDO COMPRADOS", fBody, sb, 10, SPACE + 365);
            g.DrawString("EN NUESTRAS CONFITERIAS,", fBody, sb, 10, SPACE + 380);
            g.DrawString("O CAFES DEL CINEMA.", fBody, sb, 10, SPACE + 395);

            g.DrawString("PELICULAS CON RESTRICCIÓN PARA MAYORES", fBody, sb, 10, SPACE + 415);
            g.DrawString("DE 15 O 18 AÑOS DEBE PRESENTAR", fBody, sb, 10, SPACE + 430);
            g.DrawString("DOCUMENTO DE IDENTIDAD. GRACIAS.", fBody, sb, 10, SPACE + 445);

            g.DrawString("Su transacción se ha realizado exitosamente", fBody1, sb, 30, SPACE + 465);
            g.DrawString("E-city software", fBody1, sb, 82, SPACE + 480);
        }

        private void PrintPage2(object sender, PrintPageEventArgs e)
        {

            Graphics g = e.Graphics;
            string RutaIMG = GetConfiguration("LogoComprobante");
            g.DrawImage(Image.FromFile(RutaIMG), 30, 0);
            g.DrawString(Cinema, fBody, sb, 120, SPACE);
            g.DrawString("Colombia de Cines S.A", fBody, sb, 10, SPACE + 25);
            g.DrawString("NIT:", fBody, sb, 10, SPACE + 40);
            g.DrawString(GetConfiguration("Nit"), fBody1, sb, 120, SPACE + 40);
            g.DrawString("ENTRADA A CINE - 9 DMC9 127813", fBody1, sb, 10, SPACE + 55);
            g.DrawString("Audi 051682", fBody1, sb, 10, SPACE + 70);

            g.DrawString("Película:", fBody, sb, 10, SPACE + 100);
            g.DrawString(Movie, fBody1, sb, 120, SPACE + 100);
            g.DrawString("Censura:", fBody, sb, 10, SPACE + 120);
            g.DrawString(Category, fBody1, sb, 120, SPACE + 120);
            g.DrawString("Fecha:", fBody, sb, 10, SPACE + 140);
            g.DrawString(Date, fBody1, sb, 120, SPACE + 140);
            g.DrawString("Hora:", fBody, sb, 10, SPACE + 160);
            g.DrawString(Time, fBody1, sb, 120, SPACE + 160);
            g.DrawString("Sala:", fBody, sb, 10, SPACE + 180);
            g.DrawString(Room, fBody1, sb, 120, SPACE + 180);
            g.DrawString("Silla", fBody, sb, 10, SPACE + 200);
            g.DrawString(Seat, fBody1, sb, 120, SPACE + 200);
            g.DrawString("Estado:", fBody, sb, 10, SPACE + 220);
            g.DrawString(Estado, fBody1, sb, 120, SPACE + 220);
            g.DrawString("Fecha de venta:", fBody, sb, 10, SPACE + 240);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, SPACE + 240);

            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, SPACE + 280);
            g.DrawString("Por: E-city software", fBody, sb, 10, SPACE + 295);
            int count = 315;

            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa:", fBody, sb, 10, SPACE + count);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);

            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, SPACE + count);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);
            }

            g.DrawString(Recibo.Footer, fBody, sb, 10, SPACE + 335);

            g.DrawString("Su transacción se ha realizado exitosamente", fBody1, sb, 30, SPACE + 465);
            g.DrawString("E-city software", fBody1, sb, 82, SPACE + 480);
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
            g.DrawString("ENTRADA A CINE - 9 DMC9 127813", fBody1, sb, 10, SPACE + 55);
            g.DrawString("Audi 051682", fBody1, sb, 10, SPACE + 70);
            g.DrawString("Película:", fBody, sb, 10, SPACE + 100);
            g.DrawString(Movie, fBody1, sb, 120, SPACE + 100);
            g.DrawString("Censura:", fBody, sb, 10, SPACE + 120);
            g.DrawString(Category, fBody1, sb, 120, SPACE + 120);                        
            g.DrawString("Estado:", fBody, sb, 10, SPACE + 140);
            g.DrawString(Estado, fBody1, sb, 120, SPACE + 140);
            g.DrawString("Fecha de venta:", fBody, sb, 10, SPACE + 160);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fBody1, sb, 120, SPACE + 160);

            g.DrawString("Venta realizada en el Kiosko Digital", fBody, sb, 10, SPACE + 180);
            g.DrawString("Por: E-city software", fBody, sb, 10, SPACE + 195);
            int count = 215;

            if (Estado != "Rechazada")
            {
                g.DrawString("Tarifa a devolver:", fBody, sb, 10, SPACE + count);
                g.DrawString(Valor.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);

            }
            else
            {
                g.DrawString("Devolución:", fBody, sb, 10, SPACE + count);
                g.DrawString(ValorDevuelto.ToString("C", CultureInfo.CurrentCulture), fBody1, sb, 120, SPACE + count);
            }
            
            g.DrawString("Su transacción se ha realizado exitosamente", fBody1, sb, 30, SPACE + 230);
            g.DrawString("E-city software", fBody1, sb, 82, SPACE + 245);
        }
    }
}
