using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    public partial class frmCinema : Window
    {
        public frmCinema()
        {
            InitializeComponent();
            Utilities.CinemaId = Utilities.GetConfiguration("CodCinema");
            //LblCinema.Text = Dictionaries.Cinemas[Utilities.CinemaId];

            var t = Task.Run(() =>
            {
                SendPayments();
                DesReserve();
                SendDataToDataBase();
            });

            //var task2 = Task.Run(() =>
            //{
            //    GetScreen();
            //});

            LoadData();

        }

        private void DesReserve()
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

        private void GetScreen()
        {
            ControlPantalla.ConsultarControlPantalla();
        }

        private void SendPayments()
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

        private async void SendDataToDataBase()
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

        private void LoadData()
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

        private void BtnConsult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                gridPrincipal.IsEnabled = false;
            }
            catch { }
            frmMovies frmMovies = new frmMovies();
            frmMovies.Show();
            Close();
        }
    }
}
