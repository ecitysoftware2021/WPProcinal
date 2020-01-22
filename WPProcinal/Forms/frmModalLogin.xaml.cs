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
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmModalLogin.xaml
    /// </summary>
    public partial class frmModalLogin : Window
    {
        Utilities utilities;
        public frmModalLogin()
        {
            InitializeComponent();
            utilities = new Utilities();
        }

        private async void BtIngresar_TouchDown(object sender, TouchEventArgs e)
        {
            Loguear();
        }

        private void Loguear()
        {
            try
            {
                WModal modal = new WModal();
                modal.Show();
                string user = txtUser.Text;
                string pass = txtPassword.Password;

                var t = Task.Run(() =>
                {
                    return ApiLocal.Login(user, pass).Result;
                });

                var c = t.ContinueWith((antecedent) => Dispatcher.BeginInvoke((Action)delegate
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        modal.Close();

                        if (antecedent.Result == null)
                        {
                            MessageBox.Show("Usuario y/o Contraseña incorrectos, intente de nuevo", "Usuario Incorrecto", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtPassword.Password = string.Empty;
                            return;
                        }
                        else
                        {
                            DialogResult = true;
                        }
                    }
                    else if (t.Status == TaskStatus.Faulted)
                    {
                        MessageBox.Show(t.Exception.GetBaseException().Message);
                    }
                }));
            }
            catch (Exception ex)
            {
            }
        }

        private void BtSalir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Loguear();
            }
        }

    }
}
