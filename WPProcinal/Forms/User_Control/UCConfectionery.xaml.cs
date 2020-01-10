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
        DipMap _dipMap;
        #endregion

        #region "Constructor"
        public UCConfectionery(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            try
            {
                _Seats = Seats;
                _dipMap = dipMap;
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
                        AddCombo("Combo Jumanji", 1000, C0);
                        break;
                    case 1:
                        AddCombo("Combo 1", 1000, C1);
                        break;
                    case 2:
                        AddCombo("Combo 2", 1000, C2);
                        break;
                    case 3:
                        AddCombo("Combo 3", 1000, C3);
                        break;
                    case 4:
                        AddCombo("Combo 4", 1000, C4);
                        break;
                    case 5:
                        AddCombo("Combo 5", 1000, C5);
                        break;
                    case 6:
                        AddCombo("Combo Hamburguesa", 1000, C6);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AddCombo(string comboName, decimal comboPrice, TextBlock textBlock)
        {
            int cantActual = int.Parse(textBlock.Text);
            cantActual++;
            if (cantActual <= 9)
            {
                textBlock.Text = cantActual.ToString();
                var cant = _Seats.Where(cb => cb.Name == comboName).FirstOrDefault();
                if (cant != null)
                {
                    cant.Quantity++;
                    cant.Price += comboPrice;
                }
                else
                {
                    _Seats.Add(new TypeSeat
                    {
                        Name = comboName,
                        Quantity = 1,
                        Price = comboPrice
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
                        DeleteCombo("Combo Jumanji", 1000, C0);
                        break;
                    case 1:
                        DeleteCombo("Combo 1", 1000, C1);
                        break;
                    case 2:
                        DeleteCombo("Combo 2", 1000, C2);
                        break;
                    case 3:
                        DeleteCombo("Combo 3", 1000, C3);
                        break;
                    case 4:
                        DeleteCombo("Combo 4", 1000, C4);
                        break;
                    case 5:
                        DeleteCombo("Combo 5", 1000, C5);
                        break;
                    case 6:
                        DeleteCombo("Combo Hamburguesa", 1000, C6);
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
            var cant = _Seats.Where(cb => cb.Name == comboName).FirstOrDefault();
            if (cant != null)
            {

                cant.Quantity--;
                cant.Price -= comboPrice;
                if (cant.Quantity == 0)
                {
                    _Seats.Remove(cant);
                }
            }
        }

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(_Seats, _dipMap);
            this.Opacity = 0.3;
            _frmConfirmationModal.ShowDialog();
            this.Opacity = 1;
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                if (Utilities.MedioPago == 1)
                {
                    Switcher.Navigate(new UCPayCine(_Seats, _dipMap));
                }
                else if (Utilities.MedioPago == 2)
                {
                    Switcher.Navigate(new UCCardPayment(_Seats, _dipMap));
                }
            }
            else
            {
                //cancela reserva
            }

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


    }
}
