﻿using System;
using System.Collections.Generic;
using System.Threading;
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
    /// Interaction logic for UCPayCine.xaml
    /// </summary>
    public partial class UCPayCine : UserControl
    {
        private PaymentViewModel PaymentViewModel;
        private FrmLoading frmLoading;
        private Utilities utilities;
        private LogErrorGeneral logError;
        private bool stateUpdate;
        int controlInactividad = 0;
        int controlCancel = 0;
        private bool payState;
        private int num = 1;
        TimerTiempo timer;
        Response responseGlobal = new Response();
        Utilities objUtil = new Utilities();
        private bool totalReturn = false;
        public UCPayCine(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            try
            {
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
                var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
                TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

                logError = new LogErrorGeneral
                {
                    Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    IDCorresponsal = Utilities.CorrespondentId,
                    IdTransaction = Utilities.IDTransactionDB,
                };

                stateUpdate = true;
                payState = true;
                Utilities.control.StartValues();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Window_Loaded en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

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
                        if (PaymentViewModel.ValorIngresado >= Utilities.PayVal)
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                btnCancelar.IsEnabled = false;
                                Utilities.Loading(frmLoading, true, this);
                                Utilities.control.callbackValueIn = null;

                            });
                        }
                        utilities.ProccesValue(new DataMoneyNotification
                        {
                            enterValue = enterValue,
                            opt = 2,
                            quantity = 1,
                            idTransactionAPi = Utilities.IDTransactionDB
                        });
                    }
                };

                Utilities.control.callbackTotalIn = enterTotal =>
                {


                    Utilities.control.callbackTotalIn = null;


                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    Utilities.EnterTotal = enterTotal;
                    if (enterTotal > 0 && PaymentViewModel.ValorSobrante > 0)
                    {
                        ActivateTimer(true);

                        ReturnMoney(PaymentViewModel.ValorSobrante, true);

                    }
                    else
                    {
                        Buytickets();
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.control.callbackError = null;
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.StartAceptance(PaymentViewModel.PayValue);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
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
                totalReturn = false;
                Utilities.control.callbackLog = log =>
                {
                    utilities.ProccesValue(log, Utilities.IDTransactionDB);
                };

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.control.callbackTotalOut = null;
                    Utilities.ValueDelivery = (long)totalOut;

                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    totalReturn = true;
                    if (state)
                    {
                        try
                        {
                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                        }
                        catch { }
                        Buytickets();
                    }
                    else
                    {
                        Cancelled();
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.control.callbackError = null;
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.callbackOut = delivery =>
                {

                    Utilities.control.callbackOut = null;
                    if (!totalReturn)
                    {
                        Utilities.ValueDelivery = (long)delivery;
                        Utilities.SaveLogDispenser(ControlPeripherals.log);

                        try
                        {
                            timer.CallBackStop?.Invoke(1);
                            SetCallBacksNull();
                        }
                        catch { }
                        if (PaymentViewModel.ValorIngresado >= Utilities.PayVal && state)
                        {
                            if (delivery != returnValue)
                            {
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    frmModal modal = new frmModal("Lo sentimos, no fué posible devolver todo el dinero, tienes un faltante de: " + (returnValue - delivery).ToString("#,##0") + ", presiona Salir para tomar tus boletas. Gracias");
                                    modal.ShowDialog();
                                    Buytickets();
                                    Utilities.Loading(frmLoading, false, this);
                                });
                            }
                            else
                            {
                                Buytickets();
                                Utilities.Loading(frmLoading, false, this);
                            }
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                frmModal modal = new frmModal("Lo sentimos, no fué posible devolver todo el dinero, tienes un faltante de: " + (returnValue - delivery).ToString("#,##0"));
                                modal.ShowDialog();
                                Cancelled();
                            });
                        }
                    }
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                    "ReturnMoney en frmPayCine",
                    EError.Aplication,
                    ELevelError.Medium);
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
                AdminPaypad.SaveErrorControl(ex.Message, "OrganizeValue en frmPayCine", EError.Aplication, ELevelError.Medium);
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
                    Task.Run(() =>
                    {

                        Utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 2, PaymentViewModel.ValorSobrante);
                    });
                }
            }
            catch (Exception ex)
            {
                logError.Description = "\nNo fue posible actualizar esta transacción a aprobada";
                logError.State = "Iniciada";
                Utilities.SaveLogTransactions(logError, "LogTransacciones\\Iniciadas");
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
                    try
                    {
                        Task.Run(() =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                            });
                        });
                    }
                    catch { }

                    try
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
                    }
                    catch { }

                    ActivateTimer(false);
                    ReturnMoney(Utilities.PayVal, false);

                }
                else
                {
                    objUtil.PrintTicket("Aprobada", Utilities.TypeSeats, Utilities.DipMapCurrent);
                    ApproveTrans();

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Opacity = 1;
                        Utilities.Loading(frmLoading, false, this);
                    });

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCFinalTransaction());
                    });
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SavePay en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Cancelled()
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            if (controlCancel == 0)
            {
                controlCancel = 1;
                try
                {
                    try
                    {
                        Task.Run(() =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                            });
                        });
                    }
                    catch (Exception ex)
                    {
                        LogService.CreateLogsError(
                              string.Concat("Mensaje: ", ex.Message, "-------- Inner: ",
                              ex.InnerException, "---------- Trace: ", ex.StackTrace), "Cancelled PayCine");
                    }
                    Task.Run(() =>
                    {
                        Utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, 3, Utilities.ValueDelivery);

                        logError.Description = "\nSe cancelo una transaccion";
                        logError.State = "Cancelada";
                        Utilities.SaveLogTransactions(logError, "LogTransacciones\\Cancelada");

                    });
                    Dispatcher.Invoke(() =>
                    {
                        PaymentGrid.Opacity = 0.3;
                        Utilities.Loading(frmLoading, false, this);
                        frmModal modal = new frmModal("Señor usuario, su compra fué cancelada.");
                        modal.ShowDialog();
                    });
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    AdminPaypad.SaveErrorControl(ex.Message, "Cancelled en frmPayCine", EError.Aplication, ELevelError.Medium);
                }
                Utilities.GoToInicial();
            }
        }

        private void Buytickets()
        {
            try
            {
                if (num == 2)
                {
                    return;
                }

                if (num == 1)
                {

                    payState = true;

                    if (Utilities.GetConfiguration("Ambiente").Equals("Prd"))
                    {
                        var response = WCFServices.PostBuy(Utilities.DipMapCurrent, Utilities.TypeSeats);

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
                        if (transaccionCompra.Respuesta != "Exitosa")
                        {
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
                    else
                    {
                        Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
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
            catch (Exception ex)
            {
                payState = false;
                SavePay(payState);
                AdminPaypad.SaveErrorControl(ex.Message, "BuyTicket en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }
        #endregion

        #region TimerInactividad

        void ActivateTimer(bool state)
        {
            try
            {
                string timerInactividad = Utilities.GetConfiguration("TimerInactividad");
                timer = new TimerTiempo(timerInactividad);
                timer.CallBackClose = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        try
                        {
                            timer.CallBackClose = null;
                        }
                        catch { }
                        if (controlInactividad == 0)
                        {
                            if (PaymentViewModel.ValorIngresado >= Utilities.PayVal)
                            {

                                try
                                {
                                    LogService.SaveRequestResponse(DateTime.Now + " :: ActivateTimer: ", "Tiempo Transcurrido Inactividad Dispositivos");
                                }
                                catch { }
                                controlInactividad = 1;
                                try
                                {
                                    timer.CallBackStop?.Invoke(1);
                                    SetCallBacksNull();
                                }
                                catch { }
                                if (state)
                                {
                                    Buytickets();
                                    Utilities.Loading(frmLoading, false, this);
                                }
                                else
                                {
                                    Utilities.Loading(frmLoading, false, this);
                                    frmModal modal = new frmModal("Estimado usuario, ha ocurrido un error, contacte a un administrador. Gracias");
                                    modal.ShowDialog();
                                    Cancelled();
                                }

                            }
                            else
                            {
                                try
                                {
                                    timer.CallBackStop?.Invoke(1);
                                    SetCallBacksNull();
                                }
                                catch { }
                                Utilities.Loading(frmLoading, false, this);
                                frmModal modal = new frmModal("Estimado usuario, ha ocurrido un error, contacte a un administrador. Gracias");
                                modal.ShowDialog();
                                Cancelled();
                            }
                        }
                    });
                    GC.Collect();
                };
            }
            catch { }
        }

        void SetCallBacksNull()
        {
            try
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
            }
            catch { }
        }
        #endregion

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Utilities.Loading(frmLoading, true, this);

                Utilities.control.StopAceptance();
                Thread.Sleep(200);

                if (PaymentViewModel.ValorIngresado > 0)
                {
                    ActivateTimer(false);
                    ReturnMoney(PaymentViewModel.ValorIngresado, false);
                }
                else
                {
                    Cancelled();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }


    }
}