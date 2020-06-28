using System;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for WTCModal.xaml
    /// </summary>
    public partial class WTCModal : Window
    {
        public WTCModal(string message, string url = "")
        {
            InitializeComponent();
            LblMessage.Text = string.Format(message, Environment.NewLine + Environment.NewLine);
            LblURL.Text = url;
        }

        private void BtnEnd_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
