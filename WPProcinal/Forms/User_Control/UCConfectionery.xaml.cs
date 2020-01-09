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

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCConfectionery.xaml
    /// </summary>
    public partial class UCConfectionery : UserControl
    {
        #region "Referencias"
        TimerTiempo timer;
        #endregion

        #region "Constructor"
        public UCConfectionery()
        {
            InitializeComponent();

            try
            {
                ActivateTimer();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Eventos"
        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {

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

        private void BtnPlusTemp_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void BtnLessTemp_TouchDown(object sender, TouchEventArgs e)
        {

        }
    }
}
