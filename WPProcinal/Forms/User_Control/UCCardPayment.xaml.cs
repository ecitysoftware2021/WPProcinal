using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCCardPayment.xaml
    /// </summary>
    public partial class UCCardPayment : UserControl
    {
        private FrmLoading frmLoading;
        private Utilities utilities;
        private LogErrorGeneral logError;
        private bool stateUpdate;
        private bool payState;
        Utilities objUtil = new Utilities();
        TPVOperation TPV;
        private int num = 1;
        List<Producto> productos;
        #region Propiedades Tarjeta

        private string TramaCancelar;

        private string TramaInicial;

        private string MensajeDebito { get; set; }
        private string Delimitador { get { return Utilities.GetConfiguration("Delimitador"); } }

        private string IdentificadorInicio { get { return Utilities.GetConfiguration("IdentificadorInicio"); } }

        private string TipoOperacion { get { return Utilities.GetConfiguration("TipoOperacion"); } }

        private string ValorTotal { get; set; }

        private string ValorIVA { get { return "0"; } }

        private string NumeroKiosko { get { return Utilities.GetConfiguration("NumeroKiosko"); } }

        private string NumeroTerminal { get { return Utilities.GetConfiguration("NumeroTerminal"); } }

        private string NumeroTransaccion { get; set; }

        private string ValorPropina { get { return "0"; } }

        private string CodigoUnico { get { return Utilities.GetConfiguration("CodigoUnico"); } }

        private string ValorIAC { get { return "0"; } }

        private string IdentificacionCajero { get { return Utilities.GetConfiguration("IdentificacionCajero"); } }

        string _Franchise;
        string _LastNumbers;
        string _AutorizationCode;
        string _ReceiptNumber;
        string _RRN;

        Opciones FrmOciones;

        Mensajes ModalMensajes;
        #endregion
        public UCCardPayment(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            try
            {


                frmModal Modal = new frmModal(Utilities.GetConfiguration("MensajeDatafono"));
                Modal.ShowDialog();

                lblValorPagar.Content = Utilities.PayVal.ToString("$ #,##0");
                frmLoading = new FrmLoading("Conectándose con el datáfono, espere por favor...");
                frmLoading.Show();

                utilities = new Utilities();
                ModalMensajes = new Mensajes();
                //TODO: descomentar
                //TPV = new TPVOperation();

                Utilities.TypeSeats = Seats;
                Utilities.DipMapCurrent = dipMap;
                Utilities.DipMapCurrent.Total = Convert.ToDouble(Utilities.ValorPagarScore);

                ModalMensajes.MensajePrincipal = "Conectándose con el datáfono...";
                this.DataContext = ModalMensajes;

                TramaInicial = "";


                TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
                TxtDay.Text = dipMap.Day;
                TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
                TxtHour.Text = dipMap.HourFunction;
                TxtSubTitle.Text = dipMap.Language;
                var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
                TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);


                logError = new LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                };

                stateUpdate = true;

                //TODO:descomentar hasta el activar y eliminar el buyticket
                payState = true;

                Utilities.Speack("Estableciendo conexión con el datáfono, espera por favor.");

                //Activar();

                Buytickets();
            }
            catch (Exception ex)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Task.Run(() =>
                //{
                //    if (payState)
                //    {
                //        ValorTotal = Utilities.PayVal.ToString();
                //        NumeroTransaccion = Utilities.IDTransactionDB.ToString();
                //        TramaInicial = string.Concat(IdentificadorInicio, Delimitador,
                //            TipoOperacion, Delimitador,
                //           ValorTotal, Delimitador,
                //            ValorIVA, Delimitador,
                //            NumeroKiosko, Delimitador,
                //            NumeroTerminal, Delimitador,
                //            NumeroTransaccion, Delimitador,
                //            ValorPropina, Delimitador,
                //            CodigoUnico, Delimitador,
                //            ValorIAC, Delimitador,
                //            IdentificacionCajero, "]");

                //        //Creo el LCR de la peticion a partir de la trama de inicialización del datáfono
                //        var LCRPeticion = TPV.CalculateLRC(TramaInicial);
                //        try
                //        {
                //            LogService.SaveRequestResponse(DateTime.Now + " :: Petición al datáfono: ", LCRPeticion);
                //        }
                //        catch { }
                //        //Envío la trama que intentará activar el datáfono
                //        var datos = TPV.EnviarPeticion(LCRPeticion);
                //        TPVOperation.CallBackRespuesta?.Invoke(datos);
                //    }
                //});
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Activa el callback que procesa todas las respuestas del datáfono
        /// </summary>
        private void Activar()
        {
            TPVOperation.CallBackRespuesta = Respuesta =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ProcesarRespuesta(Respuesta);
                });
            };
        }


        /// <summary>
        /// Método encargado de actualizar la transacción a aprobada.
        /// Además inserta la información de la tarjeta
        /// </summary>
        private void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {
                    Task.Run(() =>
                    {
                        Utilities.UpdateTransaction(Utilities.PayVal, (int)ETransactionState.Aproved, 0);
                    });
                    utilities.SaveCardInformation(new RequestCardInformation
                    {
                        Autorization_code = _AutorizationCode,
                        Franchise = _Franchise,
                        Last_number = _LastNumbers,
                        Receipt_number = _ReceiptNumber,
                        RRN = _RRN,
                        Transaction_id = Utilities.IDTransactionDB,
                        Quotas = TPVOperation.Quotas,
                        Card_Type = TPVOperation.Quotas != 0 ? 2 : 1
                    });
                }
            }
            catch (Exception ex)
            {
                stateUpdate = false;
            }
        }

        /// <summary>
        /// Finaliza el proceso de pago, imprime las boletas y va al formulario final
        /// </summary>
        /// <param name="task"></param>
        private async void SavePay(bool task)
        {
            try
            {
                num = 2;
                if (!task)
                {
                    try
                    {
                        WCFServices41.PostDesAssingreserva(Utilities.TypeSeats, Utilities.DipMapCurrent);
                    }
                    catch { }

                    try
                    {

                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            frmModal modal = new frmModal("No se pudo realizar la compra, por favor contacta a un administrador para anular el pago.");
                            modal.ShowDialog();

                        });
                        GC.Collect();
                    }
                    catch { }

                    Utilities.UpdateTransaction(0, 3, 0);
                    Utilities.GoToInicial();

                }
                else
                {
                    try
                    {
                        if (Utilities.dataUser.Tarjeta != null)
                        {
                            Utilities.dataUser.Puntos = Convert.ToDouble(Math.Floor(Utilities.PayVal / 1000)) + Utilities.dataUser.Puntos;
                        }
                    }
                    catch
                    {
                    }

                    objUtil.PrintTicket("Aprobada", Utilities.TypeSeats, Utilities.DipMapCurrent);
                    ApproveTrans();

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.Loading(frmLoading, false, this);
                    });

                    if (Utilities.dataUser.Tarjeta != null)
                    {
                        Switcher.Navigate(new UCPoints());
                    }
                    else
                    {
                        Switcher.Navigate(new UCFinalTransaction());
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SavePay en frmPayCine", EError.Aplication, ELevelError.Medium);
                Utilities.GoToInicial();
            }
        }

        /// <summary>
        /// Cancelar el pago
        /// </summary>
        private void Cancelled()
        {
            try
            {
                try
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            WCFServices41.PostDesAssingreserva(Utilities.TypeSeats, Utilities.DipMapCurrent);
                        });
                    });
                }
                catch (Exception ex)
                {
                    LogService.CreateLogsError(
                          string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                          ex.InnerException, "---------- Trace: ", ex.StackTrace), "Cancelled FrmCardPayment");
                }
                UnlockTPV();
                Dispatcher.Invoke(() =>
                {
                    Utilities.Loading(frmLoading, false, this);
                    frmModal modal = new frmModal("Usuario su pago fue cancelado.");
                    modal.ShowDialog();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Cancelled en FrmCardPayment", EError.Aplication, ELevelError.Medium);
            }
            Utilities.Loading(frmLoading, false, this);
            Utilities.GoToInicial();
        }
        private void CancelWithoutTPV()
        {
            try
            {
                try
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            WCFServices41.PostDesAssingreserva(Utilities.TypeSeats, Utilities.DipMapCurrent);
                        });
                    });
                }
                catch (Exception ex)
                {
                    LogService.CreateLogsError(
                          string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                          ex.InnerException, "---------- Trace: ", ex.StackTrace), "CancelWithoutTPV FrmCardPayment");
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "CancelWithoutTPV en FrmCardPayment", EError.Aplication, ELevelError.Medium);
            }
            Utilities.GoToInicial();
        }

        /// <summary>
        /// Finalizar el pago ante score
        /// </summary>
        private void Buytickets()
        {
            try
            {
                if (Utilities.GetConfiguration("Ambiente").Equals("Prd"))
                {
                    List<UbicacioneSCOINT> ubicaciones = new List<UbicacioneSCOINT>();
                    foreach (var item in Utilities.TypeSeats)
                    {
                        ubicaciones.Add(new UbicacioneSCOINT
                        {
                            Fila = item.RelativeRow,
                            Columna = item.RelativeColumn,
                            ColRelativa = int.Parse(item.Number),
                            FilRelativa = item.Letter,
                            Tarifa = int.Parse(item.CodTarifa.ToString())
                        });
                    }

                    productos = new List<Producto>();

                    foreach (var item in Utilities._Combos)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {

                            var combo = Utilities._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                            if (combo.Receta != null)
                            {
                                foreach (var receta in combo.Receta)
                                {
                                    if (receta.RecetaReceta != null)
                                    {
                                        receta.RecetaReceta = receta.RecetaReceta.Take(int.Parse(receta.Cantidad.ToString())).ToList();
                                    }
                                }
                            }
                            combo.Precio = Utilities.dataUser.Tarjeta != null ? 2 : 1;
                            productos.Add(combo);
                        }
                    }


                    string year = Utilities.DipMapCurrent.Date.Substring(0, 4);
                    string mount = Utilities.DipMapCurrent.Date.Substring(4, 2);
                    string day = Utilities.DipMapCurrent.Date.Substring(6, 2);

                    var dataClient = GetDataClient();


                    var response41 = WCFServices41.PostBuy(new SCOINT
                    {
                        Accion = "V",
                        Apellido = dataClient.Apellido,
                        ClienteFrecuente = long.Parse(dataClient.Tarjeta),
                        CorreoCliente = dataClient.Login,
                        Cortesia = string.Empty,
                        Direccion = dataClient.Direccion,
                        DocIdentidad = long.Parse(dataClient.Documento),
                        Factura = Utilities.DipMapCurrent.Secuence,
                        FechaFun = string.Concat(year, "-", mount, "-", day),
                        Funcion = Utilities.DipMapCurrent.IDFuncion,
                        InicioFun = Utilities.DipMapCurrent.HourFormat,
                        Nombre = dataClient.Nombre,
                        PagoCredito = Utilities.MedioPago == 1 ? 0 : int.Parse(Utilities.ValorPagarScore.ToString()),
                        PagoEfectivo = Utilities.MedioPago == 1 ? int.Parse(Utilities.ValorPagarScore.ToString()) : 0,
                        PagoInterno = 0,
                        Pelicula = Utilities.DipMapCurrent.MovieId,
                        Productos = productos,
                        PuntoVenta = Utilities.DipMapCurrent.PointOfSale,
                        Sala = Utilities.DipMapCurrent.RoomId,
                        teatro = Utilities.DipMapCurrent.CinemaId,
                        Telefono = long.Parse(dataClient.Telefono),
                        CodMedioPago = 6,
                        tercero = 1,
                        TipoBono = 0,
                        TotalVenta = int.Parse(Utilities.ValorPagarScore.ToString()),
                        Ubicaciones = ubicaciones
                    });
                    foreach (var item in response41)
                    {
                        if (item.Respuesta != null)
                        {
                            if (item.Respuesta.Contains("exitoso"))
                            {
                                payState = true;
                                break;
                            }
                            else
                            {
                                payState = false;
                            }
                        }
                        else
                        {
                            payState = false;
                        }

                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            WCFServices41.PostDesAssingreserva(Utilities.TypeSeats, Utilities.DipMapCurrent);
                        });
                    });
                }
                SavePay(payState);
            }
            catch (Exception ex)
            {
                payState = false;
                SavePay(payState);
                AdminPaypad.SaveErrorControl(ex.Message, "BuyTicket en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        SCOLOGResponse GetDataClient()
        {
            if (Utilities.dataUser.Tarjeta != null)
            {
                return new SCOLOGResponse
                {
                    Apellido = Utilities.dataUser.Apellido,
                    Tarjeta = Utilities.dataUser.Tarjeta,
                    Login = Utilities.dataUser.Login,
                    Direccion = Utilities.dataUser.Direccion,
                    Documento = Utilities.dataUser.Documento,
                    Nombre = Utilities.dataUser.Nombre,
                    Telefono = Utilities.dataUser.Telefono
                };
            }
            else
            {
                return new SCOLOGResponse
                {
                    Apellido = "Ecity",
                    Tarjeta = "0",
                    Login = "prueba@prueba.com",
                    Direccion = "Cra 63A # 34-70",
                    Documento = "811040812",
                    Nombre = "Kiosko",
                    Telefono = "5803033"
                };
            }
        }


        #region TransactionalMethods

        /// <summary>
        /// Procesa todas las respuestas del datáfono
        /// </summary>
        /// <param name="responseTPV"></param>
        private void ProcesarRespuesta(string responseTPV)
        {
            try
            {
                try
                {
                    LogService.SaveRequestResponse(DateTime.Now + " :: Respuesta del datáfono: ", responseTPV);
                }
                catch { }
                /**
                 * Todas las respuestas correctas tienen mas de 4 caracteres
                 * **/
                if (responseTPV.Length < 4)
                {
                    frmLoading.Close();

                    SetMessageAndPutVisibility("Datáfono sin conexión, intente de nuevo.");
                }
                else
                {
                    /**
                     * Tomo los datos que esten dentro de los corchetes
                     * **/
                    var dataSubString = responseTPV.Substring(responseTPV.IndexOf("[") + 1).Split(']')[0];

                    //Divido la respuesta para tomar la trama de la información
                    var dataResponse = dataSubString.Split(',');

                    //Una p en el primer campo significa acción que debe realizar la pantalla
                    if (dataResponse[0].Equals("P"))
                    {
                        //Valido que la respuesta no corresponda a un error
                        if (dataResponse.Length == 4 && !dataResponse[2].ToLower().Equals("error"))
                        {
                            /**
                             * Si la trama contiene 'pin' se trata de la solicitud de la clave
                             * De lo contrario se trata de informacion seleccionable en pantalla
                             */
                            if (dataResponse[3].ToLower().Contains("pin"))
                            {
                                Utilities.Speack("Digita la clave en el datáfono y presiona el boton verde");
                                MensajeDebito = "Digita la clave en el datáfono y presiona el botón verde";
                                ProcessDebitOperation(MensajeDebito);
                            }
                            else
                            {
                                ProccessPositiveResponse(dataResponse);
                            }
                        }
                        else if (dataResponse[2].ToLower().Equals("error"))
                        {
                            //Procesa  todas las tramas con error del datáfono
                            if (dataResponse[3].ToLower().Contains("pin"))
                            {
                                Utilities.Speack("Clave incorrecta, intenta nuevamente y presiona el boton verde.");

                                MensajeDebito = "Pin incorrecto, intente nuevamente y presiona el botón verde.";
                                ProcessDebitOperation(MensajeDebito);
                            }
                            else
                            {
                                ProcesarFinalError(dataResponse[3]);
                            }
                        }
                        else
                        {
                            //Procesa todas las tramas operacionales del datafono
                            ProcessOperation(dataResponse);
                        }
                    }
                    else if (dataResponse[0].Equals("F"))
                    {
                        /**
                         * Si la respuesta contiene una F se trata del final de una transacción
                         */
                        ProcesarFinal(dataResponse);
                    }
                    else
                    {
                        frmLoading.Close();
                        this.IsEnabled = true;
                        SetMessageAndPutVisibility("Datáfono sin conexión, intente de nuevo.");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Procesa todas las tramas transaccionales de tarjeta crédito
        /// </summary>
        /// <param name="response"></param>
        void ProcessOperation(string[] response)
        {
            try
            {
                var dataTransaction = response[3].Split(';');
                //Valido si la respuesta concuerda con una respuesta válida para el sistema
                if (response.Length == 4)
                {
                    this.IsEnabled = true;
                    ProccessPositiveResponse(dataTransaction);
                }
                else if (response.Length > 4)// Si la data supera los 4 espacios se trata de un pago con tarjeta de crétido
                {
                    //Armo la trama necesaria para las tarjetas de crédito
                    string Trama = string.Concat("R,", response[1], ",1,");
                    //Si la trama respondida por el datáfono contiene Ult. se trata de la peticion de los 4 últimos dígitos de la tarjeta al usuario
                    if (response[3].Contains("Ult."))
                    {
                        Utilities.Speack("Escribe los ultimos cuatro digitos de tu tarjeta.");

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            this.Opacity = 0.3;
                        });
                        //Se envía un maxlenght para el campo de ingreso de los 4 números, la trama y si es crédito
                        DataCardTransaction dataCard = new DataCardTransaction
                        {
                            imagen = string.Empty,
                            isCredit = true,
                            maxlen = 4,
                            mensaje = "Cuatro últimos dígitos de la tarjeta",
                            minlen = 4,
                            peticion = Trama,
                            visible = "Visible"
                        };
                        FrmOciones = new Opciones(dataCard);
                        FrmOciones.ShowDialog();
                    }
                    else if (response[3].Contains("Cuotas"))//Si contiene Cuotas entonces se trata de la solicitud del número de cueotas para la compra al usuario
                    {
                        Utilities.Speack("Escribe el numero de cuotas para el pago.");

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            this.Opacity = 0.3;
                        });
                        DataCardTransaction dataCard = new DataCardTransaction
                        {
                            imagen = string.Empty,
                            isCredit = true,
                            maxlen = 2,
                            mensaje = "¿Número de cuotas?",
                            minlen = 1,
                            peticion = Trama,
                            visible = "Visible"
                        };
                        FrmOciones = new Opciones(dataCard);
                        FrmOciones.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Solicita la clave en el datafono
        /// </summary>
        private void ProcessDebitOperation(string message)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
            });

            lvOpciones.Visibility = Visibility.Hidden;
            DataCardTransaction dataCard = new DataCardTransaction
            {
                imagen = "/Images/Gif/clave.Gif",
                isCredit = false,
                maxlen = 1,
                mensaje = message,
                minlen = 1,
                peticion = null,
                visible = "Visible"
            };

            FrmOciones = new Opciones(dataCard);
            FrmOciones.ShowDialog();
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
            });
        }

        /// <summary>
        /// Procesa la respuesta transaccional positiva
        /// Todas las respuestas del datafono donde se debe habilitar la selección en pantalla
        /// </summary>
        /// <param name="positiveResponse"></param>
        private void ProccessPositiveResponse(string[] positiveResponse)
        {
            try
            {
                //Separo y obtengo las opciones transaccionales que se le presentarán al usuario
                var opcionesTransaccionales = positiveResponse[3].Split(';');
                if (opcionesTransaccionales.Length > 1)
                {
                    lvOpciones.Visibility = Visibility.Visible;
                    frmLoading.Close();
                    //Asigno el título de la operación actual para el usuario
                    ModalMensajes.MensajePrincipal = positiveResponse[2];

                    //En esta lista se almanecarán las opciones que se le presentaran en la vista al usuario
                    List<FormaPago> formas = new List<FormaPago>();
                    //Este índice le da a cada opción su valor segun la trama del datáfono
                    int indiceForma = 1;
                    Utilities.Speack("Selecciona una opcion para continuar.");

                    foreach (var item in opcionesTransaccionales)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (!item.ToLower().Contains("pago movil")
                                && !item.ToLower().Contains("nfc")
                                && !item.ToLower().Contains("qr"))
                            {
                                formas.Add(new FormaPago
                                {
                                    Forma = item,
                                    Imagen = string.Concat("/Images/Buttons/", item, ".png"),
                                    Trama = string.Concat("R,", positiveResponse[1], ",", indiceForma, "]"),
                                });
                            }
                            //else if (item.ToLower().Equals("pago movil"))
                            //{
                            //    formas.Add(new FormaPago
                            //    {
                            //        Forma = item,
                            //        Imagen = string.Concat("/Images/NewDesing/Buttons/", item, ".png"),
                            //        Trama = string.Concat("R,", positiveResponse[1], ",9]"),
                            //    });
                            //}
                            //else if (item.ToLower().Equals("nfc"))
                            //{
                            //    formas.Add(new FormaPago
                            //    {
                            //        Forma = item,
                            //        Imagen = string.Concat("/Images/NewDesing/Buttons/", item, ".png"),
                            //        Trama = string.Concat("R,", positiveResponse[1], ",1]"),
                            //    });
                            //}
                            //else
                            //{
                            //    formas.Add(new FormaPago
                            //    {
                            //        Forma = item,
                            //        Imagen = string.Concat("/Images/NewDesing/Buttons/", item, ".png"),
                            //        Trama = string.Concat("R,", positiveResponse[1], ",", indiceForma, "]"),
                            //    });
                            //}
                        }
                        indiceForma++;
                    }

                    TramaCancelar = string.Concat("R,", positiveResponse[1], ",0]");

                    //Se le asigna el modelo a la vista transaccional
                    this.IsEnabled = true;
                    lvOpciones.DataContext = formas;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Procesa la respuesta final del datáfono a la pantalla
        /// </summary>
        /// <param name="response"></param>
        void ProcesarFinal(string[] response)
        {
            try
            {

                this.IsEnabled = true;
                //Transacción aprobada es igual a 00
                if (response[2].Equals("00"))
                {
                    _Franchise = response[13];
                    _LastNumbers = response[16];
                    _AutorizationCode = response[3];
                    _ReceiptNumber = response[7];
                    _RRN = response[8];
                    Buytickets();
                }
                else
                {
                    frmLoading.Close();
                    //Error de tarjeta
                    if (response[2].Equals("02"))
                    {
                        SetMessageAndPutVisibility("Transacción rechazada.");
                    }
                    else if (response[2].Equals("05"))//Error de conexión a puerto (Fisico)
                    {
                        SetMessageAndPutVisibility("Datáfono no disponible.");
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        void ProcesarFinalError(string message)
        {
            UnlockTPV();
            frmLoading.Close();
            SetMessageAndPutVisibility(message);
        }

        #endregion

        #region Events

        /// <summary>
        /// Método para cambiar mensaje y ocultar lista de opciones
        /// </summary>
        /// <param name="message"></param>
        void SetMessageAndPutVisibility(string message)
        {
            try
            {
                GC.Collect();
                ModalMensajes.MensajePrincipal = message;
                lvOpciones.Visibility = Visibility.Hidden;
                frmLoading = new FrmLoading("Cancelando transacción, espere por favor...");
                frmModal modal = new frmModal(GetMessageError(message));
                modal.ShowDialog();
                RetryPayment();
                GC.Collect();
            }
            catch (Exception ex)
            {

            }
        }

        private void UnlockTPV()
        {
            Task.Run(() =>
            {
                TPV.EnviarPeticion("[R,61,0]38");
            });
        }

        #endregion

        /// <summary>
        /// Evento de lo que selecciona el usuario en pantalla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            OptionsSelect(sender);
        }

        private void OptionsSelect(object sender)
        {
            try
            {
                this.IsEnabled = false;
                //Optengo la data contenida en el elemento seleccionado
                var data = (sender as ListBoxItem).DataContext;

                //Si se trada de una data que setee yo entonces procedo a recorrerla
                if (data is FormaPago)
                {
                    var datos = data as FormaPago;
                    var LRCPeticion = TPV.CalculateLRC(datos.Trama);

                    DataCardTransaction dataCard = new DataCardTransaction
                    {
                        isCredit = false,
                        maxlen = 1,
                        minlen = 1,
                        visible = "Visible"
                    };
                    //Swicheo todas las opciones presentadas por el datáfono para pintarle al usuario las guías correspondientes
                    switch (datos.Forma)
                    {
                        case "DESLIZAR":
                            OptionSelected(datos, LRCPeticion, dataCard, "Desliza tu tarjeta en el datáfono");
                            break;
                        case "INSERTAR":
                            OptionSelected(datos, LRCPeticion, dataCard, "Inserta tu tarjeta en el datáfono");

                            break;
                        case "ACERCAR":
                            OptionSelected(datos, LRCPeticion, dataCard, "Acerca tu tarjeta al datáfono");

                            break;
                        //case "QR":
                        //    //OptionSelected(datos, LRCPeticion, dataCard, "Lee el QR en el datáfono");

                        //    break;
                        //case "PAGO MOVIL":
                        //    //OptionSelected(datos, LRCPeticion, dataCard, "Acerca el teléfono al datáfono");

                        //    break;
                        //case "NFC":
                        //    //opciones = new Opciones("Acerca el dispositivo NFC al datáfono", peticion: LRCPeticion);
                        //    //opciones.ShowDialog();
                        //    break;
                        case "AHORROS":
                            dataCard.imagen = string.Empty;
                            ActionTPV(LRCPeticion, dataCard, MensajeDebito, "Hidden");
                            break;
                        case "CORRIENTE":
                            //dataCard.imagen = "/Images/NewDesing/Gif/clave.Gif";
                            ActionTPV(LRCPeticion, dataCard, MensajeDebito, "Hidden");

                            break;
                        case "CREDITO":
                            //dataCard.imagen = "/Images/NewDesing/Gif/clave.Gif";
                            ActionTPV(LRCPeticion, dataCard, "Cuatro últimos dígitos de la tarjeta", "Visible");

                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ActionTPV(string LRCPeticion, DataCardTransaction dataCard, string message, string visible)
        {
            dataCard.mensaje = message;
            dataCard.peticion = LRCPeticion;
            dataCard.imagen = string.Empty;
            dataCard.visible = visible;
            FrmOciones = new Opciones(dataCard);
            FrmOciones.ShowDialog();
        }

        private void OptionSelected(FormaPago datos, string LRCPeticion, DataCardTransaction dataCard, string message)
        {
            dataCard.mensaje = message;
            dataCard.peticion = LRCPeticion;
            dataCard.imagen = string.Concat("/Images/Gif/", datos.Forma, ".Gif");
            this.Opacity = 0.3;
            FrmOciones = new Opciones(dataCard);
            FrmOciones.ShowDialog();
            this.Opacity = 1;
        }

        private void RetryPayment()
        {

            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(Utilities.TypeSeats, Utilities.DipMapCurrent);
            _frmConfirmationModal.ShowDialog();
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                if (Utilities.MedioPago == 1)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCPayCine(Utilities.TypeSeats, Utilities.DipMapCurrent));
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCardPayment(Utilities.TypeSeats, Utilities.DipMapCurrent));
                    });
                }
            }
            else
            {
                CancelWithoutTPV();
            }
        }

        private string GetMessageError(string error)
        {
            if (error.ToLower().Contains("digitos"))
            {
                error = "Señor usuario, los 4 últimos dígitos de su tarjeta no coinciden con los ingresados, por favor intente de nuevo.";
            }
            else if (error.ToLower().Contains("comunicacion"))
            {
                error = "Señor usuario, No hay comunicación con el datáfono.";
            }
            else if (error.ToLower().Contains("declinada"))
            {
                error = "Señor usuario, su tarjeta fué declinada, intente con otra tarjeta.";
            }
            else if (error.ToLower().Contains("soportada"))
            {
                error = "Señor usuario, transacción no soportada, intente de otro modo.";
            }
            else if (error.ToLower().Contains("invalida"))
            {
                error = "Señor usuario, transacción inválida, intente nuevamente.";
            }
            else if (error.ToLower().Contains("rechazada"))
            {
                error = "Señor usuario, transacción rechazada, intente nuevamente.";
            }
            else if (error.ToLower().Contains("host"))
            {
                error = "Señor usuario, No hay comunicación con el datáfono.";
            }
            else if (error.ToLower().Contains("PIN Incorrecto"))
            {
                error = "El pin es incorrecto, intente nuevamente por favor.";
            }
            return error;
        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                frmLoading = new FrmLoading("Cancelando la transacción, espere por favor...");
                Utilities.Loading(frmLoading, true, this);
                Task.Run(() =>
                {
                    Utilities.UpdateTransaction(0, (int)ETransactionState.Canceled, 0);
                    logError.Description = "\nSe cancelo una transaccion";
                    logError.State = "Cancelada";
                    Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");
                });

                Cancelled();
            }
            catch (Exception ex)
            {
            }
        }


    }
    public class FormaPago
    {
        public string Imagen { get; set; }
        public string Forma { get; set; }
        public string Trama { get; set; }
    }
}
