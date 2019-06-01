using CEntidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for FrmCardPayment.xaml
    /// </summary>
    public partial class FrmCardPayment : Window
    {
        private ApiLocal api;
        private PaymentViewModel PaymentViewModel;
        private FrmLoading frmLoading;
        private Utilities utilities;
        private LogErrorGeneral logError;
        private int count;
        private bool stateUpdate;
        private int countError = 0;
        private bool payState;
        private int num = 1;
        Response responseGlobal = new Response();
        Utilities objUtil = new Utilities();

        #region Propiedades Tarjeta

        private string TramaInicial;

        private string Delimitador { get { return ","; } }

        private string IdentificadorInicio { get { return "I"; } }

        private string TipoOperacion { get { return "01"; } }

        private string ValorTotal { get; set; }

        private string ValorIVA { get { return "0"; } }

        private string NumeroKiosko { get { return "T0501"; } }

        private string NumeroTerminal { get { return "000CRE01"; } }

        private string NumeroTransaccion { get; set; }

        private string ValorPropina { get { return "0"; } }

        private string CodigoUnico { get { return "010601557"; } }

        private string ValorIAC { get { return "0"; } }

        private string IdentificacionCajero { get { return "PRU"; } }


        int reintent = 0;

        Opciones opciones;

        Mensajes mensajes;
        #endregion

        public FrmCardPayment(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();

            try
            {
                api = new ApiLocal();
                Utilities.PayVal = Utilities.RoundValue(Utilities.ValorPagar);
                lblValorPagar.Content = Utilities.PayVal.ToString("#,##0");
                frmLoading = new FrmLoading("Cargando...");
                utilities = new Utilities();
                Utilities.TypeSeats = Seats;
                Utilities.DipMapCurrent = dipMap;
                Utilities.DipMapCurrent.Total = Convert.ToDouble(Utilities.PayVal);

                mensajes = new Mensajes();
                this.DataContext = mensajes;

                TramaInicial = "";



                TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
                TxtDay.Text = dipMap.Day;
                TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
                TxtHour.Text = dipMap.HourFunction;
                TxtSubTitle.Text = dipMap.Language;

                logError = new LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                };

                count = 0;
                stateUpdate = true;
                payState = true;
                Activar();
            }
            catch (Exception ex)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    if (payState)
                    {


                        ValorTotal = Utilities.ValorPagar.ToString();
                        NumeroTransaccion = Utilities.IDTransactionDB.ToString();
                        TramaInicial = string.Concat(IdentificadorInicio, Delimitador,
                            TipoOperacion, Delimitador,
                           ValorTotal, Delimitador,
                            ValorIVA, Delimitador,
                            NumeroKiosko, Delimitador,
                            NumeroTerminal, Delimitador,
                            NumeroTransaccion, Delimitador,
                            ValorPropina, Delimitador,
                            CodigoUnico, Delimitador,
                            ValorIAC, Delimitador,
                            IdentificacionCajero, "]");

                        //Creo el LCR de la peticion a partir de la trama de inicialización del datáfono
                        var LCRPeticion = objUtil.CalculateLRC(TramaInicial);
                        //Envío la trama que intentará activar el datáfono
                        var datos = objUtil.EnviarPeticion(LCRPeticion);
                        Utilities.CallBackRespuesta?.Invoke(datos);
                    }
                });

                Task.Run(() =>
                {
                    GetReceipt();
                });
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


            //Activo el callback que procesará todas las respuestas del datáfono
            Utilities.CallBackRespuesta = Respuesta =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ProcesarRespuesta(Respuesta);
                });
            };

        }

        private void btnCancelar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                this.Opacity = 0.3;
                Utilities.Loading(frmLoading, true, this);

                Task.Run(() =>
                {
                    utilities.UpdateTransaction(0, 3, 0).Wait();
                    logError.Description = "\nNo Se cancelo una transaccion";
                    logError.State = "Cancelada";
                    Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");
                });

                Cancelled();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a aprobada, se llama en finalizar pago En caso de fallo se reintenta dos veces más actualizar el estado de la transacción, si el error persiste se guarda en un log local y en el servidor, seguido de esto se continua con la transacción normal
        /// </summary>
        private void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {
                    var state = utilities.UpdateTransaction(0, 2, 0);
                    if (!state.Result)
                    {
                        if (count < 2)
                        {
                            count++;
                            ApproveTrans();
                        }
                        else
                        {
                            logError.Description = "\nNo fue posible actualizar esta transacción a aprobada";
                            logError.State = "Iniciada";
                            Utilities.SaveLogTransactions(logError, "LogTransacciones\\Iniciadas");
                        }
                    }
                    else
                    {
                        logError.Description = "\nTransacción Exitosa";
                        logError.State = "Aprobada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Aprobadas");
                    }
                }
            }
            catch (Exception ex)
            {
                stateUpdate = false;
            }
        }

        private async void SavePay(bool task)
        {
            num = 2;

            try
            {
                if (!task)
                {
                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        PaymentGrid.Opacity = 0.3;
                        Utilities.Loading(frmLoading, false, this);
                        frmModal modal = new frmModal("No se pudo realizar el pago");
                        modal.ShowDialog();

                        Utilities.Loading(frmLoading, true, this);
                    });
                    GC.Collect();

                    Task.Run(() =>
                    {
                        utilities.UpdateTransaction(0, 3, 0).Wait();

                        logError.Description = "\nNo Se cancelo una transaccion";
                        logError.State = "Cancelada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");
                    }).Wait();

                }
                else
                {
                    ApproveTrans();

                    objUtil.ImprimirComprobante("Aprobada", Utilities.Receipt, Utilities.TypeSeats, Utilities.DipMapCurrent);

                    Utilities.control.StopAceptance();

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        frmModal _frmModal = new frmModal(string.Concat("!Muchas gracias por utilizar nuestro servicio.",
                        Environment.NewLine,
                        "Su transacción ha finalizado correctamente!"));
                        _frmModal.ShowDialog();
                        FrmFinalTransaction frmFinal = new FrmFinalTransaction();
                        frmFinal.Show();
                        this.Close();
                        //Utilities.GoToInicial(this);
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Cancelled()
        {
            try
            {
                Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                //order cancelar datafono
                Dispatcher.Invoke(() =>
                {
                    PaymentGrid.Opacity = 0.3;
                    Utilities.Loading(frmLoading, false, this);
                    frmModal modal = new frmModal("Usuario su pago fue cancelado.");
                    modal.ShowDialog();
                });
                GC.Collect();

                Utilities.GoToInicial(this);
            }
            catch (Exception ex)
            {
            }
        }

        private void GetReceipt()
        {
            var response = WCFServices.GetReceiptProcinal(ControlPantalla.IdCorrespo);
            if (response.IsSuccess)
            {
                Utilities.Receipt = JsonConvert.DeserializeObject<Receipt>(response.Result.ToString());
            }
        }

        private void Buytickets()
        {
            if (num == 2)
            {
                return;
            }

            if (countError < 3 && num == 1)
            {
                payState = true;
                var response = WCFServices.PostComprar(Utilities.DipMapCurrent, Utilities.TypeSeats);
                responseGlobal = response;
                if (!response.IsSuccess)
                {
                    Utilities.SaveLogError(new LogError
                    {
                        Message = response.Message,
                        Method = "WCFServices.PostComprar"
                    });

                    countError++;
                    Buytickets();
                }

                var transaccionCompra = WCFServices.DeserealizeXML<TransaccionCompra>(response.Result.ToString());
                if (transaccionCompra.Respuesta == "Fallida")
                {
                    payState = false;
                    Utilities.SaveLogError(new LogError
                    {
                        Message = transaccionCompra.Respuesta,
                        Method = "WCFServices.PostComprar.Fallida"
                    });
                    countError++;
                    Buytickets();
                }
                else
                {
                    var responseDB = DBProcinalController.EditPaySeat(Utilities.DipMapCurrent.DipMapId);
                    if (!response.IsSuccess)
                    {
                        Utilities.SaveLogError(new LogError
                        {
                            Message = responseDB.Message,
                            Method = "DBProcinalController.EditPaySeat"
                        });
                    }
                }

            }

            if (num == 2)
            {
                return;
            }

            if (num == 1)
            {
                SavePay(payState);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        #region TransactionalMethods

        /// <summary>
        /// Procesa todas las respuestas del datáfono
        /// </summary>
        /// <param name="respuesta"></param>
        private void ProcesarRespuesta(string respuesta)
        {
            try
            {
                //Divido la respuesta para tomar la trama de la información
                var dataResponse = respuesta.Split('[');

                /**
                 * -Si la respuesta contiene 06 en sus 2 primeros caracteres se trata de una respuesta operativa,
                 * es decir, una respuesta para dar indicaciones al usuario en la pantalla.
                 * 
                 * - Si la trama contiene una P en en segundo caracter se trata de una respuesta de error
                 * **/
                if (respuesta.Substring(0, 2).Equals("06") || respuesta.Substring(1, 1).Equals("P"))
                {
                    //Si al dividir la trama se optienen mas de 1 posicion entonces la trama se puede validar
                    if (dataResponse.Length > 1)
                    {
                        /****
                         * Si esta respuesta contiene una F en la posicion 3 entonces se trata de una respuesta final
                         * para un pago con tarjeta de crédito
                         */
                        if (respuesta.Substring(3, 1).Equals("F"))
                        {
                            //Procesa la ultima respuesta del datáfono
                            ProcesarFinal(dataResponse[1]);
                        }
                        else
                        {
                            //Procesa las tramas transaccionales
                            ProccessDataTransaction(dataResponse[1]);
                        }
                    }
                    else
                    {
                        SetMessageAndPutVisibility("Datáfono no disponible, intente de nuevo mas tarde.");
                        Task.Run(() =>
                        {
                            Thread.Sleep(2000);
                            Close();
                        });
                    }
                }
                else if (respuesta.Substring(1, 1).Equals("F"))
                {
                    //Si la respuesta contiene una F en su caracter en posición 1, se trata de la finalización de una transacción
                    // con tarjeta débito
                    ProcesarFinal(dataResponse[1]);
                }
                else
                {
                    SetMessageAndPutVisibility("No se pudo procesar la transacción, intente de nuevo mas tarde.");
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        Close();
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Procesa todas las tramas transaccionales antes de llegar al final del pago
        /// </summary>
        /// <param name="response"></param>
        void ProccessDataTransaction(string response)
        {
            try
            {
                var dataTransaction = response.Split(',');
                //Valido si la respuesta concuerda con una respuesta válida para el sistema
                if (dataTransaction.Length == 4)
                {
                    //Si la trama contiene 63 puede tratarse de un error o de la solicitud de la clave
                    if (dataTransaction[1].Equals("63"))
                    {
                        //Si se trata de un error Espero hasta que me responda la trama total del error
                        if (dataTransaction[2].Equals("ERROR"))
                        {
                            var respuestaPeticion = objUtil.EnviarPeticionEspera();
                            Utilities.CallBackRespuesta?.Invoke(respuestaPeticion);
                        }
                        else
                        {
                            lvOpciones.Visibility = Visibility.Hidden;
                            opciones = new Opciones("Digita la clave en el datáfono");
                            opciones.ShowDialog();
                            var respuestaPeticion = objUtil.EnviarPeticionEspera();
                            Utilities.CallBackRespuesta?.Invoke(respuestaPeticion);
                        }
                    }
                    else
                    {
                        //Si la respuesta es diferente a 63 entonces se trata de una trama transaccional válida
                        ProccessPositiveResponse(dataTransaction);
                    }

                }
                else if (dataTransaction.Length > 4)// Si la data supera los 4 espacios se trata de un pago con tarjeta de crétido
                {
                    //Armo la trama necesaria para las tarjetas de crédito
                    string Trama = string.Concat("R,", dataTransaction[1], ",1,");
                    //Si la trama respondida por el datáfono contiene Ult. se trata de la peticion de los 4 últimos dígitos de la tarjeta al usuario
                    if (dataTransaction[3].Contains("Ult."))
                    {
                        //Se envía un maxlenght para el campo de ingreso de los 4 números, la trama y si es crédito
                        opciones = new Opciones("Cuatro últimos dígitos de la tarjeta", len: 4, peticion: Trama, isCredit: true);
                        opciones.ShowDialog();
                    }
                    else if (dataTransaction[3].Contains("Cuotas"))//Si contiene Cuotas entonces se trata de la solicitud del número de cueotas para la compra al usuario
                    {
                        opciones = new Opciones("¿Número de cuotas?", len: 2, peticion: Trama, isCredit: true);
                        opciones.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Procesa la respuesta transaccional positiva
        /// </summary>
        /// <param name="positiveResponse"></param>
        private void ProccessPositiveResponse(string[] positiveResponse)
        {
            try
            {
                lvOpciones.Visibility = Visibility.Visible;
                //Asigno el título de la operación actual para el usuario
                mensajes.MensajePrincipal = positiveResponse[2];
                //Separo la trama requerida y pierdo el final innecesario
                var responseData = positiveResponse[3].Split(']')[0];
                //Elimino un (;) que queda el final de la trama para evitar errores
                responseData = responseData.Substring(0, responseData.Length - 1);

                //Separo y obtengo las opciones transaccionales que se le presentarán al usuario
                var opcionesTransaccionales = responseData.Split(';');
                //En esta lista se almanecarán las opciones que se le presentaran en la vista al usuario
                List<FormaPago> formas = new List<FormaPago>();
                //Este índice le da a cada opción su valor segun la trama del datáfono
                int indiceForma = 1;

                //Se recorren y agregan a la vista todas las opciones presentadas por el datáfono
                foreach (var item in opcionesTransaccionales)
                {
                    formas.Add(new FormaPago
                    {
                        Forma = item,
                        Trama = string.Concat("R,", positiveResponse[1], ",", indiceForma, "]"),
                    });
                    indiceForma++;
                }

                //Finalmente agrego otra opcion que servirá para cancelar la transacción
                formas.Add(new FormaPago
                {
                    Forma = "Cancelar",
                    Trama = string.Concat("R,", positiveResponse[1], ",0]"),
                });
                //Se le asigna el modelo a la vista transaccional
                lvOpciones.DataContext = formas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Procesa la respuesta final del datáfono a la pantalla
        /// </summary>
        /// <param name="response"></param>
        void ProcesarFinal(string response)
        {
            try
            {
                //Divido la trama separada por (,) para obtener el estado de la transacción
                var data = response.Split(',');
                if (data[2].Equals("00"))
                {
                    SetMessageAndPutVisibility("Pago Exitoso");
                }
                else
                {
                    //Si falla la comunicación re-intento por 3 ocasiones, esto reinicia la transacción
                    if (reintent < 3 && !data[2].Contains("]"))
                    {
                        reintent++;
                        SetMessageAndPutVisibility("Intente de nuevo.");
                        var hexResult = objUtil.CalculateLRC(TramaInicial);
                        var datos = objUtil.EnviarPeticion(hexResult);
                        Utilities.CallBackRespuesta?.Invoke(datos);
                    }
                    else
                    {
                        SetMessageAndPutVisibility("No se pudo procesar el pago, intente de nuevo mas tarde.");
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Método para cambiar mensaje y ocultar lista de opciones
        /// </summary>
        /// <param name="message"></param>
        void SetMessageAndPutVisibility(string message)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    mensajes.MensajePrincipal = message;
                    lvOpciones.Visibility = Visibility.Hidden;
                });
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Evento de lo que selecciona el usuario en pantalla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Optengo la data contenida en el elemento seleccionado
                var data = (sender as ListBoxItem).DataContext;

                //Si se trada de una data que setee yo entonces procedo a recorrerla
                if (data is FormaPago)
                {
                    var datos = data as FormaPago;
                    var LRCPeticion = objUtil.CalculateLRC(datos.Trama);

                    //Swicheo todas las opciones presentadas por el datáfono para pintarle al usuario las guías correspondientes
                    switch (datos.Forma)
                    {
                        case "DESLIZAR":
                        case "INSERTAR":
                        case "ACERCAR":
                            opciones = new Opciones("Pasa tu tarjeta por el datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "QR":
                            opciones = new Opciones("Lee el QR en el datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "PAGO MOVIL":
                            opciones = new Opciones("Utiliza tu teléfono para completar el pago en el datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "NFC":
                            opciones = new Opciones("Acerca el dispositivo NFC al datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "AHORROS":
                            opciones = new Opciones("Digite la clave en el datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "CORRIENTE":
                            opciones = new Opciones("Digita la clave en el datáfono", peticion: LRCPeticion);
                            opciones.ShowDialog();
                            break;
                        case "CREDITO":
                            opciones = new Opciones("¿A cuantas cuotas realizará la compra?", peticion: LRCPeticion);
                            opciones.ShowDialog();
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

        #endregion
    }
    public class FormaPago
    {
        public string Forma { get; set; }
        public string Trama { get; set; }
    }
}
