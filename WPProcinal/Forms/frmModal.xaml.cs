using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmModal.xaml
    /// </summary>
    public partial class frmModal : Window
    {
        private TimerTiempo timer;
        private bool stop;

        public frmModal(string message, bool timer = false, bool balance = false, bool CineFan = false)
        {
            InitializeComponent();
            LblMessage.Text = message;
            this.stop = timer;
            if (CineFan)
            {
                BtnSalir.Source = new BitmapImage(new Uri(@"/Images/Buttons/ContinuaCompra.png", UriKind.Relative));
            }

            if (balance)
            {
                BtnSalir.Visibility = Visibility.Hidden;
                BtnSi.Visibility = Visibility.Visible;
                BtnNo.Visibility = Visibility.Visible;
            }
            if (stop)
            {
                BtnSalir.Visibility = Visibility.Hidden;
                ActivateTimer();
            }
        }


        #region "Timer"
        private void ActivateTimer()
        {
            timer = new TimerTiempo(Utilities.GetConfiguration("TimerModal"));
            timer.CallBackClose = response =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    DialogResult = true;
                });
            };
        }

        private void SetCallBacksNull()
        {
            try
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
            }
            catch { }
        }
        #endregion

        private void BtnNo_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnSi_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

            BtnSalir.IsEnabled = false;
            DialogResult = true;
        }
    }
}
