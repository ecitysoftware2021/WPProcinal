﻿using Newtonsoft.Json;
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

                    response = response.Remove(0, response.IndexOf("PubDSK_1") + 6);

                    string documentData = string.Empty;
                    string fullName = string.Empty;

                    if (response.IndexOf("0M") > 0)
                    {
                        dataReader.Gender = "Masculino";
                        dataReader.Date = response.Substring(response.IndexOf("0M") + 2, 8);
                        documentData = response.Substring(0, response.IndexOf("0M"));
                    }
                    else
                    {
                        documentData = response.Substring(0, response.IndexOf("0F"));
                        dataReader.Date = response.Substring(response.IndexOf("0F") + 2, 8);
                        dataReader.Gender = "Femenino";
                    }

                    foreach (var item in documentData.ToCharArray())
                    {
                        if (char.IsLetter(item))
                        {
                            fullName += item;
                        }
                        else if (char.IsWhiteSpace(item) || item.Equals('\0'))
                        {
                            fullName += " ";
                        }
                        else if (char.IsNumber(item))
                        {
                            dataReader.Document += item;
                        }
                    }
                    fullName = (fullName.TrimStart()).TrimEnd();

                    dataReader.Document = dataReader.Document.Substring(dataReader.Document.Length - 10, 10);

                    foreach (var item in fullName.Split(' '))
                    {
                        if (!string.IsNullOrEmpty(item) && item.Length > 1)
                        {
                            dataReader.FullName += string.Concat(item, " ");
                        }
                    }

                    dataReader.FirstName = dataReader.FullName.Split(' ')[2] ?? string.Empty;
                    dataReader.SecondName = dataReader.FullName.Split(' ')[3] ?? string.Empty;
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
                    callbackError?.Invoke("no se logro realizar la lectura");
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
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
                LogService.SaveRequestResponse("ControlScanner>ClosePortScanner", JsonConvert.SerializeObject(ex), 1);
            }
        }
        #endregion
    }
}
