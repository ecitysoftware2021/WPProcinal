using Grabador.Transaccion;
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
        TimerTiempo timer;

        private CollectionViewSource view;
        private ObservableCollection<Producto> lstPager;
        ApiLocal api;

        CLSGrabador grabador;
        string Type;
        bool BtnPay;
        int numberupdateimages = 1;
        #endregion

        #region "Constructor"
        public UCProducts(string Type)
        {
            InitializeComponent();
            this.Type = Type;
            BtnPay = false;
            api = new ApiLocal();
            grabador = new CLSGrabador();
            this.view = new CollectionViewSource();
            this.lstPager = new ObservableCollection<Producto>();

            //SetCallBacksNull();

            loadProductos();
            InitView();
            //PaintDataCombo();
            ActivateTimer();
        }
        #endregion

        #region "ListView"
        private async void InitView()
        {
            try
            {

                FrmLoading frmLoading = new FrmLoading("¡Cargando imagenes...!");
                frmLoading.Show();

                if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.reloadImages 
                    && Utilities.updateImages == false)
                {
                    Utilities.updateImages = true;
                    await ConfigureImg();
                }

                bool isValidURL = true;
                string selected = string.Empty;
                switch (Type.ToUpper())
                {
                    case "C":
                        selected = "Combos";
                        foreach (var product in DataService41._Productos.Where(P => P.Tipo.ToUpper() == Type.ToUpper()).ToList())
                        {
                            string imgAsignada = "nofound";
                            string data = string.Empty;

                            try
                            {
                                string image =
                                $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo.ToString() + ".png"))
                                )
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        try
                                        {
                                            data = client.DownloadString(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    await client2.DownloadFileTaskAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo.ToString() + ".png"));

                                                    imgAsignada = product.Codigo.ToString(); 
                                                }
                                            }
                                        }
                                        catch (WebException weX)
                                        {
                                            isValidURL = !isValidURL;
                                            data = weX.Message;
                                        }
                                    }
                                }

                                else
                                {
                                    imgAsignada = product.Codigo.ToString().Trim();
                                }
        
                            }
                            catch (WebException weX)
                            {
                             
                                isValidURL = !isValidURL;
                                data = weX.Message;
                            }

                            product.Imagen = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location),
                                "ImagesConfiteria", imgAsignada + ".png")));
                            //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";
                            lstPager.Add(product);

                        }
                        break;

                    case "P":
                        selected = "Productos";

                        //no mostrar 165,168 toxineta y queso
                        foreach (var product in DataService41._Productos.Where(P => P.Tipo.ToUpper() == Type.ToUpper() &&
                                    !P.Descripcion.ToLower().Contains("gas") &&
                                    !P.Descripcion.ToLower().Contains("agua") &&
                                    !P.Descripcion.ToLower().Contains("gaf") &&
                                    !P.Descripcion.ToLower().Contains("adi") &&
                                    !P.Descripcion.ToLower().Contains("icee")).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {


                                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);

                                string imgAsignada = "nofound";
                                string data = string.Empty;

                               string image =
                               $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo.ToString() + ".png"))
                                )
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        try
                                        {
                                            data = client.DownloadString(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    await client2.DownloadFileTaskAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo.ToString() + ".png"));

                                                    imgAsignada = product.Codigo.ToString();
                                                }
                                            }
                                        }
                                        catch (WebException weX)
                                        {
                                            isValidURL = !isValidURL;
                                            data = weX.Message;
                                        }
                                    }
                                }

                                else
                                {
                                    imgAsignada = product.Codigo.ToString().Trim();
                                }


                                product.Imagen = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location),
                                "ImagesConfiteria", imgAsignada + ".png")));
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

                    case "B":
                        selected = "Bebidas";
                        foreach (var product in DataService41._Productos.Where(P => P.Descripcion.ToLower().Contains("gas") || P.Descripcion.ToLower().Contains("agua") || P.Descripcion.ToLower().Contains("icee")).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {
                                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);


                                string imgAsignada = "nofound";
                                string data = string.Empty;
                                string image =
                               $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo.ToString() + ".png"))
                                )
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        try
                                        {
                                            data = client.DownloadString(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    await client2.DownloadFileTaskAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo.ToString() + ".png"));

                                                    imgAsignada = product.Codigo.ToString();
                                                }
                                            }
                                        }
                                        catch (WebException weX)
                                        {
                                            isValidURL = !isValidURL;
                                            data = weX.Message;
                                        }
                                    }
                                }

                                else
                                {
                                    imgAsignada = product.Codigo.ToString().Trim();
                                }

                                product.Imagen = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location),
                                "ImagesConfiteria", imgAsignada + ".png")));
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
                        selected = "Otros";
                        foreach (var product in DataService41._Productos.Where(P => P.Descripcion.ToLower().Contains("adic") || P.Descripcion.ToLower().Contains("gaf")).ToList())
                        {
                            if (product.Precios.Count() > 0)
                            {
                                decimal General = Convert.ToDecimal(product.Precios[0].General.Split('.')[0]);
                                decimal OtroPago = Convert.ToDecimal(product.Precios[0].OtroPago.Split('.')[0]);
                                string imgAsignada = "nofound";
                                string data = string.Empty;
                                string image =
                                $"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";

                                if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location), "ImagesConfiteria", product.Codigo.ToString() + ".png"))
                                )
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        try
                                        {
                                            data = client.DownloadString(image);
                                            if (data != null && data.Length > 3)
                                            {
                                                using (WebClient client2 = new WebClient())
                                                {
                                                    await client2.DownloadFileTaskAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                                                       Assembly.GetEntryAssembly().Location),
                                                       "ImagesConfiteria", product.Codigo.ToString() + ".png"));

                                                    imgAsignada = product.Codigo.ToString();
                                                }
                                            }
                                        }
                                        catch (WebException weX)
                                        {
                                            isValidURL = !isValidURL;
                                            data = weX.Message;
                                        }
                                    }
                                }

                                else
                                {
                                    imgAsignada = product.Codigo.ToString().Trim();
                                }


                                product.Imagen = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                                Assembly.GetEntryAssembly().Location),
                                "ImagesConfiteria", imgAsignada + ".png")));
                                //$"{Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ProductsURL}{product.Codigo}.png";
                                if (General > 0)

                                {
                                    product.Precios[0].auxGeneral = General;
                                    product.Precios[0].auxOtroPago = OtroPago;
                                    product.Precio = (int)General;
                                    lstPager.Add(product);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }

                frmLoading.Close();

                if (Type.ToUpper().Equals("C"))
                {
                    lstPager = await organizaProductos(lstPager);
                }

                view.Source = lstPager;
                lv_Products.DataContext = view;
                typeSelected.Text = selected;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCProducts>InitView", JsonConvert.SerializeObject(ex), 1);
            }
        }

        public Task<ObservableCollection<Producto>> organizaProductos(ObservableCollection<Producto> productos) 
        {
            var json = JsonConvert.SerializeObject(productos);

            var prductos = new ObservableCollection<Producto>(){
               new Producto{ Codigo = Convert.ToInt32(1553), Descripcion = "Combo colombia magica", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 1553).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(1531), Descripcion = "Combo Quesudo", Tipo="C" , Imagen = lstPager.Where(x => x.Codigo == 1531).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(1532), Descripcion = "Combo Ranchero", Tipo="C" , Imagen = lstPager.Where(x => x.Codigo == 1532).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(1533), Descripcion = "Combo Tender", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 1533).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(1534), Descripcion = "Combo Salchiburguer", Tipo="C", Imagen = lstPager.Where(x => x.Codigo == 1534).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(1314), Descripcion = "Combo 5 AGR Nachos", Tipo="C", Imagen = lstPager.Where(x => x.Codigo == 1314).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(251), Descripcion = "Combo 1", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 251).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(252), Descripcion = "Combo 2", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 252).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(253), Descripcion = "Combo 3", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 253).FirstOrDefault().Imagen },
               new Producto{ Codigo = Convert.ToInt32(254), Descripcion = "Combo 4", Tipo="C" ,Imagen = lstPager.Where(x => x.Codigo == 254).FirstOrDefault().Imagen }
            };
            return Task.FromResult(prductos);
        }

        private Task<bool> ConfigureImg()
        {
            try
            {
                var task = new Task(() =>
                {
                    var ImagesConfiteria = Path.Combine(Path.GetDirectoryName(
                                           Assembly.GetEntryAssembly().Location),
                                           "ImagesConfiteria");

                    if (Directory.Exists(ImagesConfiteria))
                    {
                        foreach (var item in Directory.GetFiles(ImagesConfiteria))
                        {
                            if (!item.Contains("nofound")) File.Delete(item);
                        }
                    }
                });
                task.Start();

                if (!task.IsCompleted) { task.Wait(); }
                return Task.FromResult(task.IsCompleted);

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmUcMovies", JsonConvert.SerializeObject(ex.Message), 1);
                return Task.FromResult(false);
            }
        }

        //public static bool In<T>(this T source, params T[] list)
        //{
        //    return list.Contains(source);
        //}

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

        public void IncrementDecrementProducts(object sender, bool Operation, bool OtherProcess = false)
        {
            try
            {
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
            SetCallBacksNull();
            if (BtnPay)
            {
                this.IsEnabled = false;
                ChangePrices();
                ShowDetailModal();
            }
            else
            {
                //SetCallBacksNull();
                Switcher.Navigate(new UCSelectProducts());
            }
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
                    //ActivateTimer();
                    this.IsEnabled = true;
                    this.Opacity = 1;
                    if (DataService41._Combos != null && DataService41._Combos.Count() > 0)
                    {
                        _frmConfirmationModal.Close();
                        PaintDataCombo();
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            foreach (var ListV in lstPager)
                            {
                                foreach (var Combos in DataService41._Combos)
                                {
                                    if (ListV.Descripcion == Combos.Name)
                                    {
                                        ListV.Value = Combos.Quantity;
                                        lv_Products.Items.Refresh();
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        PaintDataCombo();
                    }
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
                    BtnPay = true;
                    BtnMore.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {

                    BtnComprar.Source = new BitmapImage(new Uri(@"/Images/anterior.png", UriKind.Relative));
                    BtnMore.Visibility = System.Windows.Visibility.Hidden;
                    BtnPay = false;
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
                    else
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
                this.tbTimer.Text = Utilities.dataPaypad.PaypadConfiguration.generiC_TIMER;
                this.timer = new TimerTiempo(this.tbTimer.Text);

                this.timer.CallBackClose = response =>
                {
                    if (response == 1)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Switcher.Navigate(new UCCinema());
                        });
                    }
                };

                this.timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.tbTimer.Text = response;
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
            if (this.timer != null)
            {
                this.timer.CallBackClose = null;
                this.timer.CallBackTimer = null;
                this.timer.CallBackStop?.Invoke(1);
            }
        }
        #endregion

        private void BtnMore_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            ChangePrices();
            Switcher.Navigate(new UCSelectProducts());
        }
    }


}
