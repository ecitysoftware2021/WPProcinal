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

        #region Precios Combos
        private int _ComboTemporada = int.Parse(Utilities.GetConfiguration("0"));
        private int _Combo1 = int.Parse(Utilities.GetConfiguration("1"));
        private int _Combo2 = int.Parse(Utilities.GetConfiguration("2"));
        private int _Combo3 = int.Parse(Utilities.GetConfiguration("3"));
        private int _Combo4 = int.Parse(Utilities.GetConfiguration("4"));
        private int _Combo5 = int.Parse(Utilities.GetConfiguration("5"));
        private int _ComboHamburguesa = int.Parse(Utilities.GetConfiguration("6"));
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
                        AddCombo(comboName: "Combo Jumanji", comboPrice: _ComboTemporada, textBlock: C0, code: "609.0");
                        break;
                    case 1:
                        AddCombo("Combo 1", _Combo1, C1, "251.0");
                        break;
                    case 2:
                        AddCombo("Combo 2", _Combo2, C2, "252.0");
                        break;
                    case 3:
                        AddCombo("Combo 3", _Combo3, C3, "253.0");
                        break;
                    case 4:
                        AddCombo("Combo 4", _Combo4, C4, "254.0");
                        break;
                    case 5:
                        AddCombo("Combo 5", _Combo5, C5, "256.0");
                        break;
                    case 6:
                        AddCombo(comboName: "Combo Hamburguesa", comboPrice: _ComboHamburguesa, textBlock: C6, code: "423.0");
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AddCombo(string comboName, decimal comboPrice, TextBlock textBlock, string code)
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
                        DeleteCombo("Combo Jumanji", _ComboTemporada, C0);
                        break;
                    case 1:
                        DeleteCombo("Combo 1", _Combo1, C1);
                        break;
                    case 2:
                        DeleteCombo("Combo 2", _Combo2, C2);
                        break;
                    case 3:
                        DeleteCombo("Combo 3", _Combo3, C3);
                        break;
                    case 4:
                        DeleteCombo("Combo 4", _Combo4, C4);
                        break;
                    case 5:
                        DeleteCombo("Combo 5", _Combo5, C5);
                        break;
                    case 6:
                        DeleteCombo("Combo Hamburguesa", _ComboHamburguesa, C6);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
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

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            ShowDetailModal();
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
                if (Utilities.MedioPago == 1)
                {
                    Switcher.Navigate(new UCPayCine(_Seats, _DipMap));
                }
                else if (Utilities.MedioPago == 2)
                {
                    Switcher.Navigate(new UCCardPayment(_Seats, _DipMap));
                }
            }
            else
            {
                Task.Run(() =>
                {
                    WCFServices41.PostDesAssingreserva(_Seats, _DipMap);
                });
                Switcher.Navigate(new UCCinema());
            }
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

        }
        #endregion

        #region "Métodos"

        #endregion

        #region "Timer"
        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerMovies");
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
    }
}
