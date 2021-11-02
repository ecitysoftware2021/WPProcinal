﻿using Newtonsoft.Json;
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
using static WPProcinal.Models.ApiLocal.Uptake;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCCardPayment.xaml
    /// </summary>
    public partial class UCCardPayment : UserControl
    {
        private bool stateUpdate;
        private bool payState;
        TPVOperation TPV;
        private int num = 1;
        List<Producto> productos;

        #region Propiedades Tarjeta

        private string TramaCancelar;

        private string TramaInicial;
        private string MensajeDebito { get; set; }
        private string ValorTotal { get; set; }
        private string ValorIVA { get { return "0"; } }
        private string NumeroTransaccion { get; set; }
        private string ValorPropina { get { return "0"; } }
        private string ValorIAC { get { return "0"; } }

        string _Franchise;
        string _LastNumbers;
        string _AutorizationCode;
        string _ReceiptNumber;
        string _RRN;

        Opciones FrmOciones;

        Mensajes ModalMensajes;
        #endregion
        public UCCardPayment()
        {
            InitializeComponent();
            try
            {
                lblValorPagar.Content = Utilities.dataTransaction.PayVal.ToString("$ #,##0");
                if (Utilities.dataTransaction.PayVal > 0)
                {
                    ModalMensajes = new Mensajes();
                    ModalMensajes.MensajePrincipal = "Conectándose con el datáfono...";
                    this.DataContext = ModalMensajes;
                }


                if (Utilities.dataTransaction.SelectedTypeSeats.Count > 0)
                {
                    TramaInicial = "";

                    TxtTitle.Text = Utilities.CapitalizeFirstLetter(Utilities.dataTransaction.DataFunction.MovieName);
                    TxtDay.Text = Utilities.dataTransaction.DataFunction.Day;
                    TxtFormat.Text = string.Format("Formato: {0}", Utilities.dataTransaction.MovieFormat.ToUpper());
                    TxtHour.Text = Utilities.dataTransaction.DataFunction.HourFunction;
                    TxtSubTitle.Text = Utilities.dataTransaction.DataFunction.Language;
                    var time = TimeSpan.FromMinutes(double.Parse(Utilities.dataTransaction.DataFunction.Duration.Split(' ')[0]));
                    TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);
                }

                stateUpdate = true;
                if (Utilities.dataTransaction.PayVal > 0)
                {
                    payState = true;
                    Utilities.Speack("Estableciendo conexión con el datáfono, espera por favor.");
                    TPV = new TPVOperation();
                    Activar();
                    //Buytickets();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>UCCardPayment", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utilities.dataTransaction.PayVal == 0)
                {
                    this.IsEnabled = false;
                    Buytickets();
                }
                else
                {
                    Opacity = 0.3;
                    frmModal Modal = new frmModal(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeDatafono);
                    Modal.ShowDialog();
                    Opacity = 1;
                    FrmLoading frmLoading = new FrmLoading("Conectándose con el datáfono, espere por favor...");
                    Task.Run(() =>
                    {
                        if (payState)
                        {
                            ValorTotal = Utilities.dataTransaction.PayVal.ToString().Split(',')[0];
                            NumeroTransaccion = Utilities.IDTransactionDB.ToString();
                            string Delimitador = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.DataDatafono.Delimitador;
                            TramaInicial = string.Concat(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.DataDatafono.IdentificadorInicio, Delimitador,
                                Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.DataDatafono.TipoOperacion, Delimitador,
                                ValorTotal, Delimitador,
                                ValorIVA, Delimitador,
                                Utilities.CorrespondentId, Delimitador,
                                Utilities.CorrespondentId, Delimitador,
                                NumeroTransaccion, Delimitador,
                                ValorPropina, Delimitador,
                                Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.DataDatafono.CodigoUnico, Delimitador,
                                ValorIAC, Delimitador,
                                Utilities.CorrespondentId, "]");

                            //Creo el LCR de la peticion a partir de la trama de inicialización del datáfono
                            var LCRPeticion = TPV.CalculateLRC(TramaInicial);
                            LogService.SaveRequestResponse("Petición al datáfono", LCRPeticion, 1);
                            //Envío la trama que intentará activar el datáfono
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                frmLoading.Show();
                            });

                            var datos = TPV.EnviarPeticion(LCRPeticion);
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                frmLoading.Close();
                            });

                            TPVOperation.CallBackRespuesta?.Invoke(datos);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>UserControl_Loaded", JsonConvert.SerializeObject(ex), 1);
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
        private async void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {

                    await Utilities.UpdateTransaction(
                         Utilities.dataTransaction.PayVal,
                         (int)ETransactionState.Aproved,
                         new List<DataModel.DenominationMoney>(),
                         0);

                    SaveCardInformation(new RequestCardInformation
                    {
                        AUTORIZATION_CODE = _AutorizationCode,
                        FRANCHISE = _Franchise,
                        CARD_LAST_NUMBER = _LastNumbers,
                        RECEIPT_NUMBER = _ReceiptNumber,
                        RRN = _RRN,
                        TRANSACTION_ID = Utilities.IDTransactionDB,
                        PAY_QUOTAS = TPVOperation.Quotas,
                        CARD_TYPE_ID = TPVOperation.Quotas != 0 ? 2 : 1
                    });
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>ApproveTrans", JsonConvert.SerializeObject(ex), 1);
                stateUpdate = false;
            }
        }

        public async void SaveCardInformation(RequestCardInformation request)
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
                        //TODO: response 200 vacio
                    }

                }

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>SaveCardInformation", JsonConvert.SerializeObject(ex), 1);
            }
        }

        /// <summary>
        /// Finaliza el proceso de pago, imprime las boletas y va al formulario final
        /// </summary>
        /// <param name="task"></param>
        private void SavePay(bool task)
        {
            try
            {
                num = 2;
                if (!task)
                {
                    FrmLoading frmLoading = new FrmLoading("Eliminando preventas...");
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                    frmLoading.Close();

                    Opacity = 0.3;
                    frmModal modal = new frmModal("No se pudo realizar la compra, por favor contacta a un administrador para anular el pago.");
                    modal.ShowDialog();
                    Opacity = 1;
                    Utilities.UpdateTransaction(0, (int)ETransactionState.Canceled, new List<DataModel.DenominationMoney>(), 0);
                    Utilities.GoToInicial();
                }
                else
                {
                    FrmLoading frmLoading = new FrmLoading("Imprimiendo tickets...");
                    frmLoading.Show();
                    Utilities.PrintTicket("Aprobada", Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                    frmLoading.Close();
                    ApproveTrans();

                    Switcher.Navigate(new UCFinalTransaction());
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>SavePay", JsonConvert.SerializeObject(ex), 1);
                Utilities.GoToInicial();
            }
        }

        /// <summary>
        /// Cancelar el pago
        /// </summary>
        private void Cancelled()
        {
            FrmLoading frmLoading = new FrmLoading("Eliminando preventas...");
            try
            {
                frmLoading.Show();
                Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                frmLoading.Close();

                UnlockTPV();
                Dispatcher.Invoke(() =>
                {
                    Opacity = 0.3;
                    frmModal modal = new frmModal("Usuario su pago fue cancelado.");
                    modal.ShowDialog();
                    Opacity = 1;
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>Cancelled", JsonConvert.SerializeObject(ex), 1);
                frmLoading.Close();
            }
            Utilities.GoToInicial();
        }
        private void CancelWithoutTPV()
        {
            FrmLoading frmLoading = new FrmLoading("Eliminando preventas...");

            this.IsEnabled = false;
            try
            {
                frmLoading.Show();
                Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                frmLoading.Close();

                this.IsEnabled = true;

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>CancelWithoutTPV", JsonConvert.SerializeObject(ex), 1);
                frmLoading.Close();
            }
            Utilities.GoToInicial();
        }

        /// <summary>
        /// Finalizar el pago ante score
        /// </summary>
        private void Buytickets()
        {
            FrmLoading frmLoading = new FrmLoading("Procesando la compra...");
            frmLoading.Show();
            try
            {
                if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ModalPlate && Utilities.PlateObligatory)
                {
                    WPlateModal wPlate = new WPlateModal();
                    wPlate.ShowDialog();
                }

                List<UbicacioneSCOINT> ubicaciones = new List<UbicacioneSCOINT>();
                foreach (var item in Utilities.dataTransaction.SelectedTypeSeats)
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

                foreach (var item in DataService41._Combos)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {

                        var combo = DataService41._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
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
                        if (Utilities.dataTransaction.dataUser.Tarjeta != null && Utilities.dataTransaction.PrecioCinefans)
                        {
                            combo.Precio = 2;
                        }
                        else
                        {
                            combo.Precio = 1;
                        }
                        productos.Add(combo);
                    }
                }


                string year = DateTime.Now.Year.ToString();
                string mount = DateTime.Now.Month.ToString();
                string day = DateTime.Now.Day.ToString();
                if (Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema)
                {
                    year = Utilities.dataTransaction.DataFunction.Date.Substring(0, 4);
                    mount = Utilities.dataTransaction.DataFunction.Date.Substring(4, 2);
                    day = Utilities.dataTransaction.DataFunction.Date.Substring(6, 2);
                }

                var dataClient = GetDataClient();
                if (dataClient != null)
                {
                    var response41 = WCFServices41.PostBuy(new SCOINT
                    {
                        Accion = Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema ? "V" : "C",
                        Placa = string.IsNullOrEmpty(Utilities.dataTransaction.PLACA) ? "0" : Utilities.dataTransaction.PLACA,
                        Apellido = string.IsNullOrEmpty(dataClient.Apellido) ? "null" : dataClient.Apellido,
                        //ClienteFrecuente = long.Parse(dataClient.Tarjeta),
                        ClienteFrecuente = !string.IsNullOrEmpty(dataClient.Tarjeta) ? long.Parse(dataClient.Tarjeta) : 0,
                        CorreoCliente = string.IsNullOrEmpty(dataClient.Login) ? "null" : dataClient.Login,
                        Cortesia = string.Empty,
                        Direccion = string.IsNullOrEmpty(dataClient.Direccion) ? "null" : dataClient.Direccion,
                        //DocIdentidad = long.Parse(dataClient.Documento),
                        DocIdentidad = !string.IsNullOrEmpty(dataClient.Documento) ? long.Parse(dataClient.Documento) : 0,
                        Factura = int.Parse(Utilities.dataTransaction.Secuencia),
                        FechaFun = string.Concat(year, "-", mount, "-", day),
                        Funcion = Utilities.dataTransaction.DataFunction.IDFuncion,
                        InicioFun = Utilities.dataTransaction.DataFunction.HourFormat,
                        Nombre = string.IsNullOrEmpty(dataClient.Nombre) ? "null" : dataClient.Nombre,
                        PagoCredito = Utilities.dataTransaction.PayVal,
                        PagoEfectivo = 0,
                        PagoInterno = Utilities.dataTransaction.PagoInterno,
                        Pelicula = Utilities.dataTransaction.DataFunction.MovieId,
                        Productos = productos,
                        PuntoVenta = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                        Sala = Utilities.dataTransaction.DataFunction.RoomId,
                        teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                        //Telefono = long.Parse(dataClient.Telefono),
                        Telefono = !string.IsNullOrEmpty(dataClient.Telefono) ? long.Parse(dataClient.Telefono) : 0,
                        CodMedioPago = Utilities.dataTransaction.PayVal > 0 ? (int)ECodigoMedioPagoScore.Tarjeta_Debito_Credi : (int)ECodigoMedioPagoScore.Todos,
                        tercero = 1,
                        TipoBono = 0,
                        TotalVenta = Utilities.dataTransaction.DataFunction.Total,
                        Ubicaciones = ubicaciones,
                        Membresia = false,
                        Obs1 = string.IsNullOrEmpty(Utilities.dataTransaction.TIPOAUTO) ? "" : Utilities.dataTransaction.TIPOAUTO
                    });

                    frmLoading.Close();

                    if (response41 != null)
                    {
                        foreach (var item in response41)
                        {
                            if (item.Respuesta != null)
                            {
                                if (item.Respuesta.Contains("exitoso"))
                                {
                                    payState = true;
                                    GetInvoice();
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
                }

                SavePay(payState);
            }
            catch (Exception ex)
            {
                frmLoading.Close();
                payState = false;
                SavePay(payState);
                LogService.SaveRequestResponse("UCCardPayment>Buytickets", JsonConvert.SerializeObject(ex), 1);
            }
        }
        private void GetInvoice()
        {
            FrmLoading frmLoading = new FrmLoading("¡Consultando resolución de factura...!");
            try
            {
                frmLoading.Show();
                DataService41._DataResolution = WCFServices41.ConsultResolution(new SCORES
                {
                    Punto = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                    Secuencial = Convert.ToInt32(Utilities.dataTransaction.Secuencia),
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                    tercero = 1
                });
                frmLoading.Close();
            }
            catch (Exception ex)
            {
                frmLoading.Close();
                LogService.SaveRequestResponse("UCCardPayment>GetInvoice", JsonConvert.SerializeObject(ex), 1);
            }
        }
        SCOLOGResponse GetDataClient()
        {
            if (Utilities.dataTransaction.dataUser.Tarjeta != null)
            {
                return new SCOLOGResponse
                {
                    Apellido = Utilities.dataTransaction.dataUser.Apellido,
                    Tarjeta = Utilities.dataTransaction.dataUser.Tarjeta,
                    Login = Utilities.dataTransaction.dataUser.Login,
                    Direccion = Utilities.dataTransaction.dataUser.Direccion,
                    Documento = Utilities.dataTransaction.dataUser.Documento,
                    Nombre = Utilities.dataTransaction.dataUser.Nombre,
                    Telefono = Utilities.dataTransaction.dataUser.Telefono
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
                LogService.SaveRequestResponse("Respuesta del datáfono", responseTPV, 1);

                //Todas las respuestas correctas tienen mas de 4 caracteres

                if (responseTPV.Length < 4)
                {
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
                LogService.SaveRequestResponse("UCCardPayment>ProcessOperation", JsonConvert.SerializeObject(ex), 1);
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
                LogService.SaveRequestResponse("UCCardPayment>ProccessPositiveResponse", JsonConvert.SerializeObject(ex), 1);

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
                    //Error de tarjeta
                    if (response[2].Equals("02"))
                    {
                        SetMessageAndPutVisibility("Transacción rechazada.");
                    }
                    else if (response[2].Equals("05"))//Error de conexión a puerto (Fisico)
                    {
                        SetMessageAndPutVisibility("Datáfono no disponible.");
                        Task.Run(() =>
                        {
                            Utilities.SendMailErrores($"Se perdió la conexión del datáfono en la transacción {Utilities.IDTransactionDB}," +
                                "Por favor contactar con 1Cero1 para la respectiva validación.");
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>ProcesarFinal", JsonConvert.SerializeObject(ex), 1);

            }
        }

        void ProcesarFinalError(string message)
        {
            UnlockTPV();
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
                Opacity = 0.3;
                frmModal modal = new frmModal(GetMessageError(message));
                modal.ShowDialog();
                Opacity = 1;
                RetryPayment();
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCardPayment>SetMessageAndPutVisibility", JsonConvert.SerializeObject(ex), 1);

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
                LogService.SaveRequestResponse("UCCardPayment>OptionsSelect", JsonConvert.SerializeObject(ex), 1);

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
            Opacity = 0.3;
            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal();
            _frmConfirmationModal.ShowDialog();
            Opacity = 1;
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                Utilities.ValidateUserBalance();
                if (Utilities.dataTransaction.MedioPago == EPaymentType.Cash)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCPayCine());
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCardPayment());
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
                Task.Run(() =>
                {
                    Utilities.UpdateTransaction(
                        0,
                        (int)ETransactionState.Canceled,
                        new List<DataModel.DenominationMoney>(),
                        0);
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
