﻿using Grabador.Transaccion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCProducts.xaml
    /// </summary>
    public partial class UCProducts : UserControl
    {
        #region "Referencias"
        private TimerTiempo timer;
        private CollectionViewSource view;
        private ObservableCollection<Producto> lstPager;
        ApiLocal api;
        CLSGrabador grabador;
        #endregion
        string Type;
        #region "Constructor"
        public UCProducts(string Type)
        {

            InitializeComponent();
            api = new ApiLocal();
            grabador = new CLSGrabador();
            this.view = new CollectionViewSource();
            this.lstPager = new ObservableCollection<Producto>();
            this.Type = Type;
            //ActivateTimer();
            loadProductos();
            InitViewAsync();
            PaintDataCombo();

        }
        #endregion

        #region "ListView"
        private async Task InitViewAsync()
        {
            try
            {
                FrmLoading frmLoading = new FrmLoading("¡Cargando imagenes...!");
                frmLoading.Show();
                bool isValidURL = true;
                switch (Type.ToUpper())
                {
                    case "C":
                        foreach (var product in DataService41._Productos.Where(P => P.Tipo.ToUpper() == Type.ToUpper()).ToList())
                        {
                            string imgAsignada = "nofound";
                            string data = string.Empty;
                            using (WebClient client = new WebClient())
                            {
                                try
                                {
                                    string image =
                                    $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                        Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo + ".png")))
                                    {
                                        data = await client.DownloadStringTaskAsync(image);
                                        if (data != null && data.Length > 3)
                                        {
                                            using (WebClient client2 = new WebClient())
                                            {
                                                client2.DownloadFileAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                   Assembly.GetEntryAssembly().Location),
                                                   "ImagesConfiteria", product.Codigo + ".png"));
                                            }

                                            imgAsignada = product.Codigo.ToString();
                                        }
                                        else
                                        {
                                            imgAsignada =  "nofound.png";
                                        }
                                    }
                                    else
                                    {
                                        imgAsignada = product.Codigo.ToString();
                                    }
                                }
                                catch (WebException weX)
                                {
                                    isValidURL = !isValidURL;
                                    data = weX.Message;
                                }
                            }
                            product.Imagen = Path.Combine(Path.GetDirectoryName(
                                             Assembly.GetEntryAssembly().Location),
                                             "ImagesConfiteria", imgAsignada + ".png");
                            
                            //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";
                            lstPager.Add(product);
                        }
                        break;

                    case "P":
                        //no mostrar 165,168 toxineta y queso
                        foreach (var product in DataService41._Productos.Where(P => P.Tipo.ToUpper() == Type.ToUpper()).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {

                                if (product.Codigo != 165 && product.Codigo != 168 && product.Codigo != 168)
                                {

                                    decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                    decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);

                                    string imgAsignada = "nofound";
                                    string data = string.Empty;
                                    using (WebClient client = new WebClient())
                                    {
                                        try
                                        {
                                            string image =
                                            $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                            if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                                Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo + ".png")))
                                            {
                                                data = await client.DownloadStringTaskAsync(image);
                                                if (data != null && data.Length > 3)
                                                {
                                                    using (WebClient client2 = new WebClient())
                                                    {
                                                        client2.DownloadFileAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                           Assembly.GetEntryAssembly().Location),
                                                           "ImagesConfiteria", product.Codigo + ".png"));
                                                    }

                                                    imgAsignada = product.Codigo.ToString();
                                                }
                                                else
                                                {
                                                    imgAsignada = "nofound.png";
                                                }
                                            }
                                            else
                                            {
                                                imgAsignada = product.Codigo.ToString();
                                            }
                                        }
                                        catch (WebException weX)
                                        {
                                            isValidURL = !isValidURL;
                                            data = weX.Message;
                                        }
                                    }

                                    
                                    product.Imagen = Path.Combine(Path.GetDirectoryName(
                                             Assembly.GetEntryAssembly().Location),
                                             "ImagesConfiteria", imgAsignada + ".png");
                                    //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                    if (General > 0)
                                    {
                                        product.Precios[0].auxGeneral = General;
                                        product.Precios[0].auxOtroPago = OtroPago;

                                        lstPager.Add(product);
                                    }
                                }

                            }
                        }
                        break;

                    case "B":
                        foreach (var product in DataService41._Productos.Where(P => P.Descripcion.ToLower().Contains("gas") || P.Descripcion.ToLower().Contains("agua") || P.Descripcion.ToLower().Contains("icee")).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {
                                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);


                                string imgAsignada = "nofound";
                                string data = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    try
                                    {
                                        string image =
                                        $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                        if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                            Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo + ".png")))
                                        {
                                            data = await client.DownloadStringTaskAsync(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    client2.DownloadFileAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo + ".png"));
                                                }

                                                imgAsignada = product.Codigo.ToString();
                                            }
                                            else
                                            {
                                                imgAsignada = "nofound.png";
                                            }
                                        }
                                        else
                                        {
                                            imgAsignada =  product.Codigo.ToString();
                                        }
                                    }
                                    catch (WebException weX)
                                    {
                                        isValidURL = !isValidURL;
                                        data = weX.Message;
                                    }
                                }

                                //product.Imagen = imgAsignada;
                                product.Imagen = Path.Combine(Path.GetDirectoryName(
                                             Assembly.GetEntryAssembly().Location),
                                             "ImagesConfiteria", imgAsignada + ".png");
                                //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                if (General > 0)
                                {
                                    product.Precios[0].auxGeneral = General;
                                    product.Precios[0].auxOtroPago = OtroPago;

                                    lstPager.Add(product);
                                }
                            }
                        }
                        break;
                    case "O":
                        foreach (var product in DataService41._Productos.Where(P => P.Descripcion.ToLower().Contains("adic")).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {
                                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);

                                string imgAsignada = "nofound";
                                string data = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    try
                                    {
                                        string image =
                                        $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                        if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                            Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo + ".png")))
                                        {
                                            data = await client.DownloadStringTaskAsync(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    await client2.DownloadFileTaskAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo + ".png"));
                                                }

                                                imgAsignada = product.Codigo.ToString();
                                            }
                                            else
                                            {
                                                imgAsignada = "nofound.png";
                                            }
                                        }
                                        else
                                        {
                                            imgAsignada = product.Codigo.ToString();
                                        }
                                    }
                                    catch (WebException weX)
                                    {
                                        isValidURL = !isValidURL;
                                        data = weX.Message;
                                    }
                                }

                                product.Imagen = Path.Combine(Path.GetDirectoryName(
                                             Assembly.GetEntryAssembly().Location),
                                             "ImagesConfiteria", imgAsignada + ".png");
                                //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";
                                if (General > 0)
                                {
                                    product.Precios[0].auxGeneral = General;
                                    product.Precios[0].auxOtroPago = OtroPago;

                                    lstPager.Add(product);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
                frmLoading.Close();
                ActivateTimer();
                view.Source = lstPager;
                lv_Products.DataContext = view;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>InitView", JsonConvert.SerializeObject(ex), 1);
            }


            //try
            //{
            //    foreach (var product in DataService41._Productos)
            //    {

            //        if (product.Tipo.ToUpper() == "P")
            //        {
            //            if (product.Precios.Count() > 0)
            //            {
            //                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
            //                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);
            //                product.Imagen = $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";
            //                if (General > 0)
            //                {
            //                    product.Precios[0].auxGeneral = General;
            //                    product.Precios[0].auxOtroPago = OtroPago;

            //                    lstPager.Add(product);
            //                }
            //            }
            //        }
            //    }

            //    view.Source = lstPager;
            //    lv_Products.DataContext = view;
            //}
            //catch (Exception ex)
            //{
            //    LogService.SaveRequestResponse("UCProducts>InitView", JsonConvert.SerializeObject(ex), 1);
            //}
        }

        private bool loadProductos()
        {
            FrmLoading frmLoading = new FrmLoading("¡Descargando la confitería...!");
            frmLoading.Show();
            var combos = WCFServices41.GetConfectionery(new SCOPRE
            {
                teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString(),
                tercero = "1"
            });

            frmLoading.Close();
            if (combos != null)
            {
                if (combos.ListaProductos.Count > 0)
                {
                    DataService41._Productos = combos.ListaProductos;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "Eventos"
        private void BtnPlus_TouchDown(object sender, TouchEventArgs e)
        {
            IncrementDecrementProducts(sender, true);
        }
        private void BtnLess_TouchDown(object sender, TouchEventArgs e)
        {
            IncrementDecrementProducts(sender, false);
        }

        private void IncrementDecrementProducts(object sender, bool Operation, bool OtherProcess = false)
        {
            try
            {
                //var data = ((sender as Image).DataContext as Producto);
                if (!OtherProcess)
                {
                    var data = ((sender as Image).DataContext as Producto);

                    switch (data.Tipo)
                    {
                        case "C":
                            if (Operation)
                            {
                                Utilities.AddCombo(new Combos
                                {
                                    Name = data.Descripcion,
                                    Code = Convert.ToInt32(data.Codigo),
                                    Price = 0,
                                    dataProduct = data,
                                    isCombo = true,
                                });
                            }
                            else
                            {
                                Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: 0, dataProduct: data);
                            }
                            break;
                        case "P":
                            if (Operation)
                            {

                                Utilities.AddCombo(new Combos
                                {
                                    Name = data.Descripcion,
                                    Code = Convert.ToInt32(data.Codigo),
                                    Price =
                                    //Utilities.dataTransaction.dataUser.Tarjeta != null ? data.Precios[0].auxOtroPago : data.Precios[0].auxGeneral,
                                    Utilities.dataTransaction.dataUser.Tarjeta != "0" ? data.Precios[0].auxOtroPago : data.Precios[0].auxGeneral,
                                    dataProduct = data,
                                    isCombo = false,
                                });
                            }
                            else
                            {
                                Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: data.Precios[0].auxOtroPago, dataProduct: data);
                            }
                            break;
                        default:
                            break;
                    }
                    ChangeImageBuy();
                }
                else
                {
                    var data = (sender as List<Producto>)[0];
                    switch (data.Tipo)
                    {
                        case "C":
                            Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: 0, dataProduct: data);
                            break;
                        case "P":
                            Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: data.Precios[0].auxOtroPago, dataProduct: data);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>IncrementDecrementProducts", JsonConvert.SerializeObject(ex), 1);
            }

            //if (Operation)
            //    {
            //        Utilities.AddCombo(new Combos
            //        {
            //            Name = data.Descripcion,
            //            Code = Convert.ToInt32(data.Codigo),
            //            Price = Utilities.dataTransaction.dataUser.Tarjeta != null ? data.Precios[0].auxOtroPago : data.Precios[0].auxGeneral,
            //            dataProduct = data,
            //            isCombo = false,

            //        });
            //    }
            //    else
            //    {
            //        Utilities.DeleteCombo(comboName: data.Descripcion, comboPrice: data.Precios[0].auxOtroPago, dataProduct: data);
            //    }

            //    ChangeImageBuy();
            //}
            //catch (Exception ex)
            //{
            //    LogService.SaveRequestResponse("UCProducts>IncrementDecrementProducts", JsonConvert.SerializeObject(ex), 1);
            //}
        }

        private void BtnCombos_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            try
            {
                Switcher.Navigate(new UCProductsCombos());
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>BtnCombos_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }

        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {

            SetCallBacksNull();
            try
            {

                this.IsEnabled = false;
                var frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                Utilities.Loading(frmLoading, true, this);
                Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                Utilities.Loading(frmLoading, false, this);
                this.IsEnabled = true;
                Switcher.Navigate(new UCCinema());
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>BtnSalir_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void BtnComprar_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            ChangePrices();
            ShowDetailModal();
        }

        private void BtnMore_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            ChangePrices();
            Switcher.Navigate(new UCSelectProducts());
        }
        #endregion

        #region "Métodos"
        private void ShowDetailModal()
        {
            try
            {
                frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal();
                this.Opacity = 0.3;
                _frmConfirmationModal.ShowDialog();
                if (_frmConfirmationModal.DialogResult.HasValue &&
                    _frmConfirmationModal.DialogResult.Value)
                {
                    GoToPay();
                }
                else
                {
                    ActivateTimer();
                    this.IsEnabled = true;
                    this.Opacity = 1;
                    PaintDataCombo();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>ShowDetailModal", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void ChangeImageBuy()
        {
            try
            {
                if (DataService41._Combos.Count > 0)
                {
                    BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/continuar.png", UriKind.Relative));
                    //BtnPay = true;
                    BtnMore.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    BtnComprar.Source = new BitmapImage(new Uri(@"/Images/buttons/omitir.png", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>ChangeImageBuy", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void PaintDataCombo()
        {
            try
            {
                List<long> codesOk = new List<long>();

                foreach (var item in DataService41._Combos)
                {
                    if (!item.isCombo)
                    {
                        foreach (var list in lstPager)
                        {
                            if (item.Code == list.Codigo)
                            {
                                list.Value = item.Quantity;
                                lv_Products.Items.Refresh();
                                codesOk.Add(list.Codigo);
                            }
                        }
                    }
                }
                foreach (var item in lstPager)
                {
                    if (codesOk.IndexOf(item.Codigo) < 0)
                    {
                        item.Value = 0;
                    }
                }
                ChangeImageBuy();
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>PaintDataCombo", JsonConvert.SerializeObject(ex), 1);
            }
        }
        public void ChangePrices()
        {
            try
            {
                List<Producto> productos = new List<Producto>();
                decimal precio = 0;
                foreach (var item in DataService41._Combos)
                {
                    precio = 0;
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var combo = DataService41._Productos.Where(pr => pr.Codigo == item.Code).FirstOrDefault();
                        if (combo.Receta != null)
                        {
                            foreach (var receta in combo.Receta)
                            {
                                if (receta.Precios != null)
                                {
                                    decimal otroPago = decimal.Parse(receta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                                    if (Utilities.dataTransaction.dataUser.Tarjeta != null && otroPago > 0)
                                    {
                                        precio += otroPago * receta.Cantidad;
                                        Utilities.dataTransaction.PrecioCinefans = true;
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
                                        if (item.Code != Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.Code4)
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
                                            decimal otroPago = decimal.Parse(preciosReceta.Precios.FirstOrDefault().OtroPago.Split('.')[0]);
                                            if (Utilities.dataTransaction.dataUser.Tarjeta != null && otroPago > 0)
                                            {
                                                precio += otroPago;
                                                Utilities.dataTransaction.PrecioCinefans = true;
                                            }
                                            else
                                            {
                                                precio += decimal.Parse(preciosReceta.Precios.FirstOrDefault().General.Split('.')[0]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            foreach (var preciosReceta in combo.Precios)
                            {
                                precio = 0;
                                var vlr = Convert.ToDecimal(preciosReceta.General.Split('.', ' ')[0].ToString().Trim());
                                if (Utilities.dataTransaction.dataUser.Tarjeta != null && preciosReceta.auxOtroPago > 0)
                                {
                                    precio = preciosReceta.auxOtroPago * item.Quantity;
                                    Utilities.dataTransaction.PrecioCinefans = true;
                                }
                                else
                                {
                                    precio = (item.Price > 0 ? vlr : preciosReceta.auxGeneral) * item.Quantity;
                                }
                            }

                            //foreach (var preciosReceta in combo.Precios)
                            //{
                            //    if (Utilities.dataTransaction.dataUser.Tarjeta != null && preciosReceta.auxOtroPago > 0)
                            //    {
                            //        precio = preciosReceta.auxOtroPago * item.Quantity;
                            //        Utilities.dataTransaction.PrecioCinefans = true;
                            //    }
                            //    else
                            //    {
                            //        precio = preciosReceta.auxGeneral * item.Quantity;
                            //    }
                            //}
                        }
                        productos.Add(combo);
                    }
                    item.Price = precio;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>ChangePrices", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void GoToPay()
        {
            FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
            frmLoading.Show();
            Utilities.ValidateUserBalance();
            var response = Utilities.CreateTransaction("Cine ").Result;
            frmLoading.Close();

            if (Utilities.eTypeBuy == ETypeBuy.JustConfectionery)
            {
                if (!GetSecuence())
                {
                    this.IsEnabled = true;
                    this.Opacity = 1;
                    return;
                }
            }

            if (!response)
            {
                Utilities.ShowModal("No se pudo crear la transacción, por favor intente de nuevo.");

                frmLoading = new FrmLoading("¡Reconectando...!");
                frmLoading.Show();
                var res = api.SecurityToken().Result;
                frmLoading.Close();
                this.IsEnabled = true;
                this.Opacity = 1;
            }
            else
            {
                if (Utilities.dataTransaction.MedioPago == EPaymentType.Cash)
                {
                    Switcher.Navigate(new UCPayCine());
                }
                else if (Utilities.dataTransaction.MedioPago == EPaymentType.Card)
                {
                    Switcher.Navigate(new UCCardPayment());
                }
            }
        }

        private bool GetSecuence()
        {
            try
            {
                FrmLoading frmLoading = new FrmLoading("¡Generando secuencia de compra!");
                frmLoading.Show();

                var responseSec41 = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                    tercero = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.tercero
                });

                frmLoading.Close();
                if (responseSec41 == null)
                {
                    Task.Run(() =>
                    {
                        Utilities.SendMailErrores($"No se pudo obtener la secuencia de compra en la transaccion: {Utilities.IDTransactionDB}" +
                            $"");
                    });
                    Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");

                    return false;
                }

                foreach (var item in responseSec41)
                {
                    Utilities.dataTransaction.Secuencia = item.Secuencia.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>GetSecuence", JsonConvert.SerializeObject(ex), 1);
                Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");
                return false;
            }
        }

        #endregion

        #region "Timer"
        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.dataPaypad.PaypadConfiguration.generiC_TIMER;
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
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>ActivateTimer", JsonConvert.SerializeObject(ex), 1);
            }
        }

        void SetCallBacksNull()
        {
            if (timer != null)
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
                timer.CallBackStop?.Invoke(1);
            }
        }
        #endregion
    }
}
