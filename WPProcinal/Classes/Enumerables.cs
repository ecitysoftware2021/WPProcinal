using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public enum ELogType
    {
        General = 0,
        Error = 1,
        Device = 2
    }
    public enum EError
    {
        Printer = 1,
        Nopapper = 2,
        Device = 3,
        Aplication = 5,
        Api = 6,
        Customer = 7,
        Internet = 8
    }
    public enum ELevelError
    {
        Mild = 3,
        Medium = 2,
        Strong = 1,
    }

    public enum ETransactionProducto
    {
        Ticket = 1,
        Confectionery = 2
    }

    public enum ETransactionType
    {
        Search = 1,
        Deposit = 2,
        Buy = 3,
        Whitdraw = 4
    }

    public enum ETransactionState
    {
        Initital = 1,
        Aproved = 2,
        Canceled = 3,
        WithoutNotifying = 4,
        WithError = 5
    }

    public enum EPaymentType
    {
        Cash = 1,
        Card = 2
    }

    public enum ETypeBuy
    {
        ConfectioneryAndCinema = 0,
        JustConfectionery = 1
    }

    public enum ECodigoMedioPagoScore
    {
        Todos = 0,
        Efectivo = 1,
        Pago_Interno = 2,
        Moneda_Alternativa = 3,
        Tarjeta_Debito_Credi = 6,
        Ventas_Credito = 7,
        Bono_Regalo = 11,
        Tarjeta_Procinal_Ant = 13,
        Diferencias_Negativa = 14,
        Migración_TP = 15,
        BONO_CONFITERIA = 16,
        BONO_BOLETERIA = 17,
        CONV_COMFAMA = 18
    }

}
