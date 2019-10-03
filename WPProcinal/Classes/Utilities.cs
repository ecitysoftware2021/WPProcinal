﻿using CEntidades;
using Grabador.Transaccion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPProcinal.ADO;
using WPProcinal.DataModel;
using WPProcinal.Forms;
using WPProcinal.Forms.User_Control;
using WPProcinal.Models;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Classes
{
    public class Utilities
    {

        public static string Duration = GetConfiguration("Duration");

        public static List<RESPONSEERROR> ResponseError { get; set; }

        public static TimeSpan time;

        public static DispatcherTimer timer;

        public static List<Pelicula> Movies = new List<Pelicula>();

        public static bool LossConnection { get; set; }
        public static string CinemaId { get; set; }

        public static DateTime FechaSeleccionada = DateTime.Today;

        public static decimal ValorPagarScore { get; set; }

        public static List<CLSDatos> LDatos = new List<CLSDatos>();

        public static List<SP_GET_INVOICE_DATA_Result> DashboardPrint;

        public static string Secuencia { get; set; }

        public static int controlStop = 0;

        private LogErrorGeneral logError;

        public static DataPaypad dataPaypad = new DataPaypad();
        public static int MedioPago { get; set; }
        public static decimal ScorePayValue { get; set; }

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

        public static int CantidadTransacciones = 0;

        public static string MovieFormat { get; set; }
        public static string TipoSala { get; set; }

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

        public static ControlPeripherals control;
        public static ControlPeripheralsNotArduino PeripheralsNotArduino;
        public static string TOKEN { get; set; }
        public static int Session { get; set; }
        public static int IDTransactionDB { get; set; }
        public static int CorrespondentId { get; set; }

        public static decimal PayVal { get; set; }

        public static decimal EnterTotal;
        public static long ValueDelivery { get; set; }
        public static decimal DispenserVal { get; set; }
        public bool StatePrint { get; private set; }

        public static bool IsRestart = false;

        public Utilities(int i)
        {
            try
            {
                control = new ControlPeripherals();
                PeripheralsNotArduino = new ControlPeripheralsNotArduino();
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
                logError = new LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                };
            }
            catch { }
        }


        /// <summary>
        /// Se usa para ocultar o mostrar la modal de carga
        /// </summary>
        /// <param name="window">objeto de la clase FrmLoading  </param>
        /// <param name="state">para saber si se oculta o se muestra true:muestra, false: oculta</param>
        public static void Loading(Window window, bool state, UserControl w)
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

        public static decimal RoundValue(decimal valor)
        {
            decimal roundVal = (Math.Ceiling(valor / 100)) * 100;

            return roundVal;
        }

        /// <summary>
        /// Método que me redirecciona a la ventana de inicio
        /// </summary>
        public static void GoToInicial()
        {

            try
            {
                CLSGrabador grabador = new CLSGrabador();
                grabador.FinalizarGrabacion();
            }
            catch { }
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Switcher.Navigate(new UCCinema());
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "Error en GoToInicial",
                    EError.Aplication,
                    ELevelError.Medium);
                }
                catch { }
                RestartApp();
            }
        }


        /// <summary>
        /// Método usado para regresar a la pantalla principal
        /// </summary>
        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), "WPProcinal.exe");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
            }
            catch { }
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
            if (File.Exists(NameFile))
            {
                File.Delete(NameFile);
            }

            var file = File.CreateText(NameFile);
            file.Close();

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

        public void PrintTicket(string Estado, List<TypeSeat> Seats, DipMap dipMap)
        {
            try
            {
                int i = 0;
                foreach (var seat in Seats)
                {
                    if (seat.Price != 0)
                    {

                        //objPrint.Cinema = GetConfiguration("NameCinema");
                        objPrint.Movie = dipMap.MovieName;
                        objPrint.Time = dipMap.HourFunction;
                        objPrint.Room = dipMap.RoomName;
                        objPrint.Date = dipMap.Day; //Fecha
                        objPrint.Seat = seat.Name;
                        objPrint.FechaPago = DateTime.Now;
                        objPrint.Valor = seat.Price;
                        objPrint.Tramite = "Boleto de Cine";
                        objPrint.Category = dipMap.Category;
                        //objPrint.Consecutivo = DashboardPrint[i].RANGO_ACTUAL.ToString();
                        objPrint.Secuencia = Secuencia;
                        objPrint.Formato = MovieFormat;
                        objPrint.TipoSala = Utilities.TipoSala;
                        i++;
                        objPrint.PrintTickets();
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                            "Error imprimiendo boletas",
                            EError.Device,
                            ELevelError.Strong);
                }
                catch { }
            }
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
            catch
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
                    TOTAL_AMOUNT = Utilities.PayVal,
                    DATE_BEGIN = DateTime.Now,
                    DESCRIPTION = "Se inició la transacción para: " + name,
                    TYPE_TRANSACTION_ID = 14,
                    STATE_TRANSACTION_ID = 1,
                    PAYER_ID = 477,
                    PAYMENT_TYPE_ID = Utilities.MedioPago
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

        public static async void SendMailNotificationError(string message)
        {
            try
            {
                ApiLocal api = new ApiLocal();

                Email mail = new Email
                {
                    Body = message,
                    paypad_id = Utilities.CorrespondentId,
                    Subject = "Error Pay+"
                };

                Task.Run(() =>
                {
                    var response = api.GetResponse(new RequestApi
                    {
                        Data = mail
                    }, "SendEmail");
                });
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Mètodo para actualizar la transacción en base de datos por el estado que coorresponda
        /// </summary>
        /// <param name="Enter">Valor ingresado. Sólo aplica para pago</param>
        /// <param name="state">Estaado por el cual se actualizará</param>
        /// <param name="Return">Valor a devolver, por defecto es 0 ya que hay transacciones donde no hay que devolver</param>
        /// <returns>Retorna un verdadero o un falso dependiendo el resultado del update</returns>
        public async static Task<bool> UpdateTransaction(decimal Enter,
            int state,
            decimal Return = 0)
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
                    TRANSACTION_ID = IDTransactionDB,
                    PAYMENT_TYPE_ID = Utilities.MedioPago
                };




                var response = await api.GetResponse(new RequestApi
                {
                    Data = Transaction
                }, "UpdateTransaction");
                var dataSerialized = JsonConvert.SerializeObject(Transaction);
                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        return true;
                    }
                    else
                    {
                        BackUpEcity(dataSerialized);
                        return false;
                    }
                }
                else
                {
                    BackUpEcity(dataSerialized);
                    return false;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                          "Error Actualizando la transacción",
                          EError.Customer,
                          ELevelError.Mild);
                }
                catch { }
                return false;
            }
            //LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: UpdateTransaction: ", "Salí");

        }

        public static void BackUpEcity(string data)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    LossConnection = true;
                    using (var con = new DB_PayPlus_LocalEntities())
                    {
                        con.BackUpEcity.Add(new DataModel.BackUpEcity
                        {
                            Data = data,
                        });
                        con.SaveChanges();
                    }
                }));
            }
            catch { }
        }

        public async Task<bool> SaveCardInformation(RequestCardInformation request)
        {
            try
            {
                ApiLocal api = new ApiLocal();



                var response = await api.GetResponse(new RequestApi
                {
                    Data = request
                }, "SaveCardInformation");


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

        public void ProccesValue(DataMoneyNotification data)
        {
            try
            {
                if (data.opt == 2)
                {
                    if (data.enterValue >= 1000)
                    {
                        data.code = "AP";
                    }
                    else
                    {
                        data.code = "MA";
                    }
                }
                else
                {
                    if (data.enterValue > 1000)
                    {
                        data.code = "DP";
                    }
                    else
                    {
                        data.code = "MD";
                    }
                }

                InsertLocalDBMoney(new RequestTransactionDetails
                {
                    Code = data.code,
                    Denomination = Convert.ToInt32(data.enterValue),
                    Operation = data.opt,
                    Quantity = data.quantity,
                    TransactionId = data.idTransactionAPi,
                    Date = DateTime.Now
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void ProccesValue(string messaje, int idTransactionAPi)
        {
            try
            {
                InsertLocalDBMoneyDispenser(new RequestTransactionDetails
                {
                    Description = messaje,
                    TransactionId = idTransactionAPi,
                    Date = DateTime.Now
                });
            }
            catch (Exception ex)
            {

            }
        }


        public async static void SaveDetailsTransaction(RequestTransactionDetails detail)
        {
            try
            {
                ApiLocal api = new ApiLocal();
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

                    var result = await api.CallApi("SaveTransactionDetail", detail);
                    int resultDb = 0;
                    try
                    {
                        resultDb = Convert.ToInt32(result);
                    }
                    catch { }
                    if (resultDb != 200 && detail.Description == null)
                    {
                        InsertLocalDBMoney(detail);
                    }
                    else if (resultDb != 200 && detail.Description != null)
                    {
                        InsertLocalDBMoneyDispenser(detail);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void InsertLocalDBMoney(RequestTransactionDetails detail)
        {
            try
            {
                using (var con = new DB_PayPlus_LocalEntities())
                {
                    NotifyMoney notify = new NotifyMoney
                    {
                        CODE = detail.Code,
                        DENOMINATION = detail.Denomination,
                        DESCRIPTION = detail.Description,
                        OPERATION = detail.Operation,
                        QUANTITY = detail.Quantity,
                        TRANSACTION_ID = detail.TransactionId,
                        DATE = detail.Date
                    };
                    con.NotifyMoney.Add(notify);
                    con.SaveChanges();
                }
            }
            catch { }
        }
        private static void InsertLocalDBMoneyDispenser(RequestTransactionDetails detail)
        {
            try
            {
                using (var con = new DB_PayPlus_LocalEntities())
                {
                    BackUpMoney notify = new BackUpMoney
                    {
                        Data = detail.Description,
                        TransactionID = detail.TransactionId,
                        Date = detail.Date
                    };
                    con.BackUpMoney.Add(notify);
                    con.SaveChanges();
                }
            }
            catch { }
        }

    }
    public class DataMoneyNotification
    {
        public decimal enterValue { get; set; }
        public int opt { get; set; }
        public int quantity { get; set; }
        public string code { get; set; }
        public int idTransactionAPi { get; set; }
        public DateTime Date { get; set; }
    }


}
