using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCSchedule.xaml
    /// </summary>
    public partial class UCSchedule : UserControl
    {
        #region References
        string MovieName = string.Empty;
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<Schedule> lstGrid = new ObservableCollection<Schedule>();
        int currentPageIndex = 0;
        int itemPerPage = 6;
        int totalPage = 0;
        int FontS = 0;
        int cinemaId = Convert.ToInt16(Utilities.GetConfiguration("CodCinema"));
        Pelicula Movie = new Pelicula();

        bool hourPresed = false;

        /*--LIST DATE--*/
        private DateTime FechaSelect = new DateTime();
        private List<DateName> dateName = new List<DateName>();
        List<Border> borders = new List<Border>();
        private int currentPageIndex2 = 0;
        private int itemPerPage2 = 3;
        private int totalPage2 = 0;
        private CollectionViewSource view2 = new CollectionViewSource();
        private ObservableCollection<DateName> lstPager2 = new ObservableCollection<DateName>();
        List<ListViewItem> grid = new List<ListViewItem>();
        /*--END DATE--*/

        TimerTiempo timer;
        #endregion
        public UCSchedule(Pelicula Movie)
        {
            InitializeComponent();
            this.Movie = Movie;
            Utilities.Movie = Movie;

            foreach (var peli in Utilities.Peliculas.Pelicula.Where(pe => pe.Data.TituloOriginal == Movie.Data.TituloOriginal))
            {
                ListFechas(peli.DiasDisponiblesTodosCinemas);
            }
            InitView2();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imgBackground.ImageSource = Utilities.ImageSelected;
            MovieName = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal);
            TxtTitle.Text = MovieName;
            tbDiaActual.Text = "Fecha Actual: " + DateTime.Now.ToLongDateString();
            var time = TimeSpan.FromMinutes(double.Parse(Movie.Data.Duracion));
            TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);
            //TxtGender.Text = string.Concat("Género: ", Movie.Data.Genero);
            DateTime fechaActual = Utilities.FechaSeleccionada;

            //TxtDay.Text = string.Format("{0} {1}, {2}", fechaActual.ToString("dddd"), fechaActual.Day, fechaActual.ToString("MMM"));

            if (Movie.Data.TituloOriginal.Length <= 15)
            {
                FontS = 55;
            }
            else
            {
                FontS = 35;
            }
            GenerateFunctions();
            if (Utilities.Movie.Data.Censura.Contains("15") || Utilities.Movie.Data.Censura.Contains("18"))
            {
                frmModal modal = new frmModal(string.Format(Utilities.GetConfiguration("MensajeCensura"), Utilities.Movie.Data.Censura));
                modal.ShowDialog();
            }
            ActivateTimer();
        }
        void ActivateTimer()
        {
            tbTimer.Text = Utilities.GetConfiguration("TimerHorario");
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

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }


        #region Methods
        //Método para generar funciones para las películas
        public void GenerateFunctions()
        {
            try
            {
                int i = 0;
                lstGrid.Clear();
                //Se arma una lista con los datos de sólo el cinema donde se encuentra la máquina
                int Hour = DateTime.Now.Hour;

                foreach (var peli in Utilities.Peliculas.Pelicula.Where(pe => pe.Data.TituloOriginal == Movie.Data.TituloOriginal))
                {

                    //ListFechas(peli.DiasDisponiblesTodosCinemas);
                    foreach (var Cinema in peli.Cinemas.Where(cine => cine.Cinema.Id == Utilities.CinemaId))
                    {
                        //Se recorre cada sala del Cinema
                        foreach (var room in Cinema.Cinema.Salas.Sala)
                        {
                            //Se arma una lista con las fechas de cada función y luego se recorre
                            var functions = room.Fecha.ToList();
                            foreach (var function in functions)
                            {
                                /*Se obtiene la fecha de la función  y luego se valida para no mostrar
                                funciones con fechas menores a la actual*/
                                var datetime = GetDateCorrectly(function.Univ);
                                var fechaSeleccionada = GetDateCorrectly(Utilities.FechaSeleccionada.ToString("yyyyMMdd"));
                                if (datetime == fechaSeleccionada)
                                {

                                    var schedules = function.Hora.OrderBy(h => h.Militar).ToList();
                                    List<HoraTMP> horatmps = new List<HoraTMP>();
                                    int count = 0;
                                    int countAvailable = 0;
                                    bool available = true;

                                    foreach (var item in schedules)
                                    {
                                        string tipoZona = "General";
                                        if (item.TipoZona.Count > 1)
                                        {
                                            tipoZona = item.TipoZona[1].TipoSilla.NombreTipoSilla;
                                        }
                                        else
                                        {
                                            tipoZona = item.TipoZona[0].TipoSilla.NombreTipoSilla;
                                        }

                                        if (fechaSeleccionada != DateTime.Today)
                                        {
                                            horatmps.Add(new HoraTMP
                                            {
                                                Horario = item.Horario,
                                                IdFuncion = item.IdFuncion,
                                                Militar = int.Parse(item.Militar),
                                                //Reservas = item.Reservas,
                                                TipoZona = tipoZona,
                                                Validaciones = item.Validaciones
                                            });
                                        }
                                        else if (int.Parse(item.Militar) >= int.Parse(DateTime.Now.AddMinutes(-40).ToString("HHmm")))
                                        {
                                            horatmps.Add(new HoraTMP
                                            {
                                                Horario = item.Horario,
                                                IdFuncion = item.IdFuncion,
                                                Militar = int.Parse(item.Militar),
                                                //Reservas = item.Reservas,
                                                TipoZona = tipoZona,
                                                Validaciones = item.Validaciones
                                            });
                                        }
                                        else
                                        {
                                            countAvailable++;
                                        }
                                        if (countAvailable == schedules.Count())
                                        {
                                            available = false;
                                        }
                                        count++;
                                    }


                                    if (available)
                                    {
                                        string ruta = GenerateTags(room.TipoSala);

                                        string[] days = function.Dia.Split(' ');
                                        lstGrid.Add(new Schedule
                                        {
                                            Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                            FontS = FontS,
                                            Language = string.Concat(peli.Data.Idioma),

                                            Gener = Movie.Data.Genero,
                                            Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                            Category = Movie.Data.Censura,
                                            Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                            Room = string.Concat("Sala ", room.NumeroSala),
                                            Hours = horatmps,
                                            RutaTipoSala = ruta,
                                            TipoSala = room.TipoSala,
                                            RoomId = int.Parse(room.NumeroSala),
                                            UnivDate = function.Univ,
                                            MovieId = int.Parse(peli.Id),
                                            Formato = peli.Data.Formato,
                                            Censura = peli.Data.Censura
                                        });
                                        foreach (var newHoras in horatmps)
                                        {
                                            newHoras.DatosPelicula = new Schedule
                                            {
                                                Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                                FontS = FontS,
                                                Language = string.Concat(peli.Data.Idioma),
                                                Gener = Movie.Data.Genero,
                                                Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                                Category = Movie.Data.Censura,
                                                Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                                Room = string.Concat("Sala ", room.NumeroSala),
                                                Hour = newHoras.Horario,
                                                RutaTipoSala = ruta,
                                                TipoSala = room.TipoSala,
                                                RoomId = int.Parse(room.NumeroSala),
                                                UnivDate = function.Univ,
                                                MovieId = int.Parse(peli.Id),
                                                Formato = peli.Data.Formato,
                                                Censura = peli.Data.Censura,
                                                IdFuncion = int.Parse(newHoras.IdFuncion.Substring(newHoras.IdFuncion.Length - 2))
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (lstGrid.Count > 0)
                {
                    CreatePages(i);
                }
                else
                {
                    Utilities.ShowModal("No hay funciones disponibles en esta fecha: " + Utilities.FechaSeleccionada.ToLongDateString());
                    return;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "GenerateFunctions en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private string GenerateTags(string name)
        {
            string Path = string.Empty;
            switch (name)
            {
                case "4DX":
                    return "/Images/Tags/cartel-4dx.png";
                case "2D":
                case "GENERAL":
                    return "/Images/Tags/cartel-2d.png";
                case "3D":
                    return "/Images/Tags/cartel-3d.png";
                case "VIBRASOUND":
                    return "/Images/Tags/cartel-vibra.png";
                case "SUPERNOVA":
                    return "/Images/Tags/cartel-supernova.png";
                case "STAR KIDS":
                    return "/Images/Tags/cartel-starkids.png";
                case "CINE ARTE":
                    return "/Images/Tags/cartel-cinearte.png";
                case "BLACK STAR":
                    return "/Images/Tags/cartel-blackstar.png";
                default:
                    return string.Empty;
            }
        }

        private void CreatePages(int i)
        {
            lvSchedule.DataContext = lstGrid;
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            int index = lstGrid.IndexOf((Schedule)e.Item);

            if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private DateTime GetDateCorrectly(string univDate)
        {
            char[] Date = univDate.ToCharArray();
            string year = univDate.Substring(0, 4);
            string month = string.Concat(Date[4], Date[5]);
            string day = string.Concat(Date[6], Date[7]);
            var dateTime = Convert.ToDateTime(string.Format("{0}/{1}/{2}", year, month, day));
            return dateTime;
        }

        private string InvertDate(string univDate)
        {
            char[] Date = univDate.ToCharArray();
            string year = univDate.Substring(0, 4);
            string month = string.Concat(Date[4], Date[5]);
            string day = string.Concat(Date[6], Date[7]);
            string DateInvert = string.Concat(day, month, year);

            return DateInvert;
        }

        private DipMap SetProperties(Schedule schedule)
        {
            int Hour = Convert.ToInt32(schedule.MilitarHour.ToString().Substring(0, 2));
            string DateFormat = InvertDate(schedule.UnivDate);
            DipMap map = new DipMap
            {
                MovieName = schedule.Title,
                Language = schedule.Language,
                Gener = schedule.Gener,
                Duration = schedule.Duration,
                HourFunction = schedule.Hour,
                RoomName = schedule.Room,
                Category = schedule.Category,
                Day = schedule.Date,
                Date = schedule.UnivDate,
                DateFormat = DateFormat,
                CinemaId = cinemaId,
                RoomId = schedule.RoomId,
                Hour = Hour,
                HourFormat = schedule.MilitarHour,
                MovieId = schedule.MovieId,
                Letter = "A",
                IsCard = "S",
                Group = 1,
                Login = "brandon-377@hotmail.com",
                PointOfSale = 77,
                TypeZona = schedule.TypeZona,
                IDFuncion = schedule.IdFuncion
            };
            //TODO: PointOfSale = 87
            return map;
        }
        #endregion

        #region ButtonsEvents


        #endregion

        #region Dias Disponibles

        #region Metodos
        int controlFechaActual = 0;
        private void ListFechas(DiasDisponiblesTodosCinemas dias)
        {
            try
            {
                foreach (var item in dias.Dia)
                {
                    string fechaCompuesta = ConvertDate(item);
                    DateTime dt2 = DateTime.Parse(fechaCompuesta);
                    var datetime = GetDateCorrectly(item.Univ);

                    //if (datetime != DateTime.Today)
                    if (datetime >= DateTime.Today)
                    {
                        string NombreDiaAdd = dt2.ToString("dddd", CultureInfo.CreateSpecificCulture("es-ES"));
                        var exist = dateName.Where(dt => dt.Mes == dt2.ToString("MMM").ToUpper() && dt.DiaNumero == dt2.ToString("dd")).Count();
                        if (exist == 0)
                        {
                            dateName.Add(new DateName
                            {
                                FechaOrigin = dt2,
                                Mes = dt2.ToString("MMM").ToUpper(),
                                NombreDia = NombreDiaAdd.ToUpper(),
                                DiaNumero = dt2.ToString("dd")
                            });

                            dateName = dateName.OrderBy(i => i.FechaOrigin).ToList();
                        }
                    }

                }
                Utilities.FechaSeleccionada = dateName[0].FechaOrigin;


            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ListFechas en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private static string ConvertDate(Dia item)
        {
            try
            {
                string anio = item.Univ.Substring(0, 4);
                string mes = item.Univ.Substring(4, 2);
                string dia = item.Univ.Substring(6, 2);
                string fechaCompuesta = string.Concat(anio, "/", mes, "/", dia);
                return fechaCompuesta;
            }
            catch (Exception ex)
            {
                return DateTime.Today.ToString();
            }
        }

        private void InitView2()
        {
            try
            {
                if (dateName.Count() > 0)
                {
                    foreach (var item in dateName)
                    {
                        lstPager2.Add(new DateName
                        {
                            Mes = item.Mes,
                            NombreDia = item.NombreDia,
                            FechaOrigin = item.FechaOrigin,
                            TextColor = "Black",
                            DiaNumero = item.DiaNumero
                        });
                    }

                    CreatePages2(dateName.Count());
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "InitView2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void CreatePages2(int i)
        {
            try
            {
                int itemcount = i;
                totalPage2 = itemcount / itemPerPage2;
                if (itemcount % itemPerPage2 != 0)
                {
                    totalPage2 += 1;
                }
                lstPager2[0].TextColor = "#FF1385FF";
                view2.Source = lstPager2;
                view2.Filter += new FilterEventHandler(View_Filter2);
                lv_DateName.DataContext = view2;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "CreatePages2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Eventos
        private void View_Filter2(object sender, FilterEventArgs e)
        {
            try
            {
                int index = lstPager2.IndexOf((DateName)e.Item);

                if (index >= itemPerPage2 * currentPageIndex2 && index < itemPerPage2 * (currentPageIndex2 + 1))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "View_Filter2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }



        private void Grid_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                var childs = (sender as Grid).Children;
                foreach (var item in childs)
                {
                    if (item is Border)
                    {
                        ClearHoursList();
                        var border = item as Border;
                        Color color2 = (Color)ColorConverter.ConvertFromString("#FFF89126");
                        border.Background = new SolidColorBrush(color2);
                        borders.Add(border);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Grid_PreviewStylus en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void ClearHoursList()
        {
            try
            {
                foreach (var item in borders)
                {
                    item.Background = Brushes.White;
                }
            }
            catch { }
        }

        #endregion

        #endregion

        private void ListViewItem_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var service = (DateName)(sender as ListViewItem).Content;

                FechaSelect = service.FechaOrigin;//poner la fecha en el formato xml
                Utilities.FechaSeleccionada = FechaSelect;
                GenerateFunctions();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "lv_Datename_previewstylusDown en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void GridFechas_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var childs = (sender as Grid).Children;
                foreach (var item in childs)
                {
                    if (item is Border)
                    {
                        ClearHoursList();
                        var border = item as Border;
                        Color color2 = (Color)ColorConverter.ConvertFromString("#FFF89126");
                        border.Background = new SolidColorBrush(color2);
                        borders.Add(border);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Grid_PreviewStylus en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void BtnPrev2_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                ActivateTimer();
                if (currentPageIndex2 > 0)
                {
                    currentPageIndex2--;
                    view2.View.Refresh();
                }

                if (currentPageIndex2 == 0)
                {
                    btnPrev2.Visibility = Visibility.Hidden;
                }

                btnNext2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "btnPrev2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void BtnNext2_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                ActivateTimer();
                if (currentPageIndex2 < totalPage2 - 1)
                {
                    currentPageIndex2++;
                    view2.View.Refresh();
                }
                if (currentPageIndex2 == totalPage2 - 1)
                {
                    btnNext2.Visibility = Visibility.Hidden;
                }

                btnPrev2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "btnNext en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }

            Switcher.Navigate(new UCMovies());
        }

        private void ListViewItem_TouchDown_1(object sender, TouchEventArgs e)
        {
            try
            {
                var selectedSchedule = (HoraTMP)(sender as ListViewItem).Content;
                Schedule schedule = new Schedule
                {
                    Category = selectedSchedule.DatosPelicula.Category,
                    Date = selectedSchedule.DatosPelicula.Date,
                    Duration = selectedSchedule.DatosPelicula.Duration,
                    FontS = 0,
                    Formato = selectedSchedule.DatosPelicula.Formato,
                    Gener = selectedSchedule.DatosPelicula.Gener,
                    Hour = selectedSchedule.DatosPelicula.Hour,
                    Id = selectedSchedule.DatosPelicula.Id,
                    Language = selectedSchedule.DatosPelicula.Language,
                    MilitarHour = selectedSchedule.Militar,
                    MovieId = selectedSchedule.DatosPelicula.MovieId,
                    Room = selectedSchedule.DatosPelicula.Room,
                    RoomId = selectedSchedule.DatosPelicula.RoomId,
                    Title = selectedSchedule.DatosPelicula.Title,
                    TypeZona = selectedSchedule.TipoZona,
                    UnivDate = selectedSchedule.DatosPelicula.UnivDate,
                    Censura = selectedSchedule.DatosPelicula.Censura,
                    IdFuncion = selectedSchedule.DatosPelicula.IdFuncion
                };
                var formato = selectedSchedule.DatosPelicula.Formato.Split(' ');
                if (formato.Length > 1)
                {
                    Utilities.MovieFormat = formato[1];
                }
                else
                {
                    Utilities.MovieFormat = formato[0];
                }
                Utilities.TipoSala = selectedSchedule.DatosPelicula.TipoSala;
                DipMap dipmap = SetProperties(schedule);

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                Switcher.Navigate(new UCSeat(dipmap));
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "Seleccionando el horario",
                    EError.Aplication,
                    ELevelError.Medium);
                }
                catch { }
                Dispatcher.BeginInvoke((Action)delegate
                {
                    frmModal modal = new frmModal("Lo sentimos, no se pudo seleccionar el horario, intente de nuevo por favor.");
                    modal.ShowDialog();
                });
            }
        }


    }
}
