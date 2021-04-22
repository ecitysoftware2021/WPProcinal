using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPProcinal.Classes
{
    public class ControlPeripheralsUnified
    {

        #region References

        #region SerialPorts

        private SerialPort _serialPortBills;//Puerto billeteros
        private SerialPort _BarcodeReader;//Puerto Scanner


        #endregion

        #region CommandsPorts

        private string _StartBills = "OR:START";//Iniciar los billeteros

        private string _AceptanceBillOn = "OR:ON:AP";//Operar billetero Aceptance

        private string _DispenserBillOn = "OR:ON:DP";//Operar billetero Dispenser

        private string _AceptanceBillOFF = "OR:OFF:AP";//Cerrar billetero Aceptance

        #endregion

        #region Callbacks

        public Action<decimal> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<decimal> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string, string, EError, ELevelError> callbackError;//Calback de error

        public Action<string> callbackLog;//Calback de devolucion

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje

        public Action<string, string, int> CallBackSaveRequestResponse;


        #endregion


        #region Variables

        private decimal payValue;//Valor a pagar

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar

        private bool stateError;

        private static string TOKEN;//Llabe que retorna el dispenser

        public string LogMessage;//Mensaje para el log
        TimerTiempo timer;

        public int WaitForCoins = 0;
        #endregion


        public static string[] ErrorVector = new string[]
       {
            "STACKER_OPEN",
            "JAM_IN_ACCEPTOR",
            "PAUSE",
            "ER:MD",
            "thickness",
            "Scan",
            "FATAL",
            "Printer"
       };
        #endregion

        #region LoadMethods

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControlPeripheralsUnified()
        {
            try
            {
                if (_serialPortBills == null)
                {
                    _serialPortBills = new SerialPort();
                }
                if (_BarcodeReader == null)
                {
                    _BarcodeReader = new SerialPort();
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "Constructor en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// We open Bills and Coins Serial Ports
        /// </summary>
        public void OpenSerialPorts(string portName, int BillsReadTime = 500, int BillsWriteTimes = 500, int BillsBaudRate = 57600)
        {
            if (!_serialPortBills.IsOpen)
            {

                _serialPortBills.DtrEnable = true;
                _serialPortBills.DiscardNull = true;
                InitPortBills(portName, BillsReadTime, BillsWriteTimes, BillsBaudRate);
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
                callbackError?.Invoke(ex.Message, "Start en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills(string portName, int BillsReadTime, int BillsWriteTimes, int BillsBaudRate)
        {
            try
            {
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = portName;
                    _serialPortBills.ReadTimeout = BillsReadTime;
                    _serialPortBills.WriteTimeout = BillsWriteTimes;
                    _serialPortBills.BaudRate = BillsBaudRate;
                    _serialPortBills.DtrEnable = true;
                    _serialPortBills.RtsEnable = true;
                    _serialPortBills.Open();
                    _serialPortBills.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
                    Thread.Sleep(1000);
                }


            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Iniciando el puerto de los billeteros", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Iniciando el puerto de los billeteros", EError.Device, ELevelError.Strong);
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
                    CallBackSaveRequestResponse?.Invoke("Mensaje a los billeteros", message, 1);
                    _serialPortBills.Write(message);
                }
            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Enviando mensaje a los billeteros", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Enviando mensaje a los billeteros", EError.Device, ELevelError.Strong);
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
                Thread.Sleep(500);
                string response = _serialPortBills.ReadLine();
                CallBackSaveRequestResponse?.Invoke("Respuesta de los billeteros", response, 1);

                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseBills(response);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "_serialPortBillsDataReceived", EError.Aplication, ELevelError.Strong);
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

                        foreach (var item in ErrorVector)
                        {
                            if (message.ToLower().Contains(item.ToLower()))
                            {
                                CallBackSaveRequestResponse?.Invoke("Respuesta de los billeteros: ", message, 2);
                                callbackError?.Invoke(message, "Respuesta de los billeteros", EError.Device, ELevelError.Strong);
                            }
                        }

                        if (!message.ToLower().Contains("abnormal near"))
                        {
                            ProcessER(response);
                        }
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
                callbackError?.Invoke(ex.Message, "ProcessResponseBills en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                        case "DP":
                            if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                            {
                                TOKEN = response[4].Replace("\r", string.Empty);
                                callbackToken?.Invoke(true);
                            }
                            break;
                        case "MA":
                        default:
                            break;
                    }
                }
                else if (response[1] == "ON")
                {
                    switch (response[2])
                    {
                        case "MA":
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "ProccessRC en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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

                }
                if (response[1] == "DP")
                {
                    stateError = true;

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
                callbackError?.Invoke(ex.Message, "ProcessER en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                    deliveryValue += decimal.Parse(response[2]);
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]));
                }
                else if (response[1] == "MD")
                {
                    deliveryValue += decimal.Parse(response[2]);
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]));
                }
                else
                {
                    if (response[1] == "AP")
                    {
                        enterValue += decimal.Parse(response[2]);
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]));
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
                callbackError?.Invoke(ex.Message, "ProcessUN en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                callbackError?.Invoke(ex.Message, "ProcessTO en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Dispenser

        /// <summary>
        /// Inicia el proceso paara el billetero dispenser
        /// </summary>
        /// <param name="valueDispenser">valor a dispensar</param>
        public void StartDispenser(decimal valueDispenser, int minValue = 2000)
        {
            try
            {
                WaitForCoins = 0;
                stateError = false;
                dispenserValue = valueDispenser;
                ConfigurateDispenser(minValue);
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "StartDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser(int minValue)
        {
            try
            {
                if (dispenserValue > 0)
                {

                    if (dispenserValue % minValue == 0 || dispenserValue < minValue)
                    {
                        WaitForCoins = 1;
                    }
                    else
                    {
                        WaitForCoins = 2;
                    }
                    DispenserMoney(dispenserValue.ToString());
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "ConfigurateDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                    ActivateTimer();
                    SendMessageBills(message);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "DispenserMoney en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "StartAceptance en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }


        /// <summary>
        /// Valida el valor que ingresa
        /// </summary>
        private void ValidateEnterValue()
        {
            try
            {

                decimal enterVal = enterValue;
                if (enterValue >= payValue)
                {
                    StopAceptance();
                    Thread.Sleep(1500);
                    callbackTotalIn?.Invoke(enterVal);
                    enterValue = 0;
                }

            }
            catch { }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            SendMessageBills(_AceptanceBillOFF);
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
                    WaitForCoins--;
                    ValidateFinal(isBX);
                }

                if (!stateError)
                {
                    ValidateFinal(isBX);
                }
                else
                {
                    if (WaitForCoins <= 0)
                    {
                        callbackOut?.Invoke(deliveryVal);
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "ConfigDataDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }

        }

        private void ValidateFinal(int isBX)
        {
            if (dispenserValue <= deliveryVal)
            {
                if (WaitForCoins <= 0)
                {
                    callbackTotalOut?.Invoke(deliveryVal);
                    SetCallBacksNull();
                }
            }
        }

        #endregion

        #region Timer Inactividad
        void ActivateTimer()
        {
            try
            {
                timer = new TimerTiempo(Utilities.dataPaypad.PaypadConfiguration.inactivitY_TIMER);
                timer.CallBackClose = response => Application.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    SetCallBacksNull();
                    callbackOut?.Invoke(deliveryVal);
                });
            }
            catch { }
        }
        void SetCallBacksNull()
        {
            if (timer != null)
            {
                timer.CallBackStop?.Invoke(1);
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
            }
        }
        #endregion


    }

    public class ControlPeripherals
    {

        #region References

        #region SerialPorts

        private SerialPort _serialPortBills;//Puerto billeteros

        private SerialPort _serialPortCoins;//Puerto Monederos


        #endregion

        #region CommandsPorts

        private string _StartBills = "OR:START";//Iniciar los billeteros

        private string _AceptanceBillOn = "OR:ON:AP";//Operar billetero Aceptance

        private string _DispenserBillOn = "OR:ON:DP";//Operar billetero Dispenser

        private string _AceptanceBillOFF = "OR:OFF:AP";//Cerrar billetero Aceptance

        private string _AceptanceCoinOn = "OR:ON:MA";//Operar Monedero Aceptance

        private string _DispenserCoinOn = "OR:ON:MD:";//Operar Monedero Dispenser

        private string _AceptanceCoinOff = "OR:OFF:MA";//Cerrar Monedero Aceptance


        #endregion

        #region Callbacks

        public Action<decimal> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<decimal> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string, string, EError, ELevelError> callbackError;//Calback de error

        public Action<string> callbackLog;//Calback de devolucion

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje
        public Action<bool> callbackStatusBillAceptance;//Calback de mensaje
        public Action<bool> callbackStatusCoinAceptanceDispenser;//Calback de mensaje
        public Action<string, string, int> CallBackSaveRequestResponse;

        #endregion

        #region EvaluationValues

        private static int thousand = 1000;
        private static int hundred = 100;

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


        public int WaitForCoins = 0;
        #endregion

        #region Errors
        public static string[] ErrorVector = new string[]
       {
            "STACKER_OPEN",
            "JAM_IN_ACCEPTOR",
            "PAUSE",
            "ER:MD",
            "thickness",
            "Scan",
            "FATAL",
            "Printer"
       };
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

            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "Constructor en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }



        /// <summary>
        /// We open Bills and Coins Serial Ports
        /// </summary>
        public void OpenSerialPorts(string portName, string portCoinsName, int BillsReadTime = 500, int BillsWriteTimes = 500, int BillsBaudRate = 57600, int CoinsReadTime = 500, int CoinsWriteTimes = 500, int CoinsBaudRate = 57600)
        {
            if (!_serialPortBills.IsOpen)
            {

                _serialPortBills.DtrEnable = true;
                _serialPortBills.DiscardNull = true;
                InitPortBills(portName, BillsReadTime, BillsWriteTimes, BillsBaudRate);
            }

            if (!_serialPortCoins.IsOpen)
            {

                _serialPortCoins.DtrEnable = true;
                _serialPortCoins.DiscardNull = true;
                InitPortCoins(portCoinsName, CoinsReadTime, CoinsWriteTimes, CoinsBaudRate);
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            SendMessageBills(_StartBills);
        }
        public void StartCoinAcceptorDispenser()
        {
            try
            {
                SendMessageCoins(_StartBills);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills(string portName, int BillsReadTime = 500, int BillsWriteTimes = 500, int BillsBaudRate = 57600)
        {
            try
            {
                if (!_serialPortBills.IsOpen)
                {
                    _serialPortBills.PortName = portName;
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
                CallBackSaveRequestResponse?.Invoke("Iniciando el puerto de los billeteros", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Iniciando el puerto de los billeteros", EError.Device, ELevelError.Strong);
            }
        }

        /// <summary>
        ///  Método para inciar el puerto de los monederos
        /// </summary>
        private void InitPortCoins(string portCoinsName, int CoinsReadTime = 500, int CoinsWriteTimes = 500, int CoinsBaudRate = 57600)
        {
            try
            {
                if (!_serialPortCoins.IsOpen)
                {
                    _serialPortCoins.PortName = portCoinsName;
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
                CallBackSaveRequestResponse?.Invoke("Iniciando el puerto de los monederos", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Iniciando el puerto de los monederos", EError.Device, ELevelError.Strong);
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
                    CallBackSaveRequestResponse?.Invoke("Mensaje al billetero", message, 1);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Enviando mensaje a los billeteros", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Enviando mensaje a los billeteros", EError.Device, ELevelError.Strong);
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

                    CallBackSaveRequestResponse?.Invoke("Mensaje al monedero", message, 1);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Enviando mensaje a los monederos", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "Enviando mensaje a los monederos", EError.Device, ELevelError.Strong);
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
                CallBackSaveRequestResponse?.Invoke("Respuesta de los monederos", response, 2);
                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseBills(response);
                }
            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Recibiendo mensaje de los billeteros", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "_serialPortBillsDataReceived", EError.Aplication, ELevelError.Strong);
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
                CallBackSaveRequestResponse?.Invoke("Respuesta de los monederos", response, 2);

                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseCoins(response);
                }
            }
            catch (Exception ex)
            {
                CallBackSaveRequestResponse?.Invoke("Recibiendo mensaje de los monederos", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "_serialPortCoinsDataReceived", EError.Aplication, ELevelError.Strong);
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
                        foreach (var item in ErrorVector)
                        {
                            if (message.ToLower().Contains(item.ToLower()))
                            {
                                CallBackSaveRequestResponse?.Invoke("Respuesta de los billeteros", message, 2);
                                callbackError?.Invoke(message, "Respuesta de los billeteros", EError.Device, ELevelError.Strong);
                            }
                        }

                        if (!message.ToLower().Contains("abnormal"))
                        {
                            ProcessER(response);
                        }
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
                callbackError?.Invoke(ex.Message, "ProcessResponseBills en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                        if (!message.Contains("ER:MD: Empty or jam in 2 motor"))
                        {
                            CallBackSaveRequestResponse?.Invoke("Respuesta de los monederos", message, 2);
                            callbackError?.Invoke(message, "Respuesta de los monederos", EError.Device, ELevelError.Strong);
                            ProcessER(response);
                        }

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
                CallBackSaveRequestResponse?.Invoke("Procesando respuesta de los monederos", ex.Message, 2);
                callbackError?.Invoke(ex.Message, "ProcessResponseCoins", EError.Aplication, ELevelError.Medium);
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
                callbackError?.Invoke(ex.Message, "ProccessRC", EError.Aplication, ELevelError.Medium);
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
                }
                if (response[1] == "DP")
                {
                    stateError = true;
                }

                if (response[1] == "AP")
                {

                }
                else if (response[1] == "FATAL")
                {
                    //Utilities.RestartApp();
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "ProcessER en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                    deliveryValue += decimal.Parse(response[2]) * thousand;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * thousand);
                }
                else if (response[1] == "MD")
                {
                    deliveryValue += decimal.Parse(response[2]) * hundred;
                    callbackValueOut?.Invoke(Convert.ToDecimal(response[2]) * hundred);
                }
                else
                {
                    if (response[1] == "AP")
                    {
                        enterValue += decimal.Parse(response[2]) * thousand;
                        callbackValueIn?.Invoke(Convert.ToDecimal(response[2]) * thousand);
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
                callbackError?.Invoke(ex.Message, "ProcessUN en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                callbackError?.Invoke(ex.Message, "ProcessTO en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Dispenser

        /// <summary>
        /// Inicia el proceso paara el billetero dispenser
        /// </summary>
        /// <param name="valueDispenser">valor a dispensar</param>
        public void StartDispenser(decimal valueDispenser, string[] dispenserConfiguration)
        {
            try
            {
                WaitForCoins = 0;
                stateError = false;
                dispenserValue = valueDispenser;
                ConfigurateDispenser(dispenserConfiguration);
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "StartDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser(string[] dispenserConfiguration)
        {
            try
            {
                RealdispenserValue = dispenserValue;
                if (dispenserValue > 0)
                {
                    decimal newAmountBills = 0;
                    decimal resta = dispenserValue;

                    foreach (var item in dispenserConfiguration)
                    {
                        int _Casete1 = int.Parse(item);

                        if (Convert.ToInt32(Math.Floor(resta / _Casete1)) > 0)
                        {
                            newAmountBills += Convert.ToInt32(Math.Floor(resta / _Casete1)) * _Casete1;
                        }
                        resta = dispenserValue - newAmountBills;
                    }
                    dispenserValue -= newAmountBills;

                    if (dispenserValue > 0 && newAmountBills > 0)
                    {
                        WaitForCoins = 2;
                        DispenserMoney((newAmountBills / thousand).ToString());
                        Thread.Sleep(2000);
                        SendMessageCoins(_DispenserCoinOn + (dispenserValue / hundred));
                        Thread.Sleep(200);
                    }
                    else if (dispenserValue > 0)
                    {
                        WaitForCoins = 1;
                        SendMessageCoins(_DispenserCoinOn + (dispenserValue / hundred));
                        Thread.Sleep(200);
                    }
                    else if (newAmountBills > 0)
                    {
                        WaitForCoins = 1;
                        DispenserMoney((newAmountBills / thousand).ToString());
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.Message, "ConfigurateDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                callbackError?.Invoke(ex.Message, "DispenserMoney en ControlPeripherals", EError.Aplication, ELevelError.Medium);
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
                callbackError?.Invoke(ex.Message, "StartAceptance", EError.Aplication, ELevelError.Medium);
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
                callbackTotalIn?.Invoke(enterVal);
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
                callbackError?.Invoke(ex.Message, "ConfigDataDispenser en ControlPeripherals", EError.Aplication, ELevelError.Medium);
            }

        }

        private void ValidateFinal(int isBX)
        {
            if (RealdispenserValue <= deliveryVal)
            {
                callbackTotalOut?.Invoke(deliveryVal);
            }
        }

        #endregion
    }
}
