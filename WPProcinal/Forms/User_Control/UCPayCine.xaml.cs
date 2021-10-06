using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Service;
using Newtonsoft.Json;
using WPProcinal.ViewModel;
using System.Windows.Data;
using WPProcinal.DataModel;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCPayCine.xaml
    /// </summary>
    public partial class UCPayCine : UserControl
    {
        private PaymentViewModel PaymentViewModel;
        private bool stateUpdate;
        int controlInactividad = 0;
        private bool payState;
        private bool state;
        TimerTiempo timer;
        private bool totalReturn = false;
        private bool BtnCancellPressed = false;
        List<Producto> productos;

        public UCPayCine()
        {
            InitializeComponent();
            try
            {
                OrganizeValues();
                state = true;

                if (Utilities.dataTransaction.SelectedTypeSeats.Count > 0)
                {
                    TxtTitle.Text = Utilities.CapitalizeFirstLetter(Utilities.dataTransaction.DataFunction.MovieName);
                    TxtDay.Text = Utilities.dataTransaction.DataFunction.Day;
                    TxtRoom.Text = Utilities.dataTransaction.DataFunction.RoomName;
                    TxtFormat.Text = string.Format("Formato: {0}", Utilities.dataTransaction.MovieFormat.ToUpper());
                    TxtHour.Text = "Hora Función: " + Utilities.dataTransaction.DataFunction.HourFunction;
                    TxtSubTitle.Text = "Idioma: " + Utilities.dataTransaction.DataFunction.Language;
                    var time = TimeSpan.FromMinutes(double.Parse(Utilities.dataTransaction.DataFunction.Duration.Split(' ')[0]));
                    TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

                }
                stateUpdate = true;

                Utilities.control.ClearValues();

                if (Utilities.dataTransaction.PayVal > 0)
                {
                    payState = true;
                    Utilities.Speack("Por favor, ingresa el dinero");
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
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
                    Task.Run(() =>
                    {
                        if (payState)
                        {
                            ActivateWallet();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "Window_Loaded en frmPayCine", EError.Aplication, ELevelError.Medium);
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
                    if (enterValue.Item1 > 0)
                    {
                        if (!this.PaymentViewModel.StatePay)
                        {
                            PaymentViewModel.ValorIngresado += enterValue.Item1;

                            PaymentViewModel.RefreshListDenomination(int.Parse(enterValue.Item1.ToString()), 1, enterValue.Item2);

                            LoadView();
                        }
                    }
                };

                Utilities.control.callbackTotalIn = enterTotal =>
                {
                    Utilities.control.callbackTotalIn = null;

                    if (!BtnCancellPressed)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            this.PaymentViewModel.ImgCancel = Visibility.Hidden;
                            Utilities.control.CloseCallbackAP();
                            Utilities.control.StopAceptance();
                        });
                        GC.Collect();

                        if (enterTotal > 0 && PaymentViewModel.ValorSobrante > 0)
                        {
                            ActivateTimer(true);

                            ReturnMoney(PaymentViewModel.ValorSobrante, true);
                        }
                        else
                        {
                            if (state)
                            {
                                state = false;
                                Buytickets();
                            }
                        }
                    }
                };

                Utilities.control.callbackError = error =>
                {
                    AdminPaypad.SaveErrorControl(error, "", EError.Device, ELevelError.Medium);
                };

                Utilities.control.CallBackSaveRequestResponse = (Title, Message, State) =>
                {
                    LogService.SaveRequestResponse(Title, Message, State);
                };

                Utilities.control.StartAceptance(PaymentViewModel.PayValue);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Método que se encarga de devolver el dinero ya sea por que se canceló la transacción o por que hay valor sobrante
        /// </summary>
        /// <param name="returnValue">valor a devolver</param>
        private void ReturnMoney(decimal returnValue, bool state)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmLoading frmLoading = new FrmLoading("Devolviendo dinero...");

                try
                {
                    Utilities.Speack("Estamos contando el dinero, espera un momento por favor.");
                    frmLoading.Show();

                    totalReturn = false;

                    Utilities.control.callbackLog = log =>
                    {
                        PaymentViewModel.SplitDenomination(log);
                    };

                    Utilities.control.callbackTotalOut = totalOut =>
                    {
                        Utilities.control.callbackTotalOut = null;
                        Utilities.dataTransaction.ValueDelivery = (long)totalOut;
                        totalReturn = true;
                        if (state)
                        {
                            try
                            {
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    SetCallBacksNull();
                                    timer.CallBackStop?.Invoke(1);
                                });
                                GC.Collect();
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
                        AdminPaypad.SaveErrorControl(error, "", EError.Device, ELevelError.Medium);
                    };

                    Utilities.control.CallBackSaveRequestResponse = (Title, Message, State) =>
                    {
                        LogService.SaveRequestResponse(Title, Message, State);
                    };

                    Utilities.control.callbackOut = delivery =>
                    {
                        Utilities.control.callbackOut = null;

                        if (!totalReturn)
                        {
                            Utilities.dataTransaction.ValueDelivery = (long)delivery;

                            SetCallBacksNull();

                            if (PaymentViewModel.ValorIngresado >= Utilities.dataTransaction.PayVal && state)
                            {
                                if (delivery != returnValue)
                                {
                                    Dispatcher.BeginInvoke((Action)delegate
                                    {
                                        frmModal modal = new frmModal("Lo sentimos, no fué posible devolver todo el dinero, tienes un faltante de: " + (returnValue - delivery).ToString("#,##0") + ", presiona Salir para tomar tus boletas. Gracias");
                                        modal.ShowDialog();
                                        Buytickets();
                                    });
                                }
                                else
                                {
                                    Buytickets();
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

                    frmLoading.Close();
                }
                catch (Exception ex)
                {
                    frmLoading.Close();
                    AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                        "ReturnMoney en frmPayCine",
                        EError.Aplication,
                        ELevelError.Medium);
                }
            });
            GC.Collect();
        }

        /// <summary>
        /// Método encargado de organizar todos los valores de la transacción en la vista
        /// </summary>
        private void OrganizeValues()
        {
            try
            {
                PaymentViewModel = new PaymentViewModel
                {
                    PayValue = Utilities.dataTransaction.PayVal,
                    ValorFaltante = Utilities.dataTransaction.PayVal,
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = 0,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                this.DataContext = PaymentViewModel;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "OrganizeValue en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        private void LoadView()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    PaymentViewModel.viewList.Source = PaymentViewModel.Denominations;
                    lv_denominations.DataContext = PaymentViewModel.viewList;
                    lv_denominations.Items.Refresh();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCPayCine>LoadView", JsonConvert.SerializeObject(ex), 1);
            }
        }

        /// <summary>
        /// Método encargado de actualizar la transacción a aprobada, se llama en finalizar pago En caso de fallo se reintenta dos veces más actualizar el estado de la transacción, si el error persiste se guarda en un log local y en el servidor, seguido de esto se continua con la transacción normal
        /// </summary>
        private async void ApproveTrans()
        {
            try
            {
                if (stateUpdate)
                {

                    await Utilities.UpdateTransaction(
                          PaymentViewModel.ValorIngresado,
                          (int)ETransactionState.Aproved,
                          PaymentViewModel.Denominations,
                          Utilities.dataTransaction.ValueDelivery);

                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCPayCine>ApproveTrans", JsonConvert.SerializeObject(ex), 1);
                stateUpdate = false;
            }
        }

        private void SavePay(bool task)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (!task)
                    {
                        this.IsEnabled = false;
                        FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");

                        try
                        {
                            frmLoading.Show();
                            Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                            frmLoading.Close();
                            this.IsEnabled = true;

                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                frmModal modal = new frmModal("No se pudo realizar la compra, se devolverá el dinero: " + Utilities.dataTransaction.PayVal.ToString("#,##0"));
                                modal.ShowDialog();
                            });
                            GC.Collect();
                        }
                        catch (Exception ex)
                        {
                            LogService.SaveRequestResponse("UCPayCine>SavePay", JsonConvert.SerializeObject(ex), 1);
                            frmLoading.Close();
                        }

                        ActivateTimer(false);
                        ReturnMoney(Utilities.dataTransaction.PayVal, false);

                    }
                    else
                    {
                        Utilities.PrintTicket("Aprobada", Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);

                        ApproveTrans();

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Switcher.Navigate(new UCFinalTransaction());
                        });
                    }
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "Cancelled en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        private async void Cancelled()
        {
            SetCallBacksNull();
            try
            {
                Dispatcher.Invoke(() =>
                {
                    FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    this.IsEnabled = false;
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                    frmLoading.Close();
                    this.IsEnabled = true;
                });
                await Utilities.UpdateTransaction(
                      PaymentViewModel.ValorIngresado,
                      (int)ETransactionState.Canceled,
                      PaymentViewModel.Denominations,
                      Utilities.dataTransaction.ValueDelivery);

                Dispatcher.Invoke(() =>
                {
                    frmModal modal = new frmModal("Señor usuario, su compra fué cancelada.");
                    modal.ShowDialog();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "Cancelled en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
            Utilities.GoToInicial();

        }

        private void Buytickets()
        {
            //TODO: VERIFICAR
            ApproveTrans();
            //Cancelled();

            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmLoading frmLoading = new FrmLoading("Procesando compra...");

                try
                {
                    if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ModalPlate && Utilities.PlateObligatory)
                    {
                        WPlateModal wPlate = new WPlateModal();
                        wPlate.ShowDialog();
                    }

                    frmLoading.Show();

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

                    var response41 = WCFServices41.PostBuy(new SCOINT
                    {
                        Accion = Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema ? "V" : "C",
                        Placa = string.IsNullOrEmpty(Utilities.dataTransaction.PLACA) ? "0" : Utilities.dataTransaction.PLACA,
                        Apellido = dataClient.Apellido,
                        ClienteFrecuente = long.Parse(dataClient.Tarjeta),
                        CorreoCliente = dataClient.Login,
                        Cortesia = string.Empty,
                        Direccion = dataClient.Direccion,
                        DocIdentidad = long.Parse(dataClient.Documento),
                        Factura = int.Parse(Utilities.dataTransaction.Secuencia),
                        FechaFun = string.Concat(year, "-", mount, "-", day),
                        Funcion = Utilities.dataTransaction.DataFunction.IDFuncion,
                        InicioFun = Utilities.dataTransaction.DataFunction.HourFormat,
                        Nombre = dataClient.Nombre,
                        PagoCredito = 0,
                        PagoEfectivo = Utilities.dataTransaction.PayVal,
                        PagoInterno = Utilities.dataTransaction.PagoInterno,
                        Pelicula = Utilities.dataTransaction.DataFunction.MovieId,
                        Productos = productos,
                        PuntoVenta = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                        Sala = Utilities.dataTransaction.DataFunction.RoomId,
                        teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                        Telefono = !string.IsNullOrEmpty(dataClient.Telefono) ? long.Parse(dataClient.Telefono) : 0,
                        tercero = 1,
                        TipoBono = 0,
                        TotalVenta = Utilities.dataTransaction.DataFunction.Total,
                        Ubicaciones = ubicaciones,
                        Membresia = false,
                        Obs1 = string.IsNullOrEmpty(Utilities.dataTransaction.TIPOAUTO) ? "" : Utilities.dataTransaction.TIPOAUTO
                    });

                    frmLoading.Close();

                    foreach (var item in response41)
                    {
                        if (item.Respuesta != null)
                        {
                            if (item.Respuesta.Contains("exitoso"))
                            {
                                GetInvoice();
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

                    SavePay(payState);
                }
                catch (Exception ex)
                {
                    frmLoading.Close();
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        frmLoading.Close();
                    });
                    payState = false;
                    SavePay(payState);
                    AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "BuyTicket en frmPayCine", EError.Aplication, ELevelError.Medium);
                }
            });
        }

        private void GetInvoice()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (DataService41._Combos.Count > 0)
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
                        LogService.SaveRequestResponse("UCPayCine>GetInvoice", JsonConvert.SerializeObject(ex), 1);
                        frmLoading.Close();
                    }
                }
            });
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
        #endregion

        #region TimerInactividad

        void ActivateTimer(bool state)
        {
            try
            {
                string timerInactividad = Utilities.dataPaypad.PaypadConfiguration.inactivitY_TIMER;
                timer = new TimerTiempo(timerInactividad);
                timer.CallBackClose = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {

                        if (timer != null)
                        {
                            timer.CallBackClose = null;
                        }
                        if (controlInactividad == 0)
                        {
                            if (PaymentViewModel.ValorIngresado >= Utilities.dataTransaction.PayVal)
                            {
                                controlInactividad = 1;
                                SetCallBacksNull();
                                if (state)
                                {
                                    Buytickets();
                                }
                                else
                                {
                                    frmModal modal = new frmModal("Estimado usuario, ha ocurrido un error, contacte a un administrador. Gracias");
                                    modal.ShowDialog();
                                    Cancelled();
                                }

                            }
                            else
                            {
                                SetCallBacksNull();
                                frmModal modal = new frmModal("Estimado usuario, ha ocurrido un error, contacte a un administrador. Gracias");
                                modal.ShowDialog();
                                Cancelled();
                            }
                        }
                    });
                    GC.Collect();
                };
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCPayCine>ActivateTimer", JsonConvert.SerializeObject(ex), 1);
            }
        }

        void SetCallBacksNull()
        {
            if (timer != null)
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
                timer.CallBackStop?.Invoke(1);
            }
        }
        #endregion

        private void BtnCancell_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.PaymentViewModel.ImgCancel = Visibility.Hidden;
                Utilities.control.callbackLog = null;

                this.BtnCancellPressed = true;
                this.IsEnabled = false;

                FrmLoading frmLoading = new FrmLoading("Apagando billeteros...");
                frmLoading.Show();

                Utilities.control.StopAceptance();

                Thread.Sleep(2000);
                frmLoading.Close();

                if (PaymentViewModel.ValorIngresado > 0)
                {
                    ActivateTimer(false);
                    ReturnMoney(PaymentViewModel.ValorIngresado, false);
                    LogService.SaveRequestResponse("Boton Cancelar", "Se presionó el botón cancelar", 1);
                }
                else
                {
                    Cancelled();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }
    }
}
