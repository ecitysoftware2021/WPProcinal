using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.DataModel;
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
        private bool stateUpdate;
        int controlInactividad = 0;
        int controlCancel = 0;
        private bool payState;
        private bool state;
        TimerTiempo timer;
        Response responseGlobal = new Response();
        private bool totalReturn = false;
        List<Producto> productos;
        private decimal pagoEfectivo = 0;
        private decimal pagoInterno = 0;
        public UCPayCine(List<ChairsInformation> Seats, FunctionInformation dipMap)
        {
            InitializeComponent();
            try
            {
                OrganizeValues();
                pagoEfectivo = Utilities.PayVal;
                state = true;

                Utilities.SelectedFunction.Total = Convert.ToDouble(Utilities.ValorPagarScore);
                Utilities.SelectedChairs = Seats;
                Utilities.SelectedFunction = dipMap;
                if (Seats.Count > 0)
                {
                    TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
                    TxtDay.Text = dipMap.Day;
                    TxtRoom.Text = dipMap.RoomName;
                    TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
                    TxtHour.Text = "Hora Función: " + dipMap.HourFunction;
                    TxtSubTitle.Text = "Idioma: " + dipMap.Language;
                    var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
                    TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

                }
                stateUpdate = true;
                payState = true;


                //Buytickets();
                Utilities.control.StartValues();
                Utilities.Speack("Por favor, ingresa el dinero");

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
                ValidateUserBalance();
                if (pagoEfectivo == 0)
                {
                    Buytickets();
                }
                else
                {
                    Utilities.PayVal = pagoEfectivo;
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
                AdminPaypad.SaveErrorControl(ex.Message, "Window_Loaded en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }
        private void ValidateUserBalance()
        {
            decimal valorPagoConSaldoFavor = 0;
            if (DataService41.dataUser.SaldoFavor != null)
            {
                if (DataService41.dataUser.SaldoFavor.Value > 0)
                {
                    frmModal Modal = new frmModal($"Tienes un saldo a favor de ${DataService41.dataUser.SaldoFavor.Value.ToString("C")}, ¿deseas utilizarlo en esta compra?", balance: true);
                    Modal.ShowDialog();
                    if (Modal.DialogResult.HasValue && Modal.DialogResult.Value)
                    {
                        valorPagoConSaldoFavor = Utilities.ValorPagarScore - DataService41.dataUser.SaldoFavor.Value;
                        if (valorPagoConSaldoFavor <= 99)
                        {
                            pagoEfectivo = 0;
                            pagoInterno = Utilities.ValorPagarScore;
                        }
                        else
                        {
                            pagoEfectivo = Utilities.ValorPagarScore - DataService41.dataUser.SaldoFavor.Value;
                            pagoInterno = DataService41.dataUser.SaldoFavor.Value;
                        }
                    }
                }
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
                                btnCancelar.Visibility = Visibility.Hidden;
                                Utilities.control.callbackValueIn = null;

                            });
                        }
                        ProccesValue(new DataMoneyNotification
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
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.control.callbackError = null;
                };

                Utilities.control.StartAceptance(PaymentViewModel.PayValue);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ActivateWallet en frmPayCine", EError.Aplication, ELevelError.Medium);
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

        /// <summary>
        /// Método que se encarga de devolver el dinero ya sea por que se canceló la transacción o por que hay valor sobrante
        /// </summary>
        /// <param name="returnValue">valor a devolver</param>
        private void ReturnMoney(decimal returnValue, bool state)
        {
            FrmLoading frmLoading = new FrmLoading("Devolviendo dinero...");
            try
            {
                Utilities.Speack("Estamos contando el dinero, espera un momento por favor.");
                frmLoading.Show();
                totalReturn = false;
                Utilities.control.callbackLog = log =>
                {
                    ProccesValue(log, Utilities.IDTransactionDB);
                };

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.control.callbackTotalOut = null;
                    Utilities.ValueDelivery = (long)totalOut;
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
                };

                Utilities.control.callbackOut = delivery =>
                {

                    Utilities.control.callbackOut = null;
                    if (!totalReturn)
                    {
                        Utilities.ValueDelivery = (long)delivery;

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
                AdminPaypad.SaveErrorControl(ex.Message,
                    "ReturnMoney en frmPayCine",
                    EError.Aplication,
                    ELevelError.Medium);
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
                Utilities.PLACA = string.Empty;
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
                        Utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, (int)ETransactionState.Aproved, PaymentViewModel.ValorSobrante);
                    });
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
                    this.IsEnabled = false;
                    FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    try
                    {
                        frmLoading.Show();
                        Utilities.CancelAssing(Utilities.SelectedChairs, Utilities.SelectedFunction);
                        frmLoading.Close();
                        this.IsEnabled = true;

                        await Dispatcher.BeginInvoke((Action)delegate
                        {
                            frmModal modal = new frmModal("No se pudo realizar la compra, se devolverá el dinero: " + Utilities.PayVal.ToString("#,##0"));
                            modal.ShowDialog();
                        });
                        GC.Collect();
                    }
                    catch { frmLoading.Close(); }

                    ActivateTimer(false);
                    ReturnMoney(Utilities.PayVal, false);

                }
                else
                {
                    Utilities.PrintTicket("Aprobada", Utilities.SelectedChairs, Utilities.SelectedFunction);

                    ApproveTrans();

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        if (DataService41.dataUser.Tarjeta != null)
                        {
                            Switcher.Navigate(new UCPoints());
                        }
                        else
                        {
                            Switcher.Navigate(new UCFinalTransaction());
                        }
                    });
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
            try
            {
                FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                try
                {
                    this.IsEnabled = false;
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.SelectedChairs, Utilities.SelectedFunction);
                    frmLoading.Close();
                    this.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    frmLoading.Close();
                }
                Task.Run(() =>
                {
                    Utilities.UpdateTransaction(PaymentViewModel.ValorIngresado, (int)ETransactionState.Canceled, Utilities.ValueDelivery);

                });
                Dispatcher.Invoke(() =>
                {
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

        private void Buytickets()
        {
            FrmLoading frmLoading = new FrmLoading("Procesando compra...");

            try
            {

                if (Utilities.GetConfiguration("ModalPlate").Equals("1") && Utilities.PlateObligatory)
                {
                    WPlateModal wPlate = new WPlateModal();
                    wPlate.ShowDialog();
                }
                frmLoading.Show();

                if (Utilities.GetConfiguration("Ambiente").Equals("1"))
                {
                    List<UbicacioneSCOINT> ubicaciones = new List<UbicacioneSCOINT>();
                    foreach (var item in Utilities.SelectedChairs)
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
                            combo.Precio = DataService41.dataUser.Tarjeta != null ? 2 : 1;
                            productos.Add(combo);
                        }
                    }


                    string year = DateTime.Now.Year.ToString();
                    string mount = DateTime.Now.Month.ToString();
                    string day = DateTime.Now.Day.ToString();
                    if (Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema)
                    {
                        year = Utilities.SelectedFunction.Date.Substring(0, 4);
                        mount = Utilities.SelectedFunction.Date.Substring(4, 2);
                        day = Utilities.SelectedFunction.Date.Substring(6, 2);
                    }
                    var dataClient = GetDataClient();

                    var response41 = WCFServices41.PostBuy(new SCOINT
                    {
                        Accion = Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema ? "V" : "C",
                        Placa = string.IsNullOrEmpty(Utilities.PLACA) ? "0" : Utilities.PLACA,
                        Apellido = dataClient.Apellido,
                        ClienteFrecuente = long.Parse(dataClient.Tarjeta),
                        CorreoCliente = dataClient.Login,
                        Cortesia = string.Empty,
                        Direccion = dataClient.Direccion,
                        DocIdentidad = long.Parse(dataClient.Documento),
                        Factura = Utilities.SelectedFunction.Secuence,
                        FechaFun = string.Concat(year, "-", mount, "-", day),
                        Funcion = Utilities.SelectedFunction.IDFuncion,
                        InicioFun = Utilities.SelectedFunction.HourFormat,
                        Nombre = dataClient.Nombre,
                        PagoCredito = 0,
                        PagoEfectivo = (int)pagoEfectivo,
                        PagoInterno = (int)pagoInterno,
                        Pelicula = Utilities.SelectedFunction.MovieId,
                        Productos = productos,
                        PuntoVenta = Utilities.SelectedFunction.PointOfSale,
                        Sala = Utilities.SelectedFunction.RoomId,
                        teatro = Utilities.SelectedFunction.CinemaId,
                        Telefono = !string.IsNullOrEmpty(dataClient.Telefono) ? long.Parse(dataClient.Telefono) : 0,
                        tercero = 1,
                        TipoBono = 0,
                        TotalVenta = int.Parse(Utilities.ValorPagarScore.ToString()),
                        Ubicaciones = ubicaciones,
                        Obs1 = string.IsNullOrEmpty(Utilities.TIPOAUTO) ? "" : Utilities.TIPOAUTO
                    });
                    frmLoading.Close();
                    foreach (var item in response41)
                    {
                        if (item.Respuesta != null)
                        {
                            if (item.Respuesta.Contains("exitoso"))
                            {
                                if (DataService41.dataUser.Tarjeta != null)
                                {
                                    DataService41.dataUser.Puntos = Convert.ToDouble(Math.Floor(Utilities.PayVal / 1000)) + DataService41.dataUser.Puntos;
                                }
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
                }
                else
                {
                    this.IsEnabled = false;
                    frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.SelectedChairs, Utilities.SelectedFunction);
                    frmLoading.Close();
                    this.IsEnabled = true;
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
                AdminPaypad.SaveErrorControl(ex.Message, "BuyTicket en frmPayCine", EError.Aplication, ELevelError.Medium);
            }
        }

        private void GetInvoice()
        {
            if (DataService41._Combos.Count > 0)
            {
                FrmLoading frmLoading = new FrmLoading("¡Consultando resolución de factura...!");
                try
                {
                    frmLoading.Show();
                    DataService41._DataResolution = WCFServices41.ConsultResolution(new SCORES
                    {
                        Punto = Convert.ToInt32(Utilities.GetConfiguration("Cinema")),
                        Secuencial = Convert.ToInt32(DataService41.Secuencia),
                        teatro = int.Parse(Utilities.GetConfiguration("CodCinema")),
                        tercero = 1
                    });
                    frmLoading.Close();
                }
                catch (Exception ex)
                {
                    frmLoading.Close();
                }
            }
        }

        SCOLOGResponse GetDataClient()
        {
            if (DataService41.dataUser.Tarjeta != null)
            {
                return new SCOLOGResponse
                {
                    Apellido = DataService41.dataUser.Apellido,
                    Tarjeta = DataService41.dataUser.Tarjeta,
                    Login = DataService41.dataUser.Login,
                    Direccion = DataService41.dataUser.Direccion,
                    Documento = DataService41.dataUser.Documento,
                    Nombre = DataService41.dataUser.Nombre,
                    Telefono = DataService41.dataUser.Telefono
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
                                try
                                {
                                    timer.CallBackStop?.Invoke(1);
                                    SetCallBacksNull();
                                }
                                catch { }
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
                this.IsEnabled = false;
                FrmLoading frmLoading = new FrmLoading("Apagando bilelteros...");
                frmLoading.Show();
                Utilities.control.StopAceptance();
                Thread.Sleep(200);
                frmLoading.Close();
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
