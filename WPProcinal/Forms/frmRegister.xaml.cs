using System;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmRegister.xaml
    /// </summary>
    public partial class frmRegister : Window
    {
        public frmRegister()
        {
            InitializeComponent();
            Utilities.dataDocument = new Models.DataDocument();
        }

        private void btOmitir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.control.callbackDocument = Document =>
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {
                        Utilities.control.callbackDocument = null;
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.dataDocument = Document;
                            gridCedula.Visibility = Visibility.Hidden;
                            gridDatos.Visibility = Visibility.Visible;
                        });

                    }
                };

                Utilities.control.num = 0;
                Utilities.control.InitializePortScanner(Utilities.GetConfiguration("PortScanner"));
            }
            catch (Exception ex)
            {
            }
        }

        private void txPhone_TouchDown(object sender, TouchEventArgs e)
        {
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = txPhone,
                eType = WPKeyboard.Keyboard.EType.Numeric,
                window = this,
                X = 250,
                Y = 780
            });
        }

        private void pxPassword_TouchDown(object sender, TouchEventArgs e)
        {
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = pxPassword,
                eType = WPKeyboard.Keyboard.EType.Standar,
                window = this,
                X = 250,
                Y = 860
            });
        }

        private void txPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txPhone.Text.Length > txPhone.MaxLength)
            {
                txPhone.Text = txPhone.Text.Substring(0, txPhone.Text.Length - 1);
            }
        }

        private void pxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pxPassword.Password.Length > pxPassword.MaxLength)
            {
                pxPassword.Password = pxPassword.Password.Substring(0, pxPassword.Password.Length - 1);
            }
        }

        private void txAddress_TouchDown(object sender, TouchEventArgs e)
        {
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = txAddress,
                eType = WPKeyboard.Keyboard.EType.Standar,
                window = this,
                X = 250,
                Y = 940
            });
        }

        private void txAddress_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txAddress.Text.Length > txAddress.MaxLength)
            {
                txAddress.Text = txAddress.Text.Substring(0, txAddress.Text.Length - 1);
            }
        }

        private void txMail_TouchDown(object sender, TouchEventArgs e)
        {
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = txMail,
                eType = WPKeyboard.Keyboard.EType.Standar,
                window = this,
                X = 250,
                Y = 1050
            });
        }

        private void txMail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txMail.Text.Length > txMail.MaxLength)
            {
                txMail.Text = txMail.Text.Substring(0, txMail.Text.Length - 1);
            }
        }
    }
}
