﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPProcinal.Classes
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private decimal _payValue;

        private decimal _valorIngresado;

        private decimal _valorFaltante;

        private decimal _valorSobrante;

        private string _imageChange;

        private string _imageReading;

        private string _imageReceipt;

        private string _imageEnterMoney;
        #endregion

        #region Properties

        public decimal PayValue
        {
            get { return _payValue; }
            set
            {
                if (_payValue != value)
                {
                    _payValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PayValue)));
                }
            }
        }

        public string ImageChange
        {
            get { return _imageChange; }
            set
            {
                if (_imageChange != value)
                {
                    _imageChange = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageChange)));
                }
            }
        }

        public string ImageReading
        {
            get { return _imageReading; }
            set
            {
                if (_imageReading != value)
                {
                    _imageReading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageReading)));
                }
            }
        }

        public string ImageReceipt
        {
            get { return _imageReceipt; }
            set
            {
                if (_imageReceipt != value)
                {
                    _imageReceipt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageReceipt)));
                }
            }
        }

        public string ImageEnterMoney
        {
            get { return _imageEnterMoney; }
            set
            {
                if (_imageEnterMoney != value)
                {
                    _imageEnterMoney = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageEnterMoney)));
                }
            }
        }

        public decimal ValorIngresado
        {
            get
            {

                return _valorIngresado;
            }
            set
            {
                if (_valorIngresado != value)
                {
                    _valorIngresado = value;
                    ValorFaltante = (ValorIngresado < PayValue) ? PayValue - ValorIngresado : 0;
                    ValorSobrante = (ValorIngresado > PayValue) ? ValorIngresado - PayValue : 0;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorIngresado)));
                }
            }
        }

        public decimal ValorFaltante
        {
            get { return _valorFaltante; }
            set
            {
                if (_valorFaltante != value)
                {
                    _valorFaltante = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorFaltante)));
                }
            }
        }

        public decimal ValorSobrante
        {
            get { return _valorSobrante; }
            set
            {
                if (_valorSobrante != value)
                {
                    _valorSobrante = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorSobrante)));
                }
            }
        }

        #endregion
    }
}
