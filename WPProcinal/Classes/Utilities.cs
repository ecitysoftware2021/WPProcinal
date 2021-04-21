using Grabador.Transaccion;
using Newtonsoft.Json;
using Peripheral.Control;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPProcinal.Forms;
using WPProcinal.Forms.User_Control;
using WPProcinal.Models;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;
using static WPProcinal.Models.ApiLocal.Uptake;
using SQLite.Connection.Ecity;

namespace WPProcinal.Classes
{
    public class Utilities
    {
        /// <summary>
        /// Objeto global para el control de los periféricos
        /// </summary>
        public static ControlPeripherals control;
        public static ControlPeripheralsUnified controlUnified;
        public Utilities()
        {
            try
            {
                if (string.IsNullOrEmpty(Utilities.dataPaypad.PaypadConfiguration.unifieD_PORT))
                {
                    control = new ControlPeripherals();
                    control.StopAceptance();
                }
                else
                {
                    controlUnified = new ControlPeripheralsUnified();
                    controlUnified.StopAceptance();
                }

                controlScanner = new ControlScanner();
                dataTransaction = new DataTransaction();
            }
            catch (Exception ex)
            {
            }
        }

        public static DataTransaction dataTransaction;

        /// <summary>
        /// Variable para poner obligatoria o no la placa
        /// </summary>
        public static bool PlateObligatory { get; set; }

        /// <summary>
        /// Objeto para el sonido de la app
        /// </summary>
        public static SpeechSynthesizer speech;

        /// <summary>
        /// Variable para reiniciar la app cuando se va el internet o se pierde comunicación con el servidor
        /// </summary>
        public static bool LossConnection { get; set; }

        /// <summary>
        /// Variable para almacenar el tipo de compra, si es solo confiteria o boleta mas confitería
        /// </summary>
        public static ETypeBuy eTypeBuy = new ETypeBuy();

        /// <summary>
        /// Variable que toma el id del cinema para saber que peliculas mostrar por cada Pay+
        /// </summary>
        public static string CinemaId = GetConfiguration("CodCinema");

        /// <summary>
        /// Variable que almacena la ruta en la que se encuentra la publicidad
        /// </summary>
        public static string PublicityPath;

        /// <summary>
        /// Variable que obtiene el estado del Pay+, indicando modo arqueo, falta de efectivo, nuevas versiones, etc...
        /// </summary>
        public static DataPaypad dataPaypad = new DataPaypad();

        /// <summary>
        /// Ruta donde se guarda toda la data del xml dentro de un txt
        /// </summary>
        public static string XMLFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlCinema", "Cinema.txt");

        /// <summary>
        /// Notifica vía mail al cinema, cuando sucede un error de score en la app
        /// </summary>
        /// <param name="data"></param>
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



        public static ControlScanner controlScanner;

        /// <summary>
        /// Token obtenido de la api
        /// </summary>
        public static string TOKEN { get; set; }

        /// <summary>
        /// Sesion obtenida de la api
        /// </summary>
        public static int Session { get; set; }

        /// <summary>
        /// ID de la transaccion creada
        /// </summary>
        public static int IDTransactionDB { get; set; }

        /// <summary>
        /// ID de Pay+
        /// </summary>
        public static int CorrespondentId { get; set; }

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

        /// <summary>
        /// Convierte en mayuscula la primer letra
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(string value)
        {
            try
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// Retorna el poster en formato bitmap
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string filename, bool type)
        {
            if (type)
            {
                return new BitmapImage(new Uri(filename));
            }

            return new BitmapImage(new Uri(string.Concat(filename), UriKind.Relative));
        }

        /// <summary>
        /// Metodo para cancelar las reservas en caso de cancelacion de la transaccion o error
        /// </summary>
        /// <param name="typeSeatsCurrent"></param>
        /// <param name="dipMapCurrent"></param>
        public static void CancelAssing(List<ChairsInformation> typeSeatsCurrent, FunctionInformation dipMapCurrent)
        {
            try
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
            catch (Exception ex)
            {
                //TODO:
            }
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
                    si.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "WPProcinal.exe");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
            }
            catch { }
        }

        /// <summary>
        /// Inicia el proceso de actualización automatica
        /// </summary>
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

        //public static void DoEvents()
        //{
        //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
        //        new Action(delegate { }));
        //}

        /// <summary>
        /// Metodo que invoca el audio
        /// </summary>
        /// <param name="text"></param>
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

        /// <summary>
        /// Metodo que obtiene la información del app config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Metodo que descarga el archivo xml de la programación
        /// </summary>
        /// <param name="content"></param>
        public static void SaveFileXML(string content)
        {
            var pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlCinema");
            if (!Directory.Exists(pat))
            {
                Directory.CreateDirectory(pat);
            }
            if (File.Exists(XMLFile))
            {
                File.Delete(XMLFile);
            }

            var file = File.CreateText(XMLFile);
            file.Close();

            using (StreamWriter sw = File.AppendText(XMLFile))
            {
                sw.WriteLine(content);
            }
        }

        /// <summary>
        /// Método que obtiene el archivo xml de la programacion desde la ruta del Pay+
        /// </summary>
        /// <returns></returns>
        public static string GetFileXML()
        {
            var file = File.ReadAllText(XMLFile);
            return file;
        }

        /// <summary>
        /// Abre la modal generica
        /// </summary>
        /// <param name="message"></param>
        public static void ShowModal(string message)
        {
            frmModal frmModal = new frmModal(message);
            frmModal.ShowDialog();
        }

        /// <summary>
        /// Imprime los tickets
        /// </summary>
        /// <param name="Estado"></param>
        /// <param name="Seats"></param>
        /// <param name="dipMap"></param>
        public static void PrintTicket(string Estado, List<ChairsInformation> Seats, FunctionInformation dipMap)
        {
            try
            {
                Print printCombo = new Print();
                int i = 0;

                printCombo.Placa = dataTransaction.PLACA;
                printCombo.Secuencia = dataTransaction.Secuencia;

                foreach (var seat in Seats)
                {
                    printCombo.Seat.Add(seat.Name);
                }


                printCombo.Movie = dipMap.MovieName.ToLower();
                printCombo.Time = dipMap.HourFunction;
                printCombo.Hora = dipMap.HourFormat;
                printCombo.Room = dipMap.RoomName;
                printCombo.Date = dipMap.Day; //Fecha
                printCombo.DateFormat = dipMap.Date; //Fecha
                printCombo.Funcion = dipMap.IDFuncion;
                //printCombo.Seat = seat.Name;
                //printCombo.Fila = seat.Letter;
                //printCombo.Columna = int.Parse(seat.Number);
                printCombo.FechaPago = DateTime.Now;
                foreach (var item in Seats)
                {
                    printCombo.Valor += item.Price;
                }
                printCombo.Category = dipMap.Category;

                printCombo.Formato = Utilities.dataTransaction.MovieFormat;
                printCombo.TipoSala = Utilities.dataTransaction.TipoSala;
                printCombo.IDTransaccion = IDTransactionDB.ToString();

                if (dataTransaction.dataUser.Tarjeta != null && dataTransaction.dataUser.SaldoFavor.Value > 0)
                {
                    printCombo.Puntos = dataTransaction.dataUser.SaldoFavor.Value.ToString("$ #,##0");
                }
                else
                {
                    printCombo.Puntos = "$ 0.00";
                }

                printCombo.PrintTickets();


                if (DataService41._Combos.Count > 0)
                {
                    printCombo.ImprimirComprobante(0);

                    var ComboTemporada = DataService41._Combos.Where(x => x.Name == GetConfiguration("0NAME")).FirstOrDefault();

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

        /// <summary>
        /// Método encargado de crear la transacciòn en bd y retornar el id de esta misma   
        /// </summary>
        /// <param name="Amount">Cantdiad a pagaar o retirar</param>
        public static async Task<bool> CreateTransaction(string name)
        {
            try
            {
                ApiLocal api = new ApiLocal();

                Transaction transaction = new Transaction
                {
                    TOTAL_AMOUNT = dataTransaction.PayVal,
                    DATE_BEGIN = DateTime.Now,
                    DESCRIPTION = "Se inició la transacción para: " + name,
                    TYPE_TRANSACTION_ID = (int)ETransactionType.Buy,
                    STATE_TRANSACTION_ID = (int)ETransactionState.Initital,
                    TRANSACTION_REFERENCE = dataTransaction.Secuencia,
                    PAYER_ID = 0,
                    PAYMENT_TYPE_ID = (int)EPaymentType.Cash
                };

                foreach (var item in dataTransaction.SelectedTypeSeats)
                {
                    var details = new TRANSACTION_DESCRIPTION
                    {
                        AMOUNT = item.Price,
                        TRANSACTION_ID = transaction.TRANSACTION_ID,
                        TRANSACTION_PRODUCT_ID = (int)ETransactionProducto.Ticket,
                        DESCRIPTION = dataTransaction.DataFunction.MovieName + " - " + item.Name,
                        EXTRA_DATA = dataTransaction.Secuencia
                    };

                    transaction.TRANSACTION_DESCRIPTION.Add(details);
                }

                foreach (var item in DataService41._Combos)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var details = new TRANSACTION_DESCRIPTION
                        {
                            AMOUNT = item.Price / item.Quantity,
                            TRANSACTION_ID = transaction.TRANSACTION_ID,
                            TRANSACTION_PRODUCT_ID = (int)ETransactionProducto.Confectionery,
                            DESCRIPTION = item.Name,
                            EXTRA_DATA = dataTransaction.Secuencia
                        };
                        transaction.TRANSACTION_DESCRIPTION.Add(details);
                    }
                }
                if (dataTransaction.dataDocument != null)
                {
                    if (dataTransaction.dataDocument.Document != null)
                    {
                        transaction.payer = new PAYER();
                        transaction.payer.IDENTIFICATION = dataTransaction.dataDocument.Document;
                        transaction.payer.NAME = dataTransaction.dataDocument.FirstName;
                        transaction.payer.LAST_NAME = dataTransaction.dataDocument.LastName;
                        transaction.payer.BIRTHDAY = dataTransaction.dataDocument.Date;

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
                    PAYMENT_TYPE_ID = (int)dataTransaction.MedioPago
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

        /// <summary>
        /// Guarda en local db la transaccion que no se logra aprobar en el servidor para luego
        /// renotificarla
        /// </summary>
        /// <param name="data"></param>
        public static void BackUpEcity(string data)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    LossConnection = true;
                    SQLConnection.SaveBackTransaction(new TransactionModel
                    {
                        DATA = data,
                        DATE = DateTime.Now.ToString()
                    });
                }));
            }
            catch { }
        }

        /// <summary>
        /// Procesa la data recibida de la cedula y la convierte en informacion legible
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

                    if (data.IndexOf("00M") > 0)
                    {
                        gender = "M";
                        date = data.Substring(data.IndexOf("00M") + 2, 8);
                        documentData = data.Substring(0, data.IndexOf("00M"));
                    }
                    else if (data.IndexOf("00F") > 0)
                    {
                        documentData = data.Substring(0, data.IndexOf("00F"));
                        date = data.Substring(data.IndexOf("00F") + 2, 8);
                        gender = "F";
                    }
                    else if (data.IndexOf("0M") > 0)
                    {
                        gender = "M";
                        date = data.Substring(data.IndexOf("0M") + 2, 8);
                        documentData = data.Substring(0, data.IndexOf("0M"));
                    }
                    else
                    {
                        documentData = data.Substring(0, data.IndexOf("0F"));
                        date = data.Substring(data.IndexOf("0F") + 2, 8);
                        gender = "F";
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

        /// <summary>
        /// deserializa la data del xml al objeto Peliculas
        /// </summary>
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
                DataService41.Peliculas = data;
                AddImageList();
            }
            catch (System.Exception ex)
            {
                //AdminPaypad.SaveErrorControl(ex.Message, "LoadData en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Descarga la publicidad
        /// </summary>
        private static void AddImageList()
        {
            try
            {
                var slides = dataPaypad.PaypadConfiguration.ExtrA_DATA.publicityRoute;
                var sliders = Directory.GetFiles(dataPaypad.PaypadConfiguration.ExtrA_DATA.publicityRoute);
                if (sliders.Length < 1)
                {
                    foreach (var item in DataService41.Peliculas.Pelicula)
                    {
                        if (item.Cinemas != null)
                        {
                            foreach (var peli in item.Cinemas.Cinema)
                            {
                                if (peli.Id == dataPaypad.PaypadConfiguration.ExtrA_DATA.codCinema.ToString())
                                {
                                    if (WCFServices41.StateImage(item.Data.Imagen))
                                    {
                                        PublicityPath = slides;
                                        using (WebClient client = new WebClient())
                                        {
                                            var fileName = Path.Combine(PublicityPath, item.Nombre + ".jpg");
                                            if (!File.Exists(fileName))
                                            {
                                                client.DownloadFile(new Uri(item.Data.Imagen), fileName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Remueve un producto especificado de la lista de productos a comprar
        /// </summary>
        /// <param name="comboName"></param>
        /// <param name="comboPrice"></param>
        /// <param name="dataProduct"></param>
        public static void DeleteCombo(string comboName, decimal comboPrice, Producto dataProduct)
        {
            int cantActual = dataProduct.Value;
            dataProduct.Value = cantActual != 0 ? (cantActual - 1) : 0;
            var existsCombo = DataService41._Combos.Where(cb => cb.Name == comboName).FirstOrDefault();
            if (existsCombo != null)
            {

                existsCombo.Quantity--;
                existsCombo.Price -= comboPrice;
                if (existsCombo.Quantity == 0)
                {
                    DataService41._Combos.Remove(existsCombo);
                }
            }
        }

        /// <summary>
        /// Agrega un producto especificado a la lista de productos a comprar
        /// </summary>
        /// <param name="data"></param>
        public static void AddCombo(Combos data)
        {
            int cantActual = data.dataProduct.Value;
            cantActual++;
            if (cantActual <= 9)
            {
                data.dataProduct.Value = cantActual;
                var existsCombo = DataService41._Combos.Where(cb => cb.Name == data.Name).FirstOrDefault();

                if (existsCombo != null)
                {
                    existsCombo.Quantity++;
                    existsCombo.Price += data.Price;

                }
                else
                {
                    DataService41._Combos.Add(new Combos
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

        public static void ValidateUserBalance()
        {
            decimal valorPagoConSaldoFavor = 0;
            if (dataTransaction.dataUser.SaldoFavor != null)
            {
                if (dataTransaction.dataUser.SaldoFavor.Value > 0)
                {
                    dataTransaction.dataUser.SaldoFavor = (dataTransaction.dataUser.SaldoFavor.Value - (dataTransaction.dataUser.SaldoFavor.Value % 100));
                    frmModal Modal = new frmModal($"Tienes un saldo a favor de {dataTransaction.dataUser.SaldoFavor.Value.ToString("C")} ¿deseas utilizarlo en esta compra?", balance: true);
                    Modal.ShowDialog();
                    if (Modal.DialogResult.HasValue && Modal.DialogResult.Value)
                    {
                        valorPagoConSaldoFavor = dataTransaction.DataFunction.Total - dataTransaction.dataUser.SaldoFavor.Value;
                        if (valorPagoConSaldoFavor <= 0)
                        {
                            dataTransaction.PayVal = 0;
                            dataTransaction.PagoInterno = dataTransaction.DataFunction.Total;
                        }
                        else
                        {
                            dataTransaction.PayVal = dataTransaction.DataFunction.Total - dataTransaction.dataUser.SaldoFavor.Value;
                            dataTransaction.PagoInterno = dataTransaction.dataUser.SaldoFavor.Value;
                        }
                    }
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
