using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPProcinal.Classes.Peripherals
{
    public class ControlPeripherals
    {
        #region References

        #region "Timer"
        private TimerTiempo timer;
        #endregion

        #region SerialPorts
        private SerialPort _serialPort;//Puerto billeteros
        #endregion

        #region ClassAceptor
        private MEIAcceptorControl AcceptorControl;
        #endregion

        #region CommandsPorts
        private string _StartPeripherals = "OR:START";//Iniciar los billeteros

        private string _AceptanceBillOn = "OR:ON:AP";//Operar billetero Aceptance

        private string _DispenserBillOn = "OR:ON:DP";//Operar billetero Dispenser

        private string _AceptanceBillOFF = "OR:OFF:AP";//Cerrar billetero Aceptance

        private string _DispenserBillOFF = "OR:OFF:DP";//Cerrar billetero Dispenser

        private string _AceptanceCoinOn = "OR:ON:MA";//Operar Monedero Aceptance

        private string _DispenserCoinOn = "OR:ON:MD:";//Operar Monedero Dispenser

        private string _AceptanceCoinOff = "OR:OFF:MA";//Cerrar Monedero Aceptance
        #endregion

        #region Callbacks
        public Action<Tuple<decimal, string>> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<Tuple<decimal, string>> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackIncompleteOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<string> callbackError;//Calback de error

        public Action<string> callbackLog;//Calback de error

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje

        public Action<string, string, int> CallBackSaveRequestResponse;

        #endregion

        #region EvaluationValues
        private int _dividerBills;
        private int _dividerCoins;
        #endregion

        #region Variables

        private decimal payValue;//Valor a pagar

        private List<Tuple<string, int>> denominationsDispenser;

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        private decimal dispenserValue;//Valor a dispensar

        private bool stateError;

        private int typeDispend;

        private bool token = false;

        private static string TOKEN;//Llabe que retorna el dispenser
        #endregion

        #endregion

        #region LoadMethods
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControlPeripherals(string portNameBills,
            string denominatios, int dividerBills = 1, int dividerCoins = 100)
        {
            try
            {
                if (_serialPort == null && !string.IsNullOrEmpty(portNameBills))
                {
                    _serialPort = new SerialPort();
                    InitPortBills(portNameBills);
                }

                if (AcceptorControl == null)
                {
                    AcceptorControl = new MEIAcceptorControl();
                    ListenerAceptance();
                }

                if (!string.IsNullOrEmpty(denominatios))
                {
                    this._dividerBills = dividerBills;
                    this._dividerCoins = dividerCoins;

                    ConfigurateDispenser(denominatios);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(string.Concat("Error (ControlPeripherals), Iniciando los perifericos", ex));
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            try
            {
                if (!SendMessageBills(_StartPeripherals))
                {
                    callbackError?.Invoke(string.Concat("Error (Start), Iniciando los perifericos", "No se pudo iniciar"));
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(string.Concat("Error (Start), Iniciando aceptacion", ex));
            }
        }

        private void ListenerAceptance()
        {
            AcceptorControl.callbackBillAccepted = Value =>
            {
                var responseAP = Convert.ToDecimal(Value);

                CallBackSaveRequestResponse?.Invoke("Respuesta del billetero AP", responseAP.ToString(), 1);

                if (responseAP > 0)
                {
                    enterValue += responseAP * _dividerBills;
                    callbackValueIn?.Invoke(Tuple.Create(responseAP * _dividerBills, "AP"));
                }

                ValidateEnterValue();
            };

            AcceptorControl.callbackMeiMessages = EMessage =>
            {
                var responseAPMS = AcceptorControl.MeiMessagesHomologate[EMessage.ToString()] + Environment.NewLine;

                CallBackSaveRequestResponse?.Invoke("Respuesta del billetero AP-MS", responseAPMS.ToString(), 1);

                var a = "Aceptador Conectado";

                if (token)
                {
                    token = false;
                    callbackToken?.Invoke(true);
                }

            };
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills(string portName)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.PortName = portName;
                    _serialPort.ReadTimeout = 3000;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.BaudRate = 57600;
                    _serialPort.DtrEnable = true;
                    _serialPort.RtsEnable = true;
                    _serialPort.Open();
                }
                else
                {
                    callbackError?.Invoke("El billetero esta cerrado o el puerto esta ocupado. " + portName);
                }

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("El puerto de los perisfericos esta ocupado. " + portName + " " + ex.ToString());
            }
        }
        #endregion

        #region SendMessage
        /// <summary>
        /// Método para enviar orden al puerto de los billeteros
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private bool SendMessageBills(string message)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    CallBackSaveRequestResponse?.Invoke("Mensaje a los billeteros", message, 1);

                    Thread.Sleep(2000);
                    _serialPort.Write(message);

                    return true;
                }
                else
                {
                    CallBackSaveRequestResponse?.Invoke("El puerto de los perisfericos esta cerrado.", message, 1);
                    callbackError?.Invoke("El puerto de los perisfericos esta cerrado.");

                    return false;
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (SendMessageBills), ha ocurrido una exepcion " + ex);
            }
            return false;
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
                string response = _serialPort.ReadLine();

                CallBackSaveRequestResponse?.Invoke("Respuesta del billetero", response, 1);

                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseBills(response.Replace("\r", string.Empty));
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (_serialPortBillsDataReceived), ha ocurrido una exepcion " + ex);
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
            string[] response = message.Split(':');
            switch (response[0])
            {
                case "RC":
                    ProcessRC(response);
                    break;
                case "ER":
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
        #endregion

        #region ProcessResponseCases
        /// <summary>
        /// Respuesta para el caso de Recepción de un mensaje enviado
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessRC(string[] response)
        {
            if (response[1] == "OK")
            {
                switch (response[2])
                {
                    case "AP":

                        break;
                    case "DP":
                        if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                        {
                            TOKEN = response[4].Replace("\r", string.Empty);

                            token = true;

                            AcceptorControl.OpenAcceptor("COM12");
                        }
                        break;
                    case "MD":
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Respuesta para el caso de error
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessER(string[] response)
        {
            if (response[1] == "DP" || response[1] == "MD")
            {
                if (response[2] == "Abnormal Near End sensor of the cassette\r")
                {
                    stateError = false;
                    callbackError?.Invoke(JsonConvert.SerializeObject(response));
                }
                else
                {
                    stateError = true;
                    callbackError?.Invoke(JsonConvert.SerializeObject(response));
                }
            }
            if (response[1] == "AP")
            {
                stateError = true;
                callbackError?.Invoke(JsonConvert.SerializeObject(response));
            }
            else if (response[1] == "FATAL")
            {
                callbackError?.Invoke(JsonConvert.SerializeObject(response));
            }
        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            if (response[1] == "DP")
            {
                deliveryValue += decimal.Parse(response[2]) * _dividerBills;
                callbackValueOut?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerBills, response[1]));
            }
            else if (response[1] == "MD")
            {
                deliveryValue += decimal.Parse(response[2]) * _dividerCoins;
                callbackValueOut?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerCoins, response[1]));
            }
            else
            {
                if (response[1] == "AP")
                {
                    enterValue += decimal.Parse(response[2]) * _dividerBills;
                    callbackValueIn?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerBills, response[1]));
                }
                else if (response[1] == "MA")
                {
                    enterValue += decimal.Parse(response[2]);
                    callbackValueIn?.Invoke(Tuple.Create(decimal.Parse(response[2]), response[1]));
                }
                ValidateEnterValue();
            }
        }

        public void ClearValues()
        {
            deliveryValue = 0;
            enterValue = 0;
            deliveryVal = 0;

            this.callbackTotalIn = null;
            this.callbackTotalOut = null;
            this.callbackValueIn = null;
            this.callbackValueOut = null;
            this.callbackOut = null;
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
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
                stateError = false;
                dispenserValue = valueDispenser;
                ActivateTimer();
                ValidateValueDispenser();
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (StartDispenser), ha ocurrido una exepcion " + ex);
            }
        }

        private void ValidateValueDispenser()
        {
            try
            {
                if (dispenserValue > 0 && denominationsDispenser.Count > 0)
                {
                    decimal amountCoins = dispenserValue;

                    decimal amountBills = 0;

                    foreach (var denomination in denominationsDispenser)
                    {
                        if (denomination.Item1.Equals("DP"))
                        {
                            var amount = ((int)(amountCoins / denomination.Item2) * denomination.Item2);

                            amountBills += amount;
                            amountCoins -= amount;
                        }
                    }

                    if (amountBills > 0 && amountCoins > 0)
                    {
                        typeDispend = 2;
                    }
                    else
                    {
                        typeDispend = 1;
                    }

                    //if (amountBills > 0)
                    //{
                    //DispenserMoney(((int)(amountBills / _dividerBills)).ToString());
                    DispenserMoney(dispenserValue.ToString());
                    //}

                    //if (amountCoins > 0)
                    //{
                    //    SendMessageCoins(_DispenserCoinOn + ((int)(amountCoins / _dividerCoins)).ToString());
                    //}
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (ValidateValueDispenser), ha ocurrido una exepcion " + ex);
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser(string values)
        {
            try
            {
                if (!string.IsNullOrEmpty(values))
                {
                    denominationsDispenser = new List<Tuple<string, int>>();
                    var denominations = values.Split('-');

                    if (denominations.Length > 0)
                    {
                        foreach (var denomination in denominations)
                        {
                            var value = denomination.Split(':');
                            if (value.Length == 2)
                            {
                                denominationsDispenser.Add(Tuple.Create(value[0], int.Parse(value[1])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (ConfigurateDispenser), ha ocurrido una exepcion " + ex);
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
                callbackError?.Invoke("Error (DispenserMoney), ha ocurrido una exepcion " + ex);
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
                AcceptorControl.InitAcceptance();
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (StartAceptance), ha ocurrido una exepcion " + ex);
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
                //StopAceptance();
                enterValue = 0;
                callbackTotalIn?.Invoke(enterVal);
            }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            AcceptorControl.StopAcceptance();
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
                    callbackLog?.Invoke(string.Concat(data.Replace("\r", string.Empty), "!"));
                    typeDispend--;
                }

                if (!stateError)
                {
                    if (dispenserValue == deliveryVal)
                    {
                        if (typeDispend == 0)
                        {
                            if (timer != null)
                            {
                                timer.CallBackClose = null;
                                timer.CallBackStop?.Invoke(1);
                            }

                            callbackTotalOut?.Invoke(deliveryVal);
                        }
                    }
                }
                else
                {
                    if (typeDispend == 0)
                    {
                        if (timer != null)
                        {
                            timer.CallBackClose = null;
                            timer.CallBackStop?.Invoke(1);
                        }

                        callbackOut?.Invoke(deliveryVal);
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (ConfigDataDispenser), ha ocurrido una exepcion " + ex);
            }
        }
        #endregion

        #region "TimerInactividad"
        private void ActivateTimer()
        {
            try
            {
                string timerInactividad = Utilities.dataPaypad.PaypadConfiguration.inactivitY_TIMER;
                timer = new TimerTiempo(timerInactividad);
                timer.CallBackClose = response =>
                {
                    try
                    {
                        timer.CallBackClose = null;
                        callbackOut?.Invoke(deliveryVal);
                    }
                    catch (Exception ex)
                    {
                        callbackError?.Invoke("ActivateTimer "+ ex.ToString());
                    }
                };
            }
            catch (Exception ex)
            {
                callbackError?.Invoke("Error (ActivateTimer), ha ocurrido una exepcion en ActivateTimer " + ex);
            }
        }
        #endregion
    }
}
