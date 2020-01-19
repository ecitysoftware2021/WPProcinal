using Grabador.Transaccion;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCConfectionery.xaml
    /// </summary>
    public partial class UCConfectionery : UserControl
    {
        #region "Referencias"
        TimerTiempo timer;
        List<TypeSeat> _Seats;
        DipMap _DipMap;
        ApiLocal api;
        CLSGrabador grabador;
        Utilities utilities;

        #region Precios Combos
        private int _ComboTemporadaPrice = int.Parse(Utilities.GetConfiguration("0"));
        private int _Combo1Price = int.Parse(Utilities.GetConfiguration("1"));
        private int _Combo2Price = int.Parse(Utilities.GetConfiguration("2"));
        private int _Combo3Price = int.Parse(Utilities.GetConfiguration("3"));
        private int _Combo4Price = int.Parse(Utilities.GetConfiguration("4"));
        private int _Combo5Price = int.Parse(Utilities.GetConfiguration("5"));
        private int _ComboHamburguesaPrice = int.Parse(Utilities.GetConfiguration("6"));
        #endregion

        #endregion

        #region "Constructor"
        public UCConfectionery(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            try
            {
                Utilities._Combos = new List<Combos>();
                _Seats = Seats;
                _DipMap = dipMap;
                ActivateTimer();
                api = new ApiLocal();
                utilities = new Utilities();
                grabador = new CLSGrabador();
                Utilities.Speack("Puedes comprar tu combo y reclamarlo en la confitería.");
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Eventos"

        /// <summary>
        /// Evento para agregar cantidades
        /// 0= Temporada, 1= Combo1, 2=Combo2, 3=Combo3, 4=Combo4
        /// 5=Combo5, 6=Combo Hamburguesa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlusTemp_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int tag = int.Parse((sender as Image).Tag.ToString());
                int cantActual = 0;
                switch (tag)
                {
                    case 0:
                        AddCombo(comboName: "Combo Jumanji", comboPrice: _ComboTemporadaPrice, textBlock: C0, code: 609);
                        break;
                    case 1:
                        AddCombo("Combo 1", _Combo1Price, C1, 251);
                        break;
                    case 2:
                        AddCombo("Combo 2", _Combo2Price, C2, 252);
                        break;
                    case 3:
                        AddCombo("Combo 3", _Combo3Price, C3, 253);
                        break;
                    case 4:
                        AddCombo("Combo 4", _Combo4Price, C4, 254);
                        break;
                    case 5:
                        AddCombo("Combo 5", _Combo5Price, C5, 256);
                        break;
                    case 6:
                        AddCombo(comboName: "Combo Hamburguesa", comboPrice: _ComboHamburguesaPrice, textBlock: C6, code: 423);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            ChangeImageBuy();
        }

        private void AddCombo(string comboName, decimal comboPrice, TextBlock textBlock, int code)
        {
            int cantActual = int.Parse(textBlock.Text);
            cantActual++;
            if (cantActual <= 9)
            {
                textBlock.Text = cantActual.ToString();
                var existsCombo = Utilities._Combos.Where(cb => cb.Name == comboName).FirstOrDefault();

                if (existsCombo != null)
                {
                    existsCombo.Quantity++;
                    existsCombo.Price += comboPrice;

                }
                else
                {
                    Utilities._Combos.Add(new Combos
                    {
                        Name = comboName,
                        Quantity = 1,
                        Price = comboPrice,
                        Code = code
                    });
                }
            }
        }


        /// <summary>
        /// Evento para eliminar cantidades
        /// 0= Temporada, 1= Combo1, 2=Combo2, 3=Combo3, 4=Combo4
        /// 5=Combo5, 6=Combo Hamburguesa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLessTemp_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int tag = int.Parse((sender as Image).Tag.ToString());

                switch (tag)
                {
                    case 0:
                        DeleteCombo("Combo Jumanji", _ComboTemporadaPrice, C0);
                        break;
                    case 1:
                        DeleteCombo("Combo 1", _Combo1Price, C1);
                        break;
                    case 2:
                        DeleteCombo("Combo 2", _Combo2Price, C2);
                        break;
                    case 3:
                        DeleteCombo("Combo 3", _Combo3Price, C3);
                        break;
                    case 4:
                        DeleteCombo("Combo 4", _Combo4Price, C4);
                        break;
                    case 5:
                        DeleteCombo("Combo 5", _Combo5Price, C5);
                        break;
                    case 6:
                        DeleteCombo("Combo Hamburguesa", _ComboHamburguesaPrice, C6);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            ChangeImageBuy();
        }

        private void DeleteCombo(string comboName, decimal comboPrice, TextBlock textBlock)
        {
            int cantActual = int.Parse(textBlock.Text);
            textBlock.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
            var existsCombo = Utilities._Combos.Where(cb => cb.Name == comboName).FirstOrDefault();
            if (existsCombo != null)
            {

                existsCombo.Quantity--;
                existsCombo.Price -= comboPrice;
                if (existsCombo.Quantity == 0)
                {
                    Utilities._Combos.Remove(existsCombo);
                }
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

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            ChangePrices();
            ShowDetailModal();

        }

        public void ChangePrices()
        {
            List<Producto> productos = new List<Producto>();
            decimal precio = 0;
            foreach (var item in Utilities._Combos)
            {
                precio = 0;
                for (int i = 0; i < item.Quantity; i++)
                {
                    var combo = Utilities._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                    foreach (var receta in combo.Receta)
                    {
                        if (receta.Precios != null)
                        {
                            if (Utilities.dataUser.Tarjeta != null)
                            {
                                precio += decimal.Parse(receta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                            }
                            else
                            {
                                precio += decimal.Parse(receta.Precios.FirstOrDefault().General.Split('.')[0]);
                            }
                        }
                        if (receta.RecetaReceta != null)
                        {
                            receta.RecetaReceta = receta.RecetaReceta.Take(int.Parse(receta.Cantidad.ToString())).ToList();
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
                    productos.Add(combo);
                }
                item.Price = precio;
            }
        }

        private void ShowDetailModal()
        {
            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(_Seats, _DipMap);
            this.Opacity = 0.3;
            _frmConfirmationModal.ShowDialog();
            this.Opacity = 1;
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                GoToPay();
            }
            else
            {
                this.IsEnabled = true;
            }
        }

        #endregion

        #region "Métodos"
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

                LogService.SaveRequestResponse("=".PadRight(5, '=') + "Transacción de " + DateTime.Now + ": ", "ID: " + Utilities.IDTransactionDB);

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            var frmLoading = new FrmLoading("¡Cargando confiteria...!");

            frmLoading.Show();
            this.IsEnabled = false;
            Task.Run(() =>
            {
                var combos = WCFServices41.GetCombos(new SCOPRE
                {
                    teatro = Utilities.GetConfiguration("CodCinema"),
                    tercero = "1"
                });

                if (combos != null)
                {
                    Utilities._Productos = combos.ListaProductos;
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {

                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        frmModal frmModal = new frmModal("No se pudo descargar la confitería, continúa el pago de tus boletas!");
                        frmModal.ShowDialog();
                        ShowDetailModal();
                    });
                }

                Dispatcher.BeginInvoke((Action)delegate
                {
                    frmLoading.Close();
                    this.IsEnabled = true;
                });
            });

        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

            Task.Run(() =>
            {
                WCFServices41.PostDesAssingreserva(_Seats, _DipMap);
            });
            Switcher.Navigate(new UCCinema());
        }
    }
}
