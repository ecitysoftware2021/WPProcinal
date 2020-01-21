using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;

namespace CEntidades
{
    public class CLSPrint
    {
        SolidBrush sb = new SolidBrush(Color.Black);
        Font fBodyTiulos = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBodyContenido = new Font("myriam pro", 12, FontStyle.Bold);
        Font fBodyAvisos = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBodySala = new Font("myriam pro", 14, FontStyle.Bold);
        Font fBodyFecha = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBody1Vertical = new Font("Arial", 6, FontStyle.Bold);

        public string Movie { get; set; }
        public string Secuencia { get; set; }
        public string IDTransaccion { get; set; }
        public string Formato { get; set; }
        public string Tramite { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
        public string Seat { get; set; }
        public string Cinema { get { return Dictionaries.Cinemas[GetConfiguration("CodCinema")]; } }
        public string Date { get; set; }
        public string Puntos { get; set; }
        public string Category { get; set; }
        public DateTime FechaPago { get; set; }
        public int SpaceY { get; set; }
        public int SpaceX { get; set; }

        public string TipoSala { get; set; }

        public string Estado { get; set; }

        public decimal Valor { get; set; }


        public void PrintTickets()
        {
            try
            {
                Estado = "Aprobada";
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                pd.PrintPage += new PrintPageEventHandler(PrintPage);
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
            SpaceY = 100;
            SpaceX = 70;

            string RutaIMG = Path.Combine(Directory.GetCurrentDirectory(), "Salas_Formatos", "logo.png");
            g.DrawImage(Image.FromFile(RutaIMG), 40, 0);

            g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, 80);
            SpaceY += 10;


            if (GetConfiguration("CodCinema").Equals("304")
                || GetConfiguration("CodCinema").Equals("302"))
            {
                g.DrawString(Cinema, fBodySala, sb, 70, SpaceY);
                SpaceY += 30;
                g.DrawString("Promotora Nacional de Cines S.A.S", fBodyTiulos, sb, 50, SpaceY - 10);
                SpaceY += 15;
                g.DrawString(GetConfiguration("NitPromotora"), fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
            }
            else
            {
                g.DrawString(Cinema, fBodySala, sb, 110, SpaceY);
                SpaceY += 30;
                g.DrawString("Colombia de Cines S.A", fBodyTiulos, sb, 80, SpaceY - 10);
                SpaceY += 15;
                g.DrawString(GetConfiguration("Nit"), fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
            }
            SpaceY += 20;
            g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
            SpaceY += 20;
            g.DrawString(FormatName(Movie).ToUpper(), fBodyContenido, sb, 50, SpaceY - 20);
            SpaceY += 20;
            g.DrawImage(Image.FromFile(GenerateTags(TipoSala)), SpaceX, SpaceY);

            g.DrawImage(Image.FromFile(GenerateTags(Formato)), SpaceX + 130, SpaceY + 60);

            g.DrawString(Room, fBodySala, sb, SpaceX + 140, SpaceY + 90);

            PointF pointF = new PointF(0, 200);
            StringFormat stringFormat = new StringFormat();
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255));
            stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            string verticalData = string.Concat(Room, " - Silla: ", Seat, " - Hora: ", Time);
            g.DrawString(verticalData, fBody1Vertical, solidBrush, pointF, stringFormat);
            g.DrawString(Room, fBodySala, sb, SpaceX + 140, SpaceY + 90);

            string RutaIMGSilla = Path.Combine(Directory.GetCurrentDirectory(), "Salas_Formatos", "silla.jpg");
            g.DrawImage(Image.FromFile(RutaIMGSilla), SpaceX + 70, SpaceY + 120);
            g.DrawString(Seat, fBodyTiulos, sb, SpaceX + 170, SpaceY + 130);
            SpaceY += 70;
            g.DrawString("Auditoría:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Secuencia, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Hora:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Time, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Fecha:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Date, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Público:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Category, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Tarifa:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Valor.ToString("$ #,##0"), fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            g.DrawString("Puntos:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Puntos, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            g.DrawString("Compra:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(IDTransaccion, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            g.DrawString("Fecha de venta:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(FechaPago.ToString("dd/MM/yyyy hh:mm:ss"), fBodyFecha, sb, SpaceX + 30, SpaceY);
            SpaceY += 30;
            g.DrawString("Solo se permite el ingreso a las instalaciones de", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 15;
            g.DrawString("Cinemas Procinal, de productos que hayan sido", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 15;
            g.DrawString("comprados en nuestras confiterias o", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 15;
            g.DrawString("Cafes del Cinema.", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 20;
            g.DrawString("Películas con restricción para mayores de 15 o 18", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 15;
            g.DrawString("años debe presentar documento de identidad", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 15;
            g.DrawString("Gracias.", fBodyAvisos, sb, 10, SpaceY);
            SpaceY += 20;
            g.DrawString("Venta realizada en el Kiosko Digital", fBodyAvisos, sb, 50, SpaceY);
            SpaceY += 15;
            g.DrawString("Por E-city software.", fBodyAvisos, sb, 90, SpaceY);
        }

        public string FormatName(string text)
        {
            try
            {
                string message = string.Empty;
                int i = 1;
                foreach (var item in text.Split(' '))
                {
                    message += string.Concat(item, " ");
                    if (i % 2 == 0)
                    {
                        message += Environment.NewLine;
                    }
                    i++;
                }
                return message;
            }
            catch (Exception ex)
            {
                return text;
            }
        }
        private string GenerateTags(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Salas_Formatos", name + ".png");
        }
    }
}
