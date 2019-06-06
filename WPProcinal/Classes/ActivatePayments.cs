using PayPad.Billetero;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPProcinal.Forms;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Classes
{
    public class ActivatePayments
    {
        CLSWCFPayPad objWCFPayPad = new CLSWCFPayPad();
        PayPad.Billetero.Verificar objVerificar = new PayPad.Billetero.Verificar();
        PayPad.Billetero.Retirar objRetirar = new PayPad.Billetero.Retirar();
        PayPad.Billetero.Ingresar objIngresar = new PayPad.Billetero.Ingresar();
        PayPad.Billetero.Propiedades objPropiedades = new PayPad.Billetero.Propiedades();
        PayPad.Billetero.PropiedadesRetiro objPropiedadesRetiro = new PayPad.Billetero.PropiedadesRetiro();

        public static List<CLSListaDenominacionValor> ListaDenominacionValor = new List<CLSListaDenominacionValor>();
        Utilities objUtil = new Utilities();
        bool banderaDevolucion = false;
        public static string DatosRecibidos = string.Empty;
        public bool EstadoR;
        int countError = 0;
        public static int Total = 0;

        public PayViewModel PayViewModel;

        static SerialPort _serialPortM = new SerialPort();
        static SerialPort _serialPort = new SerialPort();

        public async Task Devolver()
        {
            await IniciarDevolucion();
        }

        private Task IniciarDevolucion()
        {
            var t = Task.Run(() =>
            {
                Thread.Sleep(3000);
                DevolverDinero(Utilities.ValorDevolver, true);
            });

            return t;
        }

        private void DevolverDinero(decimal Valor, bool isCancel)
        {
            try
            {
                VisivilityDevolverDinero();
                Utilities.DoEvents();
                objIngresar.ValidarCantidadXBilletes();
                objRetirar = new PayPad.Billetero.Retirar();
                objRetirar.Closeport();

                bool millaresFalse = false;
                int numero = decimal.ToInt32(Valor);
                int millares = numero / 1000;
                int centenas = (numero - (millares * 1000)) / 100;
                int decenas = (numero - (millares * 1000 + centenas * 100)) / 10;
                int unidades = numero - (millares * 1000 + centenas * 100 + decenas * 10);
                centenas = centenas * 100;
                millares = millares * 1000;
                decimal ValorB = millares;
                objUtil.ValorRestante = Valor;
                if (millares >= 1000)
                {
                    objUtil.ValorRestante = millares;
                    objVerificar = objRetirar.VerificarAlmacenamiento();
                    var r = objRetirar.VerificarRetiro(Convert.ToDouble(objUtil.ValorRestante), objVerificar);
                    Billetes veri = null;
                    Billetes veriAux = null;
                    int auxDeno = 0;
                    int cant = 0;
                    int auxCant = 0;
                    foreach (var item2 in objRetirar.listatuples)
                    {
                        string[] sp = item2.Item2.Split(';');
                        foreach (var item3 in sp)
                        {
                            if (!string.IsNullOrEmpty(item3))
                            {
                                string denom = item3.Split('-')[0].Split(':')[1];
                                cant = int.Parse(item3.Split('-')[1].Trim().Split(':')[1]);
                                veri = objVerificar.Billetes.Where(b => b.Denominacion == denom && int.Parse(b.Cantidad) >= cant).SingleOrDefault();
                                if (int.Parse(denom) > auxDeno && cant > 0 && int.Parse(denom) <= Valor)
                                {
                                    if (veri != null)
                                    {
                                        veriAux = veri;
                                        if (int.Parse(veriAux.Cantidad) > 0)
                                        {
                                            auxDeno = int.Parse(denom);
                                            auxCant = cant;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    millaresFalse = true;
                    if (veriAux != null)
                    {
                        int millares2 = int.Parse(veriAux.Denominacion) * auxCant;
                        objVerificar = objRetirar.VerificarAlmacenamiento();
                        var r2 = objRetirar.VerificarRetiro(millares2, objVerificar);
                        if (r2.Estado == "OK")
                        {
                            SendUpdateRetire(r2);
                            ValorB = millares2;
                            PayPad.Billetero.PropiedadesRetiro objn = PropiedadesRetiroN(ValorB);
                            Valor = Valor - ValorB;
                            objPropiedadesRetiro = objRetirar.Iniciar(objn);
                            if (objPropiedadesRetiro.Estado == "OK")
                            {
                                DevolverDinero(Valor, isCancel);
                                if (!banderaDevolucion)
                                {
                                    millares = 0;
                                    int centenas2 = Convert.ToInt32(objUtil.ValorRestante) - millares2;
                                    centenas += centenas2;
                                }
                            }
                        }
                    }
                    else
                    {
                        millares = 0;
                        centenas += int.Parse(objUtil.ValorRestante.ToString());
                    }
                }
                else
                {
                    if (!banderaDevolucion)
                    {
                        if (centenas > 0)
                        {
                            if (millaresFalse == false)
                            {
                                Thread.Sleep(2000);
                                int canti = centenas / 100;
                                ActivarMonedero(canti);
                                Valor = Valor - centenas;
                            }
                        }
                    }
                }
                if (!banderaDevolucion)
                {
                    if (millaresFalse == true)
                    {
                        if (centenas > 0)
                        {
                            Thread.Sleep(2000);
                            int canti = centenas / 100;

                            ActivarMonedero(canti);
                            Valor = Valor - centenas;
                        }
                    }
                }

                if (!EstadoR)
                {
                    if (!isCancel)
                    {
                        PayViewModel.ImgRecibo = Visibility.Visible;
                        PayViewModel.ImgEspereCambio = Visibility.Hidden;
                        Utilities.DoEvents();
                        FinalizarPago();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.SaveLogError(new LogError
                {
                    Message = ex.Message,
                    Exception = ex,
                    Method = "Devolver Dinero",
                    Error = string.Format("el vrl ingresado {0},el valor a pagar {1}, el valor a devolver {2}", objUtil.ValorIngresado, objUtil.Valor, Valor),
                });

                Utilities.ShowModal(ex.Message);
            }
        }

        private void VisivilityDevolverDinero()
        {
            PayViewModel.ImgEspereCambio = Visibility.Visible;
            PayViewModel.ImgCancel = Visibility.Hidden;
            PayViewModel.ImgIngreseBillete = Visibility.Hidden;
            PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
            PayViewModel.ImgRecibo = Visibility.Hidden;
        }

        private void SendUpdateRetire(PropiedadesRetiro r2)
        {
            var t = Task.Run(() =>
            {
                foreach (var item in Utilities.IDTransaccionDBs)
                {
                    objWCFPayPad.InsertarDetalleTransaccion(item, WCFPayPad.CLSEstadoEstadoDetalle.Devolviendo, decimal.Parse(r2.ValorTotalARetirar));
                }
            });
        }

        private PayPad.Billetero.PropiedadesRetiro PropiedadesRetiroN(decimal ValorB)
        {
            GC.Collect();
            PayPad.Billetero.Retirar R2 = new PayPad.Billetero.Retirar();
            PayPad.Billetero.Verificar V = new PayPad.Billetero.Verificar();
            V = R2.VerificarAlmacenamiento();
            R2.listatuples.Clear();
            var n = R2.VerificarRetiro(Convert.ToDouble(ValorB), V);
            R2.listatuples.Clear();
            return n;
        }

        private void ActivarMonedero(int cantidad)
        {
            banderaDevolucion = true;
            try
            {
                Thread.Sleep(2000);
                _serialPortM.Close();
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                _serialPortM.PortName = Utilities.GetConfiguration("PortMonedero");                                                                                    // Set the read/write timeouts
                _serialPortM.ReadTimeout = 500;
                _serialPortM.WriteTimeout = 5000;
                _serialPortM.Open();
                _serialPortM.DiscardInBuffer();
                _serialPortM.DiscardOutBuffer();

                _serialPortM.WriteLine(cantidad.ToString());

                _serialPortM.WriteLine("1111");

                _serialPortM.DataReceived += new SerialDataReceivedEventHandler(_serialPortM_DataReceived);

            }
            catch (Exception ex)
            {
                Utilities.SaveLogError(new LogError
                {
                    Message = ex.Message,
                    Exception = ex,
                    Method = "ActivarMonedero",
                    Error = string.Format("el vrl ingresado {0},el valor a pagar {1}, el valor a devolver {2}", objUtil.ValorIngresado, objUtil.Valor, Utilities.ValorDevolver),
                });

                Utilities.ShowModal(ex.Message);
                return;
            }
        }

        public void TimerTime()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += (s, a) =>
            {
                MethodConsultBill();
            };

            dispatcherTimer.Start();
        }

        private void _serialPortM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                Thread.Sleep(50);
                DatosRecibidos = DatosRecibidos + _serialPortM.ReadLine();
            }
            catch (Exception)
            {
                DatosRecibidos = DatosRecibidos + _serialPortM.ReadExisting();
            }

            if (DatosRecibidos.ToUpper().Contains("OFF"))
            {
                string[] data = DatosRecibidos.Replace("\r", "").Split(';');
                int control = 0;
                foreach (var item in data)
                {
                    if (control > 0)
                    {
                        string denominacion = item.Split(':')[0];
                        int cantidad = int.Parse(item.Split(':')[1]);
                        //int idDenominacion = pagoEfectivo.getDescriptionEnum(denominacion);
                        //WCFPayPadWS.InsertarControlMonedas(idDenominacion, idCorrespo, 0, cantidad);
                    }
                    control++;
                }
            }
            else
            {
                TimerCallback();
            }
        }

        private Task Pago(CancellationToken ct)
        {
            return Task.Run(() =>
            {
                try
                {
                    objPropiedades = objIngresar.Iniciar();
                }
                catch (Exception)
                {

                }
            });
        }

        public async void PagoEfectivo()
        {
            try
            {
                //SendMessage(Utilities.ETipoAlerta.BilleteroVacio, 0);
                //SendEmail(Utilities.ETipoAlerta.BilleteroVacio, 0);

                var cts = new CancellationTokenSource();
                objUtil.Estado = false;
                objUtil.ValorIngresado = 0;
                objUtil.ValorRestante = 0;

                while (!objUtil.Estado)
                {
                    await Pago(cts.Token);
                    if (!EstadoR)
                    {
                        if (objPropiedades.Estado == "OK")
                        {
                            VarificarBaul();
                            VerificarValorIngresadoAlBaul();
                            InsertarDetalleTransaccion();

                            objUtil.ValorIngresado = objUtil.ValorIngresado + decimal.Parse(objPropiedades.ValorAlmacenado);
                            objUtil.ValorFaltante = Utilities.ValorPagarScore - objUtil.ValorIngresado;

                            UpdateValues();
                            Utilities.DoEvents();
                            Utilities.ValorDevolver = objUtil.ValorIngresado;
                            if (objUtil.ValorFaltante > 0)
                            {
                                PayViewModel.ValorFaltante = objUtil.ValorFaltante.ToString("C", CultureInfo.CurrentCulture);
                                objUtil.Estado = false;
                            }
                            else
                            {
                                objIngresar.ValidarCantidadXBilletes();
                                _serialPortM.Write("1111");
                                PayViewModel.ValorRestante = (objUtil.ValorFaltante * -1).ToString("C", CultureInfo.CurrentCulture);
                                objUtil.Estado = true;
                                PayViewModel.ImgIngreseBillete = Visibility.Hidden;
                                PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
                                Utilities.DoEvents();
                            }
                        }
                        else
                        {
                            Utilities.SaveLogError(new LogError
                            {
                                Message = objPropiedades.Mensaje,
                                Method = "Pago Efectivo",
                            });

                            Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                            Utilities.ShowModal(objPropiedades.Mensaje);
                            //Utilities.GoToInicial(this);
                        }
                    }
                }
                if (objUtil.ValorFaltante < 0)
                {
                    var value = decimal.Parse(objUtil.ValorFaltante.ToString().Replace("-", ""));
                    UpdateValues();
                    Utilities.DoEvents();
                    Thread.Sleep(1000);
                    DevolverDinero(value, false);
                }
                else
                {
                    if (!EstadoR == true)
                    {
                        InsetarAuditoriaFinalizado();
                        FinalizarPago();
                    }
                }
            }
            catch (Exception ex)
            {
                DevolverDinero(objUtil.ValorIngresado, false);
                Utilities.SaveLogError(new LogError
                {
                    Message = ex.Message,
                    Method = "Pago Efectivo",
                    Exception = ex,
                });
                Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                Utilities.ShowModal(ex.Message);
                return;
            }
        }

        private void UpdateValues()
        {
            PayViewModel.ValorFaltante = objUtil.ValorFaltante > 0 ? (objUtil.ValorFaltante * -1).ToString("C", CultureInfo.CurrentCulture) : "$ 0.00";
            PayViewModel.ValorRestante = objUtil.ValorFaltante < 0 ? (objUtil.ValorFaltante * -1).ToString("C", CultureInfo.CurrentCulture) : "$ 0.00";
            PayViewModel.ValorIngresado = objUtil.ValorIngresado.ToString("C", CultureInfo.CurrentCulture);
        }

        private void VarificarBaul()
        {
            var t = Task.Run(() =>
            {
                int BillestesBaul = objWCFPayPad.BilletesBaul(ControlPantalla.IdCorrespo);
                if (BillestesBaul >= int.Parse(ControlPantalla.MaxBaul))
                {
                    HelperEmails.SendEmail(Utilities.ETipoAlerta.BaulLLeno, objIngresar, BillestesBaul);
                }
            });
        }

        private void VerificarValorIngresadoAlBaul()
        {
            var bil = objPropiedades.ValorAlmacenado;
            //var idTypeEnum = Utilities.GetDescriptionEnum(bil);
            if (Ingresar.ObjPropiedades.Mensaje.Contains("bodega"))
            {
                ListaDenominacionValor.Add(new CLSListaDenominacionValor
                {
                    //IDDenominacion = idTypeEnum,
                    EnBaul = true
                });
            }
            else
            {
                ListaDenominacionValor.Add(new CLSListaDenominacionValor
                {
                    //IDDenominacion = idTypeEnum,
                    EnBaul = false
                });
            }
        }

        private void InsetarAuditoriaFinalizado()
        {
            var t = Task.Run(() =>
            {
                foreach (var item in Utilities.IDTransaccionDBs)
                {
                    objWCFPayPad.InsertarAuditoria(item, WCFPayPad.CLSEstadoEstadoAuditoria.FinalizandoTransaccion);
                }

                foreach (var item in ListaDenominacionValor)
                {
                    if (item.EnBaul == true)
                    {
                        // llamar al web services para insertar datos a la tabla Tbl_Billete
                        objWCFPayPad.InsertarControlBillete(ControlPantalla.IdCorrespo, item.IDDenominacion, true, false);
                    }
                    else
                    {
                        objWCFPayPad.InsertarControlBillete(ControlPantalla.IdCorrespo, item.IDDenominacion, false, true);
                    }
                }

                HelperEmails.SendEmail(Utilities.ETipoAlerta.BilleteroVacio, objIngresar, 0);
            });
        }

        private void InsertarDetalleTransaccion()
        {
            var t = Task.Run(() =>
            {
                foreach (var item in Utilities.IDTransaccionDBs)
                {
                    objWCFPayPad.InsertarDetalleTransaccion(item, WCFPayPad.CLSEstadoEstadoDetalle.Ingresando, decimal.Parse(objPropiedades.ValorAlmacenado));
                }
            });
        }

        private void FinalizarPago()
        {
            //try
            //{
            //var frmLoading = new FrmLoading("¡Actualizando Pago!");
            //frmLoading.Show();
            if (countError < 3)
            {
                Buytickets();
            }
            else
            {
                var response = DBProcinalController.SaveDipmapNotPay(Utilities.DipMapCurrent.DipMapId);
                if (!response.IsSuccess)
                {
                    Utilities.SaveLogError(new LogError
                    {
                        Message = response.Message,
                        Method = "FinalizarPago.DBProcinalController.SaveDipmapNotPay",
                        Error = Utilities.DipMapCurrent,
                    });
                }
            }

            PayViewModel.ImgRecibo = Visibility.Visible;
            PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
            PayViewModel.ImgEspereCambio = Visibility.Hidden;
            PayViewModel.ImgIngreseBillete = Visibility.Hidden;
            PayViewModel.ImgCancel = Visibility.Hidden;

            ApproveTransaction();
            //frmLoading.Close();
            objUtil.ImprimirComprobante("Aprobada", Utilities.Receipt, Utilities.TypeSeats, Utilities.DipMapCurrent);

            bool rp = objRetirar.Closeport();
            _serialPort.Close();
            objUtil.Estado = true;
            EstadoR = true;
            PayPad.Billetero.Ingresar.Running = false;
            EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    Utilities.SaveLogError(new LogError
            //    {
            //        Message = ex.Message,
            //        Method = "FinalizarPago",
            //        Exception = ex,
            //    });
            //}
        }

        private static void EndTransaction()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                frmModal _frmModal = new frmModal(string.Concat("!Muchas gracias por utilizar nuestro servicio.",
                Environment.NewLine,
                "Su transacción ha finalizado correctamente!"));
                _frmModal.ShowDialog();
                //Utilities.GoToInicial();


            }));
        }

        private void ApproveTransaction()
        {
            foreach (var item in Utilities.IDTransaccionDBs)
            {
                objWCFPayPad.ApproveTransaction(item);
            }
        }

        private void Buytickets()
        {
            if (countError < 3)
            {
                var response = WCFServices.PostComprar(Utilities.DipMapCurrent, Utilities.TypeSeats);
                if (!response.IsSuccess)
                {
                    Utilities.SaveLogError(new LogError
                    {
                        Message = response.Message,
                        Method = "WCFServices.PostComprar"
                    });

                    countError++;
                    Buytickets();
                }

                var transaccionCompra = WCFServices.DeserealizeXML<TransaccionCompra>(response.Result.ToString());
                if (transaccionCompra.Respuesta == "Fallida")
                {
                    Utilities.SaveLogError(new LogError
                    {
                        Message = transaccionCompra.Respuesta,
                        Method = "WCFServices.PostComprar.Fallida"
                    });
                    countError++;
                    Buytickets();
                }
                else
                {
                    var responseDB = DBProcinalController.EditPaySeat(Utilities.DipMapCurrent.DipMapId);
                    if (!response.IsSuccess)
                    {
                        Utilities.SaveLogError(new LogError
                        {
                            Message = responseDB.Message,
                            Method = "DBProcinalController.EditPaySeat"
                        });
                    }
                }
            }
            else
            {
                FinalizarPago();
            }
        }

        private void MethodConsultBill()
        {
            try
            {
                string estado = Ingresar.ObjPropiedades.Mensaje.ToUpper();

                if (estado.Contains("LEYENDO"))
                {
                    PayViewModel.ImgRecibo = Visibility.Hidden;
                    PayViewModel.ImgLeyendoBillete = Visibility.Visible;
                    PayViewModel.ImgEspereCambio = Visibility.Hidden;
                    PayViewModel.ImgIngreseBillete = Visibility.Hidden;
                    PayViewModel.ImgCancel = Visibility.Hidden;
                }
                else if (estado.Contains("ALMACENADO") || estado.Contains("BODEGA"))
                {
                    if (objUtil.ValorFaltante > 0)
                    {
                        PayViewModel.ImgRecibo = Visibility.Hidden;
                        PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
                        PayViewModel.ImgEspereCambio = Visibility.Hidden;
                        PayViewModel.ImgIngreseBillete = Visibility.Visible;
                        PayViewModel.ImgCancel = Visibility.Visible;
                    }
                    else
                    {
                        PayViewModel.ImgRecibo = Visibility.Visible;
                        PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
                        PayViewModel.ImgEspereCambio = Visibility.Hidden;
                        PayViewModel.ImgIngreseBillete = Visibility.Hidden;
                        PayViewModel.ImgCancel = Visibility.Hidden;

                    }
                }
                else if (estado.Contains("INGRESANDO"))
                {
                    PayViewModel.ImgRecibo = Visibility.Hidden;
                    PayViewModel.ImgLeyendoBillete = Visibility.Visible;
                    PayViewModel.ImgEspereCambio = Visibility.Hidden;
                    PayViewModel.ImgIngreseBillete = Visibility.Hidden;
                    PayViewModel.ImgCancel = Visibility.Hidden;
                }
                else if (estado.Contains("NO SE PUDO CONECTAR"))
                {
                    HideImages();
                }
                else if (objUtil.ValorFaltante < 0)
                {
                    VisivilityDevolverDinero();
                }


            }
            catch (Exception)
            {

            }
        }

        public void HideImages()
        {
            PayViewModel.ImgRecibo = Visibility.Hidden;
            PayViewModel.ImgLeyendoBillete = Visibility.Hidden;
            PayViewModel.ImgEspereCambio = Visibility.Hidden;
            PayViewModel.ImgIngreseBillete = Visibility.Visible;
            PayViewModel.ImgCancel = Visibility.Visible;
        }

        public void PagoMonedero()
        {
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            _serialPortM.PortName = Utilities.GetConfiguration("PortMonedero");                                                                                    // Set the read/write timeouts
            _serialPortM.ReadTimeout = 500;
            _serialPortM.WriteTimeout = 500;
            _serialPortM.Open();
            _serialPortM.DataReceived += new SerialDataReceivedEventHandler(_serialPortM_DataReceived);
            _serialPortM.Write("1001");
        }

        private void TimerCallback()
        {

            if (!DatosRecibidos.Equals(string.Empty))
            {
                if (!DatosRecibidos.ToUpper().Contains("O"))
                {
                    if (int.Parse(DatosRecibidos) > 99)
                    {
                        Total = int.Parse(DatosRecibidos);
                        //InsertarDetalle(CLSEstadoEstadoDetalle.Ingresando, Total);

                        // esto debemos modificarlo para que diga que la moneda ingresada se fue par el baul
                        //int idDenominacion = getDescriptionEnum(Total.ToString());
                        //WCFPayPadWS.InsertarControlMonedas(idDenominacion, idCorrespo, 1, 1);
                        DatosRecibidos = string.Empty;
                        objUtil.ValorIngresado = objUtil.ValorIngresado + Total;
                        objUtil.ValorFaltante = Utilities.ValorPagarScore - objUtil.ValorIngresado;


                        PayViewModel.ValorIngresado = objUtil.ValorIngresado.ToString("C", CultureInfo.CurrentCulture);
                        PayViewModel.ValorFaltante = objUtil.ValorFaltante.ToString("C", CultureInfo.CurrentCulture);
                        Utilities.ValorDevolver = objUtil.ValorIngresado;

                        if (objUtil.ValorFaltante == 0)
                        {
                            _serialPortM.Write("1111");
                            if (EstadoR != true)
                            {
                                FinalizarPago();
                            }
                        }
                        if (objUtil.ValorFaltante < 0)
                        {
                            PayViewModel.ValorFaltante = "$ 0.00";

                            decimal sobrante = objUtil.ValorFaltante * -1;
                            PayViewModel.ValorRestante = sobrante.ToString("C", CultureInfo.CurrentCulture);
                            DevolverDinero(objUtil.ValorFaltante * -1, false);
                        }
                    }

                }
            }
        }

        public void EndTransactionCancel(Window window)
        {
            try
            {
                frmModalCancel _frmModal = new frmModalCancel("¿Está seguro que quiere cancelar la transacción?");
                _frmModal.ShowDialog();
                if (_frmModal.DialogResult.HasValue && _frmModal.DialogResult.Value)
                {
                    _serialPort.Close();
                    _serialPortM.Close();
                    objIngresar.Closeport();
                    if (objUtil.ValorIngresado == 0)
                    {
                        Utilities.CancelAssing(Utilities.TypeSeats, Utilities.DipMapCurrent);
                        //Utilities.GoToInicial();
                    }
                    else
                    {
                        PayPad.Billetero.Ingresar.Running = false;
                        objUtil.Estado = true;
                        EstadoR = true;

                        frmCancelPay objForm = new frmCancelPay(objUtil.ValorIngresado, Utilities.TypeSeats, Utilities.DipMapCurrent);
                        objForm.Show();
                        window.Close();
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.SaveLogError(new LogError
                {
                    Message = ex.Message,
                    Method = "Salir",
                    Exception = ex,
                });

                Utilities.ShowModal(ex.Message);
                return;
            }
        }
    }
}
