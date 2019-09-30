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
            //TODO: eliminar y descomentar linea de abajo
            TxtDay.Text = dipMap.RoomName;
            //TxtDay.Text = dipMap.Day;
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
                //ImgPreferencia.Visibility = Visibility.Hidden;
                ImgPuff.Visibility = Visibility.Hidden;
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
                foreach (var item in dipMapCurrent.TypeZona)
                {
                    if (item.NombreZona == "GENERAL")
                    {
                        ImgDisponible.Visibility = Visibility.Visible;
                        ImgVibroSound.Visibility = Visibility.Hidden;
                    }
                    else if (item.NombreZona == "VIBRASOUND")
                    {
                        ImgDisponible.Visibility = Visibility.Visible;
                        ImgVibroSound.Visibility = Visibility.Visible;
                        vibraAvailable = true;
                    }
                    else if (item.NombreZona == "V")
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
                var responseV241 = WCFServices41.GetStateRoom(new SCOEST
                {
                    teatro = dipMapCurrent.CinemaId,
                    Sala = dipMapCurrent.RoomId,
                    FechaFuncion = dipMapCurrent.Date,
                    Correo = "pruebacorreo@gmail.com",
                    tercero = "1",
                    Funcion = dipMapCurrent.IDFuncion
                });
                OrganizePositionOfSeats(responseV241);
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
                //est = est.ToArray().Reverse().ToList();
                int Height = est.Count();
                int Width = Convert.ToInt32(est[0].maxCol);

                for (int i = 0; i < Height; i++)
                {
                    gridSillas.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i < Width; i++)
                {
                    gridSillas.ColumnDefinitions.Add(new ColumnDefinition());
                }
                if (dipMapCurrent.CinemaId == (int)Dictionaries.ECinemas.Monterrey)
                {
                    if (dipMapCurrent.RoomId == 4 || dipMapCurrent.RoomId == 5 || dipMapCurrent.RoomId == 1)
                    {
                        OrganizedByRoom(est);
                    }
                    else
                    {
                        //OrganizedByRoomInvert(est);
                        OrganizedByRoom(est);
                    }
                }
                else
                {
                    OrganizedByRoom(est);
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void OrganizedByRoom(List<EstadoSala41> est)
        {
            int fila = 0;
            foreach (var filas in est)
            {

                foreach (var item in filas.DescripcionSilla)
                {

                    if (item.TipoSilla != "pasillo")
                    {
                        ImageSource imageSource = null;
                        if (item.TipoSilla == "General")
                        {
                            imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(string.Empty);
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
                                Right = 5,
                                Bottom = 0,
                                Top = -60,
                                Left = 12,
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
                            RelativeColumn = Convert.ToInt32(filas.maxCol),
                            RelativeRow = filas.filRel
                        };
                        //Label labelSeat = new Label();
                        //labelSeat.FontSize = 10;
                        //labelSeat.FontWeight = FontWeights.Bold;

                        //labelSeat.Content = string.Concat(filas.filRel, item.Columna, Environment.NewLine, "- Est: " + item.EstadoSilla);

                        //labelSeat.Margin = new Thickness(12, -60, 5, 0);
                        //labelSeat.Height = 40;
                        //labelSeat.Background = Brushes.Blue;
                        //labelSeat.Foreground = Brushes.White;

                        if (item.EstadoSilla == "S" && item.TipoSilla != "Discapacitado")
                        {
                            image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, typeSeat));
                        }

                        //gridSillas.Children.Add(labelSeat);

                        //Grid.SetColumn(labelSeat, Convert.ToInt32(item.Columna));
                        //Grid.SetRow(labelSeat, fila);

                        gridSillas.Children.Add(image);

                        Grid.SetColumn(image, Convert.ToInt32(item.Columna));
                        Grid.SetRow(image, fila);


                    }
                }
                fila++;
            }

        }

        //private void OrganizedByRoomInvert(List<EstadoSala41> est)
        //{
        //    int fila = 0;
        //    foreach (var filas in est)
        //    {
        //        foreach (var item in filas.DescripcionSilla)
        //        {

        //            if (item.TipoSilla != "pasillo")
        //            {
        //                ImageSource imageSource = null;
        //                if (item.TipoSilla == "General")
        //                {
        //                    imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(string.Empty);
        //                }
        //                else
        //                {
        //                    imageSource = (item.EstadoSilla == "B") ? GetImage(item.EstadoSilla) : GetImage(item.TipoSilla);
        //                }

        //                Image image = new Image
        //                {
        //                    Source = imageSource,
        //                    Height = 26,
        //                    Width = 30,
        //                    VerticalAlignment = VerticalAlignment.Top,
        //                    HorizontalAlignment = HorizontalAlignment.Left,
        //                    Margin = new Thickness
        //                    {
        //                        Right = 5,
        //                        Bottom = 0,
        //                        Top = -60,
        //                        Left = 12,
        //                    },
        //                    Name = string.Concat(filas.filRel, item.Columna),
        //                    Tag = item.TipoSilla,
        //                };

        //                TypeSeat typeSeat = new TypeSeat
        //                {
        //                    Letter = filas.filRel,
        //                    Name = string.Concat(filas.filRel, item.Columna),
        //                    Number = item.Columna.ToString(),
        //                    Type = item.TipoSilla,

        //                };

        //                //Label labelSeat = new Label();
        //                //labelSeat.FontSize = 10;
        //                //labelSeat.FontWeight = FontWeights.Bold;

        //                //labelSeat.Content = string.Concat(filas.filRel, item.Columna);

        //                //labelSeat.Margin = new Thickness(12, -60, 5, 0);
        //                //labelSeat.Height = 20;
        //                //labelSeat.Background = Brushes.Blue;
        //                //labelSeat.Foreground = Brushes.White;

        //                if (item.EstadoSilla == "S" && item.TipoSilla != "Discapacitado")
        //                {
        //                    image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, typeSeat));
        //                }

        //                //gridSillas.Children.Add(labelSeat);

        //                //Grid.SetColumn(labelSeat, Convert.ToInt32(filas.maxCol - item.Columna));
        //                //Grid.SetRow(labelSeat, fila);

        //                gridSillas.Children.Add(image);

        //                Grid.SetColumn(image, Convert.ToInt32(item.Columna));
        //                Grid.SetRow(image, fila);

        //            }
        //        }
        //        fila++;
        //    }
        //}

        //private void OrganizePositionOfSeats(List<SeatTmp> states,  int positionX, List<string> FTDistinc, int positionY, List<TypeSeat> typeSeats, List<SeatTmp> seatTmps)
        //{
        //    try
        //    {
        //        int left = 0;
        //        int top = 0;
        //        int sum = int.Parse((1020 / positionY).ToString());
        //        int sum2 = int.Parse((700 / positionX).ToString());

        //        for (int i = 0; i < FTDistinc.Count(); i++)
        //        {
        //            int topNew = i != 0 ? top + sum2 : top;
        //            left = 35;
        //            int count = 0;
        //            foreach (var item in typeSeats)
        //            {
        //                try
        //                {
        //                    if (item.Letter.Equals(FTDistinc[i]))
        //                    {
        //                        int newleft = count != 0 ? left + sum : left;
        //                        var type = seatTmps.Where(t => t.Name == item.Name).FirstOrDefault();
        //                        if (type != null)
        //                        {
        //                            if (type.Type != "pasillo")
        //                            {
        //                                var state = states.Where(s => s.Name == item.Name).FirstOrDefault();
        //                                ImageSource imageSource = null;
        //                                if (type.Type == "General" && state.Type != "pasillo")
        //                                {
        //                                    imageSource = (state.State == "B") ? GetImage(state.State) : GetImage(string.Empty);
        //                                }
        //                                else
        //                                {
        //                                    imageSource = (state.State == "B") ? GetImage(state.State) : GetImage(state.Type);
        //                                }

        //                                Image image = new Image
        //                                {
        //                                    Source = imageSource,
        //                                    Height = 26,
        //                                    Width = 30,
        //                                    VerticalAlignment = VerticalAlignment.Top,
        //                                    HorizontalAlignment = HorizontalAlignment.Left,
        //                                    Margin = new Thickness
        //                                    {
        //                                        Right = 0,
        //                                        Bottom = 0,
        //                                        Top = topNew,
        //                                        Left = newleft,
        //                                    },
        //                                    Name = item.Name,
        //                                    Tag = state.Type,
        //                                };

        //                                item.Type = type.Type;
        //                                if (state.State == "S" && state.Type != "Discapacitado")
        //                                {
        //                                    //image.PreviewStylusDown += new StylusDownEventHandler((s, eh) => SelectSeats(s, eh, item));
        //                                    //image.MouseDown += new MouseButtonEventHandler((s, eh) => MSelectedsetas(s, eh, item));
        //                                    image.TouchDown += new EventHandler<TouchEventArgs>((s, eh) => MSelectedsetas(s, eh, item));
        //                                }


        //                                GridSeat.Children.Add(image);
        //                                Grid.SetRow(image, 2);
        //                                Grid.SetColumn(image, 0);
        //                                Grid.SetColumnSpan(image, 2);
        //                            }
        //                            left = newleft;
        //                            count++;
        //                            item.Type = type.Type;
        //                        }


        //                    }
        //                }
        //                catch { }
        //            }

        //            top = topNew;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AdminPaypad.SaveErrorControl(ex.Message, "OrganizePositionOfSeats en frmSeat", EError.Aplication, ELevelError.Medium);
        //    }
        //}

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

            LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
            Utilities.CantSeats = SelectedTypeSeats.Count;
        }

        private ImageSource GetImage(string ckeck)
        {
            string icon = "silla-disponiblev2";
            if (ckeck == "R")
            {
                icon = "silla-selecionadav2";
            }
            else if (ckeck == "B")
            {
                icon = "silla-no-disponiblev2";
                ImgNoDisponible.Visibility = Visibility.Visible;
            }
            else if (ckeck == "Discapacitado")
            {
                icon = "silla-discapacitadosv2";
                ImgDiscapacitados.Visibility = Visibility.Visible;
            }
            else if (ckeck == "F")
            {
                icon = "silla-puffv2";
                ImgPuff.Visibility = Visibility.Visible;
            }
            else if (ckeck == "Vibrasound")
            {
                if (vibraAvailable)
                {
                    icon = "silla-vibrasound";
                }
            }
            else if (ckeck == "M")
            {
                icon = "silla-preferencialv2";
                //ImgPreferencia.Visibility = Visibility.Visible;
            }

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Concat("../Images/", icon, ".png"), UriKind.Relative);
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

                var response41 = WCFServices41.GetPrices(new SCOPLA
                {
                    FechaFuncion = dipMapCurrent.Date,
                    InicioFuncion = dipMapCurrent.HourFormat,
                    Pelicula = dipMapCurrent.MovieId,
                    Sala = dipMapCurrent.RoomId,
                    teatro = dipMapCurrent.CinemaId,
                    tercero = "1"
                });

                List<string> sinTarifa = new List<string>();
                foreach (var selectedTypeSeat in SelectedTypeSeats)
                {
                    var tarifa = new ResponseTarifa();

                    if (selectedTypeSeat.Type == "General")
                    {
                        tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type).FirstOrDefault();
                    }
                    else if (selectedTypeSeat.Type == "Vibrasound")
                    {
                        tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type).FirstOrDefault();
                    }
                    if (tarifa != null)
                    {
                        control = 1;
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
                    stringerror.AppendLine();
                }
                stringerror.Append("Por favor vuelva a intentarlo o seleccione un horario diferente.");

                if (sinTarifa.Count > 0)
                {
                    Utilities.ShowModal(stringerror.ToString());
                }

                if (control == 1)
                {
                    frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(SelectedTypeSeats, dipMapCurrent);
                    _frmConfirmationModal.ShowDialog();
                    if (_frmConfirmationModal.DialogResult.HasValue &&
                        _frmConfirmationModal.DialogResult.Value)
                    {
                        Pay.IsEnabled = false;
                        SecuenceAndReserve();
                    }
                    else
                    {
                        if (controlReinicio == 0)
                        {
                            ActivateTimer();
                        }
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
        private void SecuenceAndReserve()
        {
            this.IsEnabled = false;
            activePay = true;
            this.Opacity = 0.3;
            frmLoading = new FrmLoading("¡Reservando los puestos seleccionados!");
            try
            {
                try
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                }
                catch { }
                var responseSec41 = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = dipMapCurrent.PointOfSale,
                    teatro = dipMapCurrent.CinemaId,
                    tercero = "1"
                });
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
                        Columna = int.Parse(item.Number),
                        Fila = item.Letter,
                        Tarifa = Convert.ToInt32(item.Price)
                    });
                }

                var response41 = WCFServices41.PostReserva(new SCOGRU
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
                if (response41 != null)
                {
                    foreach (var item in response41)
                    {
                        if (item.Respuesta.Contains("exitoso"))
                        {
                            SaveDataBaseLocal();
                            if (_ErrorTransaction)
                            {
                                frmLoading.Close();
                                ShowPay();
                            }
                        }
                        else
                        {
                            frmLoading.Close();
                            Utilities.ShowModal("Lo sentimos, algo ha salido mal, por favor intenta nuevamente.");
                            ReloadWindow();
                        }
                    }
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
                    Utilities.CancelAssing(lista, dipMapCurrent);

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
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }


        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {

            if (activePay)
            {
                List<TypeSeat> lista = new List<TypeSeat>();
                foreach (var item in SelectedTypeSeats)
                {
                    lista.Add(item);
                }
                Utilities.CancelAssing(lista, dipMapCurrent);
            }
            try
            {
                btnAtras.IsEnabled = false;
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
