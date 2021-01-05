using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Classes
{
    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int TYPE_TRANSACTION_ID { get; set; }
        public DateTime DATE_BEGIN { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal INCOME_AMOUNT { get; set; }
        public decimal RETURN_AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public int PAYMENT_TYPE_ID { get; set; }
        public int PAYER_ID { get; set; }
        public string TRANSACTION_REFERENCE { get; set; }
        public int STATE_TRANSACTION_ID { get; set; }
        public PAYER payer { get; set; }
        public List<TRANSACTION_DESCRIPTION> TRANSACTION_DESCRIPTION = new List<TRANSACTION_DESCRIPTION>();
    }

    public class TRANSACTION_DESCRIPTION
    {
        public int TRANSACTION_DESCRIPTION_ID { get; set; }
        public Nullable<int> TRANSACTION_ID { get; set; }
        public int TRANSACTION_PRODUCT_ID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public string EXTRA_DATA { get; set; }
        public Nullable<bool> STATE { get; set; }
    }

    public class RequestCardInformation
    {
        public string Last_number { get; set; }

        public int Quotas { get; set; }

        public string Franchise { get; set; }

        public int Card_Type { get; set; }

        public int Transaction_id { get; set; }
        public string Autorization_code { get; set; }
        public string Receipt_number { get; set; }
        public string RRN { get; set; }
    }

    public class PAYER
    {
        public int PAYER_ID { get; set; }
        public string IDENTIFICATION { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string NAME { get; set; }
        public string TYPE_PAYER { get; set; }
        public string TYPE_IDENTIFICATION { get; set; }
        public string BLOOD_TYPE { get; set; }
        public string GENDER { get; set; }
        public string NATIONALITY { get; set; }
        public string BIRTHDAY { get; set; }
    }
    public class DataTransaction
    {
        public DataTransaction()
        {
            DataTransactionEcity = new Transaction();
            DataFunction = new FunctionInformation();
            SelectedTypeSeats = new List<ChairsInformation>();
            dataDocument = new DataDocument();
            TipoSala = string.Empty;
            MovieFormat = string.Empty;
            ImageSelected = null;
            eTypeBuy = new ETypeBuy();
            FechaSeleccionada = DateTime.Today;
            dataUser = new SCOLOGResponse();
            ValorPagarScore = 0;
            PayVal = 0;
            MedioPago = new EPaymentType();
            PagoInterno = 0;
            PLACA = string.Empty;
            TIPOAUTO = string.Empty;
            Secuencia = string.Empty;
            ValueDelivery = 0;
        }
        /// <summary>
        /// Objeto para almacenar la información de la transacción para ecity
        /// </summary>
        public Transaction DataTransactionEcity;
        /// <summary>
        /// Objeto para almacenar la información de la pelicula y hora seleccionada
        /// </summary>
        public FunctionInformation DataFunction;
        /// <summary>
        /// Lista de sillas seleccionadas
        /// </summary>
        public List<ChairsInformation> SelectedTypeSeats;
        /// <summary>
        /// Objeto para almacenar la información de la cédula leída
        /// </summary>
        public DataDocument dataDocument;
        /// <summary>
        /// Esta variable solo se usa para pintar el tipo de sala en la boleta
        /// </summary>
        public string TipoSala;
        /// <summary>
        /// Formato de la pelicula, se muestra en todas las pantallas
        /// </summary>
        public string MovieFormat;

        /// <summary>
        /// Poster de la peícula seleccionada, se usa para mostrar en la pantalla de los horarios
        /// </summary>
        public ImageSource ImageSelected;
        /// <summary>
        /// Variable para almacenar el tipo de compra, si es solo confiteria o boleta mas confitería
        /// </summary>
        public ETypeBuy eTypeBuy;
        /// <summary>
        /// Variable global para conocer la fecha en la que el usuario quiere ver la pelicula
        /// </summary>
        public DateTime FechaSeleccionada;
        /// <summary>
        /// Objeto que almacena la informacion del usuario cinefan
        /// </summary>
        public SCOLOGResponse dataUser;
        /// <summary>
        /// Variable para almacenar el valor a pagar original, ya que el que se muestra al usuario es redondeado
        /// </summary>
        public decimal ValorPagarScore;
        /// <summary>
        /// Valor a pagar que se le muestra al usuario (el redondeado)
        /// </summary>
        public decimal PayVal;
        /// <summary>
        /// Almacena la opcion seleccionada por el usuario, si eligio pago en efectivo o con tarjeta
        /// </summary>
        public EPaymentType MedioPago;
        /// <summary>
        /// Valor que corresponde al total ahorrado del cliente para pago por redención
        /// </summary>
        public decimal PagoInterno { get; set; }
        /// <summary>
        /// Variable para almacenar la placa del vehículo en autocine
        /// </summary>
        public string PLACA;
        /// <summary>
        /// Variable que almacena el tipo de vehículo para el autocine
        /// </summary>
        public string TIPOAUTO;
        /// <summary>
        /// Numero de secuencia de cada compra en score, se utiliza para la notificación del pago
        /// </summary>
        public string Secuencia;
        /// <summary>
        /// Valor devuelto al usuario
        /// </summary>
        public long ValueDelivery;
    }
}
