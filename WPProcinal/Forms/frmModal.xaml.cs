using System;
using System.Windows;
using System.Windows.Input;
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

        public frmModal(string message,bool timer = false)
        {
            InitializeComponent();
            LblMessage.Text = message;
            this.stop = timer;

            if (stop)
            {
                BtnEnd.Visibility = Visibility.Hidden;
                ActivateTimer();
            }
        }

        private void BtnEnd_TouchDown(object sender, TouchEventArgs e)
        {
            BtnEnd.IsEnabled = false;
            DialogResult = true;
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
    }
}
