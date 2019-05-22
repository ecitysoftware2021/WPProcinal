﻿using Newtonsoft.Json;
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
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for ConfigurateWindow.xaml
    /// </summary>
    public partial class frmConfigurate : Window
    {
        ApiLocal api;
        bool state;

        public frmConfigurate()
        {
            InitializeComponent();
            api = new ApiLocal();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                GetToken();
            });
        }

        /// <summary>
        /// Método encargado de obtener el token necesario para que el corresponsal pueda operar, seguido de esto se consulta el estado inicial del corresponsal
        /// para saber si se pueden realizar transacciones
        /// </summary>
        private async void GetToken()
        {
            try
            {
                //Utilities util = new Utilities(1);
                //state = await api.SecurityToken();
                //if (state)
                //{
                //    var response = await api.GetResponse(new Uptake.RequestApi(), "InitPaypad");
                //    if (response.CodeError == 200)
                //    {
                //        DataPaypad data = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());

                //        if (data.State)
                //        {
                //            if (data.StateAceptance && data.StateDispenser)
                //            {
                //                Utilities.control.callbackError = error =>
                //                {
                //                    GetToken();
                //                };
                //                Utilities.control.callbackToken = isSucces =>
                //                {
                                    Dispatcher.BeginInvoke((Action)delegate
                                    {
                                        frmCinema inicio = new frmCinema();
                                        inicio.Show();
                                        Close();
                                    });
                //                };
                //                Utilities.control.Start();
                //            }
                //            else
                //            {
                //                ShowModalError("No están disponibles los billeteros");
                //            }
                //        }
                //        else
                //        {
                //            ShowModalError("No se pudo verificar el estado de los periféricos");
                //        }
                //    }
                //    else
                //    {
                //        ShowModalError("No se pudo iniciar el cajero");
                //    }
                //}
                //else
                //{
                //    ShowModalError("No se pudo verificar las credenciales.");
                //}
            }
            catch (Exception ex)
            {
                ShowModalError(ex.Message, ex.StackTrace);
            }
        }

        private void ShowModalError(string description, string message = "")
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                //Modal modal = new Modal(string.Concat("Lo sentimos,", Environment.NewLine, "el cajero no se encuentra disponible.\nError: ", description), false);
                //modal.ShowDialog();
                //if (modal.DialogResult.HasValue)
                //{
                //    ConfigurateWindow configurate = new ConfigurateWindow();
                //    configurate.Show();
                //    this.Close();
                //}
            });
        }
    }
}