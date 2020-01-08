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
        List<TypeSeat> SelectedTypeSeats = new List<TypeSeat>();
        DipMap dipMapCurrent = new DipMap();
        CLSGrabador grabador = new CLSGrabador();
        bool _ErrorTransaction = true;
        Utilities utilities = new Utilities();
        FrmLoading frmLoading;
        bool vibraAvailable = false;
        TimerTiempo timer;
        int controlReinicio = 0;
        bool activePay = false;
        public UCSeat(DipMap dipMap)
        {
            InitializeComponent();
            frmLoading = new FrmLoading("¡Cargando la sala!");
            dipMapCurrent = dipMap;
            TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
            ////TODO: eliminar y descomentar linea de abajo
            //TxtDay.Text = dipMap.RoomName;
            TxtDay.Text = dipMap.Day;
            TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
            TxtHour.Text = dipMap.HourFunction;
            TxtSubTitle.Text = dipMap.Language;
            var time = TimeSpan.FromMinutes(double.Parse(dipMap.Duration.Split(' ')[0]));
            TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

            LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
            HideImages();
            ActivateTimer();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {

                    frmLoading.Show();
                    LoadSeats();
                    frmLoading.Close();
                });

                Utilities.DoEvents();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
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

            try
            {
                var response41 = WCFServices41.GetStateRoom(new SCOEST
                {
                    teatro = dipMapCurrent.CinemaId,
                    Sala = dipMapCurrent.RoomId,
                    FechaFuncion = dipMapCurrent.Date,
                    Correo = "prueba@prueba.com",
                    tercero = "1",
                    Funcion = dipMapCurrent.IDFuncion
                });
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
                else
                {
                    OrganizePositionOfSeats(response41);
                }

            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadSeats en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void OrganizePositionOfSeats(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = Convert.ToInt32(est[0].maxCol);

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


                        TypeSeat typeSeat = new TypeSeat
                        {
                            Letter = filas.filRel,
                            Name = string.Concat(filas.filRel, item.Columna),
                            Number = item.Columna.ToString(),
                            Type = item.TipoSilla,
                            RelativeColumn = Convert.ToInt32(filas.maxCol - columnaScore),
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
                int Width = Convert.ToInt32(est[0].maxCol);

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


                        TypeSeat typeSeat = new TypeSeat
                        {
                            Letter = est[filaScore].filRel,
                            Name = string.Concat(est[filaScore].filRel, item.Columna),
                            Number = item.Columna.ToString(),
                            Type = item.TipoSilla,
                            RelativeColumn = Convert.ToInt32(filas.maxCol - columnaScore),
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

        private void MSelectedsetas(object sender, TouchEventArgs eh, TypeSeat item)
        {
            SelectedSeatsMethod(sender, item);
        }

        private void SelectedSeatsMethod(object sender, TypeSeat item)
        {
            try
            {
                SetCallBacksNull();
                ActivateTimer();
            }
            catch (Exception) { }
            try
            {
                Image image = (Image)sender;
                var seatcurrent = SelectedTypeSeats.Where(s => s.Name == item.Name).FirstOrDefault();
                if (seatcurrent == null)
                {
                    if (SelectedTypeSeats.Count < 10)
                    {
                        SelectedTypeSeats.Add(item);
                        image.Source = GetImage("R");
                    }
                }
                else
                {
                    SelectedTypeSeats.Remove(item);
                    image.Source = GetImage(image.Tag.ToString());
                }

                //Label image = (Label)sender;
                //var seatcurrent = SelectedTypeSeats.Where(s => s.Name == item.Name).FirstOrDefault();
                //if (seatcurrent == null)
                //{
                //    if (SelectedTypeSeats.Count < 10)
                //    {
                //        SelectedTypeSeats.Add(item);
                //        image.Foreground = Brushes.Black;
                //    }
                //}
                //else
                //{
                //    SelectedTypeSeats.Remove(item);
                //    image.Foreground = Brushes.White;
                //}

                LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
                Utilities.CantSeats = SelectedTypeSeats.Count;
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
            string icon = "s-disponible";
            if (ckeck == "R")
            {
                icon = "s-seleccionada";
            }
            else if (ckeck == "B")
            {
                icon = "s-bloqueada";
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
                    icon = "s-ocupada";
                }
            }
            else if (ckeck == "M")
            {
                icon = "silla-preferencialv2";

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

                this.IsEnabled = false;
                var frmLoading = new FrmLoading("¡Descargando precios...!");
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
                    ReloadWindow();
                    return;
                }
                List<string> sinTarifa = new List<string>();
                foreach (var selectedTypeSeat in SelectedTypeSeats)
                {
                    var tarifa = new ResponseTarifa();
                    tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type).FirstOrDefault();

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
                    ReloadWindow();
                    return;
                }

                frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(SelectedTypeSeats, dipMapCurrent);
                this.Opacity = 0.3;
                _frmConfirmationModal.ShowDialog();
                if (_frmConfirmationModal.DialogResult.HasValue &&
                    _frmConfirmationModal.DialogResult.Value)
                {
                    Pay.IsEnabled = false;
                    this.Opacity = 1;
                    SecuenceAndReserve();
                }
                else
                {
                    this.Opacity = 1;
                    if (controlReinicio == 0)
                    {
                        ActivateTimer();
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SendData en frmSeat", EError.Aplication, ELevelError.Medium);
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
        private void SecuenceAndReserve()
        {
            this.IsEnabled = false;
            activePay = true;
            frmLoading = new FrmLoading("¡Generando secuencia de compra!");
            try
            {
                try
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                }
                catch { }

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
                    Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");
                    ReloadWindow();
                    return;
                }

                foreach (var item in responseSec41)
                {
                    dipMapCurrent.Secuence = int.Parse(item.Secuencia.ToString());
                    Utilities.Secuencia = item.Secuencia.ToString();
                }

                List<Ubicacione> ubicacione = new List<Ubicacione>();
                foreach (var item in SelectedTypeSeats)
                {
                    ubicacione.Add(new Ubicacione
                    {
                        Columna = item.RelativeColumn,
                        Fila = item.RelativeRow,
                        Tarifa = Convert.ToInt32(item.CodTarifa)
                    });
                }

                frmLoading = new FrmLoading("¡Reservando los puestos seleccionados!");
                frmLoading.Show();
                var response41 = WCFServices41.PostPreventa(new SCOGRU
                {
                    Apellido = "Ecity",
                    Descripcion = dipMapCurrent.MovieName,
                    FechaFuncion = dipMapCurrent.Date,
                    HoraFuncion = dipMapCurrent.Hour,
                    InicioFuncion = dipMapCurrent.HourFormat,
                    Nombre = "Kiosko",
                    Pelicula = dipMapCurrent.MovieId,
                    PuntoVenta = dipMapCurrent.PointOfSale,
                    Sala = dipMapCurrent.RoomId,
                    Secuencia = dipMapCurrent.Secuence,
                    teatro = dipMapCurrent.CinemaId,
                    Telefono = 5803033,
                    tercero = 1,
                    Ubicaciones = ubicacione
                });
                frmLoading.Close();
                if (response41 != null)
                {
                    //foreach (var item in response41)
                    //{
                    //    if (item.Respuesta.Contains("exitoso"))
                    //    {
                    //        //SaveDataBaseLocal();
                    //        if (_ErrorTransaction)
                    //        {
                                ShowPay();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                    //        ReloadWindow();
                    //        break;
                    //    }
                    //}
                }
                else
                {
                    Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                    ReloadWindow();
                    return;
                }

            }
            catch (Exception ex)
            {
                frmLoading.Close();
                AdminPaypad.SaveErrorControl(ex.Message, "SecueceAndReserve en frmSeat", EError.Aplication, ELevelError.Medium);
            }
            frmLoading.Close();
        }

        private void SaveDataBaseLocal()
        {
            try
            {
                _ErrorTransaction = true;
                var response = DBProcinalController.SaveDipMap(dipMapCurrent);
                if (!response.IsSuccess)
                {
                    Utilities.ShowModal(response.Message);
                    _ErrorTransaction = false;
                    return;
                }

                int dipMap = int.Parse(response.Result.ToString());
                dipMapCurrent.DipMapId = dipMap;
                var responseSeat = DBProcinalController.SaveTypeSeats(SelectedTypeSeats, dipMap);
                if (!responseSeat.IsSuccess)
                {
                    Utilities.ShowModal(responseSeat.Message);
                    _ErrorTransaction = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SaveDataBaseLocal en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }



        /// <summary>
        /// Envía al formulario de pagos
        /// </summary>
        private async void ShowPay()
        {
            try
            {
                btnAtras.IsEnabled = false;
                FrmLoading frmLoading = new FrmLoading("¡Creando la transacción...!");
                frmLoading.Show();
                var response = await utilities.CreateTransaction("Cine ", dipMapCurrent, SelectedTypeSeats);
                frmLoading.Close();

                Dispatcher.BeginInvoke((Action)delegate
                {
                    this.IsEnabled = true;
                    btnAtras.IsEnabled = true;
                });

                if (!response)
                {
                    List<TypeSeat> lista = new List<TypeSeat>();
                    foreach (var item in SelectedTypeSeats)
                    {
                        lista.Add(item);
                    }
                    WCFServices41.PostDesAssingreserva(lista, dipMapCurrent);

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Opacity = 0.3;
                        Pay.IsEnabled = true;
                        this.IsEnabled = true;
                        Utilities.ShowModal("No se pudo crear la transacción, por favor intente de nuevo.");
                        this.Opacity = 1;
                    });
                    GC.Collect();
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
                    Utilities.ScorePayValue = Utilities.ValorPagarScore;

                    frmPointsConfirmation _frmModalPuntos = new frmPointsConfirmation();
                    this.Opacity = 0.3;
                    _frmModalPuntos.ShowDialog();
                    if (_frmModalPuntos.DialogResult.HasValue && _frmModalPuntos.DialogResult.Value)
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        Switcher.Navigate(new UCDatos(SelectedTypeSeats, dipMapCurrent));
                    }
                    else
                    {
                        if (Utilities.MedioPago == 1)
                        {
                            try
                            {
                                SetCallBacksNull();
                                timer.CallBackStop?.Invoke(1);
                            }
                            catch { }

                            LogService.SaveRequestResponse("=".PadRight(5, '=') + "Transacción de " + DateTime.Now + ": ", "ID: " + Utilities.IDTransactionDB);
                            Utilities.controlStop = 0;
                            int i = 0;

                            Switcher.Navigate(new UCPayCine(SelectedTypeSeats, dipMapCurrent));
                        }
                        else
                        {
                            try
                            {
                                SetCallBacksNull();
                                timer.CallBackStop?.Invoke(1);
                            }
                            catch { }
                            Switcher.Navigate(new UCCardPayment(SelectedTypeSeats, dipMapCurrent));
                        }
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
                List<TypeSeat> lista = new List<TypeSeat>();
                foreach (var item in SelectedTypeSeats)
                {
                    lista.Add(item);
                }
                WCFServices41.PostDesAssingreserva(lista, dipMapCurrent);
            }
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            Switcher.Navigate(new UCSchedule(Utilities.Movie));
        }

        private void Pay_TouchDown(object sender, TouchEventArgs e)
        {
            SendData();
        }


    }
}
