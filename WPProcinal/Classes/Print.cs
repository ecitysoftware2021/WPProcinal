using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace WPProcinal.Classes
{
    class Print
    {
        #region Confiteria

        SolidBrush sb = new SolidBrush(Color.Black);
        Font fBodyTiulos = new Font("myriam pro", 8, FontStyle.Regular);
        Font fBodySala = new Font("myriam pro", 9.5f, FontStyle.Bold);


        public decimal Valor { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public int SpaceY { get; set; }
        public int SpaceX { get; set; }
        public string Cinema { get { return Dictionaries.Cinemas[Utilities.GetConfiguration("CodCinema")]; } }

        public void ImprimirComprobante()
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);

                pd.PrintPage += new PrintPageEventHandler(PrintPageCombos);

                pd.Print();
            }
            catch (Exception ex)
            {
            }
        }


        private void PrintPageCombos(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                SpaceY = 100;
                SpaceX = 70;

                string RutaIMG = Path.Combine(Directory.GetCurrentDirectory(), "Salas_Formatos", "logo.png");
                g.DrawImage(Image.FromFile(RutaIMG), 40, 0);

                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, 80);
                SpaceY += 10;


                if (Utilities.GetConfiguration("CodCinema").Equals("304")
                    || Utilities.GetConfiguration("CodCinema").Equals("302"))
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Promotora Nacional de Cines S.A.S", fBodyTiulos, sb, 50, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.GetConfiguration("NitPromotora"), fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                else
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Colombia de Cines S.A", fBodyTiulos, sb, 80, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.GetConfiguration("Nit"), fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 20;

                if (Utilities._DataResolution != null && Utilities._DataResolution.Count > 0)
                {
                    foreach (var data in Utilities._DataResolution)
                    {
                        g.DrawString("Factura ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Factura.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                        g.DrawString("Prefijo ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Prefijo.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                        g.DrawString("Resolución ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Resolucion.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                        g.DrawString("Inicio ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Inicio.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                        g.DrawString("Fin ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Fin.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                        g.DrawString("Vencimiento ", fBodyTiulos, sb, 10, SpaceY);
                        g.DrawString(data.Vencimiento.ToString(), fBodyTiulos, sb, 140, SpaceY);
                        SpaceY += 20;
                    }
                }

                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 20;
                g.DrawString("Factura de Compra", fBodyTiulos, sb, 90, SpaceY);
                SpaceY += 35;
                g.DrawString("Producto ", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("Valor ", fBodyTiulos, sb, 140, SpaceY);
                g.DrawString("Cantidad ", fBodyTiulos, sb, 220, SpaceY);
                SpaceY += 15;
                g.DrawString("========================================", fBodyTiulos, sb, 10, SpaceY);
                SpaceY += 15;

                foreach (var item in Utilities._Combos)
                {
                    g.DrawString(item.Name, fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(item.Price.ToString("$ #,#00"), fBodyTiulos, sb, 140, SpaceY);
                    g.DrawString(item.Quantity.ToString(), fBodyTiulos, sb, 220, SpaceY);
                    Valor += item.Price;
                    SpaceY += 15;
                }

                g.DrawString("========================================", fBodyTiulos, sb, 10, SpaceY);

                SpaceY += 20;
                g.DrawString("Base IMP al Consumo 8%", fBodyTiulos, sb, 40, SpaceY);
                var baseIMP = double.Parse(Valor.ToString()) / 1.08;
                g.DrawString(baseIMP.ToString("$ #,#00"), fBodyTiulos, sb, 200, SpaceY);

                SpaceY += 20;
                g.DrawString("Subtotal", fBodyTiulos, sb, 40, SpaceY);
                g.DrawString(baseIMP.ToString("$ #,#00"), fBodyTiulos, sb, 200, SpaceY);
                SpaceY += 20;
                var IMP = double.Parse(baseIMP.ToString()) * 0.08;
                g.DrawString("IMP al Consumo 8%", fBodyTiulos, sb, 40, SpaceY);
                g.DrawString(IMP.ToString("$ #,#00"), fBodyTiulos, sb, 200, SpaceY);


                SpaceY += 20;
                g.DrawString("Valor Total:", fBodyTiulos, sb, 40, SpaceY);
                g.DrawString((baseIMP + IMP).ToString("$ #,#00"), fBodyTiulos, sb, 200, SpaceY);
                SpaceY += 20;
                g.DrawString("RECLAMA TU COMBO EN LA CONFITERÍA", fBodySala, sb, 0, SpaceY);
                SpaceY += 20;
                g.DrawString("Venta realizada en el Kiosko Digital", fBodyTiulos, sb, 50, SpaceY);
                SpaceY += 15;
                g.DrawString("Por E-city software.", fBodyTiulos, sb, 90, SpaceY);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

    }
}
