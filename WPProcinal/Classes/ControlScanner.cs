using System;
using System.IO.Ports;
using System.Threading;
using WPProcinal.Models;

namespace WPProcinal.Classes
{
    public class ControlScanner
    {
        public int num = 0;

        #region References
        private SerialPort _BarcodeReader;//Puerto Scanner
        #endregion

        #region CallBacks
        public Action<DataDocument> callbackDocument;//Calback de la lectura de la cedula
        #endregion

        public ControlScanner()
        {
            if (_BarcodeReader == null)
            {
                _BarcodeReader = new SerialPort();
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
                AdminPaypad.SaveErrorControl(ex.Message, "Iniciando el puerto del scanner", EError.Device, ELevelError.Medium);
            }
        }
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
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Scanner_DataReceived", EError.Aplication, ELevelError.Mild);
            }
        }
        #region "Scanner"
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
            }
        }
        #endregion
    }
}
