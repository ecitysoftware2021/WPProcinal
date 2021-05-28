using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPProcinal.DataModel;

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

        private List<DenominationMoney> _denominations;
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

        public List<DenominationMoney> Denominations
        {
            get { return _denominations; }
            set
            {
                _denominations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Denominations)));
            }
        }

        public void RefreshListDenomination(int denomination, int quantity, string code)
        {
            try
            {
                var itemDenomination = this.Denominations.Where(d => d.Denominacion == denomination && d.Code == code).FirstOrDefault();
                if (itemDenomination == null)
                {
                    this.Denominations.Add(new DenominationMoney
                    {
                        Denominacion = denomination,
                        Quantity = quantity,
                        Total = denomination * quantity,
                        Code = code,
                        OperationType = DenominationDestiny.ACEPTAR,
                        CurrencyID = (int)CurrencyDenomination.COP
                    });
                }
                else
                {
                    itemDenomination.Quantity += quantity;
                    itemDenomination.Total = denomination * itemDenomination.Quantity;
                }
            }
            catch (Exception ex)
            {
                //Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PaymentViewModel", ex);
            }
        }

        public void SplitDenomination(string data, bool isBX)
        {
            try
            {
                string[] values = data.Replace("!", "").Split(':')[1].Split(';');
                foreach (var value in values)
                {
                    int denomination = int.Parse(value.Split('-')[0]);
                    int quantity = int.Parse(value.Split('-')[1]);
                    string code = denomination < 1000 ? "MD" : "DP";


                    if (quantity > 0)
                    {
                        DenominationMoney denominationMoney = Denominations.Where(d => d.Denominacion == denomination && d.Code == code).FirstOrDefault();
                        if (!isBX)
                        {
                            if (denominationMoney == null)
                            {
                                Denominations.Add(new DenominationMoney
                                {
                                    Denominacion = denomination,
                                    Quantity = quantity,
                                    Total = denomination * quantity,
                                    Code = code,
                                    OperationType = DenominationDestiny.DISPENSAR,
                                    CurrencyID = (int)CurrencyDenomination.COP
                                });
                            }
                        }
                        else
                        {
                            if (denominationMoney != null)
                            {
                                if (quantity > denominationMoney.Quantity && denominationMoney.Code == "DP")
                                {
                                    Denominations.Add(new DenominationMoney
                                    {
                                        Denominacion = denomination,
                                        Quantity = quantity - denominationMoney.Quantity,
                                        Total = denomination * (quantity - denominationMoney.Quantity),
                                        Code = code,
                                        OperationType = DenominationDestiny.REJECT,
                                        CurrencyID = (int)CurrencyDenomination.COP
                                    });
                                }
                            }
                            else
                            {
                                Denominations.Add(new DenominationMoney
                                {
                                    Denominacion = denomination,
                                    Quantity = quantity,
                                    Total = denomination * quantity,
                                    Code = code,
                                    OperationType = DenominationDestiny.REJECT,
                                    CurrencyID = (int)CurrencyDenomination.COP
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PaymentViewModel", ex);
            }
        }
        #endregion
    }
}
