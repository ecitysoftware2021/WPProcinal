using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using WPProcinal.Models;

namespace WPProcinal.Classes
{
    public class ControlPeripherals
    {

        #region References

        #region SerialPorts

        private SerialPort _serialPortBills;//Puerto billeteros

        private SerialPort _serialPortCoins;//Puerto Monederos

        private SerialPort _BarcodeReader;//Puerto Huellero

        #endregion

        #region CommandsPorts

        private string _StartBills = "OR:START";//Iniciar los billeteros

        private string _AceptanceBillOn = "OR:ON:AP";//Operar billetero Aceptance

        private string _DispenserBillOn = "OR:ON:DP";//Operar billetero Dispenser

        private string _AceptanceBillOFF = "OR:OFF:AP";//Cerrar billetero Aceptance

        private string _DispenserBillOFF = "OR:OFF:DP";//Cerrar billetero Dispenser

        private string _AceptanceCoinOn = "OR:ON:MA";//Operar Monedero Aceptance

        private string _DispenserCoinOn = "OR:ON:MD:";//Operar Monedero Dispenser

        private string _AceptanceCoinOff = "OR:OFF:MA";//Cerrar Monedero Aceptance

        private string _CoinAceptanceStatus = ConfigurationManager.AppSettings["CheckStatusCoin"];//Preguntar estado del aceptador

        #endregion

        #region Callbacks

        public Action<decimal> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<decimal> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string> callbackError;//Calback de error

        public Action<string> callbackLog;//Calback de devolucion

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje
        public Action<bool> callbackStatusBillAceptance;//Calback de mensaje
        public Action<bool> callbackStatusCoinAceptanceDispenser;//Calback de mensaje

        public Action<DataDocument> callbackDocument;//Calback de la lectura de la cedula

        #endregion

        #region EvaluationValues

        private static int _mil = 1000;
        private static int _hundred = 100;
        private static int _tens = 10;

        #endregion

        #region Variables

        private decimal payValue;//Valor a pagar

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar
        private decimal RealdispenserValue;//Valor a dispensar

        private bool stateError;

        private static string TOKEN;//Llabe que retorna el dispenser

        public string LogMessage;//Mensaje para el log

        public static LogDispenser log;//Log del dispenser

        public int WaitForCoins = 0;
        #endregion

        #region Casettes
        private int _Casete1 = int.Parse(ConfigurationManager.AppSettings["Casete1"]);//Denominacion cassete 1

        private int _Casete2 = int.Parse(ConfigurationManager.AppSettings["Casete2"]);//Denominacion cassete 2

        private int _Casete3 = int.Parse(ConfigurationManager.AppSettings["Casete3"]);//Denominacion cassete 3
        #endregion

        #region Ports Times
        private int BillsReadTime = int.Parse(ConfigurationManager.AppSettings["BillsReadTime"]);//Denominacion cassete 1

        private int BillsWriteTimes = int.Parse(ConfigurationManager.AppSettings["BillsWriteTimes"]);//Denominacion cassete 2

        private int BillsBaudRate = int.Parse(ConfigurationManager.AppSettings["BillsBaudRate"]);//Denominacion cassete 3

        private int CoinsReadTime = int.Parse(ConfigurationManager.AppSettings["CoinsReadTime"]);//Denominacion cassete 3

        private int CoinsWriteTimes = int.Parse(ConfigurationManager.AppSettings["CoinsWriteTimes"]);//Denominacion cassete 3

        private int CoinsBaudRate = int.Parse(ConfigurationManager.AppSettings["CoinsBaudRate"]);//Denominacion cassete 3
        #endregion


        #endregion

        #region LoadMethods

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControlPeripherals()
        {
            try
            {
                if (_serialPortBills == null)
                {
                    _serialPortBills = new SerialPort();
                }
                if (_serialPortCoins == null)
                {
                    _serialPortCoins = new SerialPort();
                }
                if (_BarcodeReader == null)
                {
                    _BarcodeReader = new SerialPort();
                }
                if (log == null)
                {
                    log = new LogDispenser();
                }

            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Constructor en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }



        /// <summary>
        /// We open Bills and Coins Serial Ports
        /// </summary>
        public void OpenSerialPorts()
        {
            if (!_serialPortBills.IsOpen)
            {

                _serialPortBills.DtrEnable = true;
                _serialPortBills.DiscardNull = true;
                InitPortBills();
            }

            if (!_serialPortCoins.IsOpen)
            {

                _serialPortCoins.DtrEnable = true;
                _serialPortCoins.DiscardNull = true;
                InitPortCoins();
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            try
            {
                SendMessageBills(_StartBills);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Start en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }
        public void StartCoinAcceptorDispenser()
        {
            try
            {
                SendMessageCoins(_CoinAceptanceStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills()
        {
            try
            {
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = Utilities.GetConfiguration("PortBills");
                    _serialPortBills.ReadTimeout = BillsReadTime;
                    _serialPortBills.WriteTimeout = BillsWriteTimes;
                    _serialPortBills.BaudRate = BillsBaudRate;
                    _serialPortBills.Open();
                    Thread.Sleep(1000);
                }

                _serialPortBills.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                    "Iniciando el puerto de los billeteros",
                    EError.Device,
                    ELevelError.Strong);
                callbackError?.Invoke(string.Concat("Billetero: ", ex.Message));
            }
        }

        /// <summary>
        ///  Método para inciar el puerto de los monederos
        /// </summary>
        private void InitPortCoins()
        {
            try
            {
                if (!_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.PortName = Utilities.GetConfiguration("PortCoins");
                    _serialPortCoins.ReadTimeout = CoinsReadTime;
                    _serialPortCoins.WriteTimeout = CoinsWriteTimes;
                    _serialPortCoins.BaudRate = CoinsBaudRate;
                    _serialPortCoins.DtrEnable = true;
                    _serialPortCoins.RtsEnable = true;
                    _serialPortCoins.Open();
                    Thread.Sleep(1000);
                }

                _serialPortCoins.DataReceived += new SerialDataReceivedEventHandler(_serialPortCoinsDataReceived);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                    "Iniciando el puerto de los monederos",
                    EError.Device,
                    ELevelError.Strong);
                callbackError?.Invoke(string.Concat("Monedero: ", ex.Message));
            }
        }

        /// <summary>
        ///  Método para inciar el puerto del scanner
        /// </summary>
        public void InitializePortScanner(string portName)
        {
            try
            {
                if (!_BarcodeReader.IsOpen)
                {
                    _BarcodeReader.PortName = portName;
                    _BarcodeReader.BaudRate = 57600;
                    _BarcodeReader.Open();
                    _BarcodeReader.ReadTimeout = 200;
                    _BarcodeReader.DtrEnable = true;
                    _BarcodeReader.RtsEnable = true;
                    _BarcodeReader.DataReceived += new SerialDataReceivedEventHandler(Scanner_DataReceived);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SendMessage

        /// <summary>
        /// Método para enviar orden al puerto de los billeteros
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private void SendMessageBills(string message)
        {
            try
            {
                if (_serialPortBills.IsOpen)
                {

                    _serialPortBills.Write(message);
                    try
                    {
                        LogService.SaveRequestResponse(DateTime.Now + " :: Mensaje al billetero: ", message);
                    }
                    catch { }
                    Thread.Sleep(1000);
                    log.SendMessage += string.Format("Billetero: {0}\n", message);
                }
            }
            catch (Exception ex)
            {

                AdminPaypad.SaveErrorControl(ex.Message,
                    "Enviando mensaje a los billeteros",
                    EError.Device,
                    ELevelError.Strong);
                callbackError?.Invoke(string.Concat("Billetero: ", ex.Message));
            }
        }

        /// <summary>
        /// Método para enviar orden al puerto de los monederos
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private void SendMessageCoins(string message)
        {
            try
            {
                if (_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.Write(message);
                    try
                    {
                        LogService.SaveRequestResponse(DateTime.Now + " :: Mensaje al monedero: ", message);
                    }
                    catch { }
                    Thread.Sleep(1000);
                    log.SendMessage += string.Format("Monedero: {0}\n", message);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                    "Enviando mensaje a los monederos",
                    EError.Device,
                    ELevelError.Strong);
                callbackError?.Invoke(string.Concat("Monedero: ", ex.Message));
            }
        }

        #endregion

        #region Listeners

        /// <summary>
        /// Método que escucha la respuesta del puerto del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPortBillsDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string response = _serialPortBills.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Billetero:{0}\n", response);
                    ProcessResponseBills(response);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                "_serialPortBillsDataReceived",
                EError.Aplication,
                ELevelError.Strong);
            }
        }

        /// <summary>
        /// Método que escucha la respuesta del puerto del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPortCoinsDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string response = _serialPortCoins.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    log.ResponseMessage += string.Format("Respuesta Monedero: {0}\n", response);
                    ProcessResponseCoins(response);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message,
                "_serialPortCoinsDataReceived",
                EError.Aplication,
                ELevelError.Strong);
            }
        }

        public int num = 0;
        /// <summary>
        /// Método que escucha la respuesta del puerto del scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (num == 0)
                {
                    num = 1;
                    Thread.Sleep(1000);
                    var data = _BarcodeReader.ReadExisting();
                    var response = Utilities.ProccesDocument(data);
                    callbackDocument?.Invoke(response);
                    //_BarcodeReader.DiscardInBuffer();
                    //_BarcodeReader.DiscardOutBuffer();
                }
            }
            catch (Exception ex)
            {
                //callbackError?.Invoke(Tuple.Create("AP", "Error, ha ocurrido una exepcion " + ex));
            }
        }

        #endregion

        #region ProcessResponse

        /// <summary>
        /// Método que procesa la respuesta del puerto de los billeteros
        /// </summary>
        /// <param name="message">respuesta del puerto de los billeteros</param>
        private void ProcessResponseBills(string message)
        {
            try
            {
                message = message.Replace("\r", string.Empty);
                string[] response = message.Split(':');
                switch (response[0])
                {
                    case "RC":
                        ProcessRC(response);
                        break;
                    case "ER":
                        try
                        {
                            AdminPaypad.SaveErrorControl(message,
                            "Respuesta de los billeteros",
                            EError.Device,
                            ELevelError.Strong);
                        }
                        catch { }
                        ProcessER(response);
                        break;
                    case "UN":
                        ProcessUN(response);
                        break;
                    case "TO":
                        ProcessTO(response);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProcessResponseBills en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Método que procesa la respuesta del puerto de los monederos
        /// </summary>
        /// <param name="message">respuesta del puerto de los monederos</param>
        private void ProcessResponseCoins(string message)
        {
            try
            {
                message = message.Replace("\r", string.Empty);
                string[] response = message.Split(':');
                switch (response[0])
                {
                    case "RC":
                        ProcessRC(response);
                        break;
                    case "ER":
                        try
                        {
                            AdminPaypad.SaveErrorControl(message,
                            "Respuesta de los monederos",
                            EError.Device,
                            ELevelError.Strong);
                        }
                        catch { }
                        ProcessER(response);
                        break;
                    case "UN":
                        ProcessUN(response);
                        break;
                    case "TO":
                        ProcessTO(response);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProcessResponseCoins en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region ProcessResponseCases

        /// <summary>
        /// Respuesta para el caso de Recepción de un mensaje enviado
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessRC(string[] response)
        {
            try
            {
                if (response[1] == "OK")
                {
                    switch (response[2])
                    {
                        case "AP":
                            callbackStatusBillAceptance?.Invoke(true);
                            break;
                        case "DP":
                            if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                            {
                                TOKEN = response[4].Replace("\r", string.Empty);
                                callbackToken?.Invoke(true);
                            }
                            break;
                        case "MA":
                        case "MD":
                            callbackStatusCoinAceptanceDispenser?.Invoke(true);
                            break;
                        default:
                            break;
                    }
                }
                else if (response[1] == "ON")
                {
                    switch (response[2])
                    {
                        case "MA":
                        case "MD":
                            callbackStatusCoinAceptanceDispenser?.Invoke(true);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProccessRC en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Respuesta para el caso de error
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessER(string[] response)
        {
            try
            {
                if (response[1] == "MD")
                {
                    stateError = true;
                    //Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    //{
                    //    callbackError?.Invoke(deliveryValue.ToString());
                    //}));
                }
                if (response[1] == "DP")
                {
                    stateError = true;
                    //callbackError?.Invoke(string.Concat("Error, se alcanzó a entregar:", deliveryValue));
                }

                if (response[1] == "AP")
                {
                    //callbackError?.Invoke("Error, en el billetero Aceptance");
                }
                else if (response[1] == "FATAL")
                {
                    //Utilities.RestartApp();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProcessER en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            try
            {
                if (response[1] == "DP")
                {
                    deliveryValue += decimal.Parse(response[2]) * _mil;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * _mil);
                }
                else if (response[1] == "MD")
                {
                    deliveryValue += decimal.Parse(response[2]) * _hundred;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * _hundred);
                }
                else
                {
                    if (response[1] == "AP")
                    {
                        enterValue += decimal.Parse(response[2]) * _mil;
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]) * _mil);
                    }
                    else if (response[1] == "MA")
                    {
                        enterValue += decimal.Parse(response[2]);
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]));
                    }
                    ValidateEnterValue();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProcessUN en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        public void StartValues()
        {
            deliveryValue = 0;
            enterValue = 0;
            deliveryVal = 0;
            LogMessage = string.Empty;
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
        {
            try
            {
                string responseFull;
                if (response[1] == "OK")
                {
                    responseFull = string.Concat(response[2], ":", response[3]);
                    if (response[2] == "DP")
                    {
                        ConfigDataDispenser(responseFull, 1);
                    }

                    if (response[2] == "MD")
                    {
                        ConfigDataDispenser(responseFull);
                    }
                }
                else
                {
                    responseFull = string.Concat(response[2], ":", response[3]);
                    if (response[2] == "DP")
                    {
                        ConfigDataDispenser(responseFull, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ProcessTO en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Dispenser

        /// <summary>
        /// Inicia el proceso paara el billetero dispenser
        /// </summary>
        /// <param name="valueDispenser">valor a dispensar</param>
        public void StartDispenser(decimal valueDispenser)
        {
            try
            {
                WaitForCoins = 0;
                stateError = false;
                dispenserValue = valueDispenser;
                ConfigurateDispenser();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "StartDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser()
        {
            try
            {
                RealdispenserValue = dispenserValue;
                if (dispenserValue > 0)
                {
                    decimal newAmountBills = 0;
                    decimal resta = dispenserValue;

                    if (dispenserValue >= 2000)
                    {
                        do
                        {
                            if (_Casete1 > 0)
                            {
                                if (Convert.ToInt32(Math.Floor(resta / _Casete1)) > 0)
                                {
                                    newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete1)) * _Casete1;
                                }
                                else if (Convert.ToInt32(Math.Floor(resta / _Casete2)) > 0)
                                {
                                    newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete2)) * _Casete2;
                                    Console.WriteLine(_Casete2 + ": " + Convert.ToInt32(Math.Floor(resta / _Casete2)));
                                }
                                else if (Convert.ToInt32(Math.Floor(resta / _Casete3)) > 0)
                                {
                                    newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete3)) * _Casete3;
                                    Console.WriteLine(_Casete3 + ": " + Convert.ToInt32(Math.Floor(resta / _Casete3)));
                                }
                            }
                            else if (Convert.ToInt32(Math.Floor(resta / _Casete2)) > 0)
                            {
                                newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete2)) * _Casete2;
                                Console.WriteLine(_Casete2 + ": " + Convert.ToInt32(Math.Floor(resta / _Casete2)));
                            }
                            else if (Convert.ToInt32(Math.Floor(resta / _Casete3)) > 0)
                            {
                                newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete3)) * _Casete3;
                                Console.WriteLine(_Casete3 + ": " + Convert.ToInt32(Math.Floor(resta / _Casete3)));
                            }
                            resta = dispenserValue - newAmountBills;
                        } while (resta >= 2000);
                        dispenserValue -= newAmountBills;
                        DispenserMoney((newAmountBills / _mil).ToString());
                        Thread.Sleep(2000);

                        if (dispenserValue > 0)
                        {
                            WaitForCoins = 2;
                            SendMessageCoins(_DispenserCoinOn + (dispenserValue / _hundred));
                            Thread.Sleep(200);
                        }
                        else
                        {
                            WaitForCoins = 1;
                        }
                    }
                    else
                    {
                        WaitForCoins = 1;
                        SendMessageCoins(_DispenserCoinOn + (dispenserValue / _hundred));
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ConfigurateDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Enviar la orden de dispensar al billetero
        /// </summary>
        /// <param name="valuePay"></param>
        private void DispenserMoney(string valuePay)
        {
            try
            {
                if (!string.IsNullOrEmpty(TOKEN))
                {
                    string message = string.Format("{0}:{1}:{2}", _DispenserBillOn, TOKEN, valuePay);
                    SendMessageBills(message);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "DispenserMoney en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Aceptance

        /// <summary>
        /// Inicia la operación de billetero aceptance
        /// </summary>
        /// <param name="payValue">valor a pagar</param>
        public void StartAceptance(decimal payValue)
        {
            try
            {
                this.payValue = payValue;
                SendMessageBills(_AceptanceBillOn);
                Thread.Sleep(200);
                SendMessageCoins(_AceptanceCoinOn);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "StartAceptance en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }


        /// <summary>
        /// Valida el valor que ingresa
        /// </summary>
        private void ValidateEnterValue()
        {
            decimal enterVal = enterValue;
            if (enterValue >= payValue)
            {
                StopAceptance();
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {

                    callbackTotalIn?.Invoke(enterVal);
                }));
                enterValue = 0;
            }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            SendMessageBills(_AceptanceBillOFF);
            Thread.Sleep(200);
            SendMessageCoins(_AceptanceCoinOff);
        }

        #endregion

        #region Responses

        public decimal deliveryVal;
        /// <summary>
        /// Procesa la respuesta de los dispenser M y B
        /// </summary>
        /// <param name="data">respuesta</param>
        /// <param name="isRj">si se fue o no al reject</param>
        private void ConfigDataDispenser(string data, int isBX = 0)
        {
            try
            {

                string[] values = data.Split(':')[1].Split(';');
                if (isBX < 2)
                {
                    foreach (var value in values)
                    {

                        int denominacion = int.Parse(value.Split('-')[0]);
                        int cantidad = int.Parse(value.Split('-')[1]);
                        deliveryVal += denominacion * cantidad;

                    }
                }

                if (isBX == 0 || isBX == 2)
                {

                    LogMessage += string.Concat(data.Replace("\r", string.Empty), "!");

                    callbackLog?.Invoke(string.Concat(data.Replace("\r", string.Empty), "!"));

                    ValidateFinal(isBX);
                    WaitForCoins--;
                }

                if (!stateError)
                {

                    ValidateFinal(isBX);
                }
                else
                {
                    if (WaitForCoins == 0)
                    {
                        callbackOut?.Invoke(deliveryVal);

                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ConfigDataDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }

        }

        private void ValidateFinal(int isBX)
        {
            if (RealdispenserValue <= deliveryVal)
            {
                //if (WaitForCoins == 0)
                //{
                    callbackTotalOut?.Invoke(deliveryVal);
                //}
            }
        }

        #endregion

        public void ClosePortScanner()
        {
            try
            {
                if (_BarcodeReader.IsOpen)
                {
                    _BarcodeReader.DiscardInBuffer();
                    _BarcodeReader.DiscardOutBuffer();
                    _BarcodeReader.Close();
                }
            }
            catch (Exception ex)
            {
                //callbackError?.Invoke(Tuple.Create("AP", "Error, ClosePortsScanner ha ocurrido una exepcion " + ex));
            }
        }
    }
}

