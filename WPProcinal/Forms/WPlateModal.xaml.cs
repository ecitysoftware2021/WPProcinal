using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for WPlateModal.xaml
    /// </summary>
    public partial class WPlateModal : Window
    {
        string[] TiposAuto;
        public WPlateModal()
        {
            InitializeComponent();
            LoadTypeAuto();
        }

        private void LoadTypeAuto()
        {
            try
            {
                TiposAuto = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.TiposAutos.Split(';');
                var dataSource = new List<TypeAuto>();
                foreach (var item in TiposAuto)
                {
                    dataSource.Add(new TypeAuto() { Name = item, Value = item });
                }
                cb_TypeAuto.ItemsSource = dataSource;
                cb_TypeAuto.DisplayMemberPath = "Name";
                cb_TypeAuto.SelectedValuePath = "Value";
                cb_TypeAuto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("WPlateModal>LoadTypeAuto", JsonConvert.SerializeObject(ex), 1);
            }

        }

        private void BtnContinue_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Utilities.dataTransaction.PLACA = txPlaca.Text.Trim();
                Utilities.dataTransaction.TIPOAUTO = cb_TypeAuto.SelectedValue.ToString();

                if (Utilities.PlateObligatory)
                {
                    if (!string.IsNullOrEmpty(Utilities.dataTransaction.PLACA) && Utilities.dataTransaction.PLACA.Length > 5)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        lblInformation.Text = "INGRESE UNA PLACA VÁLIDA";
                    }
                }
                else
                {
                    DialogResult = true;
                }

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("WPlateModal>BtnContinue_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void Label_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var text = (sender as Label).Tag.ToString();
                if (text != ".")
                {
                    if (txPlaca.Text.Length < 6)
                    {
                        txPlaca.Text += text;
                    }
                }
                else
                {
                    if (txPlaca.Text.Length > 0)
                    {
                        txPlaca.Text = txPlaca.Text.Substring(0, txPlaca.Text.Length - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("WPlateModal>Label_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }
    }
    public class TypeAuto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
