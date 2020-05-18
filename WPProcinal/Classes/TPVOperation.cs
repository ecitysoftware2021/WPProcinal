using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trx;

namespace WPProcinal.Classes
{
    public class TPVOperation
    {

        #region Sección Datáfono


        public static int Quotas { get; set; }

        TEFTransactionManager transactionManager;

        public static Action<string> CallBackRespuesta;
        public TPVOperation()
        {
            transactionManager = new TEFTransactionManager();
        }

        public string EnviarPeticion(string data)
        {
            try
            {
                return transactionManager.getTEFAuthorization(data);
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("Enviando peticion al datáfono", ex.Message, 1);
                return ex.Message;
            }

        }

        public string EnviarPeticionEspera()
        {
            return transactionManager.getTEFAuthorization();
        }

        public string CalculateLRC(string s)
        {
            int checksum = 0;
            foreach (Char c in GetStringFromHex(s))
            {
                checksum = checksum ^ Convert.ToByte(c);
            }
            string nuevaCadena = string.Concat("[", s, checksum.ToString("X2"));
            return nuevaCadena;
        }

        private string GetStringFromHex(string s2)
        {
            string result = string.Empty;
            var result2 = string.Join("", s2.Select(c => ((int)c).ToString("X2")));
            for (int i = 0; i < result2.Length; i = i + 2)
            {
                result += Convert.ToChar(int.Parse(result2.Substring(i, 2),
               System.Globalization.NumberStyles.HexNumber));
            }
            return result;
        }
        #endregion
    }
}
