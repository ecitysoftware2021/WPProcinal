using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmRegister.xaml
    /// </summary>
    public partial class frmRegister : Window
    {
        private bool ShowPass = false;
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
            txPhone.Background = Brushes.Transparent;
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = txPhone,
                eType = WPKeyboard.Keyboard.EType.Numeric,
                window = this,
                X = 250,
                Y = 780
            });
        }
        private void txPhoneVerification_TouchDown(object sender, TouchEventArgs e)
        {
            txPhoneVerification.Background = Brushes.Transparent;
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = txPhoneVerification,
                eType = WPKeyboard.Keyboard.EType.Numeric,
                window = this,
                X = 250,
                Y = 780
            });
        }

        private void pxPassword_TouchDown(object sender, TouchEventArgs e)
        {
            pxPassword.Background = Brushes.Transparent;
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

        private void txPhoneVerification_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txPhoneVerification.Text.Length > txPhoneVerification.MaxLength)
            {
                txPhoneVerification.Text = txPhoneVerification.Text.Substring(0, txPhoneVerification.Text.Length - 1);
            }
        }

        private void pxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pxPassword.Password.Length > pxPassword.MaxLength)
            {
                pxPassword.Password = pxPassword.Password.Substring(0, pxPassword.Password.Length - 1);
            }
            txPasswordShow.Text = pxPassword.Password;
        }

        private void imgShowPass_TouchDown(object sender, TouchEventArgs e)
        {
            ShowPass = !ShowPass;
            if (ShowPass)
            {
                txPasswordShow.Visibility = Visibility.Visible;
                pxPassword.Visibility = Visibility.Hidden;
                imgShowPass.Opacity = 1;
            }
            else
            {
                txPasswordShow.Visibility = Visibility.Hidden;
                pxPassword.Visibility = Visibility.Visible;
                imgShowPass.Opacity = 0.3;
            }
        }

        private void txPasswordShow_TouchDown(object sender, TouchEventArgs e)
        {
            txPasswordShow.Background = Brushes.Transparent;
            WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
            {
                control = pxPassword,
                eType = WPKeyboard.Keyboard.EType.Standar,
                window = this,
                X = 250,
                Y = 860
            });
        }

        private void txAddress_TouchDown(object sender, TouchEventArgs e)
        {
            txAddress.Background = Brushes.Transparent;
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
            txMail.Background = Brushes.Transparent;
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

        private void btSiguiente_TouchDown(object sender, TouchEventArgs e)
        {
            if (ValidateControls())
            {
                bool regState = false;
                Utilities.dataDocument.Address = txAddress.Text;
                Utilities.dataDocument.Phone = txPhone.Text;
                Utilities.dataDocument.Key = pxPassword.Password;
                Utilities.dataDocument.Email = txMail.Text;
                int edad = 18;
                try
                {
                    edad = DateTime.Now.Year - int.Parse(Utilities.dataDocument.Date.Substring(0, 4));
                }
                catch
                {
                }
                FrmLoading frmLoading = new FrmLoading("¡Realizando registro...!");
                frmLoading.Show();
                Task.Run(() =>
                {
                    var responseReg = WCFServices41.PersonRegister(new SCOCYA
                    {
                        Apellido = string.Concat(Utilities.dataDocument.LastName, " ", Utilities.dataDocument.SecondLastName),
                        Celular = Utilities.dataDocument.Phone,
                        Clave = Utilities.dataDocument.Key,
                        Login = Utilities.dataDocument.Email,
                        Direccion = Utilities.dataDocument.Address,
                        Documento = Utilities.dataDocument.Document,
                        Edad = edad,
                        Fecha_Nacimiento = Utilities.dataDocument.Date,
                        Nombre = string.Concat(Utilities.dataDocument.FirstName, " ", Utilities.dataDocument.SecondName),
                        Sexo = Utilities.dataDocument.Gender,
                    });

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        frmLoading.Close();
                    });

                    foreach (var item in responseReg)
                    {
                        if (item.Respuesta != null)
                        {
                            if (item.Respuesta.Contains("éxito"))
                            {
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    frmModal modal = new frmModal($"¡Bienvenido!{Environment.NewLine}Ahora eres un CineFans", CineFan: true);
                                    modal.ShowDialog();
                                });
                                regState = true;
                                break;
                            }
                            else
                            {
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    frmModal modal = new frmModal(item.Respuesta);
                                    modal.ShowDialog();
                                });
                                regState = false;
                                break;
                            }
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                frmModal modal = new frmModal(item.Validacion);
                                modal.ShowDialog();
                            });
                            regState = false;
                            break;
                        }
                    }

                    if (regState)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {

                            DialogResult = true;
                        });
                    }
                    else
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            DialogResult = false;
                        });
                    }
                });
            }
        }
        private bool ValidateControls()
        {
            bool result = true;
            if (txPhone.Text.Length < 8)
            {
                txPhone.Background = Brushes.Red;
                result = false;
            }
            if (txPhoneVerification.Text != txPhone.Text)
            {
                txPhoneVerification.Background = Brushes.Red;
                result = false;
            }
            if (pxPassword.Password.Length < 5)
            {
                pxPassword.Background = Brushes.Red;
                txPasswordShow.Background = Brushes.Red;
                result = false;
            }
            if (txAddress.Text.Length < 8)
            {
                txAddress.Background = Brushes.Red;
                result = false;
            }
            if (!ValidateMail(txMail.Text))
            {
                txMail.Background = Brushes.Red;
                result = false;
            }
            return result;
        }

        private bool ValidateMail(string mail)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(mail);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
