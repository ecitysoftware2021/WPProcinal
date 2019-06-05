using CEntidades;
using Grabador.Transaccion;
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPProcinal.ADO;
using WPProcinal.Forms;
using WPProcinal.Models;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;
using trx;
using WPProcinal.DataModel;

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

        public static string Duration = GetConfiguration("Duration");
        public static TimeSpan time;
        public static DispatcherTimer timer;
        #region Sección Datáfono

        TEFTransactionManager transactionManager;

        public static Action<string> CallBackRespuesta;


        public string EnviarPeticion(string data)
        {
            return transactionManager.getTEFAuthorization(data);
        }

        public string EnviarPeticionEspera()
        {
            return transactionManager.getTEFAuthorization();
        }

        public string CalculateLRC(string s)
        {
            int checksum = 0;
            foreach (Char c in GetStringFromHex(s))
            {
                checksum = checksum ^ Convert.ToByte(c);
            }
            string nuevaCadena = string.Concat("[", s, checksum.ToString("X2"));
            return nuevaCadena;
        }

        private string GetStringFromHex(string s2)
        {
            string result = string.Empty;
            var result2 = string.Join("", s2.Select(c => ((int)c).ToString("X2")));
            for (int i = 0; i < result2.Length; i = i + 2)
            {
                result += Convert.ToChar(int.Parse(result2.Substring(i, 2),
               System.Globalization.NumberStyles.HexNumber));
            }
            return result;
        }
        #endregion

        public static List<Pelicula> Movies = new List<Pelicula>();

        public static string CinemaId { get; set; }
        public static DateTime FechaSeleccionada = DateTime.Today;
        public static decimal ValorPagar { get; set; }

        public static List<CLSDatos> LDatos = new List<CLSDatos>();

        public static List<SP_GET_INVOICE_DATA_Result> DashboardPrint;

        public static string Secuencia { get; set; }
        public bool Estado { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorIngresado { get; set; }

        public decimal ValorFaltante { get; set; }

        public decimal ValorRestante { get; set; }

        public static int MedioPago { get; set; }

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

        public static int CantSeats;

        public static Peliculas Peliculas;

        public static ImageSource ImageSelected;

        public static ObservableCollection<MoviesViewModel> LstMovies = new ObservableCollection<MoviesViewModel>();

        CLSPrint objPrint = new CLSPrint();

        public static List<int> IDTransaccionDBs = new List<int>();

        public static List<TypeSeat> TypeSeats = new List<TypeSeat>();

        public static DipMap DipMapCurrent = new DipMap();

        public static Receipt Receipt = new Receipt();

        public static void Timer(TextBlock textblock, Window window)
        {
            time = TimeSpan.Parse(Duration);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (a, b) =>
            {
                time = time.Subtract(new TimeSpan(0, 0, 1));
                textblock.Text = (time.Seconds < 10) ? string.Format("0{0}:0{1}", time.Minutes, time.Seconds) : string.Format("0{0}:{1}", time.Minutes, time.Seconds);

                if (time.Minutes == 0 && time.Seconds == 0)
                {
                    timer.Stop();
                    GoToInicial(window);
                }
                Debug.WriteLine(time.Seconds);
            };

            timer.Start();
        }

        public static void ResetTimer()
        {
            time = TimeSpan.Parse(Duration);
            timer.Stop();
        }

        public static ControlPeripherals control;
        public static string TOKEN { get; set; }
        public static int Session { get; set; }
        public static int IDTransactionDB { get; set; }
        public static int CorrespondentId = int.Parse(GetConfiguration("IDCorresponsal"));
        public static decimal PayVal { get; set; }
        public static decimal EnterTotal;
        public static long ValueDelivery { get; set; }
        public static decimal DispenserVal { get; set; }

        public Utilities(int i)
        {
            try
            {
                control = new ControlPeripherals();
                control.StopAceptance();
            }
            catch (Exception ex)
            {
            }
        }

        public Utilities()
        {
            try
            {
                transactionManager = new TEFTransactionManager();
            }
            catch { }
        }

        public enum ETipoAlerta
        {
            BaulLLeno = 0,
            BilleteroVacio = 1,
        }

        /// <summary>
        /// Se usa para ocultar o mostrar la modal de carga
        /// </summary>
        /// <param name="window">objeto de la clase FrmLoading  </param>
        /// <param name="state">para saber si se oculta o se muestra true:muestra, false: oculta</param>
        public static void Loading(Window window, bool state, Window w)
        {
            try
            {
                if (state)
                {
                    window.Show();
                    w.IsEnabled = false;
                }
                else
                {
                    window.Hide();
                    w.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
            }
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
                LogService.CreateLogsPeticionRespuestaDispositivos("PostDesAssingreserva: ", JsonConvert.SerializeObject(response));
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

        public static decimal RoundValue(decimal valor)
        {
            decimal roundVal = (Math.Ceiling(valor / 100)) * 100;
            //decimal RoundTo = 100;
            //decimal Amount = valor;
            //decimal sobrante = 0;
            //decimal ExcessAmount = Amount % RoundTo;
            //decimal a = 0;
            //if (ExcessAmount < (RoundTo / 2))
            //{
            //    Amount -= ExcessAmount;
            //    Amount = Amount + RoundTo;
            //    a = Amount - RoundTo;
            //}
            //else
            //{
            //    Amount += (RoundTo - ExcessAmount);
            //    a = Amount - RoundTo;
            //}

            //sobrante = valor - a;
            //if (sobrante > 0)
            //{
            //    if (RoundTo / sobrante == 2)
            //    {
            //        a = a + RoundTo;
            //    }
            //}

            return roundVal;
        }

        /// <summary>
        /// Método que me redirecciona a la ventana de inicio
        /// </summary>
        public static void GoToInicial(Window window)
        {
            Task.Run(() =>
            {
                try
                {
                    CLSGrabador grabador = new CLSGrabador();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        grabador.FinalizarGrabacion();
                    }));

                }
                catch (Exception)
                {
                    //TODO: guardar en log
                }
            });
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    frmCinema main = new frmCinema();
                    main.Show();
                    window.Close();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                RestartApp();
            }
        }

        public static void CloseWindows(string Title)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.IsLoaded && w.Title != Title)
                {
                    w.Close();
                }
            }
        }

        /// <summary>
        /// Método usado para regresar a la pantalla principal
        /// </summary>
        public static void RestartApp()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Process pc = new Process();
                Process pn = new Process();
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = Path.Combine(Directory.GetCurrentDirectory(), "WPProinal.exe");
                pn.StartInfo = si;
                pn.Start();
                pc = Process.GetCurrentProcess();
                pc.Kill();
            }));
            GC.Collect();
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

        public static void ShowModal(string message)
        {
            frmModal frmModal = new frmModal(message);
            frmModal.ShowDialog();
        }

        public void ImprimirComprobante(string Estado, Receipt receipt, List<TypeSeat> Seats, DipMap dipMap)
        {
            try
            {
                int i = 0;
                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ImprimirComprobante: ", "Ingresé");
                }
                catch { }
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
                    objPrint.Consecutivo = DashboardPrint[i].RANGO_ACTUAL.ToString();
                    objPrint.Secuencia = Secuencia;
                    objPrint.Formato = MovieFormat;
                    i++;
                    objPrint.ImprimirComprobante();
                }
                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("ImprimirComprobante: ", "Salí");
                }
                catch { }
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
                string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                ex.InnerException, "---------- Trace: ", ex.StackTrace), "ImprimirComprobante");
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

        public static void SaveLogDispenser(LogDispenser log)
        {
            try
            {
                LogService logService = new LogService
                {
                    NamePath = "C:\\LogDispenser",
                    FileName = string.Concat("Log", DateTime.Now.ToString("yyyyMMdd"), ".json")
                };

                logService.CreateLogs(log);
            }
            catch (Exception ex)
            {
            }
        }

        public static void SaveLogTransactions(LogErrorGeneral log, string path)
        {
            try
            {
                LogService logService = new LogService
                {
                    NamePath = path,
                    FileName = string.Concat("Log", DateTime.Now.ToString("yyyyMMdd"), ".json")
                };

                logService.CreateLogsTransactions(log);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Método encargado de crear la transacciòn en bd y retornar el id de esta misma   
        /// </summary>
        /// <param name="Amount">Cantdiad a pagaar o retirar</param>
        public async Task<bool> CreateTransaction(string name, DipMap movie, List<TypeSeat> Seats)
        {
            try
            {
                ApiLocal api = new ApiLocal();

                Transaction transaction = new Transaction
                {
                    TOTAL_AMOUNT = ValorPagar,
                    DATE_BEGIN = DateTime.Now,
                    DESCRIPTION = "Se inició la transacción para: " + name,
                    TYPE_TRANSACTION_ID = 14,
                    STATE_TRANSACTION_ID = 1,
                    PAYER_ID = 477
                };

                foreach (var item in Seats)
                {
                    var details = new TRANSACTION_DESCRIPTION
                    {
                        AMOUNT = item.Price,
                        TRANSACTION_ID = transaction.TRANSACTION_ID,
                        REFERENCE = Utilities.Secuencia,
                        OBSERVATION = movie.MovieName + " - " + item.Name,
                        TRANSACTION_DESCRIPTION_ID = 0,
                        STATE = true
                    };

                    transaction.TRANSACTION_DESCRIPTION.Add(details);
                }

                var response = await api.GetResponse(new RequestApi
                {
                    Data = transaction
                }, "SaveTransaction");


                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        IDTransactionDB = Convert.ToInt32(response.Data);
                        return true;
                    }

                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Mètodo para actualizar la transacción en base de datos por el estado que coorresponda
        /// </summary>
        /// <param name="Enter">Valor ingresado. Sólo aplica para pago</param>
        /// <param name="state">Estaado por el cual se actualizará</param>
        /// <param name="Return">Valor a devolver, por defecto es 0 ya que hay transacciones donde no hay que devolver</param>
        /// <returns>Retorna un verdadero o un falso dependiendo el resultado del update</returns>
        public async Task<bool> UpdateTransaction(decimal Enter, int state, decimal Return = 0)
        {
            try
            {
                ApiLocal api = new ApiLocal();

                Transaction Transaction = new Transaction
                {
                    STATE_TRANSACTION_ID = state,
                    DATE_END = DateTime.Now,
                    INCOME_AMOUNT = Enter,
                    RETURN_AMOUNT = Return,
                    TRANSACTION_ID = IDTransactionDB
                };

                var response = await api.GetResponse(new RequestApi
                {
                    Data = Transaction
                }, "UpdateTransaction");

                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreatePrintDashboard()
        {
            try
            {
                DashboardPrint = new List<SP_GET_INVOICE_DATA_Result>();

                ApiLocal api = new ApiLocal();

                for (int i = 0; i < CantSeats; i++)
                {
                    var response = await api.GetResponse(new RequestApi
                    {
                        Data = null
                    }, "GetInvoiceData");

                    if (response != null)
                    {
                        if (response.CodeError == 200)
                        {
                            var responseApi = JsonConvert.DeserializeObject<SP_GET_INVOICE_DATA_Result>(response.Data.ToString());

                            if (responseApi.IS_AVAILABLE == true)
                            {
                                DashboardPrint.Add(responseApi);
                            }
                        }
                    }
                }

                if (DashboardPrint.Count() == CantSeats)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void ProccesValue(decimal enterValue, int opt, int quantity, string code, int idTransactionAPi)
        {
            try
            {
                if (opt == 2)
                {
                    if (enterValue > 1000)
                    {
                        code = "AP";
                    }
                    else
                    {
                        code = "MA";
                    }
                }
                else
                {
                    if (enterValue > 1000)
                    {
                        code = "DP";
                    }
                    else
                    {
                        code = "MD";
                    }
                }

                Task.Run(() =>
                {
                    SaveDetailsTransaction(new RequestTransactionDetails
                    {
                        Code = code,
                        Denomination = Convert.ToInt32(enterValue),
                        Operation = opt,
                        Quantity = quantity,
                        TransactionId = idTransactionAPi,
                    });
                });
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
                string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                ex.InnerException, "---------- Trace: ", ex.StackTrace), "ProccesValue(decimal enterValue, int opt, int quantity, string code, int idTransactionAPi)");

            }
        }

        public void ProccesValue(string messaje, int idTransactionAPi)
        {
            try
            {
                Task.Run(() =>
                {
                    SaveDetailsTransaction(new RequestTransactionDetails
                    {
                        Description = messaje,
                        TransactionId = idTransactionAPi
                    });
                });
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
                string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                ex.InnerException, "---------- Trace: ", ex.StackTrace), "ProccesValue(string messaje, int idTransactionAPi)");
            }
        }

        public async static void SaveDetailsTransaction(RequestTransactionDetails detail)
        {
            try
            {
                ApiLocal api = new ApiLocal();

                var se = Utilities.Session;
                if (detail != null)
                {
                    var data = new TRANSACTION_DETAIL
                    {
                        TRANSACTION_ID = detail.TransactionId,
                        DENOMINATION = detail.Denomination,
                        QUANTITY = detail.Quantity,
                        OPERATION = detail.Operation,
                        CODE = detail.Code,
                        DESCRIPTION = detail.Description
                    };

                    api.CallApi("SaveTransactionDetail", detail);
                }
            }
            catch (Exception ex)
            {
                //logErrors.Add(new LogError
                //{
                //    Fecha = DateTime.Now,
                //    Operacion = "SaveDetailsTransaction",
                //    Error = ex.Message,
                //    Exception = ex
                //});
                //Utilities.CrearLogErrores(logErrors);
            }
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

    public class Mensajes : INotifyPropertyChanged
    {
        private string mensajePrincipal;

        public string MensajePrincipal
        {
            get { return this.mensajePrincipal; }
            set
            {
                if (this.mensajePrincipal != value)
                {
                    this.mensajePrincipal = value;
                    this.NotifyPropertyChanged("MensajePrincipal");
                }
            }
        }

        private string mensajeModal;

        public string MensajeModal
        {
            get { return this.mensajeModal; }
            set
            {
                if (this.mensajeModal != value)
                {
                    this.mensajeModal = value;
                    this.NotifyPropertyChanged("MensajeModal");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

}
