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
                            //Validate();
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
    }
}
