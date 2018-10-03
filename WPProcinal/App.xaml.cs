using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPProcinal.Classes;

namespace WPProcinal
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var task2 = Task.Run(() =>
            {
                GetScreen();
            });
        }

        private void GetScreen()
        {
            ControlPantalla.ConsultarControlPantalla();
        }
    }
}
