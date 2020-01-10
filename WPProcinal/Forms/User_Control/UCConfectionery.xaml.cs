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
        /// Evento para eliminar cantidades
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
                        cantActual = int.Parse(C0.Text);
                        cantActual++;
                        if (cantActual <= 9)
                        {
                            C0.Text = cantActual.ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo Jumanji").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price += 1000;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo Jumanji",
                                    Quantity = 1,
                                    Price = 1000
                                });
                            }
                        }
                        break;
                    case 1:
                        cantActual = int.Parse(C1.Text);
                        if (cantActual <= 9)
                        {
                            C1.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo 1").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo 1",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                    case 2:
                        cantActual = int.Parse(C2.Text);
                        if (cantActual <= 9)
                        {
                            C2.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo 2").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo 2",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                    case 3:
                        cantActual = int.Parse(C3.Text);
                        if (cantActual <= 9)
                        {
                            C3.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo 3").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo 3",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                    case 4:
                        cantActual = int.Parse(C4.Text);
                        if (cantActual <= 9)
                        {
                            C4.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo 4").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo 4",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                    case 5:
                        cantActual = int.Parse(C5.Text);
                        if (cantActual <= 9)
                        {
                            C5.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo 5").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo 5",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                    case 6:
                        cantActual = int.Parse(C6.Text);
                        if (cantActual <= 9)
                        {
                            C6.Text = (cantActual + 1).ToString();
                            var cant = _Seats.Where(cb => cb.Name == "Combo Hamburguesa").FirstOrDefault();
                            if (cant != null)
                            {
                                cant.Quantity++;
                                cant.Price = cant.Quantity * cant.Price;
                            }
                            else
                            {
                                _Seats.Add(new TypeSeat
                                {
                                    Name = "Combo Hamburguesa",
                                    Quantity = 1,
                                    Price = 2000
                                });
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

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
                int cantActual = 0;
                switch (tag)
                {
                    case 0:
                        cantActual = int.Parse(C0.Text);
                        C0.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        var cant = _Seats.Where(cb => cb.Name == "Combo Jumanji").FirstOrDefault();
                        if (cant != null)
                        {

                            cant.Quantity--;
                            cant.Price = cant.Quantity * cant.Price;
                            if (cant.Quantity == 0)
                            {
                                _Seats.Remove(cant);
                            }
                        }
                        break;
                    case 1:
                        cantActual = int.Parse(C1.Text);
                        C1.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                    case 2:
                        cantActual = int.Parse(C2.Text);
                        C2.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                    case 3:
                        cantActual = int.Parse(C3.Text);
                        C3.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                    case 4:
                        cantActual = int.Parse(C4.Text);
                        C4.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                    case 5:
                        cantActual = int.Parse(C5.Text);
                        C5.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                    case 6:
                        cantActual = int.Parse(C6.Text);
                        C6.Text = cantActual != 0 ? (cantActual - 1).ToString() : "0";
                        break;
                }
            }
            catch (Exception ex)
            {

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
