using Grabador.Transaccion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCProducts.xaml
    /// </summary>
    public partial class UCProducts : UserControl
    {
        #region "Referencias"
        private TimerTiempo timer;
        private List<TypeSeat> _Seats;
        private DipMap _DipMap;
        private CollectionViewSource view;
        private ObservableCollection<Producto> lstPager;
        ApiLocal api;
        CLSGrabador grabador;
        Utilities utilities;
        private int _Combo4Code = int.Parse(Utilities.GetConfiguration("4Code"));
        #endregion

        #region "Constructor"
        public UCProducts(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            _Seats = Seats;
            _DipMap = dipMap;
            api = new ApiLocal();
            utilities = new Utilities();
            grabador = new CLSGrabador();
            this.view = new CollectionViewSource();
            this.lstPager = new ObservableCollection<Producto>();
            ActivateTimer();
            InitView();
            PaintDataCombo();
        }
        #endregion

        #region "ListView"
        private void InitView()
        {
            try
            {
                foreach (var product in Utilities._Productos)
                {

                    if (product.Tipo.ToUpper() == "P")
                    {
                        if (product.Precios.Count() > 0)
                        {
                            decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                            decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);
                            product.Imagen = $"http://localhost/Procinal/images/{product.Codigo}.jpg";
                            if (General > 0 && OtroPago > 0)
                            {
                                product.Precios[0].auxGeneral = General;
                                product.Precios[0].auxOtroPago = OtroPago;

                                lstPager.Add(product);
                            }
                        }
                    }
                }

                view.Source = lstPager;
                lv_Products.DataContext = view;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Eventos"
        private void BtnPlus_TouchDown(object sender, TouchEventArgs e)
        {
            IncrementDecrementProducts(sender, true);
        }
        private void BtnLess_TouchDown(object sender, TouchEventArgs e)
        {
            IncrementDecrementProducts(sender, false);
        }

        private void IncrementDecrementProducts(object sender, bool Operation)
        {
            try
            {
                var data = ((sender as Image).DataContext as Producto);

                if (Operation)
                {
                    Utilities.AddCombo(new Combos
                    {
                        Name = data.Descripcion,
                        Code = Convert.ToInt32(data.Codigo),
                        Price = Utilities.dataUser.Tarjeta != null ? data.Precios[0].auxOtroPago : data.Precios[0].auxGeneral,
                        dataProduct = data,
                        isCombo = false,

                    });
                }
                else
                {
                    Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: data.Precios[0].auxOtroPago, dataProduct: data);
                }

                ChangeImageBuy();
            }
            catch (Exception ex)
            {
            }
        }

        private void BtnCombos_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Switcher.Navigate(new UCProductsCombos(_Seats, _DipMap));
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            this.IsEnabled = false;
            var frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
            Utilities.Loading(frmLoading, true, this);
            Utilities.CancelAssing(_Seats, _DipMap);
            Utilities.Loading(frmLoading, false, this);
            this.IsEnabled = true;
            Switcher.Navigate(new UCCinema());
        }

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            ChangePrices();
            ShowDetailModal();
        }
        #endregion

        #region "Métodos"
        private void ShowDetailModal()
        {
            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(_Seats, _DipMap);
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
                PaintDataCombo();
            }
        }

        private void ChangeImageBuy()
        {
            if (Utilities._Combos.Count > 0)
            {
                BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/continuar.png", UriKind.Relative));
            }
            else
            {
                BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/omitir.png", UriKind.Relative));
            }
        }

        private void PaintDataCombo()
        {
            List<long> codesOk = new List<long>();

            foreach (var item in Utilities._Combos)
            {
                if (!item.isCombo)
                {
                    foreach (var list in lstPager)
                    {
                        if (item.Code == list.Codigo)
                        {
                            list.Value = item.Quantity;
                            lv_Products.Items.Refresh();
                            codesOk.Add(list.Codigo);
                        }
                    }
                }
            }
            foreach (var item in lstPager)
            {
                if (codesOk.IndexOf(item.Codigo) < 0)
                {
                    item.Value = 0;
                }
            }
            ChangeImageBuy();
        }
        public void ChangePrices()
        {
            List<Producto> productos = new List<Producto>();
            //Utilities.dataUser = new SCOLOGResponse();
            decimal precio = 0;
            foreach (var item in Utilities._Combos)
            {
                precio = 0;
                for (int i = 0; i < item.Quantity; i++)
                {
                    var combo = Utilities._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                    if (combo.Receta != null)
                    {
                        foreach (var receta in combo.Receta)
                        {
                            if (receta.Precios != null)
                            {
                                if (Utilities.dataUser.Tarjeta != null)
                                {
                                    precio += decimal.Parse(receta.Precios.FirstOrDefault().OtroPago.Split('.')[0]) * receta.Cantidad;
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
                                    if (item.Code != _Combo4Code)
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
                                        if (Utilities.dataUser.Tarjeta != null)
                                        {
                                            precio += decimal.Parse(preciosReceta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
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
                            if (Utilities.dataUser.Tarjeta != null)
                            {
                                precio = preciosReceta.auxOtroPago * item.Quantity;
                            }
                            else
                            {
                                precio = preciosReceta.auxGeneral * item.Quantity;
                            }
                        }
                    }
                    productos.Add(combo);
                }
                item.Price = precio;
            }
        }

        private async void GoToPay()
        {
            FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
            frmLoading.Show();
            var response = await utilities.CreateTransaction("Cine ", _DipMap, _Seats);
            frmLoading.Close();

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
                try
                {
                    Task.Run(() =>
                    {
                        grabador.Grabar(Utilities.IDTransactionDB);
                    });
                }
                catch { }

                if (Utilities.MedioPago == 1)
                {
                    Switcher.Navigate(new UCPayCine(_Seats, _DipMap));
                }
                else if (Utilities.MedioPago == 2)
                {
                    Switcher.Navigate(new UCCardPayment(_Seats, _DipMap));
                }
            }


        }

        #endregion

        #region "Timer"
        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerConfiteria");
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
            catch { }
        }

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }
        #endregion
    }
}
