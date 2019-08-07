﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{

    public class CLSDatos
    {
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public long NumeroBastidor { get; set; }
        public string TipoVehiculo { get; set; }
        public string CategoriaVehiculo { get; set; }
        public string Referencia { get; set; }
    }

    public class Mensajes : INotifyPropertyChanged
    {
        private string mensajePrincipal;

        public string MensajePrincipal
        {
            get { return this.mensajePrincipal; }
            set
            {
                if (this.mensajePrincipal != value)
                {
                    this.mensajePrincipal = value;
                    this.NotifyPropertyChanged("MensajePrincipal");
                }
            }
        }

        private string mensajeModal;

        public string MensajeModal
        {
            get { return this.mensajeModal; }
            set
            {
                if (this.mensajeModal != value)
                {
                    this.mensajeModal = value;
                    this.NotifyPropertyChanged("MensajeModal");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }


    public class DataCardTransaction : INotifyPropertyChanged
    {
        private string Mensaje;
        public string mensaje
        {
            get { return this.Mensaje; }
            set
            {
                if (this.Mensaje != value)
                {
                    this.Mensaje = value;
                    this.NotifyPropertyChanged("Mensaje");
                }
            }
        }
        public int maxlen { get; set; }
        public int minlen { get; set; }
        public string peticion { get; set; }
        public bool isCredit { get; set; }
        public string imagen { get; set; }

        private string Visible;
        public string visible
        {
            get { return this.Visible; }
            set
            {
                if (this.Visible != value)
                {
                    this.Visible = value;
                    this.NotifyPropertyChanged("Visible");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

}
