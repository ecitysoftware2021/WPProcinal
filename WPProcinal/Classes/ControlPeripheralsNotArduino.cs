using System;
using WPProcinal.Classes;

namespace WPProcinal
{
    public class ControlPeripheralsNotArduino
    {
        #region Callbacks
        public static Action<ErrorCodes> callbackStatusPrinter;//Calback printer status
        #endregion

        public ControlPeripheralsNotArduino()
        {

        }

        #region ProccessResponse
        /// <summary>
        /// Método que procesa la respuesta de la impresora
        /// </summary>
        /// <param name="message">respuesta del puerto de los monederos</param>
        public void ProcessResponsePrinter(int code)
        {

            string message = string.Empty;
            ErrorCodes errorCodes = new ErrorCodes();
            switch (code)
            {
                case 0:
                    errorCodes.STATUS = "OK";
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_OK_MESSAGE;
                    break;
                case 1:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = string.Concat(ErrorCodes.PRINTER_DISCONNECT_MESSAGE, Utilities.GetConfiguration("PortPrinter"));
                    break;
                case 2:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_LIBRARY_DOES_NOT_MATCH_MESSAGE;
                    break;
                case 3:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_HEADER_OPEN_MESSAGE;
                    break;
                case 4:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_KHIFE_NO_RESET_MESSAGE;
                    break;
                case 5:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_OVERHEATING_MESSAGE;
                    break;
                case 6:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_BLACK_LABEL_MESSAGE;
                    break;
                case 7:
                    errorCodes.STATUS = "ERROR";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_NO_PAPER_MESSAGE;
                    break;
                case 8:
                    errorCodes.STATUS = "ALERT";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = ErrorCodes.PRINTER_LOW_PAPER_MESSAGE;
                    break;
                default:
                    errorCodes.STATUS = "OK";
                    errorCodes.ERROR_CODE = code.ToString();
                    errorCodes.ERROR_MESSAGE = string.Empty;
                    break;
            }
            callbackStatusPrinter?.Invoke(errorCodes);
        }
        #endregion


    }
}
