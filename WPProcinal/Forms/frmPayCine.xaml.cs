﻿using CEntidades;
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
        Response responseGlobal = new Response();
        Utilities objUtil = new Utilities();

        #region LoadMethods
        public frmPayCine(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();

            try
            {
                api = new ApiLocal();
                Utilities.PayVal = Utilities.RoundValue(Utilities.ValorPagar);
                OrganizeValues();
                frmLoading = new FrmLoading("Cargando...");
                utilities = new Utilities();
                Utilities.TypeSeats = Seats;
                Utilities.DipMapCurrent = dipMap;
                Utilities.DipMapCurrent.Total = Convert.ToDouble(Utilities.PayVal);

                logError = new LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                    //ValuePay = Utilities.PayVal,
                    //TypeConsult = Utilities.TypeConsult,
                    //Consult = Utilities.Consult,
                    //Id_Nit = Utilities.Id_Nit,
                    //Name = Utilities.Name,
                    //Phone = Utilities.Phone,
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
                        //Buytickets();
                        //SavePay(true);
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
                    Utilities.GoToInicial(this);
                }
            }
            catch (Exception ex)
            {
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
                        Buytickets();
                        //ReturnMoney(PaymentViewModel.ValorSobrante, true);
                    }
                    else
                    {
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
                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    Utilities.ValueDelivery = (long)totalOut;
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
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Método encargado de dar el estado inicial de todas las imagenes/botones de la vista
        /// </summary>
        private void VisibilityImage()
        {
            try
            {
                PaymentViewModel.ImgCancel = Visibility.Visible;
                PaymentViewModel.ImgIngreseBillete = Visibility.Visible;
                PaymentViewModel.ImgEspereCambio = Visibility.Hidden;
                PaymentViewModel.ImgLeyendoBillete = Visibility.Hidden;
                PaymentViewModel.ImgRecibo = Visibility.Hidden;
            }
            catch (Exception ex)
            {
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

                VisibilityImage();
                this.DataContext = PaymentViewModel;
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
                    });
                    GC.Collect();

                    Task.Run(() =>
                    {
                        //utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 3, PaymentViewModel.ValorSobrante).Wait();

                        logError.Description = "\nNo Se cancelo una transaccion";
                        logError.State = "Cancelada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");
                    }).Wait();

                    ReturnMoney(Utilities.PayVal, false);
                }
                else
                {
                   // ApproveTrans();

                    objUtil.ImprimirComprobante("Aprobada", Utilities.Receipt, Utilities.TypeSeats, Utilities.DipMapCurrent);

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        frmModal _frmModal = new frmModal(string.Concat("!Muchas gracias por utilizar nuestro servicio.",
                        Environment.NewLine,
                        "Su transacción ha finalizado correctamente!"));
                        _frmModal.ShowDialog();
                        Utilities.GoToInicial(this);
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
            //TODO: descomentar
            //if (countError < 3)
            //{
            bool payState = true;
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
                    //Buytickets();
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
                    //Buytickets();
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
            //}
            SavePay(payState);
        }
        #endregion
    }
}
