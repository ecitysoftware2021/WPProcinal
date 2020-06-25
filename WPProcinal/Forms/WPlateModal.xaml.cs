using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for WPlateModal.xaml
    /// </summary>
    public partial class WPlateModal : Window
    {
        public WPlateModal()
        {
            InitializeComponent();
        }

        private void BtnContinue_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.PLACA = txPlaca.Text.Trim();
            if (Utilities.PlateObligatory)
            {
                if (!string.IsNullOrEmpty(Utilities.PLACA) && Utilities.PLACA.Length > 5)
                {
                    DialogResult = true;
                }
                else
                {
                    lblInformation.Text = "INGRESE UNA PLACA VÁLIDA";
                }
            }
            else
            {
                DialogResult = true;
            }

        }

        private void Label_TouchDown(object sender, TouchEventArgs e)
        {
            var text = (sender as Label).Tag.ToString();
            if (text != ".")
            {
                if (txPlaca.Text.Length < 6)
                {
                    txPlaca.Text += text;
                }
            }
            else
            {
                if (txPlaca.Text.Length > 0)
                {
                    txPlaca.Text = txPlaca.Text.Substring(0, txPlaca.Text.Length - 1);
                }
            }
        }
    }
}
