using Newtonsoft.Json;
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

        public Action<string> callbackError;
        public Action<DataDocument> callbackDocument;
        public bool Isbusy { get; set; }
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
                    _BarcodeReader.DataReceived += new SerialDataReceivedEventHandler(_readerBarcode_DataReceived);
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
        private void _readerBarcode_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (Isbusy == false)
                {
                    Isbusy = true;
                    //Thread.Sleep(1000);
                    string response = _BarcodeReader.ReadExisting();
                    if (!string.IsNullOrEmpty(response))
                    {
                        ProcessResponseBarcode(response);
                    }
                    else
                    {
                        Isbusy = false;
                    }
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
                Isbusy = false;
            }
        }

        private void ProcessResponseBarcode(string response)
        {
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var dataReader = new DataDocument();

                    //response = response.Remove(0, response.IndexOf("PubDSK_1") + 6);
                    response = response.Remove(0, response.IndexOf("PubDSK_1") + 1);
                    var dataDocument = response.Split('\t');
                    
                    if (dataDocument != null && Convert.ToInt32(dataDocument.Length) == 8)
                    {
                        var document = dataDocument[0];
                        var fName = $"{dataDocument[1] + " " + dataDocument[2] + " " + dataDocument[3] + " " + dataDocument[4]}";
                        //var uuu = dataDocument[4];

                        string documentData = string.Empty;
                        string fullName = string.Empty;

                        string fechacedula = dataDocument[6].ToString();
                        var date = fechacedula.Substring(4, 4) + fechacedula.Substring(2, 2) + fechacedula.Substring(0, 2);

                        if (dataDocument[5].Contains("M"))
                        {
                            dataReader.Gender = "Masculino";
                            
                                                                                                                                                       
                            dataReader.Date = date;
                            documentData = response.Substring(0, response.IndexOf("\tM"));
                        }
                        else
                        {
                            documentData = response.Substring(0, response.IndexOf("F"));
                            dataReader.Date = date;
                            dataReader.Gender = "Femenino";
                        }

                        fullName = (fName.TrimStart()).TrimEnd();
                        dataReader.Document = document;
                        dataReader.FullName = fName;

                        dataReader.FirstName = dataReader.FullName.Split(' ')[2] ?? string.Empty;
                        dataReader.SecondName = dataDocument[4] ?? string.Empty;
                        dataReader.LastName = dataReader.FullName.Split(' ')[0] ?? string.Empty;
                        dataReader.SecondLastName = dataReader.FullName.Split(' ')[1] ?? string.Empty;

                        if (!string.IsNullOrEmpty(dataReader.Document) && !string.IsNullOrEmpty(dataReader.FullName))
                        {
                            callbackDocument?.Invoke(dataReader);
                         
                        }
                        else
                        {
                            callbackError?.Invoke("Datos de lectura imcompletos");
                        
                        }
                    }
                    else
                    {
                        callbackError?.Invoke("tipo de documento no valido...");
                    }
                }
                else
                {
                    callbackError?.Invoke("no se logro realizar la lectura...");
                }
                
            }
            catch (Exception ex)
            {
                Isbusy = false;
                callbackError?.Invoke(ex.ToString());
            }
            Isbusy = false;

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
                LogService.SaveRequestResponse("ControlScanner>ClosePortScanner", JsonConvert.SerializeObject(ex), 1);
            }
        }
        #endregion
    }
}
