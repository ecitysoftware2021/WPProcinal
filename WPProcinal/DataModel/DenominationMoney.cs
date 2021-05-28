using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.DataModel
{
    public enum DenominationDestiny
    {
        DISPENSAR = 1,
        ACEPTAR = 2,
        REJECT = 3
    }

    public enum CurrencyDenomination
    {
        COP = 1,
        MX = 2,
        SOL = 3,
        USD = 4
    }
    public class DenominationMoney : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private decimal _Denominacion;

        private decimal _Quantity;

        private decimal _Total;

        private string _Code;

        #endregion

        #region Properties

        public decimal Denominacion
        {
            get { return _Denominacion; }
            set
            {
                if (_Denominacion != value)
                {
                    _Denominacion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Denominacion)));
                }
            }
        }

        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                }
            }
        }

        public decimal Total
        {
            get { return _Total; }
            set
            {
                if (_Total != value)
                {
                    _Total = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                }
            }
        }

        public string Code
        {
            get { return _Code; }
            set
            {
                if (_Code != value)
                {
                    _Code = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Code)));
                }
            }
        }
        public int CurrencyID { get; set; }
        public DenominationDestiny OperationType { get; set; }
        #endregion
    }
}
