using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Reflection;
using WPProcinal.Service;
using Zen.Barcode;

namespace WPProcinal.Classes
{
    class Print
    {
        #region Confiteria

        SolidBrush sb = new SolidBrush(Color.Black);
        Font fBodyTiulos = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBodyContenido = new Font("myriam pro", 12, FontStyle.Bold);
        Font fBodyAvisos = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBodySala = new Font("myriam pro", 9, FontStyle.Bold);
        Font fBodyFecha = new Font("myriam pro", 8, FontStyle.Bold);
        Font fBody1Vertical = new Font("Arial", 6, FontStyle.Bold);

        public Print()
        {
            Seat = new List<string>();
        }

        public decimal Valor { get; set; }
        public int Hora { get; set; }
        public int SpaceY { get; set; }
        public int SpaceX { get; set; }
        public string Cinema { get { return Dictionaries.Cinemas[Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString()]; } }

        public string Movie { get; set; }
        public string Secuencia { get; set; }
        public string IDTransaccion { get; set; }
        public string Formato { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
        public List<string> Seat { get; set; }
        public string Fila { get; set; }
        public int Columna { get; set; }
        public int Funcion { get; set; }
        public string Date { get; set; }
        public string DateFormat { get; set; }
        public string Puntos { get; set; }
        public string Category { get; set; }
        public DateTime FechaPago { get; set; }

        public string TipoSala { get; set; }

        public string Estado { get; set; }
        public string Placa { get; set; }



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


        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            SpaceY = 100;
            SpaceX = 70;    
            var pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Salas_Formatos", "logo.png");
            string RutaIMG = pat;
            g.DrawImage(Image.FromFile(RutaIMG), 40, 0);
            SpaceY += 10;


            if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 304
                || Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 302)
            {
                g.DrawString(Cinema, fBodySala, sb, 70, SpaceY);
                SpaceY += 30;
                g.DrawString("Promotora Nacional de Cines S.A.S", fBodyTiulos, sb, 50, SpaceY - 10);
                SpaceY += 15;
                g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.NitPromotora, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
            }
            else
            {
                g.DrawString(Cinema, fBodySala, sb, 110, SpaceY);
                SpaceY += 30;
                g.DrawString("Colombia de Cines S.A", fBodyTiulos, sb, 80, SpaceY - 10);
                SpaceY += 15;
                g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.Nit, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
            }
            SpaceY += 20;

            PointF pointF = new PointF(0, 0);
            StringFormat stringFormat = new StringFormat();
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255));
            stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            string verticalData = string.Concat("Película: ", Movie, " - ", Room, " - Sillas: ", String.Join(",", Seat), " - Fecha: ", Date, " - Hora: ", Time, " - Valor: ", Valor.ToString("$ #,##0"));
            g.DrawString(verticalData, fBody1Vertical, solidBrush, pointF, stringFormat);
            pointF = new PointF(275, 0);
            g.DrawString(verticalData, fBody1Vertical, solidBrush, pointF, stringFormat);


            g.DrawString("Auditoría:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Secuencia, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Película:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Movie, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            g.DrawString("Sala:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString($"{Room} - {Formato} - {TipoSala} ", fBodyTiulos, sb, SpaceX, SpaceY);
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
            g.DrawString((Valor / Seat.Count).ToString("$ #,##0"), fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Total:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(Valor.ToString("$ #,##0"), fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;
            g.DrawString("Sillas:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(string.Join(",", Seat), fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            //public int Cashback_Acu { get; set; }
            //public int Cashback_Red { get; set; }
            //public int Cashback_Total { get; set; }
            //public int Cashback_Compra { get; set; }
            //public string Cashback_Venci { get; set; }
            //public int Visitas_ultima { get; set; }
            if (DataService41._DataResolution != null && DataService41._DataResolution.DatosCliente != null && DataService41._DataResolution.DatosCliente.Cashback_Compra > 0)
            {
                g.DrawString("Saldo Acumulado:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString(DataService41._DataResolution.DatosCliente.Cashback_Compra.ToString("C"), fBodyTiulos, sb, SpaceX + 50, SpaceY);
                SpaceY += 20;
                g.DrawString("Saldo Total:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString(DataService41._DataResolution.DatosCliente.Cashback_Acu.ToString("C"), fBodyTiulos, sb, SpaceX + 50, SpaceY);
                SpaceY += 20;
                g.DrawString("Vencimiento Saldo:", fBodyTiulos, sb, 10, SpaceY);

                DateTime dt;
                DateTime.TryParseExact(DataService41._DataResolution.DatosCliente.Cashback_Venci.ToString(), "yyyyMMdd",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out dt);

                g.DrawString(dt.ToString("yyyy-MM-dd"), fBodyTiulos, sb, SpaceX + 50, SpaceY);
                SpaceY += 20;
                g.DrawString("Saldo Utilizado:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString(Utilities.dataTransaction.PagoInterno.ToString("C"), fBodyTiulos, sb, SpaceX + 50, SpaceY);
                SpaceY += 20;
            }


            g.DrawString("Compra:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(IDTransaccion, fBodyTiulos, sb, SpaceX, SpaceY);
            SpaceY += 20;

            g.DrawString("Fecha de venta:", fBodyTiulos, sb, 10, SpaceY);
            g.DrawString(FechaPago.ToString("dd/MM/yyyy hh:mm:ss"), fBodyFecha, sb, SpaceX + 30, SpaceY);
            SpaceY += 30;
            //g.DrawString("Vehículo:", fBodyTiulos, sb, 10, SpaceY);
            //g.DrawString(Placa, fBodyFecha, sb, SpaceX + 30, SpaceY);
            //SpaceY += 30;
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
            SpaceY += 50;
            DateTime fechaConvertida = DateTime.ParseExact(DateFormat, "yyyyMMdd", CultureInfo.InvariantCulture);

        }



        public void ImprimirComprobante(int type)
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);

                if (type == 0)
                {
                    pd.PrintPage += new PrintPageEventHandler(PrintPageCombos);
                }
                else
                {
                    pd.PrintPage += new PrintPageEventHandler(PrintPageComboTemporada);
                }

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
                string DataQr = "cnv" + Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta.ToString() + " " + Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema + " " + Secuencia + "-" + Utilities.dataTransaction.FechaSeleccionada.Date.ToString("yyyy-MM-dd").Replace("-", String.Empty);
                Graphics g = e.Graphics;
                SpaceY = 100;
                SpaceX = 70;
                string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Salas_Formatos", "logo.png");
                string RutaIMG = pat;
                g.DrawImage(Image.FromFile(RutaIMG), 40, 0);

                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, 80);
                SpaceY += 10;


                if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 304
                    || Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 302)
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Promotora Nacional de Cines S.A.S", fBodyTiulos, sb, 50, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.NitPromotora, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                else
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Colombia de Cines S.A", fBodyTiulos, sb, 80, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.Nit, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 20;

                if (DataService41._DataResolution != null)
                {
                    g.DrawString("Factura ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Factura.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
                    g.DrawString("Prefijo ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Prefijo.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
                    g.DrawString("Resolución ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Resolución.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
                    g.DrawString("Inicio ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Inicio.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
                    g.DrawString("Fin ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Fin.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
                    g.DrawString("Vencimiento ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(DataService41._DataResolution.Vencimiento.ToString(), fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;

                    g.DrawString("Secuencia ", fBodyTiulos, sb, 10, SpaceY);
                    g.DrawString(Secuencia, fBodyTiulos, sb, 140, SpaceY);
                    SpaceY += 20;
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

                foreach (var item in DataService41._Combos)
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
                //SpaceY += 20;
                //g.DrawString("Vehículo:", fBodyTiulos, sb, 40, SpaceY);
                //g.DrawString(Placa, fBodyTiulos, sb, 200, SpaceY);
                SpaceY += 20;
                g.DrawString("RECLAMA TU COMBO EN LA CONFITERÍA", fBodySala, sb, 0, SpaceY);
                SpaceY += 20;
                g.DrawImage(Utilities.GenerateCodeQr(DataQr, 2), 90, SpaceY);
                SpaceY += 100;
                g.DrawString("Venta realizada en el Kiosko Digital", fBodyTiulos, sb, 50, SpaceY);
                SpaceY += 15;
                g.DrawString("Por E-city software.", fBodyTiulos, sb, 90, SpaceY);
            }
            catch (Exception ex)
            {
            }
        }


        private void PrintPageComboTemporada(object sender, PrintPageEventArgs e)
        {
            try
            {
                string DataQr = "cnv" + Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta.ToString() + " " + Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema + " " + Secuencia + "-" + Utilities.dataTransaction.FechaSeleccionada.Date.ToString("yyyy-MM-dd").Replace("-", String.Empty);
                Graphics g = e.Graphics;
                SpaceY = 100;
                SpaceX = 70;
                string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Salas_Formatos", "logo.png");
                string RutaIMG = pat;
                g.DrawImage(Image.FromFile(RutaIMG), 40, 0);

                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, 80);
                SpaceY += 10;


                if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 304
                    || Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema == 302)
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Promotora Nacional de Cines S.A.S", fBodyTiulos, sb, 50, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.NitPromotora, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                else
                {
                    g.DrawString(Cinema, fBodySala, sb, 90, SpaceY);
                    SpaceY += 30;
                    g.DrawString("Colombia de Cines S.A", fBodyTiulos, sb, 80, SpaceY - 10);
                    SpaceY += 15;
                    g.DrawString(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.Nit, fBodyTiulos, sb, SpaceX + 40, SpaceY - 10);
                }
                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 10;
                g.DrawString("COMPROBANTE DE SOLICITUD DE DATOS", fBodyTiulos, sb, 15, SpaceY - 10);
                SpaceY += 20;

                g.DrawString("Producto comprado", fBodyTiulos, sb, 10, SpaceY - 10);
                SpaceY += 15;

                g.DrawString("Combo Jumanji", fBodySala, sb, 20, SpaceY - 10);

                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 10;

                g.DrawString("Nombre:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 15;
                g.DrawString("Cédula:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 15;
                g.DrawString("Teléfono Fijo:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(35, '-'), fBodySala, sb, 85, SpaceY);
                SpaceY += 15;
                g.DrawString("Celular:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 15;
                g.DrawString("Correo:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 15;
                g.DrawString("Ciudad:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 15;
                g.DrawString("Dirección:", fBodyTiulos, sb, 10, SpaceY);
                g.DrawString("-".PadRight(40, '-'), fBodySala, sb, 60, SpaceY);
                SpaceY += 20;

                SpaceY += 20;
                g.DrawString("-".PadRight(50, '-'), fBodySala, sb, 10, SpaceY - 15);
                SpaceY += 20;
                g.DrawImage(Utilities.GenerateCodeQr(DataQr, 2), 90, SpaceY);
                SpaceY += 100;
                g.DrawString("Venta realizada en el Kiosko Digital", fBodyTiulos, sb, 50, SpaceY);
                SpaceY += 15;
                g.DrawString("Por E-city software.", fBodyTiulos, sb, 90, SpaceY);
            }
            catch (Exception ex)
            {
            }
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
            string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Salas_Formatos", name + ".png");
            return pat;
        }

        public static System.Drawing.Image GenerateCode(string code)
        {
            CodeQrBarcodeDraw qrcode = BarcodeDrawFactory.CodeQr;
            return qrcode.Draw(code, 50);
        }

        #endregion

    }
}
