using CEntidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
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

        bool _ErrorTransaction = true;

        Utilities utilities = new Utilities();

        public frmSeat(DipMap dipMap)
        {
            InitializeComponent();
            var frmLoading = new FrmLoading("¡Cargando la sala!");
            frmLoading.Show();

            dipMapCurrent = dipMap;
            imgBackground.Source = Utilities.ImageSelected;
            TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
            TxtDay.Text = dipMap.Day;
            TxtDuration.Text = string.Format("Duración: {0}", dipMap.Duration);
            TxtGenero.Text = string.Format("Género: {0}", dipMap.Gener);
            TxtFormat.Text = string.Format("Formato: {0}", Utilities.MovieFormat.ToUpper());
            TxtCategory.Text = string.Format("Censura: {0}", dipMap.Category);
            TxtHour.Text = dipMap.HourFunction;
            TxtSubTitle.Text = dipMap.Language;
            LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
            HideImages();
            this.Dispatcher.Invoke(() =>
            {
                LoadSeats();
                frmLoading.Close();
            });

            Utilities.DoEvents();
        }

        private void HideImages()
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

        private void VisibilityImages()
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
            }
        }

        private void LoadSeats()
        {

            var response = WCFServices.GetDipMap(dipMapCurrent);
            if (!response.IsSuccess)
            {
                Utilities.SaveLogError(new LogError
                {
                    Message = response.Message,
                    Method = "WCFServices.GetDipMap",

                });

                frmModal _frmModal = new frmModal("El servicio no esta disponible.");
                _frmModal.ShowDialog();

                CloseApplication(this);
            }

            var responseV2 = WCFServices.GetStateRoom(dipMapCurrent);
            if (!response.IsSuccess)
            {
                Utilities.SaveLogError(new LogError
                {
                    Message = response.Message,
                    Method = "WCFServices.GetStateRoom",

                });
                frmModal _frmModal = new frmModal("El servicio no esta disponible.");
                _frmModal.ShowDialog();
                CloseApplication(this);
            }

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

        private void CloseApplication(Window form)
        {
            form.Close();
            Utilities.GoToInicial();
        }


        private void OrganizePositionOfSeats(List<SeatTmp> states,
            int positionX, List<string> FTDistinc,
            int positionY, List<TypeSeat>
            typeSeats, List<SeatTmp> seatTmps)
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

                    if (item.Letter.Equals(FTDistinc[i]))
                    {
                        int newleft = count != 0 ? left + sum : left;
                        var type = seatTmps.Where(t => t.Name == item.Name).FirstOrDefault();
                        if (type.Type != "P")
                        {
                            var state = states.Where(s => s.Name == item.Name).FirstOrDefault();
                            ImageSource imageSource = null;
                            if (dipMapCurrent.TypeZona == "G")
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
                            if (state.State == "S")
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

                top = topNew;
            }

            //GifLoadder.Visibility = Visibility.Hidden;
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
            Image image = (Image)sender;
            var seatcurrent = SelectedTypeSeats.Where(s => s.Name == item.Name).FirstOrDefault();
            if (seatcurrent == null)
            {
                SelectedTypeSeats.Add(item);
                image.Source = GetImage("R");
            }
            else
            {
                SelectedTypeSeats.Remove(item);
                image.Source = GetImage(image.Tag.ToString());
            }

            LblNumSeats.Content = SelectedTypeSeats.Count.ToString();
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
                icon = "silla-vibrasound";
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
            if (SelectedTypeSeats.Count == 0)
            {
                Utilities.ShowModal("Debe seleccionar almenos un puesto");
                return;
            }


            var response = WCFServices.GetPrices(dipMapCurrent);
            if (!response.IsSuccess)
            {
                Utilities.ShowModal(response.Message);
                ReloadWindow();
                return;
            }

            var plantillatmp = WCFServices.DeserealizeXML<Plantilla>(response.Result.ToString());
            if (!string.IsNullOrEmpty(plantillatmp.Tarifas.Codigo.Error_en_Sistema))
            {
                Utilities.ShowModal(plantillatmp.Tarifas.Codigo.ToString());
                ReloadWindow();
            }

            var plantilla = WCFServices.DeserealizeXML<Plantilla2>(response.Result.ToString());
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

                selectedTypeSeat.Price = decimal.Parse(tarifa.Valor.Split(',')[0]);
                selectedTypeSeat.CodTarifa = tarifa.Codigo;
            }

            frmConfirmationModal _frmConfirmationModal = new frmConfirmationModal(SelectedTypeSeats, dipMapCurrent);
            _frmConfirmationModal.ShowDialog();
            if (_frmConfirmationModal.DialogResult.HasValue &&
                _frmConfirmationModal.DialogResult.Value)
            {
                //this.Dispatcher.BeginInvoke(new ThreadStart(() =>
                //{
                //    GifLoadder.Visibility = Visibility.Visible;
                //}));

                SecuenceAndReserve();
            }
        }

        private void ReloadWindow()
        {
            frmSeat _frmSeat = new frmSeat(dipMapCurrent);
            _frmSeat.Show();
            this.Close();
        }

        private void SecuenceAndReserve()
        {
            var frmLoadding = new FrmLoading("¡Reservando los puestos seleccionados!");
            frmLoadding.Show();            
            var responseSec = WCFServices.GetSecuence(dipMapCurrent);
            if (!responseSec.IsSuccess)
            {
                Utilities.ShowModal(responseSec.Message);
                ReloadWindow();
            }

            var secuence = WCFServices.DeserealizeXML<SecuenciaVenta>(responseSec.Result.ToString());
            if (!string.IsNullOrEmpty(secuence.Error))
            {
                Utilities.ShowModal(secuence.Error);
                ReloadWindow();
            }

            dipMapCurrent.Secuence = int.Parse(secuence.Secuencia);
            foreach (var item in SelectedTypeSeats)
            {
                var response = WCFServices.PostReserva(dipMapCurrent, item);
                if (!response.IsSuccess)
                {
                    item.IsReserved = false;
                    Utilities.ShowModal(response.Message);
                }

                var reserve = WCFServices.DeserealizeXML<Reserva>(response.Result.ToString());
                if (!string.IsNullOrEmpty(reserve.Error_en_proceso))
                {
                    item.IsReserved = false;
                    Utilities.ShowModal(reserve.Error_en_proceso);
                }
                else
                {
                    item.NumSecuencia = reserve.Secuencia_reserva;
                    item.IsReserved = true;
                }
            }

            var tyseat = SelectedTypeSeats.Where(s => s.IsReserved == false).ToList();
            if (tyseat.Count > 0)
            {
                ShowModalError(tyseat);
                ReloadWindow();
                return;
            }

            SaveDataBaseLocal();
           // GenerateTransactions();
            if (_ErrorTransaction)
            {
                frmLoadding.Close();
                ShowPay();
            }
        }

        private void GenerateTransactions()
        {
            _ErrorTransaction = true;
            foreach (var selectedTypeSeat in SelectedTypeSeats)
            {
                var reference = GetReferencetransaction(dipMapCurrent, selectedTypeSeat);
                selectedTypeSeat.TransactionId = CrearTransaccion(reference, selectedTypeSeat.Price);
            }
        }

        private int CrearTransaccion(string reference, decimal monto)
        {
            try
            {
                CLSWCFPayPad objWCFPayPad = new CLSWCFPayPad();
                WCFPayPad.CLSTransaction objTransaction = new WCFPayPad.CLSTransaction
                {
                    IDCorresponsal = Utilities.CorrespondetId,
                    IDTramite = Utilities.TramiteId,
                    Referencia = reference,
                    Total = monto,
                    
                };

                var transactionId = objWCFPayPad.InsertarTransaccion(objTransaction);
                Utilities.IDTransaccionDBs.Add(transactionId);
                return transactionId;
            }
            catch (Exception ex)
            {
                _ErrorTransaction = false;
                Utilities.SaveLogError(new LogError
                {
                    Method = "Crear transacción",
                    Exception = ex,
                    Message = ex.Message,
                });

                Utilities.CancelAssing(SelectedTypeSeats, dipMapCurrent);
                Utilities.ShowModal("Servicio no disponible");
                Utilities.GoToInicial();
                return 0;
            }
        }

        private void SaveDataBaseLocal()
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

        private void ShowModalError(List<TypeSeat> tyseats)
        {
            var stringerror = new StringBuilder();
            stringerror.Append("No se puerieron reservar los puestos: ");
            stringerror.AppendLine();
            foreach (var tyseat in tyseats)
            {
                stringerror.Append(tyseat.Name);
                stringerror.AppendLine();
            }

            stringerror.Append("Por favor vuelva a intentarlo.");
            Utilities.ShowModal(stringerror.ToString());
        }

        private string GetReferencetransaction(DipMap dipMap, TypeSeat typeSeat)
        {
            return string.Concat(dipMap.CinemaId,
                                    dipMap.Secuence);
        }

        private async void ShowPay()
        {
            var response = await utilities.CreateTransaction("Cine", dipMapCurrent);

            if (!response)
            {
                await Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.ShowModal("No se pudo crear la transacción...");
                });
                GC.Collect();
            }
            else
            {
                frmPayCine pay = new frmPayCine(SelectedTypeSeats, dipMapCurrent);
                pay.Show();
                this.Close();
            }
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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

        }
    }
}
