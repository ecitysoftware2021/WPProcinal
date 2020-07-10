using Grabador.Transaccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCSeat.xaml
    /// </summary>
    public partial class UCSeat : UserControl
    {
        List<ChairsInformation> SelectedTypeSeats = new List<ChairsInformation>();
        FunctionInformation dipMapCurrent = new FunctionInformation();

        bool _ErrorTransaction = true;

        bool vibraAvailable = false;
        TimerTiempo timer;
        bool activePay = false;

        ApiLocal api;
        CLSGrabador grabador;

        public UCSeat(FunctionInformation dipMap)
        {
            InitializeComponent();
            try
            {

                api = new ApiLocal();
                grabador = new CLSGrabador();
                DataService41._Combos = new List<Combos>();

                dipMapCurrent = dipMap;
                TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
                TxtRoom.Text = dipMap.RoomName;
                TxtDay.Text = dipMap.Day;
                TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
                TxtHour.Text = "Hora Función: " + dipMap.HourFunction;
                TxtSubTitle.Text = "Idioma: " + dipMap.Language;
                tbDiaActual.Text = "Fecha Actual: " + DateTime.Now.ToLongDateString();

                var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
                TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

                LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
                HideImages();
                ActivateTimer();

                Utilities.Speack("Elige tus ubicaciones y presiona comprar.");

            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "UCSeat en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    LoadSeats();
                });

                //Utilities.DoEvents();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "UserControl_Loaded en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerSilla");
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
                        tbTimer.Text = "Tiempo de transacción: " + response;
                        tbHoraActual.Text = "Hora actual: " + DateTime.Now.ToString("hh:mm:ss");
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

        private void HideImages()
        {
            try
            {
                ImgDiscapacitados.Visibility = Visibility.Hidden;
                ImgDisponible.Visibility = Visibility.Visible;
                ImgNoDisponible.Visibility = Visibility.Visible;
                ImgBlack.Visibility = Visibility.Hidden;
                ImgSeleccionada.Visibility = Visibility.Visible;
                ImgVibroSound.Visibility = Visibility.Hidden;

                VisibilityImages();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "HideImages en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void VisibilityImages()
        {
            try
            {

                if (dipMapCurrent.TypeZona == "General")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Hidden;
                }
                else if (dipMapCurrent.TypeZona == "Vibrasound")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Visible;
                    vibraAvailable = true;
                }
                else if (dipMapCurrent.TypeZona == "V")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    //ImgPreferencia.Visibility = Visibility.Visible;
                }
                else
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Visible;
                    //ImgPreferencia.Visibility = Visibility.Visible;
                    vibraAvailable = true;
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void LoadSeats()
        {

            FrmLoading frmLoading = new FrmLoading("¡Descargando la sala!");
            try
            {
                frmLoading.Show();
                var response41 = WCFServices41.GetStateRoom(new SCOEST
                {
                    teatro = dipMapCurrent.CinemaId,
                    Sala = dipMapCurrent.RoomId,
                    FechaFuncion = dipMapCurrent.Date,
                    Correo = "prueba@prueba.com",
                    tercero = "1",
                    Funcion = dipMapCurrent.IDFuncion
                });
                frmLoading.Close();
                if (response41 == null)
                {
                    try
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                    }
                    catch { }
                    Utilities.ShowModal("Lo sentimos, no se pudo descargar la sala, por favor intente de nuevo");
                    Switcher.Navigate(new UCCinema());
                    return;
                }
                if (dipMapCurrent.CinemaId == (int)Dictionaries.ECinemas.Monterrey)
                {
                    if (dipMapCurrent.RoomId == 4 || dipMapCurrent.RoomId == 5)
                    {
                        OrganizePositionOfSeats(response41);
                    }
                    else
                    {
                        OrganizePositionOfSeatsInverted(response41);
                    }
                }
                else if (dipMapCurrent.CinemaId == (int)Dictionaries.ECinemas.Mayorca)
                {
                    if (dipMapCurrent.RoomId == 10)
                    {
                        Utilities.PlateObligatory = true;

                        WTCModal modal = new WTCModal(Utilities.GetConfiguration("MensajeUbicaciones"));
                        modal.ShowDialog();

                        OrganizePositionOfSeatsInvertedAutocine(response41);
                    }
                    else
                    {
                        Utilities.PlateObligatory = false;
                        OrganizePositionOfSeats(response41);
                    }
                }
                else
                {
                    OrganizePositionOfSeats(response41);
                }

            }
            catch (Exception ex)
            {
                frmLoading.Close();
                AdminPaypad.SaveErrorControl(ex.Message, "LoadSeats en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void OrganizePositionOfSeats(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());

                for (int i = 0; i <= Height; i++)
                {
                    gridSillas.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i <= Width; i++)
                {
                    gridSillas.ColumnDefinitions.Add(new ColumnDefinition());
                }
                int fila = 0;
                int columnaUsuario = 0;
                int columnaScore = Width - 1;
                int filaScore = Height - 1;
                foreach (var filas in est)
                {
                    foreach (var item in filas.DescripcionSilla)
                    {


                        ImageSource imageSource = null;
                        if (item.TipoSilla == "General")
                        {
                            imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(string.Empty);
                        }
                        else if (item.TipoSilla == "pasillo")
                        {
                            imageSource = GetImage(item.TipoSilla);

                        }
                        else
                        {
                            imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(item.TipoSilla);
                        }

                        Image image = new Image
                        {
                            Source = imageSource,
                            Height = 26,
                            Width = 30,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness
                            {
                                Right = 0,
                                Bottom = 0,
                                Top = 0,
                                Left = 0,
                            },
                            Name = string.Concat(filas.filRel, item.Columna),
                            Tag = item.TipoSilla,
                        };


                        ChairsInformation typeSeat = new ChairsInformation
                        {
                            Letter = filas.filRel,
                            Name = string.Concat(filas.filRel, item.Columna),
                            Number = item.Columna.ToString(),
                            Type = item.TipoSilla,
                            RelativeColumn = int.Parse((filas.maxCol - columnaScore).ToString()),
                            RelativeRow = filas.filRel,
                        };


                        Label labelSeat = new Label();
                        labelSeat.FontSize = 10;
                        labelSeat.FontWeight = FontWeights.Bold;
                        labelSeat.Content = string.Concat(filas.filRel, item.Columna);
                        labelSeat.Margin = new Thickness(0, 10, 0, 0);
                        labelSeat.Height = 25;

                        if (item.EstadoSilla == "S" && item.TipoSilla != "Discapacitado" && item.TipoSilla != "pasillo")
                        {
                            image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, typeSeat));
                        }

                        if (item.TipoSilla != "pasillo")
                        {
                            gridSillas.Children.Add(labelSeat);
                            Grid.SetColumn(labelSeat, columnaUsuario);
                            Grid.SetRow(labelSeat, fila);

                            gridSillas.Children.Add(image);
                            Grid.SetColumn(image, columnaUsuario);
                            Grid.SetRow(image, fila);
                        }


                        columnaUsuario++;
                        columnaScore--;
                    }

                    columnaUsuario = 0;
                    columnaScore = Width - 1;
                    fila++;
                }


            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "OrganizePositionOfSeats",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
            }
        }

        private void OrganizePositionOfSeatsInverted(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());

                for (int i = 0; i <= Height; i++)
                {
                    gridSillas.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i <= Width; i++)
                {
                    gridSillas.ColumnDefinitions.Add(new ColumnDefinition());
                }
                int fila = 0;
                int columnaUsuario = 0;
                int columnaScore = Width - 1;
                int filaScore = Height - 1;
                foreach (var filas in est)
                {
                    foreach (var item in filas.DescripcionSilla)
                    {


                        ImageSource imageSource = null;
                        if (item.TipoSilla == "General")
                        {
                            imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(string.Empty);
                        }
                        else if (item.TipoSilla == "pasillo")
                        {
                            imageSource = GetImage(item.TipoSilla);

                        }
                        else
                        {
                            imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(item.TipoSilla);
                        }

                        Image image = new Image
                        {
                            Source = imageSource,
                            Height = 26,
                            Width = 30,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness
                            {
                                Right = 0,
                                Bottom = 0,
                                Top = 0,
                                Left = 0,
                            },
                            Name = string.Concat(est[filaScore].filRel, item.Columna),
                            Tag = item.TipoSilla,
                        };


                        ChairsInformation typeSeat = new ChairsInformation
                        {
                            Letter = est[filaScore].filRel,
                            Name = string.Concat(est[filaScore].filRel, item.Columna),
                            Number = item.Columna.ToString(),
                            Type = item.TipoSilla,
                            RelativeColumn = int.Parse((filas.maxCol - columnaScore).ToString()),
                            RelativeRow = filas.filRel,
                        };


                        Label labelSeat = new Label();
                        labelSeat.FontSize = 10;
                        labelSeat.FontWeight = FontWeights.Bold;
                        labelSeat.Content = string.Concat(est[filaScore].filRel, item.Columna);
                        labelSeat.Margin = new Thickness(0, 10, 0, 0);
                        labelSeat.Height = 25;

                        if (item.EstadoSilla == "S" && item.TipoSilla != "Discapacitado" && item.TipoSilla != "pasillo")
                        {
                            image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, typeSeat));
                        }

                        if (item.TipoSilla != "pasillo")
                        {
                            gridSillas.Children.Add(labelSeat);
                            Grid.SetColumn(labelSeat, columnaUsuario);
                            Grid.SetRow(labelSeat, fila);

                            gridSillas.Children.Add(image);
                            Grid.SetColumn(image, columnaUsuario);
                            Grid.SetRow(image, fila);

                        }

                        columnaUsuario++;
                        columnaScore--;
                    }
                    columnaUsuario = 0;
                    columnaScore = Width - 1;
                    if (filas.DescripcionSilla.Count > 0)
                    {

                        filaScore--;
                        fila++;
                    }
                }


            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "OrganizePositionOfSeatsInverted",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
            }
        }

        private void OrganizePositionOfSeatsInvertedAutocine(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());
                //est.Reverse();

                for (int i = 0; i <= Height; i++)
                {
                    gridSillas.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i <= Width; i++)
                {
                    gridSillas.ColumnDefinitions.Add(new ColumnDefinition());
                }
                int fila = 0;
                int columnaUsuario = 0;
                int columnaScore = Width - 1;
                int filaScore = Height - 1;
                foreach (var filas in est)
                {
                    foreach (var item in filas.DescripcionSilla)
                    {


                        ImageSource imageSource = null;
                        if (item.TipoSilla == "General")
                        {
                            imageSource = ((item.EstadoSilla == "B") || (item.EstadoSilla == "O")) ? GetImage(item.EstadoSilla) : GetImage(string.Empty);
                        }
                        else if (item.TipoSilla == "pasillo")
                        {
                            imageSource = GetImage(item.TipoSilla);

                        }
                        else
                        {
                            imageSource = ((item.EstadoSilla == "B") || (item.EstadoSilla == "O")) ? GetImage(item.EstadoSilla) : GetImage(item.TipoSilla);
                        }

                        Image image = new Image
                        {
                            Source = imageSource,
                            Height = 35,
                            Width = 35,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness
                            {
                                Right = 0,
                                Bottom = 20,
                                Top = 0,
                                Left = 0,
                            },
                            Name = string.Concat(filas.filRel, (columnaScore + 1)),
                            Tag = item.TipoSilla,
                        };


                        ChairsInformation typeSeat = new ChairsInformation
                        {
                            Letter = filas.filRel,
                            Name = string.Concat(filas.filRel, (columnaScore + 1)),
                            Number = (columnaScore + 1).ToString(),
                            Type = item.TipoSilla,
                            RelativeColumn = int.Parse((filas.maxCol - columnaScore).ToString()),
                            RelativeRow = filas.filRel,
                        };


                        Label labelSeat = new Label();
                        labelSeat.FontSize = 20;
                        labelSeat.FontWeight = FontWeights.Bold;
                        labelSeat.Content = string.Concat(filas.filRel, (columnaScore + 1));
                        labelSeat.Margin = new Thickness(0, 0, 0, 70);
                        labelSeat.Height = 35;


                        if (item.EstadoSilla == "S" && item.TipoSilla != "Discapacitado" && item.TipoSilla != "pasillo")
                        {
                            image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, typeSeat));
                        }

                        if (item.TipoSilla != "pasillo")
                        {
                            gridSillas.Children.Add(labelSeat);
                            Grid.SetColumn(labelSeat, columnaScore);
                            Grid.SetRow(labelSeat, fila);

                            gridSillas.Children.Add(image);
                            Grid.SetColumn(image, columnaScore);
                            Grid.SetRow(image, fila);

                        }

                        columnaUsuario++;
                        columnaScore--;
                    }
                    columnaUsuario = 0;
                    columnaScore = Width - 1;
                    if (filas.DescripcionSilla.Count > 0)
                    {

                        filaScore--;
                        fila++;
                    }
                }


            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "OrganizePositionOfSeatsInverted",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
            }
        }


        private void MSelectedsetas(object sender, TouchEventArgs eh, ChairsInformation item)
        {
            SelectedSeatsMethod(sender, item);
        }

        private void SelectedSeatsMethod(object sender, ChairsInformation item)
        {
            try
            {
                Image image = (Image)sender;
                var seatcurrent = SelectedTypeSeats.Where(s => s.Name == item.Name).FirstOrDefault();
                if (seatcurrent == null)
                {
                    if (SelectedTypeSeats.Count < 10)
                    {
                        item.Quantity = 1;
                        SelectedTypeSeats.Add(item);
                        image.Source = GetImage("R");
                    }
                }
                else
                {
                    SelectedTypeSeats.Remove(item);
                    image.Source = GetImage(image.Tag.ToString());
                }

                LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "SelectedSeatsMethod",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
            }
        }

        private ImageSource GetImage(string ckeck)
        {
            string icon = !Utilities.PlateObligatory ? "s-disponible" : "s-disponible-car";
            if (ckeck == "R")
            {
                icon = !Utilities.PlateObligatory ? "s-seleccionada" : "s-seleccionada-car";
            }
            else if (ckeck == "B" || ckeck == "O")
            {
                icon = !Utilities.PlateObligatory ? "s-bloqueada" : "s-bloqueada-car";
                ImgNoDisponible.Visibility = Visibility.Visible;
            }
            else if (ckeck == "Discapacitado")
            {
                icon = "discapacitado";
                ImgDiscapacitados.Visibility = Visibility.Visible;
            }
            else if (ckeck == "F")
            {
                icon = "silla-puffv2";
            }
            else if (ckeck == "Vibrasound")
            {
                if (vibraAvailable)
                {
                    icon = !Utilities.PlateObligatory ? "s-ocupada" : "s-ocupada-car";
                }
            }
            else if (ckeck == "M")
            {
                icon = "s-preferencial";

            }
            else if (ckeck == "Black Star")
            {
                icon = "s-black";
                ImgBlack.Visibility = Visibility.Visible;
            }
            else if (ckeck == "pasillo")
            {
                icon = "";
            }

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Concat("/Images/Seats/", icon, ".png"), UriKind.Relative);
            logo.EndInit();
            return logo;
        }

        private void SendData()
        {
            try
            {
                int control = 0;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                if (SelectedTypeSeats.Count == 0)
                {
                    Utilities.ShowModal("Debe seleccionar almenos un puesto");
                    ActivateTimer();
                    return;
                }

                if (GetPrices())
                {

                    if (Utilities.FechaSeleccionada.ToString("dd/MM/yyyy") != DateTime.Now.ToString("dd/MM/yyyy") || Utilities.PlateObligatory)
                    {
                        frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(SelectedTypeSeats, dipMapCurrent);
                        this.Opacity = 0.3;
                        _frmConfirmationModal.ShowDialog();
                        this.Opacity = 1;
                        if (_frmConfirmationModal.DialogResult.HasValue &&
                            _frmConfirmationModal.DialogResult.Value)
                        {

                            WTCModal modal = new WTCModal(Utilities.GetConfiguration("MensajeCinefans"), Utilities.GetConfiguration("MensajeURL"));
                            modal.ShowDialog();
                            GoToPay();
                        }
                        else
                        {
                            ActivateTimer();
                        }

                    }
                    else
                    {
                        if (!GetCombo())
                        {
                            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(SelectedTypeSeats, dipMapCurrent);
                            this.Opacity = 0.3;
                            _frmConfirmationModal.ShowDialog();
                            this.Opacity = 1;
                            if (_frmConfirmationModal.DialogResult.HasValue &&
                                _frmConfirmationModal.DialogResult.Value)
                            {

                                WTCModal modal = new WTCModal(Utilities.GetConfiguration("MensajeCinefans"), Utilities.GetConfiguration("MensajeURL"));
                                modal.ShowDialog();
                                GoToPay();
                            }
                            else
                            {
                                ActivateTimer();
                            }
                        }
                        else
                        {
                            SecuenceAndReserveToConfectionery();
                        }
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        Utilities.SendMailErrores($"No fúe posible obtener las tarífas en Score");
                    });
                    ReloadWindow();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SendData en frmSeat", EError.Aplication, ELevelError.Medium);

                Utilities.ShowModal("Lo sentimos, no fué posible consultar las tarifas, por favor intente de nuevo.");
                ReloadWindow();
            }
        }

        private void GoToPay()
        {
            List<Ubicacione> ubicacione = OrganizeSeatsTuReserve();
            var sec = GetSecuence();
            if (sec)
            {
                List<ResponseScogru> responseReserve = Reserve(ubicacione);
                if (responseReserve != null)
                {
                    foreach (var item in responseReserve)
                    {
                        if (item.Respuesta.Contains("exitoso"))
                        {
                            ShowPay();
                            break;
                        }
                        else
                        {
                            Task.Run(() =>
                            {
                                Utilities.SendMailErrores($"No fúe posible realizar la reserva en la transaccion: {Utilities.IDTransactionDB}" +
                                    $" <br> Error: {item.Respuesta}");
                            });
                            Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                            ReloadWindow();
                            break;
                        }
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        Utilities.SendMailErrores($"No fúe posible realizar la reserva en la transaccion: {Utilities.IDTransactionDB} <br> no hubo respuesta del servicio");
                    });
                    Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                    ReloadWindow();
                    return;
                }
            }
            else
            {
                ReloadWindow();
            }
        }

        private bool GetCombo()
        {
            FrmLoading frmLoading = new FrmLoading("¡Descargando la confitería...!");
            try
            {
                frmLoading.Show();
                var combos = WCFServices41.GetConfectionery(new SCOPRE
                {
                    teatro = Utilities.GetConfiguration("CodCinema"),
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
            catch (Exception ex)
            {
                frmLoading.Close();
                return false;
            }



        }

        private bool GetPrices()
        {
            this.IsEnabled = false;
            FrmLoading frmLoading = new FrmLoading("¡Descargando los precios...!");
            try
            {
                frmLoading.Show();
                var response41 = WCFServices41.GetPrices(new SCOPLA
                {
                    FechaFuncion = dipMapCurrent.Date,
                    InicioFuncion = dipMapCurrent.HourFormat,
                    Pelicula = dipMapCurrent.MovieId,
                    Sala = dipMapCurrent.RoomId,
                    teatro = dipMapCurrent.CinemaId,
                    tercero = "1"
                });
                this.IsEnabled = true;
                frmLoading.Close();
                if (response41 == null)
                {
                    Utilities.ShowModal("Lo sentimos, no fué posible consultar las tarifas, por favor intente de nuevo.");
                    return false;
                }
                List<string> sinTarifa = new List<string>();
                foreach (var selectedTypeSeat in SelectedTypeSeats)
                {
                    var tarifa = new ResponseTarifa();
                    if (DataService41.dataUser.Tarjeta != null)
                    {
                        tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type && t.ClienteFrecuente.ToLower() == "habilitado").FirstOrDefault();
                    }
                    else
                    {
                        tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type).FirstOrDefault();
                    }

                    if (tarifa != null)
                    {
                        if (tarifa.silla != null)
                        {
                            selectedTypeSeat.Price = Convert.ToDecimal(tarifa.valor);
                            selectedTypeSeat.CodTarifa = tarifa.codigo.ToString();
                        }
                        else
                        {
                            sinTarifa.Add(selectedTypeSeat.Name);
                        }
                    }
                    else
                    {
                        sinTarifa.Add(selectedTypeSeat.Name);
                    }
                }

                var stringerror = new StringBuilder();
                stringerror.Append("No se encontraron tarifas para los puestos: ");
                stringerror.AppendLine();
                foreach (var item in sinTarifa)
                {
                    stringerror.Append(item);
                    stringerror.Append(", ");

                }
                stringerror.AppendLine();
                stringerror.Append("Por favor vuelva a intentarlo o seleccione un horario diferente.");

                if (sinTarifa.Count > 0)
                {
                    Utilities.ShowModal(stringerror.ToString());
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                frmLoading.Close();
                return false;
            }
        }

        /// <summary>
        /// Vuelve a cargar la vista si hay error en las sillas
        /// </summary>
        private void ReloadWindow()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                Switcher.Navigate(new UCSeat(dipMapCurrent));
            });
        }

        /// <summary>
        /// Reserva los puestos
        /// </summary>
        /// 
        private void SecuenceAndReserveToConfectionery()
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);

            this.IsEnabled = false;
            activePay = true;
            try
            {


                var sec = GetSecuence();
                if (sec)
                {
                    List<Ubicacione> ubicacione = OrganizeSeatsTuReserve();


                    List<ResponseScogru> response41 = Reserve(ubicacione);
                    if (response41 != null)
                    {
                        foreach (var item in response41)
                        {
                            if (item.Respuesta.Contains("exitoso"))
                            {
                                NavigateToConfectionery();
                                break;
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    Utilities.SendMailErrores($"No fúe posible realizar la reserva en la transaccion: {Utilities.IDTransactionDB}" +
                                        $" <br> Error: {item.Respuesta}");
                                });
                                Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                                ReloadWindow();
                                break;
                            }
                        }
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            Utilities.SendMailErrores($"No fúe posible realizar la reserva en la transaccion: {Utilities.IDTransactionDB}" +
                                $"");
                        });
                        Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                        ReloadWindow();
                        return;
                    }
                }
                else
                {
                    ReloadWindow();
                }

            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                AdminPaypad.SaveErrorControl(ex.Message, "SecueceAndReserve en frmSeat", EError.Aplication, ELevelError.Medium);

                Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                ReloadWindow();
            }
        }

        private List<Ubicacione> OrganizeSeatsTuReserve()
        {
            List<Ubicacione> ubicacione = new List<Ubicacione>();
            foreach (var item in SelectedTypeSeats)
            {
                ubicacione.Add(new Ubicacione
                {
                    Columna = item.RelativeColumn,
                    Fila = item.RelativeRow,
                    Tarifa = int.Parse(item.CodTarifa)
                });
            }

            return ubicacione;
        }

        private List<ResponseScogru> Reserve(List<Ubicacione> ubicacione)
        {
            var dataClient = GetDataClient();
            FrmLoading frmLoading = new FrmLoading("¡Reservando los puestos seleccionados!");
            frmLoading.Show();

            var response41 = WCFServices41.PostPreventa(new SCOGRU
            {
                Apellido = dataClient.Apellido,
                Descripcion = dipMapCurrent.MovieName,
                FechaFuncion = dipMapCurrent.Date,
                HoraFuncion = dipMapCurrent.Hour,
                InicioFuncion = dipMapCurrent.HourFormat,
                Nombre = dataClient.Nombre,
                Pelicula = dipMapCurrent.MovieId,
                PuntoVenta = dipMapCurrent.PointOfSale,
                Sala = dipMapCurrent.RoomId,
                Secuencia = dipMapCurrent.Secuence,
                teatro = dipMapCurrent.CinemaId,
                Telefono = !string.IsNullOrEmpty(dataClient.Telefono) ? long.Parse(dataClient.Telefono) : 0,
                tercero = 1,
                Ubicaciones = ubicacione
            });
            frmLoading.Close();
            return response41;
        }

        private bool GetSecuence()
        {
            FrmLoading frmLoading = new FrmLoading("¡Generando secuencia de compra!");
            try
            {
                frmLoading.Show();
                var responseSec41 = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = dipMapCurrent.PointOfSale,
                    teatro = dipMapCurrent.CinemaId,
                    tercero = "1"
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
                    dipMapCurrent.Secuence = int.Parse(item.Secuencia.ToString());
                    DataService41.Secuencia = item.Secuencia.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                frmLoading.Close();
                Task.Run(() =>
                {
                    Utilities.SendMailErrores($"No se pudo obtener la secuencia de compra en la transaccion: {Utilities.IDTransactionDB}" +
                        $"");
                });
                Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");
                return false;
            }
        }

        SCOLOGResponse GetDataClient()
        {
            if (DataService41.dataUser.Tarjeta != null)
            {
                return new SCOLOGResponse
                {
                    Apellido = DataService41.dataUser.Apellido,
                    Tarjeta = DataService41.dataUser.Tarjeta,
                    Login = DataService41.dataUser.Login,
                    Direccion = DataService41.dataUser.Direccion,
                    Documento = DataService41.dataUser.Documento,
                    Nombre = DataService41.dataUser.Nombre,
                    Telefono = DataService41.dataUser.Telefono
                };
            }
            else
            {
                return new SCOLOGResponse
                {
                    Apellido = "Ecity",
                    Tarjeta = "0",
                    Login = "prueba@prueba.com",
                    Direccion = "Cra 63A # 34-70",
                    Documento = "811040812",
                    Nombre = "Kiosko",
                    Telefono = "5803033"
                };
            }
        }

        private void NavigateToConfectionery()
        {
            this.IsEnabled = false;
            Switcher.Navigate(new UCProductsCombos(SelectedTypeSeats, dipMapCurrent));
        }

        /// <summary>
        /// Envía al formulario de pagos
        /// </summary>
        private async void ShowPay()
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
                frmLoading.Show();
                var response = await Utilities.CreateTransaction("Cine ", dipMapCurrent, SelectedTypeSeats);
                frmLoading.Close();

                if (!response)
                {
                    List<ChairsInformation> lista = new List<ChairsInformation>();
                    foreach (var item in SelectedTypeSeats)
                    {
                        lista.Add(item);
                    }
                    this.IsEnabled = false;
                    frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    frmLoading.Show();
                    Utilities.CancelAssing(lista, dipMapCurrent);
                    frmLoading.Close();
                    this.IsEnabled = true;
                    Utilities.ShowModal("No se pudo crear la transacción, por favor intente de nuevo.");

                    frmLoading = new FrmLoading("¡Reconectando...!");
                    frmLoading.Show();
                    await api.SecurityToken();
                    frmLoading.Close();
                    ReloadWindow();
                }
                else
                {
                    try
                    {
                        Task.Run(() =>
                        {
                            grabador.Grabar(Utilities.IDTransactionDB, 0);
                        });
                    }
                    catch { }

                    if (Utilities.MedioPago == EPaymentType.Cash)
                    {
                        Switcher.Navigate(new UCPayCine(SelectedTypeSeats, dipMapCurrent));
                    }
                    else if (Utilities.MedioPago == EPaymentType.Card)
                    {
                        Switcher.Navigate(new UCCardPayment(SelectedTypeSeats, dipMapCurrent));
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }



        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            if (activePay)
            {
                List<ChairsInformation> lista = new List<ChairsInformation>();
                foreach (var item in SelectedTypeSeats)
                {
                    lista.Add(item);
                }
                this.IsEnabled = false;
                FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                frmLoading.Show();
                Utilities.CancelAssing(lista, dipMapCurrent);
                frmLoading.Close();
                this.IsEnabled = true;
            }
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            Switcher.Navigate(new UCSchedule(DataService41.Movie));
        }

        private void Pay_TouchDown(object sender, TouchEventArgs e)
        {
            SendData();
        }


    }
}
