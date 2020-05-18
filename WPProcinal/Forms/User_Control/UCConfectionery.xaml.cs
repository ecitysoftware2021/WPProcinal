using Grabador.Transaccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCConfectionery.xaml
    /// </summary>
    public partial class UCConfectionery : UserControl
    {
        #region "Referencias"
        TimerTiempo timer;
        List<TypeSeat> _Seats;
        DipMap _DipMap;
        ApiLocal api;
        CLSGrabador grabador;
        Utilities utilities;

        #region Información Combos
        private int _ComboTemporadaPrice = int.Parse(Utilities.GetConfiguration("0"));
        private int _ComboTemporadaCode = int.Parse(Utilities.GetConfiguration("0Code"));
        private string _ComboTemporadaName = Utilities.GetConfiguration("0NAME");

        private int _Combo1Price = int.Parse(Utilities.GetConfiguration("1"));
        private int _Combo1Code = int.Parse(Utilities.GetConfiguration("1Code"));
        private string _Combo1Name = Utilities.GetConfiguration("1NAME");

        private int _Combo2Price = int.Parse(Utilities.GetConfiguration("2"));
        private int _Combo2Code = int.Parse(Utilities.GetConfiguration("2Code"));
        private string _Combo2Name = Utilities.GetConfiguration("2NAME");

        private int _Combo3Price = int.Parse(Utilities.GetConfiguration("3"));
        private int _Combo3Code = int.Parse(Utilities.GetConfiguration("3Code"));
        private string _Combo3Name = Utilities.GetConfiguration("3NAME");

        private int _Combo4Price = int.Parse(Utilities.GetConfiguration("4"));
        private int _Combo4Code = int.Parse(Utilities.GetConfiguration("4Code"));
        private string _Combo4Name = Utilities.GetConfiguration("4NAME");

        private int _Combo5Price = int.Parse(Utilities.GetConfiguration("5"));
        private int _Combo5Code = int.Parse(Utilities.GetConfiguration("5Code"));
        private string _Combo5Name = Utilities.GetConfiguration("5NAME");

        private int _ComboHamburguesaPrice = int.Parse(Utilities.GetConfiguration("6"));
        private int _ComboHamburguesaCode = int.Parse(Utilities.GetConfiguration("6Code"));
        private string _ComboHamburguesaName = Utilities.GetConfiguration("6NAME");
        #endregion

        #endregion

        #region "Constructor"
        public UCConfectionery(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            try
            {

                plusTemp.IsEnabled = Utilities.GetConfiguration("0Disponible").Equals("1") ? true : false;
                lessTemp.IsEnabled = Utilities.GetConfiguration("0Disponible").Equals("1") ? true : false;

                _Seats = Seats;
                _DipMap = dipMap;
                api = new ApiLocal();
                utilities = new Utilities();
                grabador = new CLSGrabador();
                //PaintDataCombo();
                Utilities.Speack("Puedes comprar tu combo y reclamarlo en la confitería.");
            }
            catch (Exception ex)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            var frmLoading = new FrmLoading("¡Cargando confiteria...!");

            frmLoading.Show();
            this.IsEnabled = false;
            Task.Run(() =>
            {
                var combos = WCFServices41.GetConfectionery(new SCOPRE
                {
                    teatro = Utilities.GetConfiguration("CodCinema"),
                    tercero = "1"
                });

                if (combos != null)
                {
                    Utilities._Productos = combos.ListaProductos;
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {

                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        frmModal frmModal = new frmModal("No se pudo descargar la confitería, continúa el pago de tus boletas!");
                        frmModal.ShowDialog();
                        ShowDetailModal();
                    });
                }

                Dispatcher.BeginInvoke((Action)delegate
                {

                    ActivateTimer();
                    frmLoading.Close();
                    this.IsEnabled = true;
                });
                //AddCombo("Combo Hamburguesa", _ComboHamburguesaPrice, C6, 423);
                //ChangePrices();
            });

        }

        #endregion

        #region "Eventos"

        /// <summary>
        /// Evento para agregar cantidades
        /// 0= Temporada, 1= Combo1, 2=Combo2, 3=Combo3, 4=Combo4
        /// 5=Combo5, 6=Combo Hamburguesa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlusTemp_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int tag = int.Parse((sender as Image).Tag.ToString());
                int cantActual = 0;
                //switch (tag)
                //{
                //    case 0:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _ComboTemporadaName,
                //            Code = _ComboTemporadaCode,
                //            Price = _ComboTemporadaPrice,
                //            textBlock = C0,
                //            isCombo = true
                //        });
                //        break;
                //    case 1:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _Combo1Name,
                //            Code = _Combo1Code,
                //            Price = _Combo1Price,
                //            textBlock = C1,
                //            isCombo = true
                //        });
                //        break;
                //    case 2:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _Combo2Name,
                //            Code = _Combo2Code,
                //            Price = _Combo2Price,
                //            textBlock = C2,
                //            isCombo = true
                //        });
                //        break;
                //    case 3:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _Combo3Name,
                //            Code = _Combo3Code,
                //            Price = _Combo3Price,
                //            textBlock = C3,
                //            isCombo = true
                //        });
                //        break;
                //    case 4:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _Combo4Name,
                //            Code = _Combo4Code,
                //            Price = _Combo4Price,
                //            textBlock = C4,
                //            isCombo = true
                //        });
                //        break;
                //    case 5:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _Combo5Name,
                //            Code = _Combo5Code,
                //            Price = _Combo5Price,
                //            textBlock = C5,
                //            isCombo = true
                //        });
                //        break;
                //    case 6:
                //        Utilities.AddCombo(new Combos
                //        {
                //            Name = _ComboHamburguesaName,
                //            Code = _ComboHamburguesaCode,
                //            Price = _ComboHamburguesaPrice,
                //            textBlock = C6,
                //            isCombo = true
                //        });
                //        break;
                //}
            }
            catch (Exception ex)
            {

            }
            ChangeImageBuy();
        }




        /// <summary>
        /// Evento para eliminar cantidades
        /// 0= Temporada, 1= Combo1, 2=Combo2, 3=Combo3, 4=Combo4
        /// 5=Combo5, 6=Combo Hamburguesa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLessTemp_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int tag = int.Parse((sender as Image).Tag.ToString());

                //switch (tag)
                //{
                //    case 0:
                //        Utilities.DeleteCombo("Combo Jumanji", _ComboTemporadaPrice, C0);
                //        break;
                //    case 1:
                //        Utilities.DeleteCombo("Combo 1", _Combo1Price, C1);
                //        break;
                //    case 2:
                //        Utilities.DeleteCombo("Combo 2", _Combo2Price, C2);
                //        break;
                //    case 3:
                //        Utilities.DeleteCombo("Combo 3", _Combo3Price, C3);
                //        break;
                //    case 4:
                //        Utilities.DeleteCombo("Combo 4", _Combo4Price, C4);
                //        break;
                //    case 5:
                //        Utilities.DeleteCombo("Combo 5", _Combo5Price, C5);
                //        break;
                //    case 6:
                //        Utilities.DeleteCombo("Combo Hamburguesa", _ComboHamburguesaPrice, C6);
                //        break;
                //}
            }
            catch (Exception ex)
            {

            }
            ChangeImageBuy();
        }



        private void ChangeImageBuy()
        {
            if (Utilities._Combos.Count > 0)
            {
                BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/continuar.png", UriKind.Relative));
            }
            else
            {
                BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/omitir.png", UriKind.Relative));
            }
        }

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            ChangePrices();
            ShowDetailModal();

        }

        public void ChangePrices()
        {
            List<Producto> productos = new List<Producto>();
            //Utilities.dataUser = new SCOLOGResponse();
            List<Combos> combosMalos = new List<Combos>();

            decimal precio = 0;
            foreach (var item in Utilities._Combos)
            {
                precio = 0;
                for (int i = 0; i < item.Quantity; i++)
                {
                    var combo = Utilities._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                    if (combo != null)
                    {
                        foreach (var receta in combo.Receta)
                        {
                            if (receta.Precios != null)
                            {
                                if (Utilities.dataUser.Tarjeta != null)
                                {
                                    precio += decimal.Parse(receta.Precios.FirstOrDefault().OtroPago.Split('.')[0]) * receta.Cantidad;
                                }
                                else
                                {
                                    precio += decimal.Parse(receta.Precios.FirstOrDefault().General.Split('.')[0]) * receta.Cantidad;
                                }
                            }
                            if (receta.RecetaReceta != null)
                            {
                                List<Receta> recetaAux = new List<Receta>();
                                for (int e = 0; e < int.Parse(receta.Cantidad.ToString()); e++)
                                {

                                    var responseRecetaBebida = receta.RecetaReceta.Where(rc => rc.Descripcion.ToLower().Contains("gaseosa")).FirstOrDefault();
                                    Receta responseRecetaComida = null;
                                    //Si el combo es el combo 4, solo tomamos por defecto las gaseosas, las comidas se dejan tal cual
                                    if (item.Code != _Combo4Code)
                                    {
                                        responseRecetaComida = receta.RecetaReceta.Where(rc => rc.Descripcion.ToLower().Contains("perro")).FirstOrDefault();
                                    }
                                    if (responseRecetaBebida != null)
                                    {
                                        recetaAux.Add(responseRecetaBebida);
                                    }
                                    else if (responseRecetaComida != null)
                                    {
                                        recetaAux.Add(responseRecetaComida);
                                    }

                                }
                                if (recetaAux.Count != 0)
                                {
                                    receta.RecetaReceta = recetaAux;
                                }
                                else
                                {
                                    receta.RecetaReceta = receta.RecetaReceta.Take(int.Parse(receta.Cantidad.ToString())).ToList();
                                }
                                foreach (var preciosReceta in receta.RecetaReceta)
                                {
                                    if (preciosReceta.Precios != null)
                                    {
                                        if (Utilities.dataUser.Tarjeta != null)
                                        {
                                            precio += decimal.Parse(preciosReceta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                                        }
                                        else
                                        {
                                            precio += decimal.Parse(preciosReceta.Precios.FirstOrDefault().General.Split('.')[0]);
                                        }
                                    }
                                }
                            }
                        }
                        productos.Add(combo);
                    }
                    else
                    {
                       var data=   Utilities._Combos.Where(pr => pr.Code == item.Code).FirstOrDefault();
                       combosMalos.Add(data);
                    }
                }

                item.Price = precio;
            }

            foreach (var combo in combosMalos)
            {
                Utilities._Combos.Remove(combo);
            }
        }

        private void ShowDetailModal()
        {
            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(_Seats, _DipMap);
            this.Opacity = 0.3;
            _frmConfirmationModal.ShowDialog();
            this.Opacity = 1;
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                GoToPay();
            }
            else
            {
                ActivateTimer();
                this.IsEnabled = true;
            }
        }

        #endregion

        #region "Métodos"

        //private void PaintDataCombo()
        //{
        //    foreach (var item in Utilities._Combos)
        //    {
        //        if (item.isCombo)
        //        {
        //            var block =  (item.textBlock as TextBlock);
        //            Dispatcher.BeginInvoke((Action)delegate
        //            {
        //                block.Text = item.textBlock.Text;
        //            });
        //        }
        //    }
        //}

        private async void GoToPay()
        {
            FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
            frmLoading.Show();
            var response = await utilities.CreateTransaction("Cine ", _DipMap, _Seats);
            frmLoading.Close();

            if (!response)
            {
                Utilities.ShowModal("No se pudo crear la transacción, por favor intente de nuevo.");

                frmLoading = new FrmLoading("¡Reconectando...!");
                frmLoading.Show();
                await api.SecurityToken();
                frmLoading.Close();
                this.IsEnabled = true;
            }
            else
            {
                try
                {
                    Task.Run(() =>
                    {
                        grabador.Grabar(Utilities.IDTransactionDB);
                    });
                }
                catch { }

                if (Utilities.MedioPago == 1)
                {
                    Switcher.Navigate(new UCPayCine(_Seats, _DipMap));
                }
                else if (Utilities.MedioPago == 2)
                {
                    Switcher.Navigate(new UCCardPayment(_Seats, _DipMap));
                }
            }


        }
        #endregion

        #region "Timer"
        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerConfiteria");
                timer = new TimerTiempo(tbTimer.Text);
                timer.CallBackClose = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCinema());
                    });
                };
                timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        tbTimer.Text = response;
                    });
                };
            }
            catch { }
        }

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }

        #endregion


        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
           
            this.IsEnabled = false;
            var frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
            frmLoading.Show();
            Utilities.CancelAssing(_Seats, _DipMap);
            frmLoading.Close();
            this.IsEnabled = true;
            Switcher.Navigate(new UCCinema());
        }

        private void BtnMoreProducts_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);

            Switcher.Navigate(new UCProducts(_Seats, _DipMap));
        }
    }
}
