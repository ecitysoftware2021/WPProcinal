using CEntidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for frmPayCine.xaml
    /// </summary>
    public partial class frmPayCine : Window
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

        #region LoadMethods
        public frmPayCine(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();

            try
            {
                api = new ApiLocal();
                //Utilities.PayVal = Utilities.RoundValue(Utilities.ValorPagarScore);
                OrganizeValues();
                frmLoading = new FrmLoading("Cargando...");
                utilities = new Utilities();
                Utilities.TypeSeats = Seats;
                Utilities.DipMapCurrent = dipMap;
                Utilities.DipMapCurrent.Total = Convert.ToDouble(Utilities.ValorPagarScore);

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
                Utilities.control.StartValues();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    if (payState)
                    {
                        ActivateWallet();
                    }
                });

                Task.Run(() =>
                {
                    GetReceipt();
                });
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
              string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
              ex.InnerException, "---------- Trace: ", ex.StackTrace), "Window_Loaded PayCine");
            }
        }

        private void btnCancelar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                this.Opacity = 0.3;
                Utilities.Loading(frmLoading, true, this);

                Utilities.control.StopAceptance();

                Task.Run(() =>
                {
                    utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 3, PaymentViewModel.ValorSobrante).Wait();

                    logError.Description = "\nNo Se cancelo una transaccion";
                    logError.State = "Cancelada";
                    Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");

                });
                if (PaymentViewModel.ValorIngresado > 0)
                {
                    Utilities.DispenserVal = PaymentViewModel.ValorIngresado;
                    Utilities.control.callbackTotalOut = totalOut =>
                    {
                        Cancelled();
                    };

                    Utilities.control.callbackLog = log =>
                    {
                        utilities.ProccesValue(log, Utilities.IDTransactionDB);
                    };

                    Utilities.control.callbackOut = quiantityOut =>
                    {
                        Cancelled();
                    };

                    Utilities.control.callbackError = error =>
                    {
                        Utilities.SaveLogDispenser(ControlPeripherals.log);

                    };

                    Utilities.control.StartDispenser(Utilities.DispenserVal);
                }
                else
                {
                    Cancelled();
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
              string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
              ex.InnerException, "---------- Trace: ", ex.StackTrace), "btnCancelar_PreviewStylusDown PayCine");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Método encargado de activar el billetero aceptance, seguido de esto crea un callback esperando a que este le indique que puede finalizar la transacción
        /// </summary>
        private void ActivateWallet()
        {
            try
            {
                payState = false;
                Utilities.control.callbackValueIn = enterValue =>
                {
                    if (enterValue > 0)
                    {
                        PaymentViewModel.ValorIngresado += enterValue;
                        utilities.ProccesValue(enterValue, 2, 1, "", Utilities.IDTransactionDB);
                    }
                };

                Utilities.control.callbackTotalIn = enterTotal =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Opacity = 0.3;
                        Utilities.Loading(frmLoading, true, this);
                    });
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    Utilities.EnterTotal = enterTotal;
                    if (enterTotal > 0 && PaymentViewModel.ValorSobrante > 0)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            btnCancelar.IsEnabled = false;
                        });
                        ReturnMoney(PaymentViewModel.ValorSobrante, true);
                    }
                    else
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            btnCancelar.IsEnabled = false;
                        });
                        Buytickets();
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);

                };

                Utilities.control.StartAceptance(PaymentViewModel.PayValue);
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
               string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
               ex.InnerException, "---------- Trace: ", ex.StackTrace), "ActivateWallet");
            }
        }

        /// <summary>
        /// Método que se encarga de devolver el dinero ya sea por que se canceló la transacción o por que hay valor sobrante
        /// </summary>
        /// <param name="returnValue">valor a devolver</param>
        private void ReturnMoney(decimal returnValue, bool state)
        {
            try
            {

                Utilities.control.callbackLog = log =>
                {
                    utilities.ProccesValue(log, Utilities.IDTransactionDB);
                };

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    Utilities.ValueDelivery = (long)totalOut;
                    try
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("ReturnMoney: ", "Ingresé(" + state + ")");
                    }
                    catch { }
                    if (state)
                    {
                        Buytickets();
                    }
                    else
                    {
                        Cancelled();
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    if (PaymentViewModel.ValorIngresado >= Utilities.PayVal)
                    {
                        frmModal _frmModal = new frmModal(error);
                        _frmModal.ShowDialog();
                        FrmFinalTransaction frmFinal = new FrmFinalTransaction();
                        frmFinal.Show();
                        this.Close();
                    }
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
               string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
               ex.InnerException, "---------- Trace: ", ex.StackTrace), "ReturnMoney");
            }
        }

        /// <summary>
        /// Método encargado de organizar todos los valores de la transacción en la vista
        /// </summary>
        private void OrganizeValues()
        {
            try
            {
                lblValorPagar.Content = string.Format("{0:C0}", Utilities.PayVal);
                PaymentViewModel = new PaymentViewModel
                {
                    PayValue = Utilities.PayVal,
                    ValorFaltante = Utilities.PayVal,
                    ValorSobrante = 0,
                    ValorIngresado = 0
                };

                this.DataContext = PaymentViewModel;
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
             string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
             ex.InnerException, "---------- Trace: ", ex.StackTrace), "OrganizeValues PayCine");
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
                    var state = utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 2, PaymentViewModel.ValorSobrante);
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
                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("SavePay: ", task.ToString());
                }
                catch { }
                if (!task)
                {
                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        PaymentGrid.Opacity = 0.3;
                        Utilities.Loading(frmLoading, false, this);
                        frmModal modal = new frmModal("No se pudo realizar la compra, se devolverá el dinero: " + Utilities.PayVal.ToString("#,##0"));
                        modal.ShowDialog();

                        Utilities.Loading(frmLoading, true, this);
                    });
                    GC.Collect();

                    Task.Run(() =>
                    {
                        utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 3, PaymentViewModel.ValorSobrante).Wait();

                        logError.Description = "\nNo Se cancelo una transaccion";
                        logError.State = "Cancelada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");
                    }).Wait();

                    ReturnMoney(Utilities.PayVal, false);
                }
                else
                {

                    ApproveTrans();

                    objUtil.ImprimirComprobante("Aprobada", Utilities.Receipt, Utilities.TypeSeats, Utilities.DipMapCurrent);

                    Utilities.control.StopAceptance();

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Opacity = 1;
                        Utilities.Loading(frmLoading, false, this);
                    });

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        FrmFinalTransaction frmFinal = new FrmFinalTransaction();
                        frmFinal.Show();
                        this.Close();
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
                string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                ex.InnerException, "---------- Trace: ", ex.StackTrace), "SavePay(" + task + ")");
            }
        }

        private void Cancelled()
        {
            try
            {
                Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                Utilities.control.StopAceptance();

                Dispatcher.Invoke(() =>
                {
                    PaymentGrid.Opacity = 0.3;
                    Utilities.Loading(frmLoading, false, this);
                    frmModal modal = new frmModal("Usuario su pago fue cancelado.");
                    modal.ShowDialog();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
           string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
           ex.InnerException, "---------- Trace: ", ex.StackTrace), "Cancelled PayCine");
            }
            Utilities.GoToInicial(this);
        }

        private void GetReceipt()
        {
            try
            {
                var response = WCFServices.GetReceiptProcinal(ControlPantalla.IdCorrespo);
                if (response.IsSuccess)
                {
                    Utilities.Receipt = JsonConvert.DeserializeObject<Receipt>(response.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
           string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
           ex.InnerException, "---------- Trace: ", ex.StackTrace), "GetReceipt PayCine");
            }
        }

        private void Buytickets()
        {
            try
            {

                try
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos("BuyTickets: ", "Ingresé");
                }
                catch { }
                if (num == 2)
                {
                    return;
                }

                if (num == 1)
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
                    }

                    var transaccionCompra = WCFServices.DeserealizeXML<TransaccionCompra>(response.Result.ToString());
                    try
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("PostComprar: ", transaccionCompra.Respuesta);
                    }
                    catch { }
                    if (transaccionCompra.Respuesta != "Exitosa")
                    {
                        Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                        payState = false;
                        Utilities.SaveLogError(new LogError
                        {
                            Message = transaccionCompra.Respuesta,
                            Method = "WCFServices.PostComprar.Fallida"
                        });
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
                    try
                    {
                        LogService.CreateLogsOperation("Llamé a SavePay(" + payState + ")");
                    }
                    catch { }
                    SavePay(payState);
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogsError(
                string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                ex.InnerException, "---------- Trace: ", ex.StackTrace), "Buytickets");
            }
        }
        #endregion
    }
}
