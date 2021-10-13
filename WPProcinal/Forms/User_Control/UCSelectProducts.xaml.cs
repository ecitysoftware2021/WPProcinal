using Grabador.Transaccion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCSelectProducts.xaml
    /// </summary>
    public partial class UCSelectProducts : UserControl
    {
        #region "References"

        private TimerTiempo timer;
        ApiLocal api;
        CLSGrabador grabador;
        private CollectionViewSource view;
        private ObservableCollection<Producto> lstPager;
        #endregion

        #region "Constructor"

        public UCSelectProducts()
        {
            InitializeComponent();
            api = new ApiLocal();
            grabador = new CLSGrabador();
            this.view = new CollectionViewSource();
            this.lstPager = new ObservableCollection<Producto>();
            ActivateTimer();
        }


        #endregion

        #region "Events"

        private void BrdCombos_TouchDown(object sender, TouchEventArgs e)
        {
            DistributeProducts(sender);
        }

        private void BrdDrinks_TouchDown(object sender, TouchEventArgs e)
        {
            DistributeProducts(sender);
        }

        private void BrdPackages_TouchDown(object sender, TouchEventArgs e)
        {
            DistributeProducts(sender);
        }

        private void BrdOthers_TouchDown(object sender, TouchEventArgs e)
        {
            DistributeProducts(sender);
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            this.IsEnabled = false;
            var frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
            Utilities.Loading(frmLoading, true, this);
            Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
            Utilities.Loading(frmLoading, false, this);
            this.IsEnabled = true;
            Switcher.Navigate(new UCCinema());
        }

        private void BtnOmitir_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            ChangePrices();
            ShowDetailModal();
        }

        #endregion

        #region "Methods"

        public void ChangePrices()
        {
            try
            {
                List<Producto> productos = new List<Producto>();
                decimal precio = 0;
                foreach (var item in DataService41._Combos)
                {
                    precio = 0;
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var combo = DataService41._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                        if (combo.Receta != null)
                        {
                            foreach (var receta in combo.Receta)
                            {
                                if (receta.Precios != null)
                                {
                                    decimal otroPago = decimal.Parse(receta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                                    if (Utilities.dataTransaction.dataUser.Tarjeta != null && otroPago > 0)
                                    {
                                        precio += otroPago * receta.Cantidad;
                                        Utilities.dataTransaction.PrecioCinefans = true;
                                    }
                                    else
                                    {
                                        precio += decimal.Parse(receta.Precios.FirstOrDefault().General.Split('.')[0]) * receta.Cantidad;
                                    }
                                }
                                if (receta.RecetaReceta != null)
                                {
                                    List<Receta> recetaAux = new List<Receta>();
                                    for (int e = 0; e < int.Parse(receta.Cantidad.ToString()); e++)
                                    {

                                        var responseRecetaBebida = receta.RecetaReceta.Where(rc => rc.Descripcion.ToLower().Contains("gaseosa")).FirstOrDefault();
                                        Receta responseRecetaComida = null;
                                        //Si el combo es el combo 4, solo tomamos por defecto las gaseosas, las comidas se dejan tal cual
                                        if (item.Code != Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.Code4)
                                        {
                                            responseRecetaComida = receta.RecetaReceta.Where(rc => rc.Descripcion.ToLower().Contains("perro")).FirstOrDefault();
                                        }
                                        if (responseRecetaBebida != null)
                                        {
                                            recetaAux.Add(responseRecetaBebida);
                                        }
                                        else if (responseRecetaComida != null)
                                        {
                                            recetaAux.Add(responseRecetaComida);
                                        }

                                    }
                                    if (recetaAux.Count != 0)
                                    {
                                        receta.RecetaReceta = recetaAux;
                                    }
                                    else
                                    {
                                        receta.RecetaReceta = receta.RecetaReceta.Take(int.Parse(receta.Cantidad.ToString())).ToList();
                                    }
                                    foreach (var preciosReceta in receta.RecetaReceta)
                                    {
                                        if (preciosReceta.Precios != null)
                                        {
                                            decimal otroPago = decimal.Parse(preciosReceta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                                            if (Utilities.dataTransaction.dataUser.Tarjeta != null && otroPago > 0)
                                            {
                                                precio += otroPago;
                                                Utilities.dataTransaction.PrecioCinefans = true;
                                            }
                                            else
                                            {
                                                precio += decimal.Parse(preciosReceta.Precios.FirstOrDefault().General.Split('.')[0]);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            foreach (var preciosReceta in combo.Precios)
                            {
                                if (Utilities.dataTransaction.dataUser.Tarjeta != null && preciosReceta.auxOtroPago > 0)
                                {
                                    precio = preciosReceta.auxOtroPago;
                                    Utilities.dataTransaction.PrecioCinefans = true;
                                }
                                else
                                {
                                    precio = preciosReceta.auxGeneral;
                                }
                            }
                        }
                        productos.Add(combo);
                    }
                    item.Price = precio;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProductsCombos>ChangePrices", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void ShowDetailModal()
        {
            try
            {
                frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal();
                this.Opacity = 0.3;
                _frmConfirmationModal.ShowDialog();
                if (_frmConfirmationModal.DialogResult.HasValue &&
                    _frmConfirmationModal.DialogResult.Value)
                {
                    GoToPay();
                }
                else
                {
                    ActivateTimer();
                    this.IsEnabled = true;
                    this.Opacity = 1;
                    //PaintDataCombo();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProductsCombos>ShowDetailModal", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private async void GoToPay()
        {
            FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
            frmLoading.Show();
            Utilities.ValidateUserBalance();
            var response = await Utilities.CreateTransaction("Cine ");
            frmLoading.Close();
            if (Utilities.eTypeBuy == ETypeBuy.JustConfectionery)
            {
                if (!GetSecuence())
                {
                    this.IsEnabled = true;
                    this.Opacity = 1;
                    return;
                }
            }

            if (!response)
            {
                Utilities.ShowModal("No se pudo crear la transacción, por favor intente de nuevo.");

                frmLoading = new FrmLoading("¡Reconectando...!");
                frmLoading.Show();
                await api.SecurityToken();
                frmLoading.Close();
                this.IsEnabled = true;
                this.Opacity = 1;
            }
            else
            {
                if (Utilities.dataTransaction.MedioPago == EPaymentType.Cash)
                {
                    Switcher.Navigate(new UCPayCine());
                }
                else if (Utilities.dataTransaction.MedioPago == EPaymentType.Card)
                {
                    Switcher.Navigate(new UCCardPayment());
                }
            }


        }

        private bool GetSecuence()
        {
            try
            {
                FrmLoading frmLoading = new FrmLoading("¡Generando secuencia de compra!");
                frmLoading.Show();

                var responseSec41 = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                    tercero = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.tercero
                });

                frmLoading.Close();
                if (responseSec41 == null)
                {
                    Task.Run(() =>
                    {
                        Utilities.SendMailErrores($"No se pudo obtener la secuencia de compra en la transaccion: {Utilities.IDTransactionDB}" +
                            $"");
                    });
                    Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");

                    return false;
                }

                foreach (var item in responseSec41)
                {
                    Utilities.dataTransaction.Secuencia = item.Secuencia.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProductsCombos>GetSecuence", JsonConvert.SerializeObject(ex), 1);
                Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");
                return false;
            }
        }

        private void DistributeProducts(object sender)
        {
            try
            {
                SetCallBacksNull();
                string TypeProduct;
                var data = (sender as Border).Tag;
                switch (data)
                {
                    case 1:
                        TypeProduct = "C";
                        Switcher.Navigate(new UCProducts(TypeProduct));
                        break;

                    case 2:
                        TypeProduct = "P";
                        Switcher.Navigate(new UCProducts(TypeProduct));
                        break;

                    case 3:
                        TypeProduct = "P";
                        Switcher.Navigate(new UCProducts(TypeProduct));
                        break;

                    case 4:
                        TypeProduct = "P";
                        Switcher.Navigate(new UCProducts(TypeProduct));
                        break;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>BtnCombos_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        #endregion

        #region "Timer"

        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.dataPaypad.PaypadConfiguration.generiC_TIMER;
                timer = new TimerTiempo(tbTimer.Text);
                timer.CallBackClose = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCinema());
                    });
                };
                timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        tbTimer.Text = response;
                    });
                };
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>ActivateTimer", JsonConvert.SerializeObject(ex), 1);
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
    }
}
