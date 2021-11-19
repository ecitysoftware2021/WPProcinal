using Newtonsoft.Json;
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
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCSeat.xaml
    /// </summary>
    public partial class UCSeat : UserControl
    {
        bool vibraAvailable = false;
        TimerTiempo timer;
        bool activePay = false;
        public List<ValidationSeats> listLockedSeats;
        public string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        ApiLocal api;
        public UCSeat()
        {
            InitializeComponent();
            try
            {
                listLockedSeats = new List<ValidationSeats>();
                api = new ApiLocal();
                DataService41._Combos = new List<Combos>();
                Utilities.dataTransaction.SelectedTypeSeats = new List<ChairsInformation>();
                TxtTitle.Text = Utilities.CapitalizeFirstLetter(Utilities.dataTransaction.DataFunction.MovieName);
                TxtRoom.Text = Utilities.dataTransaction.DataFunction.RoomName;
                TxtDay.Text = Utilities.dataTransaction.DataFunction.Day;
                TxtFormat.Text = string.Format("Formato: {0}", Utilities.dataTransaction.MovieFormat.ToUpper());
                TxtHour.Text = "Hora Función: " + Utilities.dataTransaction.DataFunction.HourFunction;
                TxtSubTitle.Text = "Idioma: " + Utilities.dataTransaction.DataFunction.Language;
                tbDiaActual.Text = "Fecha Actual: " + DateTime.Now.ToLongDateString();

                var time = TimeSpan.FromMinutes(double.Parse(Utilities.dataTransaction.DataFunction.Duration.Split(' ')[0]));
                TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

                LblNumSeats.Content = Utilities.dataTransaction.SelectedTypeSeats.Count.ToString();
                HideImages();
                ActivateTimer();

                Utilities.Speack("Elige tus ubicaciones y presiona comprar.");

            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "UCSeat>frmSeat", EError.Aplication, ELevelError.Medium);
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
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "UserControl_Loaded>frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

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
                        tbTimer.Text = "Tiempo de transacción: " + response;
                        tbHoraActual.Text = "Hora actual: " + DateTime.Now.ToString("hh:mm:ss");
                    });
                };
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCSeat>ActivateTimer", JsonConvert.SerializeObject(ex), 1);
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

        private void HideImages()
        {
            try
            {
                ImgDiscapacitados.Visibility = Visibility.Hidden;
                ImgDisponible.Visibility = Visibility.Visible;
                ImgNoDisponible.Visibility = Visibility.Visible;
                ImgBlack.Visibility = Visibility.Hidden;
                ImgVibroSound.Visibility = Visibility.Hidden;

                VisibilityImages();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "HideImages en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void VisibilityImages()
        {
            try
            {
                ImgDisponible.Visibility = Visibility.Visible;
                if (Utilities.dataTransaction.DataFunction.TypeZona == "General")
                {
                    ImgVibroSound.Visibility = Visibility.Hidden;
                    ImgPreferencial.Visibility = Visibility.Hidden;
                }
                else if (Utilities.dataTransaction.DataFunction.TypeZona == "Vibrasound")
                {
                    ImgVibroSound.Visibility = Visibility.Visible;
                    ImgPreferencial.Visibility = Visibility.Hidden;
                    vibraAvailable = true;
                }
                else if (Utilities.dataTransaction.DataFunction.TypeZona == "V")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                }
                else if (Utilities.dataTransaction.DataFunction.TypeZona == "Preferencial")
                {
                    ImgPreferencial.Visibility = Visibility.Visible;
                }
                else
                {
                    ImgVibroSound.Visibility = Visibility.Visible;
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
                    teatro = Utilities.dataTransaction.DataFunction.CinemaId,
                    Sala = Utilities.dataTransaction.DataFunction.RoomId,
                    FechaFuncion = Utilities.dataTransaction.DataFunction.Date,
                    Correo = "prueba@prueba.com",
                    tercero = "1",
                    Funcion = Utilities.dataTransaction.DataFunction.IDFuncion
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
                if (Utilities.dataTransaction.DataFunction.CinemaId == (int)Dictionaries.ECinemas.Monterrey)
                {
                    if (Utilities.dataTransaction.DataFunction.RoomId == 4 || Utilities.dataTransaction.DataFunction.RoomId == 5)
                    {
                        OrganizePositionOfSeats(response41);
                    }
                    else
                    {
                        OrganizePositionOfSeatsInverted(response41);
                    }
                }
                else if (Utilities.dataTransaction.DataFunction.CinemaId == (int)Dictionaries.ECinemas.Mayorca)
                {
                    if (Utilities.dataTransaction.DataFunction.RoomId == 10)
                    {
                        Utilities.PlateObligatory = true;

                        WTCModal modal = new WTCModal(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeUbicaciones);
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
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "LoadSeats en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private ImageSource ValidateSeatsAndLock(EstadoSala41 filas, DescripcionSilla item, ImageSource imageSource)
        {
            var seatsToValidate = listLockedSeats.Where(lv => lv.letter == filas.filRel);

            foreach (var valida in seatsToValidate)
            {
                //if ((valida.number - 1 == item.Columna
                //|| valida.number - 2 == item.Columna
                //|| valida.number + 1 == item.Columna
                //|| valida.number + 2 == item.Columna)
                //&& item.TipoSilla != "pasillo"
                //&& item.EstadoSilla != "B"
                //&& item.EstadoSilla != "O")

                if (
                item.TipoSilla == "pasillo"
                && item.EstadoSilla != "B"
                && item.EstadoSilla != "O")
                {
                    imageSource = GetImage("CO");
                    item.TipoSilla = "Discapacitado";
                }
            }

            //int position = letters.ToList().IndexOf(filas.filRel);

            //if (position - 1 >= 0)
            //{
            //    var downLetter = listLockedSeats.Where(lv =>
            //    lv.letter == letters[position - 1]
            //    &&
            //    (lv.number == item.Columna
            //    || lv.number == item.Columna
            //    || lv.number == item.Columna)
            //    && item.TipoSilla != "pasillo"
            //    && item.EstadoSilla != "B"
            //    && item.EstadoSilla != "O").FirstOrDefault();
            //    if (downLetter != null)
            //    {
            //        imageSource = GetImage("CO");
            //        item.TipoSilla = "Discapacitado";
            //    }
            //}

            //var upLetter = listLockedSeats.Where(lv =>
            //lv.letter == letters[position + 1]
            //&&
            //(lv.number == item.Columna
            //|| lv.number == item.Columna
            //|| lv.number == item.Columna)
            //&& item.TipoSilla != "pasillo"
            //&& item.EstadoSilla != "B"
            //&& item.EstadoSilla != "O").FirstOrDefault();
            //if (upLetter != null)
            //{
            //    imageSource = GetImage("CO");
            //    item.TipoSilla = "Discapacitado";
            //}

            return imageSource;
        }

        public void GetLockedSeats(List<EstadoSala41> est)
        {
            try
            {
                foreach (var filas in est)
                {
                    foreach (var item in filas.DescripcionSilla)
                    {
                        ImageSource imageSource = null;
                        if (item.TipoSilla == "General" || item.TipoSilla != "pasillo")
                        {
                            if (item.EstadoSilla == "B" || item.EstadoSilla == "O")
                            {
                                imageSource = GetImage(item.EstadoSilla);
                                listLockedSeats.Add(new ValidationSeats
                                {
                                    letter = filas.filRel,
                                    number = item.Columna
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCSeat>GetLockedSeats", JsonConvert.SerializeObject(ex), 1);

            }
        }


        private void OrganizePositionOfSeats(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());
                GetLockedSeats(est);
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
                        imageSource = ValidateSeatsAndLock(filas, item, imageSource);
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
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                "OrganizePositionOfSeats",
                EError.Aplication,
                ELevelError.Mild);
            }
        }

        private void OrganizePositionOfSeatsInverted(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());
                GetLockedSeats(est);
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
                        imageSource = ValidateSeatsAndLock(filas, item, imageSource);
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
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                "OrganizePositionOfSeatsInverted",
                EError.Aplication,
                ELevelError.Mild);
            }
        }

        private void OrganizePositionOfSeatsInvertedAutocine(List<EstadoSala41> est)
        {
            try
            {
                int Height = est.Count();
                int Width = int.Parse(est[0].maxCol.ToString());
                //est.Reverse();
                GetLockedSeats(est);
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
                        imageSource = ValidateSeatsAndLock(filas, item, imageSource);
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
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                "OrganizePositionOfSeatsInverted",
                EError.Aplication,
                ELevelError.Mild);
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
                var seatcurrent = Utilities.dataTransaction.SelectedTypeSeats.Where(s => s.Name == item.Name).FirstOrDefault();
                if (seatcurrent == null)
                {
                    if (Utilities.dataTransaction.SelectedTypeSeats.Count < Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MaxSillas)
                    {
                        item.Quantity = 1;
                        Utilities.dataTransaction.SelectedTypeSeats.Add(item);
                        image.Source = GetImage("R");
                    }
                }
                else
                {
                    Utilities.dataTransaction.SelectedTypeSeats.Remove(item);
                    image.Source = GetImage(image.Tag.ToString());
                }

                LblNumSeats.Content = Utilities.dataTransaction.SelectedTypeSeats.Count.ToString();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex),
                "SelectedSeatsMethod",
                EError.Aplication,
                ELevelError.Mild);
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
            else if (ckeck == "CO")
            {
                icon = "s-reservada";
            }
            else if (ckeck == "Preferencial")
            {
                icon = "s-preferencial";
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
                if (Utilities.dataTransaction.SelectedTypeSeats.Count == 0)
                {
                    Utilities.ShowModal("Debe seleccionar mínimo un puesto");
                    ActivateTimer();
                    return;
                }

                if (GetPrices())
                {

                    if (Utilities.dataTransaction.FechaSeleccionada.ToString("dd/MM/yyyy") != DateTime.Now.ToString("dd/MM/yyyy") || Utilities.PlateObligatory)
                    {

                        frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal();
                        this.Opacity = 0.3;
                        _frmConfirmationModal.ShowDialog();
                        this.Opacity = 1;
                        if (_frmConfirmationModal.DialogResult.HasValue &&
                            _frmConfirmationModal.DialogResult.Value)
                        {
                            if (Utilities.dataTransaction.DataFunction.CinemaId ==
                                (int)Dictionaries.ECinemas.Mayorca && Utilities.dataTransaction.DataFunction.RoomId == 10)
                            {
                                WTCModal modal = new WTCModal(
                                    Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeCinefans,
                                    Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeURL);
                                modal.ShowDialog();
                            }
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
                            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal();
                            this.Opacity = 0.3;
                            _frmConfirmationModal.ShowDialog();
                            this.Opacity = 1;
                            if (_frmConfirmationModal.DialogResult.HasValue &&
                                _frmConfirmationModal.DialogResult.Value)
                            {

                                if (Utilities.dataTransaction.DataFunction.CinemaId == (int)Dictionaries.ECinemas.Mayorca
                                    && Utilities.dataTransaction.DataFunction.RoomId == 10)
                                {
                                    WTCModal modal = new WTCModal(
                                        Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeCinefans,
                                    Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeURL);
                                    modal.ShowDialog();
                                }
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
                        Utilities.SendMailErrores($"No fue posible obtener las tarífas en Score");
                    });
                    ReloadWindow();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "SendData en frmSeat", EError.Aplication, ELevelError.Medium);

                Utilities.ShowModal("Lo sentimos, no fué posible consultar las tarifas, por favor intente de nuevo.");
                ReloadWindow();
            }
        }

        private void GoToPay()
        {
            try
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
                                Utilities.ShowModal(string.Concat("No se pudieron reservar los puestos: ", item.Respuesta));
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
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCSeat>GoToPay", JsonConvert.SerializeObject(ex), 1);
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
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString(),
                    tercero = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.tercero
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
                LogService.SaveRequestResponse("UCSeat>GetCombo", JsonConvert.SerializeObject(ex), 1);
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
                var response41 = WCFServices41.GetPricesKio(new SCOPLA
                {
                    FechaFuncion = Utilities.dataTransaction.DataFunction.Date,
                    InicioFuncion = Utilities.dataTransaction.DataFunction.HourFormat,
                    Pelicula = Utilities.dataTransaction.DataFunction.MovieId,
                    Sala = Utilities.dataTransaction.DataFunction.RoomId,
                    teatro = Utilities.dataTransaction.DataFunction.CinemaId,
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
                foreach (var selectedTypeSeat in Utilities.dataTransaction.SelectedTypeSeats)
                {
                    var tarifa = new ResponseTarifa();
                    if (Utilities.dataTransaction.dataUser.Tarjeta != null)
                    {
                        if (Utilities.dataTransaction.DataFunction.Validaciones.ToLower().Equals("no"))
                        {
                            tarifa = response41.Where(t => t.silla.ToLower() == "general" && t.ClienteFrecuente.ToLower() == "habilitado").FirstOrDefault();
                        }
                        else
                        {
                            tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type && t.ClienteFrecuente.ToLower() == "habilitado").FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (Utilities.dataTransaction.DataFunction.Validaciones.ToLower().Equals("no"))
                        {
                            tarifa = response41.Where(t => t.silla.ToLower() == "general").FirstOrDefault();
                        }
                        else
                        {
                            tarifa = response41.Where(t => t.silla == selectedTypeSeat.Type).FirstOrDefault();
                        }
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
                LogService.SaveRequestResponse("UCSeat>GetPrices", JsonConvert.SerializeObject(ex), 1);
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
                Switcher.Navigate(new UCSeat());
            });
        }

        /// <summary>
        /// Reserva los puestos
        /// </summary>
        /// 
        private void SecuenceAndReserveToConfectionery()
        {
            SetCallBacksNull();

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
                                Utilities.ShowModal(string.Concat("No se pudieron reservar los puestos: ", item.Respuesta));
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
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "SecueceAndReserve en frmSeat", EError.Aplication, ELevelError.Medium);

                Utilities.ShowModal("Lo sentimos, no se pudieron reservar los puestos, por favor intente de nuevo.");
                ReloadWindow();
            }
        }

        private List<Ubicacione> OrganizeSeatsTuReserve()
        {
            List<Ubicacione> ubicacione = new List<Ubicacione>();
            foreach (var item in Utilities.dataTransaction.SelectedTypeSeats)
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
            try
            {
                var dataClient = GetDataClient();
                FrmLoading frmLoading = new FrmLoading("¡Reservando los puestos seleccionados!");
                frmLoading.Show();

                var response41 = WCFServices41.PostPreventa(new SCOGRU
                {
                    Apellido = dataClient.Apellido,
                    Descripcion = Utilities.dataTransaction.DataFunction.MovieName,
                    FechaFuncion = Utilities.dataTransaction.DataFunction.Date,
                    HoraFuncion = Utilities.dataTransaction.DataFunction.Hour,
                    InicioFuncion = Utilities.dataTransaction.DataFunction.HourFormat,
                    Nombre = dataClient.Nombre,
                    Pelicula = Utilities.dataTransaction.DataFunction.MovieId,
                    PuntoVenta = Utilities.dataTransaction.DataFunction.PointOfSale,
                    Sala = Utilities.dataTransaction.DataFunction.RoomId,
                    Secuencia = Utilities.dataTransaction.DataFunction.Secuence,
                    teatro = Utilities.dataTransaction.DataFunction.CinemaId,
                    Telefono = !string.IsNullOrEmpty(dataClient.Telefono) ? long.Parse(dataClient.Telefono) : 0,
                    tercero = 1,
                    Ubicaciones = ubicacione
                });
                frmLoading.Close();
                return response41;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool GetSecuence()
        {
            FrmLoading frmLoading = new FrmLoading("¡Generando secuencia de compra!");
            try
            {
                frmLoading.Show();
                var responseSec41 = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = Utilities.dataTransaction.DataFunction.PointOfSale,
                    teatro = Utilities.dataTransaction.DataFunction.CinemaId,
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
                    Utilities.dataTransaction.DataFunction.Secuence = int.Parse(item.Secuencia.ToString());
                    Utilities.dataTransaction.Secuencia = item.Secuencia.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCSeat>GetSecuence", JsonConvert.SerializeObject(ex), 1);
                frmLoading.Close();
                Utilities.ShowModal("Lo sentimos, no se pudo obtener la secuencia de compra, por favor intente de nuevo.");
                return false;
            }
        }

        SCOLOGResponse GetDataClient()
        {
            if (Utilities.dataTransaction.dataUser.Tarjeta != null)
            {
                return new SCOLOGResponse
                {
                    Apellido = Utilities.dataTransaction.dataUser.Apellido,
                    Tarjeta = Utilities.dataTransaction.dataUser.Tarjeta,
                    Login = Utilities.dataTransaction.dataUser.Login,
                    Direccion = Utilities.dataTransaction.dataUser.Direccion,
                    Documento = Utilities.dataTransaction.dataUser.Documento,
                    Nombre = Utilities.dataTransaction.dataUser.Nombre,
                    Telefono = Utilities.dataTransaction.dataUser.Telefono
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
            Switcher.Navigate(new UCSelectProducts());
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
                Utilities.ValidateUserBalance();
                var response = await Utilities.CreateTransaction("Cine ");
                frmLoading.Close();

                if (!response)
                {
                    List<ChairsInformation> lista = new List<ChairsInformation>();
                    foreach (var item in Utilities.dataTransaction.SelectedTypeSeats)
                    {
                        lista.Add(item);
                    }
                    this.IsEnabled = false;
                    frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
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
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                if (activePay)
                {
                    List<ChairsInformation> lista = new List<ChairsInformation>();
                    foreach (var item in Utilities.dataTransaction.SelectedTypeSeats)
                    {
                        lista.Add(item);
                    }
                    this.IsEnabled = false;
                    FrmLoading frmLoading = new FrmLoading("Eliminando preventas, espere por favor...");
                    frmLoading.Show();
                    Utilities.CancelAssing(Utilities.dataTransaction.SelectedTypeSeats, Utilities.dataTransaction.DataFunction);
                    frmLoading.Close();
                    this.IsEnabled = true;
                }
                SetCallBacksNull();
                Switcher.Navigate(new UCSchedule(DataService41.Movie));
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCSeat>BtnAtras_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void Pay_TouchDown(object sender, TouchEventArgs e)
        {
            SendData();
        }


    }
}
