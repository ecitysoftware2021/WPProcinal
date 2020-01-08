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
using WPProcinal.Keyboard;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCDatos.xaml
    /// </summary>
    public partial class UCDatos : UserControl
    {
        #region "Referencias"
        private TimerTiempo timer;
        private DipMap dipMapCurrent;
        private List<TypeSeat> Seats;
        private string mensaje;
        #endregion

        #region "Constructor"
        public UCDatos(List<TypeSeat> seats, DipMap dipMap)
        {
            InitializeComponent();

            try
            {
                TouchScreenKeyboard.PositionX = 200;
                Seats = seats;
                dipMapCurrent = dipMap;
                Utilities.dataDocument = new DataDocument();
                TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
                TxtDay.Text = dipMap.Day;
                TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
                TxtHour.Text = dipMap.HourFunction;
                TxtSubTitle.Text = dipMap.Language;
                var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
                TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);
                ActivateTimer();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Eventos"
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.control.callbackDocument = Document =>
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            txtIdentificationNit.Text = Document.Document;
                            txtNombre.Text = Document.FirstName + " " + Document.SecondName;
                            txtApellido.Text = Document.LastName + " " + Document.SecondLastName;
                        });
                        Utilities.dataDocument.Date = Document.Date;
                        Utilities.control.callbackDocument = null;
                        Utilities.control.ClosePortScanner();
                    }
                };

                Utilities.control.num = 0;
                Utilities.control.InitializePortScanner(Utilities.GetConfiguration("PortScanner"));
            }
            catch (Exception ex)
            {
            }
        }

        private void Pay_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    Utilities.dataDocument.Document = txtIdentificationNit.Text;
                    Utilities.dataDocument.FirstName = txtNombre.Text;
                    Utilities.dataDocument.LastName = txtApellido.Text;
                    Utilities.dataDocument.Phone = txtTelefono.Text;
                    Utilities.dataDocument.Email = txtCorreo.Text;

                    if (Utilities.MedioPago == 1)
                    {
                        try
                        {
                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                        }
                        catch { }

                        LogService.SaveRequestResponse("=".PadRight(5, '=') + "Transacción de " + DateTime.Now + ": ", "ID: " + Utilities.IDTransactionDB);
                        Utilities.controlStop = 0;
                        int i = 0;

                        Switcher.Navigate(new UCPayCine(Seats, dipMapCurrent));
                    }
                    else
                    {
                        try
                        {
                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                        }
                        catch { }
                        Switcher.Navigate(new UCCardPayment(Seats, dipMapCurrent));
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Cancelled_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                CancelledTransaction();
            }
            catch (Exception ex)
            {
            }
        }

        private void Txt_TextChanged_Num(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox txt = (TextBox)sender;
                int texto = int.Parse(txt.Tag.ToString());

                if (txt.Text.Length == 1 && txt.Text == "0")
                {
                    txt.Text = string.Empty;
                }

                switch (texto)
                {
                    case 1:
                        {
                            if (txtIdentificationNit.Text.Length > 12)
                            {
                                txtIdentificationNit.Text = txtIdentificationNit.Text.Remove(12, 1);
                                return;
                            }
                        }
                        break;
                    case 3:
                        {
                            if (txtTelefono.Text.Length > 10)
                            {
                                txtTelefono.Text = txtTelefono.Text.Remove(10, 1);
                                return;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Txt_TextChanged_Email(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox textBox = (TextBox)sender;
                string text = textBox.Text;
                int length = text.Length;
                if (length <= 47)
                {

                }
                else
                {
                    textBox.Text = text.Remove(text.Length - 1);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Txt_TextChanged_Char(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox textBox = (TextBox)sender;
                string text = textBox.Text;
                int length = text.Length;
                if (length <= 20)
                {
                    if (length >= 1)
                    {
                        char last = text[text.Length - 1];
                        int character = Convert.ToInt32(last);
                        if ((character >= 65 && character <= 90) || (character >= 97 && character <= 122) || (character == 32) || (character == 241))
                            e.Handled = false;
                        else
                            textBox.Text = text.Remove(text.Length - 1);
                    }
                }
                else
                {
                    textBox.Text = text.Remove(text.Length - 1);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Métodos"
        private void EmptyData()
        {
            try
            {
                TxtErrorID.Text = string.Empty;
                TxtErrorEmail.Text = string.Empty;
                TxtErrorLastName.Text = string.Empty;
                TxtErrorName.Text = string.Empty;
                TxtErrorPhone.Text = string.Empty;
            }
            catch (Exception ex)
            {
            }
        }

        private bool ValidarCampos()
        {
            try
            {
                if (string.IsNullOrEmpty(txtIdentificationNit.Text))
                {
                    mensaje = "Debe ingresar una Identificación";
                    TxtErrorID.Text = mensaje;
                    return false;
                }
                else
                if (txtIdentificationNit.Text.Length < 6)
                {
                    mensaje = "Debe ingresar una Identificación válida";
                    TxtErrorID.Text = mensaje;
                    return false;
                }
                else
                if (string.IsNullOrEmpty(txtCorreo.Text))
                {
                    mensaje = "Debe ingresar un correo electrónico";
                    TxtErrorEmail.Text = mensaje;
                    return false;
                }
                else
                if (!Utilities.IsValidEmailAddress(txtCorreo.Text))
                {
                    mensaje = "No se indicó un correo válido";
                    TxtErrorEmail.Text = mensaje;
                    return false;
                }
                else
                if (string.IsNullOrEmpty(txtTelefono.Text))
                {
                    mensaje = "Debe ingresar un teléfono";
                    TxtErrorPhone.Text = mensaje;
                    return false;
                }
                else
                if (txtTelefono.Text.Length < 7)
                {
                    mensaje = "Debe ingresar un teléfono válido";
                    TxtErrorPhone.Text = mensaje;
                    return false;
                }
                else
                if (string.IsNullOrEmpty(txtNombre.Text))
                {
                    mensaje = "Debe ingresar un nombre";
                    TxtErrorName.Text = mensaje;
                    return false;
                }
                else
                if (txtNombre.Text.Length < 3)
                {
                    mensaje = "Debe ingresar un nombre válido";
                    TxtErrorName.Text = mensaje;
                    return false;
                }
                else
                if (string.IsNullOrEmpty(txtApellido.Text))
                {
                    mensaje = "Debe ingresar un apellido";
                    TxtErrorLastName.Text = mensaje;
                    return false;
                }
                else
                if (txtApellido.Text.Length < 3)
                {
                    mensaje = "Debe ingresar un apellido válido";
                    TxtErrorLastName.Text = mensaje;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void CancelledTransaction()
        {
            try
            {
                Utilities.control.callbackDocument = null;
                Utilities.control.ClosePortScanner();

                Task.Run(() =>
                {
                    Utilities.UpdateTransaction(0, 3, 0);
                });

                var frmLoading = new FrmLoading("¡Cancelando transacción...!");
                frmLoading.Show();
                WCFServices41.PostDesAssingreserva(Seats, dipMapCurrent);
                frmLoading.Close();

                Utilities.GoToInicial();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region "Timer"
        void ActivateTimer()
        {
            tbTimer.Text = Utilities.GetConfiguration("TimerHorario");
            timer = new TimerTiempo(tbTimer.Text);
            timer.CallBackClose = response =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    CancelledTransaction();
                });
            };
            timer.CallBackTimer = response =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "Tiempo de transacción: " + response;
                    tbHoraActual.Text = "Hora actual: " + DateTime.Now.ToString("hh:mm:ss");
                });
            };
        }

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }
        #endregion
    }
}
