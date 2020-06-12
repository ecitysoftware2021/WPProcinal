using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;
using System;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmConfirmationModal.xaml
    /// </summary>
    public partial class frmConfirmationModal : Window
    {
        #region Variables Locales
        int payCardState = int.Parse(Utilities.GetConfiguration("CardPayState"));
        int payCashState = int.Parse(Utilities.GetConfiguration("CashPayState"));
        bool _visibleCombo = false;

        decimal totalModal = 0;
        decimal totalPago = 0;

        List<ChairsInformation> _typeSeats;
        FunctionInformation _dipMap;
        List<Combos> _View;
        #endregion

        public frmConfirmationModal(List<ChairsInformation> typeSeats,
            FunctionInformation dipMap,
            bool visibleCombo = false)
        {
            InitializeComponent();
            _visibleCombo = visibleCombo;
            _typeSeats = typeSeats;
            _dipMap = dipMap;
            _View = new List<Combos>();
            ConfigureView();
        }

        private void ConfigureView()
        {
            HideOrShowButtons();
            if (DataService41._Combos != null)
            {
                foreach (var item in DataService41._Combos)
                {
                    _View.Add(new Combos
                    {
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Code = item.Code,
                        Visible = "Visible"
                    });
                }
            }

            foreach (var item in _typeSeats)
            {
                _View.Add(new Combos
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Visible = "Hidden"
                });
            }
            OrganizeValues();
            Utilities.Speack("Elige el modo como deseas realizar el pago.");
        }

        private void OrganizeValues()
        {

            totalPago = 0;
            totalModal = 0;
            foreach (var item in _View)
            {
                totalPago += item.Price;
                item.Price = RoundValue(item.Price);
                totalModal += item.Price;
            }

            SetTextView();

            lvListSeats.ItemsSource = _View.OrderBy(s => s.Name);
            Utilities.ValorPagarScore = totalPago;
            Utilities.PayVal = totalModal;
        }
        /// <summary>
        /// Asigno los textos a cada variable de la vista
        /// </summary>
        private void SetTextView()
        {
            TxtTitle.Text = Utilities.CapitalizeFirstLetter(_dipMap.MovieName);
            TxtRoom.Text = _dipMap.RoomName;
            TxtDate.Text = string.Format("{0} {1}", _dipMap.Day, _dipMap.HourFunction);
            TxtTotal.Text = string.Format("{0:C0}", totalModal);
        }

        /// <summary>
        /// Oculta o mustra los botones segun la configuracion
        /// </summary>
        private void HideOrShowButtons()
        {
            BtnCard.Visibility = payCardState == 1 ? Visibility.Visible : Visibility.Hidden;
            BtnCash.Visibility = payCashState == 1 ? Visibility.Visible : Visibility.Hidden;
        }


        private void BtnNo_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnCard_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            Utilities.MedioPago = EPaymentType.Card;
            DialogResult = true;
        }

        private void BtnCash_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            Utilities.MedioPago = EPaymentType.Cash;
            DialogResult = true;
        }

        private void BtnDelete_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var combo = ((sender as Image).DataContext as Combos);

                DeleteCombo(combo);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    _View.Remove(combo);
                    lvListSeats.ItemsSource = _View;
                    lvListSeats.Items.Refresh();
                    OrganizeValues();
                });

            }
            catch (System.Exception ex)
            {
            }
        }
        private void DeleteCombo(Combos combo)
        {
            try
            {
                var dataToDelete = DataService41._Combos.Where(com => com.Code == combo.Code).FirstOrDefault();
                if (dataToDelete != null)
                {
                    DataService41._Combos.Remove(dataToDelete);
                }
            }
            catch (Exception)
            {

            }
        }
        public static decimal RoundValue(decimal valor)
        {
            decimal roundVal = (Math.Ceiling(valor / 100)) * 100;

            return roundVal;
        }
    }
}
