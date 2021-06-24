using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        }

        private void btOmitir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.controlScanner.callbackDocument = Document =>
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {
                        Utilities.controlScanner.callbackDocument = null;
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.dataTransaction.dataDocument = Document;
                            gridCedula.Visibility = Visibility.Hidden;
                            gridDatos.Visibility = Visibility.Visible;
                        });

                    }
                };

                Utilities.controlScanner.num = 0;
                Utilities.controlScanner.InitializePortScanner(Utilities.dataPaypad.PaypadConfiguration.scanneR_PORT);
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>Window_Loaded", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txPhone_TouchDown(object sender, TouchEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txPhone_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }
        private void txPhoneVerification_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                txPhoneVerification.Background = Brushes.Transparent;
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = txPhoneVerification,
                    eType = WPKeyboard.Keyboard.EType.Numeric,
                    window = this,
                    X = 250,
                    Y = 850
                });
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txPhoneVerification_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void pxPassword_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                pxPassword.Background = Brushes.Transparent;
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = pxPassword,
                    eType = WPKeyboard.Keyboard.EType.Standar,
                    window = this,
                    X = 250,
                    Y = 930
                });
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>pxPassword_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (txPhone.Text.Length > txPhone.MaxLength)
                {
                    txPhone.Text = txPhone.Text.Substring(0, txPhone.Text.Length - 1);
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txPhone_TextChanged", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txPhoneVerification_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (txPhoneVerification.Text.Length > txPhoneVerification.MaxLength)
                {
                    txPhoneVerification.Text = txPhoneVerification.Text.Substring(0, txPhoneVerification.Text.Length - 1);
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txPhoneVerification_TextChanged", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void pxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pxPassword.Password.Length > pxPassword.MaxLength)
                {
                    pxPassword.Password = pxPassword.Password.Substring(0, pxPassword.Password.Length - 1);
                }
                txPasswordShow.Text = pxPassword.Password;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>pxPassword_PasswordChanged", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void imgShowPass_TouchDown(object sender, TouchEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>imgShowPass_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txPasswordShow_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                txPasswordShow.Background = Brushes.Transparent;
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = pxPassword,
                    eType = WPKeyboard.Keyboard.EType.Standar,
                    window = this,
                    X = 250,
                    Y = 930
                });
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txPasswordShow_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txMail_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                txMail.Background = Brushes.Transparent;
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = txMail,
                    eType = WPKeyboard.Keyboard.EType.Standar,
                    window = this,
                    X = 250,
                    Y = 1020
                });
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txMail_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void txMail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (txMail.Text.Length > txMail.MaxLength)
                {
                    txMail.Text = txMail.Text.Substring(0, txMail.Text.Length - 1);
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmRegister>txMail_TextChanged", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void btSiguiente_TouchDown(object sender, TouchEventArgs e)
        {
            if (ValidateControls())
            {
                try
                {
                    bool regState = false;
                    Utilities.dataTransaction.dataDocument.Phone = txPhone.Text;
                    Utilities.dataTransaction.dataDocument.Key = pxPassword.Password;
                    Utilities.dataTransaction.dataDocument.Email = txMail.Text;
                    int edad = 18;
                    try
                    {
                        edad = DateTime.Now.Year - int.Parse(Utilities.dataTransaction.dataDocument.Date.Substring(0, 4));
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
                            Apellido = string.Concat(Utilities.dataTransaction.dataDocument.LastName, " ", Utilities.dataTransaction.dataDocument.SecondLastName),
                            Celular = Utilities.dataTransaction.dataDocument.Phone,
                            Clave = Utilities.dataTransaction.dataDocument.Key,
                            Login = Utilities.dataTransaction.dataDocument.Email,
                            Direccion = Utilities.dataTransaction.dataDocument.Address,
                            Documento = Utilities.dataTransaction.dataDocument.Document,
                            Edad = edad,
                            Fecha_Nacimiento = Utilities.dataTransaction.dataDocument.Date,
                            Nombre = string.Concat(Utilities.dataTransaction.dataDocument.FirstName, " ", Utilities.dataTransaction.dataDocument.SecondName),
                            Sexo = Utilities.dataTransaction.dataDocument.Gender,
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
                catch (Exception ex)
                {
                    LogService.SaveRequestResponse("frmRegister>btSiguiente_TouchDown", JsonConvert.SerializeObject(ex), 1);
                }
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
