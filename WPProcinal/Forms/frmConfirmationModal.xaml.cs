using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

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

        List<TypeSeat> _typeSeats;
        DipMap _dipMap;
        List<Combos> _View;
        #endregion

        public frmConfirmationModal(List<TypeSeat> typeSeats,
            DipMap dipMap,
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
            if (Utilities._Combos != null)
            {
                foreach (var item in Utilities._Combos)
                {
                    _View.Add(new Combos
                    {
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }
            }

            foreach (var item in _typeSeats)
            {
                _View.Add(new Combos
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            foreach (var item in _View)
            {
                totalPago += item.Price;
                item.Price = Utilities.RoundValue(item.Price);
                totalModal += item.Price;
            }

            SetTextView();

            lvListSeats.ItemsSource = _View.OrderBy(s => s.Name);
            Utilities.ValorPagarScore = totalPago;
            Utilities.PayVal = totalModal;
            Utilities.Speack("Elige el modo como deseas realizar el pago.");
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
            Utilities.MedioPago = 2;
            DialogResult = true;
        }

        private void BtnCash_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            Utilities.MedioPago = 1;
            DialogResult = true;
        }
    }
}
