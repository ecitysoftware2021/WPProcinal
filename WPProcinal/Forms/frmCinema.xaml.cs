﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using WPProcinal.ADO;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Models.ApiLocal;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    public partial class frmCinema : Window
    {
        ApiLocal api;
        Utilities printService;
        public frmCinema()
        {
            InitializeComponent();
            printService = new Utilities();
            api = new ApiLocal();
            Utilities.CinemaId = Utilities.GetConfiguration("CodCinema");
           
            Task.Run(() =>
            {
                SendPayments();
            });
            Task.Run(() =>
            {
                DesReserve();
            });
            Task.Run(() =>
            {
                SendDataToDataBase();
            });
            Task.Run(() =>
            {
                NotifyPending();
            });
            Task.Run(() =>
            {
                NotifyPendingMoney();
            });
            LoadData();
        }

        private void DesReserve()
        {
            try
            {
                var seats = DBProcinalController.GetSeatsreverve();
                if (seats != null)
                {
                    var dipmaps = seats.Select(s => s.DipMapId).Distinct().ToList();
                    foreach (var item in dipmaps)
                    {
                        var dipmap = DBProcinalController.GetDipMap2(item.Value);
                        if (dipmap != null)
                        {
                            var dd = seats.Where(s => s.DipMapId == dipmap.DipMapId).ToList();
                            var dipMap = Utilities.ConvertDipMap(dipmap);
                            var seatst = Utilities.ConvertSeats(dd);
                            Utilities.CancelAssing(seatst, dipMap);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "DesReserve en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void SendPayments()
        {
            try
            {
                var notpayments = DBProcinalController.GetDipmapNotPay();
                if (notpayments != null)
                {
                    foreach (var notpayment in notpayments)
                    {
                        var dipmap = DBProcinalController.GetDipMap2(notpayment.DipMapId.Value);
                        if (dipmap != null)
                        {
                            var seats = DBProcinalController.GetSeats(dipmap.DipMapId);
                            var total = seats.Select(s => s.Price).Sum();
                            var response = WCFServices.PostComprar(dipmap, double.Parse(total.Value.ToString()), "E", seats.Count());
                            if (response.IsSuccess)
                            {
                                var res = DBProcinalController.UpdateDipmapNotPay(dipmap.DipMapId);
                                var res2 = DBProcinalController.UpdateDipMap(dipmap.DipMapId);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SendPayments en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private async void SendDataToDataBase()
        {
            try
            {
                var dipmaps = DBProcinalController.GetDipMapActive();
                if (dipmaps == null)
                {
                    return;
                }

                foreach (var dipmap in dipmaps)
                {
                    var seats = DBProcinalController.GetSeats(dipmap.DipMapId);
                    if (seats == null)
                    {
                        return;
                    }

                    var response = await WCFServices.InsertDBProcinal(dipmap, seats);
                    if (!response.IsSuccess)
                    {
                        Utilities.SaveLogError(new LogError
                        {
                            Message = response.Message,
                            Method = "SendDataToDataBase",
                        });
                    }

                    var res = DBProcinalController.RemoveDipmap(dipmap.DipMapId);
                    if (!res.IsSuccess)
                    {
                        Utilities.SaveLogError(new LogError
                        {
                            Message = response.Message,
                            Method = "DBProcinalController.RemoveDipmap",
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SendDataToDatabase en frmCinema", EError.Aplication, ELevelError.Medium);

            }
        }

        private void LoadData()
        {
            try
            {
                Utilities.LstMovies.Clear();
                this.Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    string dataXml = string.Empty;
                    if (!Utilities.IfExistFileXML())
                    {
                        var response = WCFServices.DownloadData();
                        if (!response.IsSuccess)
                        {
                            Utilities.ShowModal(response.Message);
                            return;
                        }

                        Utilities.SaveFileXML(response.Result.ToString());
                        dataXml = response.Result.ToString();
                    }
                    else
                    {
                        dataXml = Utilities.GetFileXML();
                    }

                    Peliculas data = WCFServices.DeserealizeXML<Peliculas>(dataXml);
                    Utilities.Peliculas = data;
                }));
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadData en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void BtnConsult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int statusCode = printService.Init();

                if (!printService.StatePrint)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        string message = printService.MessageStatus(statusCode);
                        if (!string.IsNullOrEmpty(message))
                        {
                            frmModal modal = new frmModal(message);
                            modal.ShowDialog();
                        }
                    });
                }
                else
                {
                    try
                    {
                        gridPrincipal.IsEnabled = false;
                    }
                    catch { }
                    if (statusCode == 8)
                    {

                        string message = printService.MessageStatus(statusCode);
                        frmModal modal = new frmModal(message);
                        modal.ShowDialog();
                    }
                    frmMovies frmMovies = new frmMovies();
                    frmMovies.Show();
                    Close();
                }

            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        public void NotifyPending()
        {
            try
            {
                using (var con = new DBProcinalEntities())
                {
                    var notifies = con.NotifyPay.ToList();

                    foreach (var item in notifies)
                    {
                        Transaction Transaction = new Transaction
                        {
                            STATE_TRANSACTION_ID = item.STATE_TRANSACTION_ID.Value,
                            DATE_END = item.DATE_END.Value,
                            INCOME_AMOUNT = item.INCOME_AMOUNT.Value,
                            RETURN_AMOUNT = item.RETURN_AMOUNT.Value,
                            TRANSACTION_ID = item.TRANSACTION_ID.Value
                        };

                        var response = api.GetResponse(new Uptake.RequestApi()
                        {
                            Data = Transaction
                        }, "UpdateTransaction");

                        if (response != null)
                        {
                            if (response.Result.CodeError == 200)
                            {
                                con.NotifyPay.Remove(item);
                                con.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "NotifyPending en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        public void NotifyPendingMoney()
        {
            try
            {
                using (var con = new DBProcinalEntities())
                {
                    var notifies = con.NotifyMoney.ToList();

                    foreach (var item in notifies)
                    {
                        if (item.DATE.Value.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")))
                        {
                            printService.ProccesValue(new DataMoneyNotification
                            {
                                enterValue= item.DENOMINATION.Value,
                                opt= item.OPERATION.Value,
                                quantity= item.QUANTITY.Value,
                                idTransactionAPi= item.TRANSACTION_ID.Value
                            });
                        }
                        con.NotifyMoney.Remove(item);
                    }
                    con.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "NotifyPending en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Image_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ok");
        }
    }
}
