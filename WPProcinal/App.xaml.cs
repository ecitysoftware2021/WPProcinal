using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace WPProcinal
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var ProcessApp = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);
            if (ProcessApp.Length > 1)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
