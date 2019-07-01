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
using WPProcinal.ADO;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmSeat.xaml
    /// </summary>
    public partial class frmSeat : Window
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

        public frmSeat(DipMap dipMap)
        {
            InitializeComponent();

            frmLoading = new FrmLoading("¡Cargando la sala!");
            dipMapCurrent = dipMap;
            TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
            TxtDay.Text = dipMap.Day;
            TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
            TxtHour.Text = dipMap.HourFunction;
            TxtSubTitle.Text = dipMap.Language;
            LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
            HideImages();
            ActivateTimer();
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
                        frmCinema main = new frmCinema();
                        main.Show();
                        this.Close();
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

        private void HideImages()
        {
            try
            {
                ImgDiscapacitados.Visibility = Visibility.Hidden;
                ImgDisponible.Visibility = Visibility.Visible;
                ImgNoDisponible.Visibility = Visibility.Visible;
                ImgPreferencia.Visibility = Visibility.Hidden;
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
                if (dipMapCurrent.TypeZona == "G")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Hidden;
                }
                else if (dipMapCurrent.TypeZona == "P")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Visible;
                    vibraAvailable = true;
                }
                else if (dipMapCurrent.TypeZona == "V")
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgPreferencia.Visibility = Visibility.Visible;
                }
                else
                {
                    ImgDisponible.Visibility = Visibility.Visible;
                    ImgVibroSound.Visibility = Visibility.Visible;
                    ImgPreferencia.Visibility = Visibility.Visible;
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
                var response = WCFServices.GetDipMap(dipMapCurrent);
                if (!response.IsSuccess)
                {
                    frmLoading.Close();

                    Utilities.SaveLogError(new LogError
                    {
                        Message = response.Message,
                        Method = "WCFServices.GetDipMap",

                    });

                    frmModal _frmModal = new frmModal("El servicio no esta disponible.");
                    _frmModal.ShowDialog();

                    Utilities.GoToInicial(this);
                }
                else
                {
                    var responseV2 = WCFServices.GetStateRoom(dipMapCurrent);
                    if (!response.IsSuccess)
                    {
                        frmLoading.Close();
                        Utilities.SaveLogError(new LogError
                        {
                            Message = response.Message,
                            Method = "WCFServices.GetStateRoom",

                        });
                        frmModal _frmModal = new frmModal("El servicio no esta disponible.");
                        _frmModal.ShowDialog();

                        Utilities.GoToInicial(this);
                    }
                    else
                    {
                        var mapaSala = WCFServices.DeserealizeXML<MapaSala>(response.Result.ToString());
                        var estadoSala = WCFServices.DeserealizeXML<EstadoSala>(responseV2.Result.ToString());

                        List<SeatTmp> states = new List<SeatTmp>();
                        foreach (var fila in estadoSala.FILA)
                        {
                            var split = fila.Num_Col.Split('-');
                            int i = 0;
                            foreach (var item in split)
                            {
                                if (!string.IsNullOrEmpty(item) && item != " ")
                                {
                                    states.Add(new SeatTmp
                                    {
                                        Name = item,
                                        State = fila.Des_Fila[i + 1].ToString(),
                                        Type = fila.Des_Fila[i].ToString(),
                                    });
                                    i += 2;
                                }
                            }
                        }

                        var FTSplit = mapaSala.FT.Split(',');
                        int positionX = FTSplit.Distinct().Count() - 1;
                        var FTDistinc = FTSplit.Distinct().ToList();
                        var CTSplit = mapaSala.CT.Split(',');
                        int positionY = CTSplit.Distinct().Count() - 1;
                        var CTDistinc = CTSplit.Distinct();
                        var FRSplit = mapaSala.FR.Split(',');
                        var FRDistinc = FRSplit.Distinct();
                        var CRSplit = mapaSala.CR.Split(',');
                        var CRDistinc = CRSplit.Distinct();
                        var TSSplit = mapaSala.TS.Split(',');
                        var TSDistinc = TSSplit.Distinct();
                        int count = 0;

                        List<TypeSeat> typeSeats = new List<TypeSeat>();
                        List<SeatTmp> seatTmps = new List<SeatTmp>();
                        foreach (var item in FTSplit)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                typeSeats.Add(new TypeSeat
                                {
                                    Letter = item,
                                    Name = string.Format("{0}{1}", item, CTSplit[count]),
                                    Number = CTSplit[count]
                                });

                                seatTmps.Add(new SeatTmp
                                {
                                    Name = string.Format("{0}{1}", item, CRSplit[count]),
                                    Type = TSSplit[count],
                                });
                            }

                            count++;
                        }

                        OrganizePositionOfSeats(states, positionX, FTDistinc, positionY, typeSeats, seatTmps);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadSeats en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void CloseApplication(Window form)
        {
            form.Close();
            Utilities.GoToInicial(this);
        }


        private void OrganizePositionOfSeats(List<SeatTmp> states,
            int positionX, List<string> FTDistinc,
            int positionY, List<TypeSeat>
            typeSeats, List<SeatTmp> seatTmps)
        {
            try
            {
                int left = 0;
                int top = 0;
                int sum = int.Parse((1020 / positionY).ToString());
                int sum2 = int.Parse((700 / positionX).ToString());

                for (int i = 0; i < FTDistinc.Count() - 1; i++)
                {
                    int topNew = i != 0 ? top + sum2 : top;
                    left = 35;
                    int count = 0;
                    foreach (var item in typeSeats)
                    {
                        try
                        {
                            if (item.Letter.Equals(FTDistinc[i]))
                            {
                                int newleft = count != 0 ? left + sum : left;
                                var type = seatTmps.Where(t => t.Name == item.Name).FirstOrDefault();
                                if (type.Type != "P")
                                {
                                    var state = states.Where(s => s.Name == item.Name).FirstOrDefault();
                                    ImageSource imageSource = null;
                                    if (dipMapCurrent.TypeZona == "G" && state.Type != "D")
                                    {
                                        imageSource = (state.State == "B") ? GetImage(state.State) : GetImage(string.Empty);
                                    }
                                    else
                                    {
                                        imageSource = (state.State == "B") ? GetImage(state.State) : GetImage(state.Type);
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
                                            Top = topNew,
                                            Left = newleft,
                                        },
                                        Name = item.Name,
                                        Tag = state.Type,
                                    };

                                    item.Type = type.Type;
                                    if (state.State == "S" && state.Type != "D")
                                    {
                                        image.PreviewStylusDown += new StylusDownEventHandler((s, eh) =>
                                                                                                        SelectSeats(s, eh, item));
                                        //image.MouseDown += new MouseButtonEventHandler((s, eh) => MSelectedsetas(s, eh, item));
                                    }


                                    GridSeat.Children.Add(image);
                                    Grid.SetRow(image, 2);
                                    Grid.SetColumn(image, 0);
                                    Grid.SetColumnSpan(image, 2);
                                }

                                left = newleft;
                                count++;
                                item.Type = type.Type;
                            }
                        }
                        catch { }
                    }

                    top = topNew;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "OrganizePositionOfSeats en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void MSelectedsetas(object sender, MouseButtonEventArgs eh, TypeSeat item)
        {
            SelectedSeatsMethod(sender, item);
        }

        private void SelectSeats(object sender, StylusDownEventArgs eh, TypeSeat item)
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
            else if (ckeck == "D")
            {
                icon = "silla-discapacitadosv2";
                ImgDiscapacitados.Visibility = Visibility.Visible;
            }
            else if (ckeck == "F")
            {
                icon = "silla-puffv2";
                ImgPuff.Visibility = Visibility.Visible;
            }
            else if (ckeck == "A")
            {
                if (vibraAvailable)
                {
                    icon = "silla-vibrasound";
                }
            }
            else if (ckeck == "M")
            {
                icon = "silla-preferencialv2";
                ImgPreferencia.Visibility = Visibility.Visible;
            }

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Concat("../Images/", icon, ".png"), UriKind.Relative);
            logo.EndInit();
            return logo;
        }

        private void Pay_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {

            SendData();
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

                    this.Opacity = 0.3;
                    Utilities.ShowModal("Debe seleccionar almenos un puesto");
                    this.Opacity = 1;
                    ActivateTimer();
                    return;
                }

                var response = WCFServices.GetPrices(dipMapCurrent);
                if (!response.IsSuccess)
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SendData > GetPrices en frmSeat", response.Message);
                    Utilities.ShowModal("No fúe posible obtener las tarífas, por favor intente de nuevo");
                    ReloadWindow();
                    return;
                }

                var plantillatmp = WCFServices.DeserealizeXML<Plantilla>(response.Result.ToString());
                if (!string.IsNullOrEmpty(plantillatmp.Tarifas.Codigo.Error_en_Sistema))
                {
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SendData > plantillatmp en frmSeat", plantillatmp.Tarifas.Codigo.Error_en_Sistema);
                    Utilities.ShowModal(plantillatmp.Tarifas.Codigo.Error_en_Sistema.ToString());
                    ReloadWindow();
                }

                var plantilla = WCFServices.DeserealizeXML<Plantilla2>(response.Result.ToString());
                List<string> sinTarifa = new List<string>();
                foreach (var selectedTypeSeat in SelectedTypeSeats)
                {
                    var tarifa = new Tarifas3();
                    if (dipMapCurrent.TypeZona != "G")
                    {
                        if (selectedTypeSeat.Type == "A")
                        {
                            tarifa = plantilla.Tarifas.Where(t => t.Tipo == "P").FirstOrDefault();
                        }
                        else if (selectedTypeSeat.Type == "M")
                        {
                            tarifa = plantilla.Tarifas.Where(t => t.Tipo == "V").FirstOrDefault();
                        }
                        else
                        {
                            tarifa = plantilla.Tarifas.Where(t => t.Tipo == "G").FirstOrDefault();
                        }
                    }
                    else
                    {
                        tarifa = plantilla.Tarifas.Where(t => t.Tipo == dipMapCurrent.TypeZona).FirstOrDefault();
                    }
                    if (tarifa != null)
                    {
                        control = 1;
                        selectedTypeSeat.Price = decimal.Parse(tarifa.Valor.Split(',')[0]);
                        selectedTypeSeat.CodTarifa = tarifa.Codigo;
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
                    this.Opacity = 0.3;
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


                    this.Opacity = 1;
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
                frmSeat _frmSeat = new frmSeat(dipMapCurrent);
                _frmSeat.Show();
                this.Close();
            });
        }

        /// <summary>
        /// Reserva los puestos
        /// </summary>
        private void SecuenceAndReserve()
        {
            frmLoading = new FrmLoading("¡Reservando los puestos seleccionados!");
            try
            {
                try
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                }
                catch { }

                frmLoading.Show();
                var responseSec = WCFServices.GetSecuence(dipMapCurrent);

                LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve > GetSecuence", responseSec.Result.ToString());

                if (!responseSec.IsSuccess)
                {
                    frmLoading.Close();
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve >  responseSec.Message en frmSeat", responseSec.Message);

                    Utilities.ShowModal("Lo sentimos, algo ha salido mal, por favor intenta nuevamente.");
                    ReloadWindow();
                }

                var secuence = WCFServices.DeserealizeXML<SecuenciaVenta>(responseSec.Result.ToString());
                if (!string.IsNullOrEmpty(secuence.Error))
                {
                    frmLoading.Close();
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve >  secuence.Error en frmSeat", secuence.Error);

                    Utilities.ShowModal("Lo sentimos, algo salió mal, intenta nuevamente por favor.");
                    ReloadWindow();
                }

                dipMapCurrent.Secuence = int.Parse(secuence.Secuencia);
                Utilities.Secuencia = secuence.Secuencia;

                foreach (var item in SelectedTypeSeats)
                {
                    var response = WCFServices.PostReserva(dipMapCurrent, item);
                    LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve > PostReserva > response.Result en frmSeat", response.Result.ToString());

                    if (!response.IsSuccess)
                    {
                        frmLoading.Close();
                        LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve > response.Message en frmSeat", response.Message);
                        item.IsReserved = false;

                        Utilities.ShowModal("Lo sentimos, algo ha salido mal, por favor intenta nuevamente.");
                    }

                    var reserve = WCFServices.DeserealizeXML<Reserva>(response.Result.ToString());
                    if (!string.IsNullOrEmpty(reserve.Error_en_proceso))
                    {
                        frmLoading.Close();
                        LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: SecuenceAndReserve > reserve.Error_en_proceso en frmSeat", reserve.Error_en_proceso);
                        item.IsReserved = false;
                        Utilities.ShowModal(reserve.Error_en_proceso);
                    }
                    else
                    {
                        frmLoading.Close();
                        item.NumSecuencia = reserve.Secuencia_reserva;
                        item.IsReserved = true;
                    }
                }
                frmLoading.Close();
                var tyseat = SelectedTypeSeats.Where(s => s.IsReserved == false).ToList();
                if (tyseat.Count > 0)
                {
                    var tyseatDesre = SelectedTypeSeats.Where(s => s.IsReserved != false).ToList();
                    if (tyseatDesre.Count > 0)
                    {
                        foreach (var item in tyseatDesre)
                        {
                            List<TypeSeat> lista = new List<TypeSeat>();
                            lista.Add(item);
                            Utilities.CancelAssing(lista, dipMapCurrent);
                        }
                    }


                    ShowModalError(tyseat);

                    ReloadWindow();
                    return;
                }

                SaveDataBaseLocal();
                if (_ErrorTransaction)
                {
                    frmLoading.Close();
                    ShowPay();
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

        private void ShowModalError(List<TypeSeat> tyseats)
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            var stringerror = new StringBuilder();
            stringerror.Append("No se puedieron reservar los puestos: ");
            stringerror.AppendLine();
            foreach (var tyseat in tyseats)
            {
                stringerror.Append(tyseat.Name + "-");
                //stringerror.AppendLine();
            }

            stringerror.Append("Por favor vuelva a intentarlo.");

            Utilities.ShowModal(stringerror.ToString());
            controlReinicio++;
        }


        /// <summary>
        /// Envía al formulario de pagos
        /// </summary>
        private async void ShowPay()
        {
            try
            {
                frmLoading = new FrmLoading("¡Creando la transacción...!");
                frmLoading.Show();
                var response = await utilities.CreateTransaction("Cine ", dipMapCurrent, SelectedTypeSeats);

                var responseDash = await utilities.CreatePrintDashboard();
                frmLoading.Close();
                LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: Transaction | PrintID > ", response + "|" + responseDash);

                if (!response || !responseDash)
                {
                    foreach (var item in SelectedTypeSeats)
                    {
                        List<TypeSeat> lista = new List<TypeSeat>();
                        lista.Add(item);
                        Utilities.CancelAssing(lista, dipMapCurrent);
                    }

                    await Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Opacity = 0.3;
                        Pay.IsEnabled = true;
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

                    catch (Exception ex)
                    {
                        AdminPaypad.SaveErrorControl(ex.Message, "Grabador en ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
                    }

                    if (Utilities.MedioPago == 1)
                    {
                        try
                        {
                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                        }
                        catch { }

                        LogService.CreateLogsPeticionRespuestaDispositivos("=".PadRight(5, '=') + "Transacción de " + DateTime.Now + ": ", "ID: " + Utilities.IDTransactionDB);
                        Utilities.controlStop = 0;
                        int i = 0;
                        using (var conexion = new DBProcinalEntities())
                        {
                            foreach (var item in SelectedTypeSeats)
                            {
                                conexion.RePrint.Add(new RePrint
                                {
                                    Category = dipMapCurrent.Category,
                                    Cinema = Utilities.GetConfiguration("NameCinema"),
                                    Consecutivo = Utilities.DashboardPrint[i].RANGO_ACTUAL.ToString(),
                                    Date = dipMapCurrent.Date,
                                    FechaPago = DateTime.Now,
                                    Formato = Utilities.MovieFormat,
                                    Movie = dipMapCurrent.MovieName,
                                    Room = dipMapCurrent.RoomName,
                                    Seat = item.Name,
                                    Secuencia = Utilities.Secuencia,
                                    Time = dipMapCurrent.HourFunction,
                                    Tramite = "Boleto de Cine",
                                    Valor = item.Price,
                                    IDTransaccion = Utilities.IDTransactionDB
                                });
                                conexion.SaveChanges();
                                i++;
                            }

                        }

                        frmPayCine pay = new frmPayCine(SelectedTypeSeats, dipMapCurrent);
                        pay.Show();
                        this.Close();
                    }
                    else
                    {
                        try
                        {
                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                        }
                        catch { }
                        FrmCardPayment pay = new FrmCardPayment(SelectedTypeSeats, dipMapCurrent);
                        pay.Show();
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ShowPay en frmSeat", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                btnAtras.IsEnabled = false;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            frmSchedule frmSchedule = new frmSchedule(Utilities.Movie);
            frmSchedule.Show();
            Close();
        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SendData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

    }
}
