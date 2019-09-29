﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCFinalTransaction.xaml
    /// </summary>
    public partial class UCFinalTransaction : UserControl
    {
        public UCFinalTransaction()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.GoToInicial();
                });
            });
        }
    }
}