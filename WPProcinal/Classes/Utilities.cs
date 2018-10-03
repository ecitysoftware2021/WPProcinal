using CEntidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPProcinal.ADO;
using WPProcinal.Forms;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Classes
{
    public class Utilities
    {
        private static DateTime _Date;
        public static DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        public static List<Pelicula> Movies = new List<Pelicula>();

        public static string CinemaId { get; set; }
        //GetDatosTCRClient WCFTCR = new GetDatosTCRClient();
        public static decimal ValorPagar { get; set; }

        public static List<CLSDatos> LDatos = new List<CLSDatos>();

        public bool Estado { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorIngresado { get; set; }

        public decimal ValorFaltante { get; set; }

        public decimal ValorRestante { get; set; }

        internal static DipMap ConvertDipMap(TblDipMap dipmap)
        {
            return new DipMap
            {
                Category = dipmap.Category,
                CinemaId = dipmap.CinemaId.Value,
                Date = dipmap.Date,
                DateFormat = dipmap.DateFormat,
                Day = dipmap.Day,
                DipMapId = dipmap.DipMapId,
                Duration = dipmap.Duration,
                Gener = dipmap.Gener,
                Group = dipmap.Group.Value,
                Hour = dipmap.Hour.Value,
                HourFormat = dipmap.HourFormat.Value,
                HourFunction = dipmap.HourFunction,
                IsCard = dipmap.IsCard,
                Language = dipmap.Language,
                Letter = dipmap.Letter,
                Login = dipmap.Login,
                MovieId = dipmap.MovieId.Value,
                MovieName = dipmap.MovieName,
                PointOfSale = dipmap.PointOfSale.Value,
                RoomId = dipmap.RoomId.Value,
                Secuence = dipmap.Secuence.Value,
                RoomName = dipmap.RoomName,
            };
        }

        internal static List<TypeSeat> ConvertSeats(List<TblTypeSeat> tblTypeSeats)
        {
            try
            {
                List<TypeSeat> typeSeats = new List<TypeSeat>();
                foreach (var item in tblTypeSeats)
                {
                    typeSeats.Add(new TypeSeat
                    {
                        CodTarifa = item.CodTarifa,
                        IsReserved = item.IsReserved.Value,
                        Letter = item.Letter,
                        Name = item.Name,
                        Number = item.Number,
                        NumSecuencia = item.NumSecuencia,
                        Price = item.Price.Value,
                        Type = item.Type
                    });
                }

                return typeSeats;
            }
            catch (Exception ex)
            {
                return new List<TypeSeat>();
            }
        }

        public static decimal ValorDevolver { get; set; }

        public static string TramitePagado { get; set; }

        public static int ConceptoContable { get; set; }

        public static string IdentificadorArchivo { get; set; }

        public static string MovieFormat { get; set; }

        public static int CorrespondetId = int.Parse(GetConfiguration("IDCorresponsal"));

        public static int TramiteId = int.Parse(GetConfiguration("IDTramite"));

        public static string NamePath = GetConfiguration("NamePath");

        public static string NameFile = string.Format("{0}\\Cinema-{1}.txt", NamePath, DateTime.Now.ToString("yyyyMMdd"));

        public static string NamePathLog = GetConfiguration("NamePathLog");

        public static string NameFileLog = string.Format("{0}\\Error-{1}.json", NamePathLog, DateTime.Now.ToString("yyyyMMdd"));

        public static string UrlImages = Utilities.GetConfiguration("UrlImages");

        public static Pelicula Movie;

        public static Peliculas Peliculas;

        public static ImageSource ImageSelected;

        public static ObservableCollection<MoviesViewModel> LstMovies = new ObservableCollection<MoviesViewModel>();

        CLSPrint objPrint = new CLSPrint();

        public static List<int> IDTransaccionDBs = new List<int>();

        public static List<TypeSeat> TypeSeats = new List<TypeSeat>();

        public static DipMap DipMapCurrent = new DipMap();

        public static Receipt Receipt = new Receipt();

        public enum ETipoAlerta
        {
            BaulLLeno = 0,
            BilleteroVacio = 1,
        }

        public static string CapitalizeFirstLetter(string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static BitmapImage LoadImage(string filename, bool type)
        {
            if (type)
            {
                return new BitmapImage(new Uri(filename));
            }

            return new BitmapImage(new Uri(string.Concat(filename), UriKind.Relative));
        }

        public static bool IfExistUrl(string url)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                WebResponse webResponse = webRequest.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public static void CancelAssing(List<TypeSeat> typeSeatsCurrent, DipMap dipMapCurrent)
        {
            foreach (var typeSeat in typeSeatsCurrent)
            {
                var response = WCFServices.PostDesAssingreserva(dipMapCurrent, typeSeat);
                if (!response.IsSuccess)
                {
                    Utilities.SaveLogError(new LogError
                    {
                        Message = response.Message,
                        Method = "CancelAssing",
                        Error = response.Result
                    });
                    //Utilities.ShowModal(response.Message);
                }

                var desAssing = WCFServices.DeserealizeXML<DesAsignarReserva>(response.Result.ToString());
                if (!string.IsNullOrEmpty(desAssing.Error_en_proceso))
                {
                    SaveLogError(new LogError
                    {
                        Message = desAssing.Error_en_proceso,
                        Method = "CancelAssing",
                        Error = response.Result
                    });

                    if (desAssing.Error_en_proceso == "Error en proceso - 7")
                    {
                        var responseDb = DBProcinalController.EditSeatDesAssingReserve(dipMapCurrent.DipMapId);
                        if (!response.IsSuccess)
                        {
                            SaveLogError(new LogError
                            {
                                Message = responseDb.Message,
                                Method = "EditSeatDesAssingReserve",
                            });
                        }
                    }
                }
                else
                {
                    var responseDb = DBProcinalController.EditSeatDesAssingReserve(dipMapCurrent.DipMapId);
                    if (!response.IsSuccess)
                    {
                        SaveLogError(new LogError
                        {
                            Message = responseDb.Message,
                            Method = "EditSeatDesAssingReserve",
                        });
                    }
                }
            }
        }

        public static void SetReceipt(object result)
        {
            var receipt = JsonConvert.DeserializeObject<Receipt>(result.ToString());
        }

        public static void GoToInicial()
        {
            Process pc = new Process();
            Process pn = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string apppath = Path.Combine(path, "WPProcinal.exe");
            si.FileName = apppath;
            pn.StartInfo = si;
            pn.Start();
            pc = Process.GetCurrentProcess();
            pc.Kill();
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }

        public static Dictionary<int, string> Days = new Dictionary<int, string>()
        {
            {0,"Lunes"},
            {1,"Martes"},
            {2,"Miércoles"},
            {3,"Jueves"},
            {4,"Viernes"},
        };

        public enum EDenominacion
        {
            [Description("100")]
            Cien = 1,
            [Description("200")]
            Doscientos = 2,
            [Description("500")]
            Quinientos = 3,
            [Description("1000")]
            Mil = 4,
            [Description("2000,00")]
            BDosMil = 5,
            [Description("5000,00")]
            BCincoMil = 6,
            [Description("10000,00")]
            BDiezMil = 7,
            [Description("20000,00")]
            BVeinteMil = 8,
            [Description("50000,00")]
            BCincuentaMil = 9,
            [Description("1000,00")]
            BMil = 10
        }

        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }

        public static ImageSource GenerateSource(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(string.Concat("../Images/Background/", path, ".jpg"), UriKind.Relative);
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static bool IfExistFileXML()
        {            
            if (!Directory.Exists(NamePath))
            {
                Directory.CreateDirectory(NamePath);
            }
            
            if (!File.Exists(NameFile))
            {
                return false;
            }

            return true;
        }

        public static void SaveFileXML(string content)
        {            
            if (!File.Exists(NameFile))
            {
                var file = File.CreateText(NameFile);
                file.Close();
            }

            using (StreamWriter sw = File.AppendText(NameFile))
            {
                sw.WriteLine(content);
            }
        }
        
        public static void SaveLogError(LogError logError)
        {
            if (!Directory.Exists(NamePathLog))
            {
                Directory.CreateDirectory(NamePathLog);
            }

            if (!File.Exists(NameFileLog))
            {
                var file = File.CreateText(NameFileLog);
                file.Close();
            }

            var serializar = JsonConvert.SerializeObject(logError);

            using (StreamWriter sw = File.AppendText(NameFileLog))
            {
                sw.WriteLine(serializar);
            }
        }

        public static string GetFileXML()
        {
            var file = File.ReadAllText(NameFile);
            return file;
        }

        public static int GetDescriptionEnum(string value)
        {
            int idTypeEnum = 0;
            try
            {
                Type enumType = typeof(EDenominacion);
                var values = enumType.GetEnumValues();
                foreach (EDenominacion item in values)
                {
                    MemberInfo info = enumType.GetMember(item.ToString()).First();
                    var description = info.GetCustomAttribute<DescriptionAttribute>();
                    if (value == description.Description.Replace(",00", ""))
                    {
                        EDenominacion enumm = (EDenominacion)Enum.Parse(typeof(EDenominacion), item.ToString());
                        idTypeEnum = (int)enumm;
                        break;
                    }
                }
            }
            catch { }
            return idTypeEnum;
        }

        public static void ShowModal(string message)
        {
            frmModal frmModal = new frmModal(message);
            frmModal.ShowDialog();
        }

        public void ImprimirComprobante(string Estado, Receipt receipt, List<TypeSeat> Seats, DipMap dipMap)
        {

            foreach (var seat in Seats)
            {
                objPrint.Cinema = GetConfiguration("NameCinema");
                objPrint.Movie = dipMap.MovieName;
                objPrint.Time = dipMap.HourFunction;
                objPrint.Room = dipMap.RoomName;
                objPrint.Date = dipMap.Day; //Fecha
                objPrint.Seat = seat.Name;
                objPrint.FechaPago = DateTime.Now;
                objPrint.Valor = seat.Price;
                objPrint.Tramite = "Boleto de Cine";
                objPrint.Category = dipMap.Category;
                objPrint.Recibo = receipt;
                objPrint.ImprimirComprobante();
            }
        }

        public void ImprimirComprobanteCancelado(string state, decimal value, DipMap dipMap)
        {
            objPrint.Cinema = GetConfiguration("NameCinema");
            objPrint.Movie = dipMap.MovieName;
            objPrint.Time = dipMap.HourFunction;
            objPrint.Room = dipMap.RoomName;
            objPrint.Date = dipMap.Day; //Fecha            
            objPrint.FechaPago = DateTime.Now;
            objPrint.Valor = value;
            objPrint.Tramite = "Boleto de Cine";
            objPrint.Category = dipMap.Category;
            objPrint.Estado = "Cancelado";
            objPrint.ImprimirComprobanteCancelar();
        }
    }

    public class CLSDatos
    {
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public long NumeroBastidor { get; set; }
        public string TipoVehiculo { get; set; }
        public string CategoriaVehiculo { get; set; }
        public string Referencia { get; set; }
    }

    public class CLSTipoExpresion
    {
        public static string Numerica { get { return "[^0-9 ]+$"; } }
        public static string Texto { get { return "[^a-zA-ZÁáÉéÍíÓóÚú]"; } }
        public static string AlfaNumerico { get { return "[^A-Za-z0-9]+"; } }
    }
}
