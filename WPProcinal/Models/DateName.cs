using System;
using System.ComponentModel;

namespace WPProcinal.Models
{
    public class DateName : INotifyPropertyChanged
    {
        public string NombreDia { get; set; }
        public string Mes { get; set; }
        public string DiaNumero { get; set; }
        public string TextColor { get; set; }

        private string backColor;

        public string BackColor
        {
            get { return this.backColor; }
            set
            {
                if (this.backColor != value)
                {
                    this.backColor = value;
                    this.NotifyPropertyChanged("BackColor");
                }
            }
        }
        public DateTime FechaOrigin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
