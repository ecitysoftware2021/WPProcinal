﻿using Grabador.Transaccion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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

        public static string APISCORE = GetConfiguration("ScoreService");
        public static string SCOREKEY = GetConfiguration("ScoreKey");
        public static string UrlImages = GetConfiguration("UrlImages");

        public static List<string> Imagenes = new List<string>();

        /// <summary>
        /// Lista global para registrar los productos que se seleccionen
        /// en la pantalla de confiteria
        /// </summary>
        public static List<Combos> _Combos;
        public static List<ResponseScores> _DataResolution;

        /// <summary>
        /// Lista global para almacenar los productos que devuelva  el servicio SCOPRE
        /// </summary>
        public static List<Producto> _Productos;

        public static List<Pelicula> Movies = new List<Pelicula>();

        public static DataDocument dataDocument;

        public static SCOLOGResponse dataUser;


        public static SpeechSynthesizer speech;

        public static Action<bool> CallBackPublicity;

        public static bool LossConnection { get; set; }
        public static string CinemaId { get; set; }

        public static DateTime FechaSeleccionada = DateTime.Today;

        public static decimal ValorPagarScore { get; set; }
        private static bool ImgValidatorFlag = true;
        public static string Secuencia { get; set; }

        public static string path;

        public static DataPaypad dataPaypad = new DataPaypad();
        public static List<DataImagesScore> BadImages;
        public static int MedioPago { get; set; }
        public static decimal ScorePayValue { get; set; }

        public static string MovieFormat { get; set; }
        public static string TipoSala { get; set; }

        public static string NameFile = string.Format("{0}\\Cinema.txt", Directory.GetCurrentDirectory());

        public static string NamePathLog = GetConfiguration("NamePathLog");

        public static string NameFileLog = string.Format("{0}\\Error-{1}.json", NamePathLog, DateTime.Now.ToString("yyyyMMdd"));

        public static Pelicula Movie;

        public static int CantSeats;

        public static Peliculas Peliculas;

        public static ImageSource ImageSelected;

        internal static void ValidateImages()
        {
            if (ImgValidatorFlag)
            {
                ImgValidatorFlag = false;
                try
                {
                    BadImages = new List<DataImagesScore>();
                    ObservableCollection<MoviesViewModel> images = new ObservableCollection<MoviesViewModel>();
                    foreach (var item in LstMovies)
                    {
                        images.Add(item);
                    }
                    foreach (var item in images)
                    {
                        if (!WCFServices41.StateImage(item.ImageTag))
                        {
                            BadImages.Add(new DataImagesScore
                            {
                                Pelicula = item.Title,
                                Url = item.ImageTag
                            });
                        }
                    }

                    if (BadImages.Count > 0)
                    {
                        SendMailImages();
                    }
                }
                catch (Exception)
                {

                }
                ImgValidatorFlag = true;
            }
        }

        public static void SendMailImages()
        {
            try
            {
                ApiLocal api = new ApiLocal();
                Email mail;
                string data = string.Empty;

                foreach (var item in BadImages)
                {
                    data += $"Película: {item.Pelicula}, Imágen: {item.Url} <br>";
                }

                mail = new Email
                {
                    Body = $"No fué posible descargar las siguientes imágenes:<br> {data} <br>" +
                            $"Por favor revisar el repositorio de imagenes Url: {UrlImages} <br>" +
                            "Nota: revisar que el nombre de la imágen este bien escrito o que la imágen si exista.",
                    Subject = "Alerta Información Pay+",
                    paypad_id = CorrespondentId
                };

                var response = api.GetResponse(mail, "SendEmail");
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendMailErrores(string data)
        {
            try
            {
                ApiLocal api = new ApiLocal();
                Email mail;


                mail = new Email
                {
                    Body = data,
                    Subject = "Alerta Inconsistencia en Pay+",
                    paypad_id = CorrespondentId
                };

                var response = api.GetResponse(mail, "SendEmail");
            }
            catch (Exception ex)
            {
            }
        }

        public static ObservableCollection<MoviesViewModel> LstMovies = new ObservableCollection<MoviesViewModel>();

        Print printCombo = new Print();

        public static List<TypeSeat> TypeSeats = new List<TypeSeat>();

        public static DipMap DipMapCurrent = new DipMap();

        public static ControlPeripherals control;

        public static ControlPeripheralsNotArduino PeripheralsNotArduino;
        public static string TOKEN { get; set; }
        public static int Session { get; set; }
        public static int IDTransactionDB { get; set; }
        public static int CorrespondentId { get; set; }

        public static decimal PayVal { get; set; }

        public static decimal EnterTotal;
        public static long ValueDelivery { get; set; }

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

        public static void CancelAssing(List<TypeSeat> typeSeatsCurrent, DipMap dipMapCurrent)
        {
            bool sendMail = false;
            string message = string.Empty;
            string infoUbicaciones = string.Empty;
            foreach (var typeSeat in typeSeatsCurrent)
            {
                var response = WCFServices41.PostDesAssingreserva(typeSeat, dipMapCurrent);

                if (response != null)
                {
                    if (!response[0].Respuesta.ToLower().Contains("exitoso"))
                    {
                        sendMail = true;
                        message = response[0].Respuesta;
                        foreach (var item in typeSeatsCurrent)
                        {
                            infoUbicaciones += $"Película: {dipMapCurrent.MovieName}, Horario: {dipMapCurrent.HourFunction}, Ubicacion: {item.Name} <br>";
                        }
                    }
                }
                else
                {
                    sendMail = true;
                    foreach (var item in typeSeatsCurrent)
                    {
                        infoUbicaciones += $"Película: {dipMapCurrent.MovieName}, Horario: {dipMapCurrent.HourFunction}, Ubicacion: {item.Name} <br>";
                    }
                }
            }

            if (sendMail)
            {
                Task.Run(() =>
                {
                    Utilities.SendMailErrores($"No se pudo eliminar las preventas de la transacción {IDTransactionDB} para las siguientes ubicaciones:<br>{infoUbicaciones}" +
                        $"<br>Error: {message}");
                });
            }
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
                LogService.SaveRequestResponse("GoToInitial", ex.Message, 2);
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

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }

        #region SPEAK

        public static void Speack(string text)
        {
            try
            {
                if (GetConfiguration("Speack").Equals("1"))
                {
                    if (speech == null)
                    {
                        speech = new SpeechSynthesizer();
                    }
                    speech.SpeakAsyncCancelAll();
                    speech.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Child);
                    speech.SpeakAsync(text);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        public static string GetConfiguration(string key)
        {
            AppSettingsReader reader = new AppSettingsReader();
            return reader.GetValue(key, typeof(String)).ToString();
        }


        public static void SaveFileXML(string content)
        {
            if (!Directory.Exists("Procinal"))
            {
                Directory.CreateDirectory("Procinal");
                Directory.CreateDirectory(Path.Combine("Procinal", "XmlCinema"));
            }
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

        public static string GetFileXML()
        {
            var file = File.ReadAllText(NameFile);
            return file;
        }

        public static string GetFileCombos()
        {
            var file = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Confiteria", "Confiteria.txt"));
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
                        printCombo.Movie = dipMap.MovieName;
                        printCombo.Time = dipMap.HourFunction;
                        printCombo.Hora = dipMap.HourFormat;
                        printCombo.Room = dipMap.RoomName;
                        printCombo.Date = dipMap.Day; //Fecha
                        printCombo.DateFormat = dipMap.Date; //Fecha
                        printCombo.Funcion = dipMap.IDFuncion;
                        printCombo.Seat = seat.Name;
                        printCombo.Fila = seat.RelativeRow;
                        printCombo.Columna = seat.RelativeColumn;
                        printCombo.FechaPago = DateTime.Now;
                        printCombo.Valor = seat.Price;
                        printCombo.Tramite = "Boleto de Cine";
                        printCombo.Category = dipMap.Category;
                        printCombo.Secuencia = Secuencia;
                        printCombo.Formato = MovieFormat;
                        printCombo.TipoSala = TipoSala;
                        printCombo.IDTransaccion = IDTransactionDB.ToString();

                        if (dataUser.Tarjeta != null && dataUser.Puntos > 0)
                        {
                            printCombo.Puntos = dataUser.Puntos.ToString();
                        }
                        else
                        {
                            printCombo.Puntos = "0";
                        }

                        i++;
                        printCombo.PrintTickets();
                    }
                }

                if (_Combos.Count > 0)
                {
                    _DataResolution = WCFServices41.ConsultResolution(new SCORES
                    {
                        Punto = Convert.ToInt32(GetConfiguration("Cinema")),
                        Secuencial = Convert.ToInt32(Secuencia),
                        teatro = DipMapCurrent.CinemaId,
                        tercero = 1
                    });


                    printCombo.ImprimirComprobante(0);

                    var ComboTemporada = _Combos.Where(x => x.Name == GetConfiguration("0NAME")).FirstOrDefault();

                    if (ComboTemporada != null && GetConfiguration("0Cupon").Equals("1"))
                    {
                        printCombo.ImprimirComprobante(1);
                    }
                }

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("Imprimiendo las boletas", ex.Message, 2);
                AdminPaypad.SaveErrorControl(ex.Message,
                        "Error imprimiendo boletas",
                        EError.Device,
                        ELevelError.Strong);
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
                    TYPE_TRANSACTION_ID = (int)ETransactionType.Buy,
                    STATE_TRANSACTION_ID = (int)ETransactionState.Initital,
                    TRANSACTION_REFERENCE = Utilities.Secuencia,
                    PAYER_ID = 0,
                    PAYMENT_TYPE_ID = (int)EPaymentType.Cash
                };

                foreach (var item in Seats)
                {
                    var details = new TRANSACTION_DESCRIPTION
                    {
                        AMOUNT = item.Price,
                        TRANSACTION_ID = transaction.TRANSACTION_ID,
                        TRANSACTION_PRODUCT_ID = (int)ETransactionProducto.Ticket,
                        DESCRIPTION = movie.MovieName + " - " + item.Name,
                        EXTRA_DATA = Utilities.Secuencia
                    };

                    transaction.TRANSACTION_DESCRIPTION.Add(details);
                }

                foreach (var item in _Combos)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var details = new TRANSACTION_DESCRIPTION
                        {
                            AMOUNT = item.Price / item.Quantity,
                            TRANSACTION_ID = IDTransactionDB,
                            TRANSACTION_PRODUCT_ID = (int)ETransactionProducto.Confectionery,
                            DESCRIPTION = item.Name,
                            EXTRA_DATA = Utilities.Secuencia
                        };
                        transaction.TRANSACTION_DESCRIPTION.Add(details);
                    }
                }
                if (dataDocument != null)
                {
                    if (dataDocument.Document != null)
                    {
                        transaction.payer = new PAYER();
                        transaction.payer.IDENTIFICATION = dataDocument.Document;
                        transaction.payer.NAME = dataDocument.FirstName;
                        transaction.payer.LAST_NAME = dataDocument.LastName;
                        transaction.payer.BIRTHDAY = dataDocument.Date;

                        var resultPayer = await api.CallApi("SavePayer", transaction.payer);

                        if (resultPayer != null)
                        {
                            transaction.PAYER_ID = JsonConvert.DeserializeObject<int>(resultPayer.ToString());
                        }
                    }
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

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static DataDocument ProccesDocument(string data)
        {

            DataDocument documentDataReturn = new DataDocument();

            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    data = data.Remove(0, data.IndexOf("PubDSK_1") + 6);

                    string date = string.Empty;
                    string documentData = string.Empty;
                    string gender = string.Empty;
                    string name = string.Empty;
                    string document = string.Empty;

                    if (data.IndexOf("0M") > 0)
                    {
                        gender = "Masculino";
                        date = data.Substring(data.IndexOf("0M") + 2, 8);
                        documentData = data.Substring(0, data.IndexOf("0M"));
                    }
                    else
                    {
                        documentData = data.Substring(0, data.IndexOf("0F"));
                        date = data.Substring(data.IndexOf("0F") + 2, 8);
                        gender = "Femenino";
                    }

                    char[] cedulaNombreChar = documentData.ToCharArray();

                    foreach (var item in cedulaNombreChar)
                    {

                        if (char.IsLetter(item) || char.IsWhiteSpace(item) || item.Equals('\0'))
                        {
                            name += item;
                            name = name.Replace("\0", " ");
                        }
                        else
                        {
                            document += item;
                        }
                    }
                    name = name.TrimStart();
                    name = name.TrimEnd();

                    var nuevaCedula = document.Replace("\0", string.Empty).Replace(" ", string.Empty);
                    document = nuevaCedula.Substring(nuevaCedula.Length - 10, 10);

                    documentDataReturn.Date = date;
                    documentDataReturn.Document = document;
                    documentDataReturn.Gender = gender;
                    var fullName = FormatName(name);
                    documentDataReturn.FirstName = fullName[2];
                    if (fullName.Count() > 3)
                    {
                        documentDataReturn.SecondName = fullName[3];
                    }
                    documentDataReturn.LastName = fullName[0];
                    documentDataReturn.SecondLastName = fullName[1];
                }

                return documentDataReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string[] FormatName(string nameClient)
        {
            try
            {
                string message = string.Empty;
                foreach (var item in nameClient.Split(' '))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (item.Length > 1)
                        {
                            message += string.Concat(item, " ");
                        }
                    }
                }

                return message.Split(' ');
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void LoadData()
        {
            try
            {

                string dataXml = string.Empty;

                var response = WCFServices41.DownloadData();
                if (!response.IsSuccess)
                {

                    return;
                }

                Utilities.SaveFileXML(response.Result.ToString());
                dataXml = response.Result.ToString();

                dataXml = Utilities.GetFileXML();
                Peliculas data = WCFServices41.DeserealizeXML<Peliculas>(dataXml);
                Utilities.Peliculas = data;
                Utilities.AddImageList();
            }
            catch (System.Exception ex)
            {
                //AdminPaypad.SaveErrorControl(ex.Message, "LoadData en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }
        public static void AddImageList()
        {
            try
            {
                foreach (var item in Peliculas.Pelicula)
                {
                    if (WCFServices41.StateImage(item.Data.Imagen))
                    {
                        if (!Directory.Exists("Slider"))
                        {
                            Directory.CreateDirectory("Slider");
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "Slider");
                        using (WebClient client = new WebClient())
                        {
                            var fileName = string.Concat(path, "\\", item.Nombre + ".jpg");
                            if (!File.Exists(fileName))
                            {
                                client.DownloadFile(new Uri(item.Data.Imagen), fileName);
                            }
                        }
                    }

                    Imagenes.Add(item.Data.Imagen);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public static void DeleteCombo(string comboName, decimal comboPrice, Producto dataProduct)
        {
            int cantActual = dataProduct.Value;
            dataProduct.Value = cantActual != 0 ? (cantActual - 1) : 0;
            var existsCombo = Utilities._Combos.Where(cb => cb.Name == comboName).FirstOrDefault();
            if (existsCombo != null)
            {

                existsCombo.Quantity--;
                existsCombo.Price -= comboPrice;
                if (existsCombo.Quantity == 0)
                {
                    Utilities._Combos.Remove(existsCombo);
                }
            }
        }

        public static void AddCombo(Combos data)
        {
            int cantActual = data.dataProduct.Value;
            cantActual++;
            if (cantActual <= 9)
            {
                data.dataProduct.Value = cantActual;
                var existsCombo = Utilities._Combos.Where(cb => cb.Name == data.Name).FirstOrDefault();

                if (existsCombo != null)
                {
                    existsCombo.Quantity++;
                    existsCombo.Price += data.Price;

                }
                else
                {
                    Utilities._Combos.Add(new Combos
                    {
                        Name = data.Name,
                        Quantity = 1,
                        Price = data.Price,
                        Code = data.Code,
                        dataProduct = data.dataProduct,
                        isCombo = data.isCombo
                    });
                }
            }
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
    public class DataImagesScore
    {
        public string Pelicula { get; set; }
        public string Url { get; set; }
    }

}
