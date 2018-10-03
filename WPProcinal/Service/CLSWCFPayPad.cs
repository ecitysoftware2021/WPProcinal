using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.WCFPayPad;

namespace WPProcinal.Service
{
    public class CLSWCFPayPad
    {
        ServicePayPadClient servicePayPadClient = new ServicePayPadClient();

        public int InsertarTransaccion(CLSTransaction objTransaction)
        {
            return servicePayPadClient.InsertarTransaccion(objTransaction);
        }

        public bool InsertarDetalleTransaccion(int IDTransaccion, CLSEstadoEstadoDetalle Estado, decimal Valor)
        {
            return servicePayPadClient.InsertarDetalleTransaccion(IDTransaccion, Estado, Valor);
        }
        public bool InsertarAuditoria(int IDTransaccion, CLSEstadoEstadoAuditoria Estado)
        {
            return servicePayPadClient.InsertarAuditoria(IDTransaccion, Estado);
        }
        public string ConsultaEstadoFactura(int IDCorresponsal, int IDTramite, string Referencia)
        {
            return servicePayPadClient.ConsultaEstadoFactura(IDCorresponsal, IDTramite, Referencia);
        }
        public bool InsertException(int IDCorresponsal, string Exception)
        {
            return servicePayPadClient.InsertException(IDCorresponsal, Exception);
        }

        public void UpdateTransactionCancel(int item)
        {
            servicePayPadClient.ActualizarEstadoTransaccion(item, CLSEstadoEstadoTransaction.Cancelada);
        }

        public void ApproveTransaction(int item)
        {
            servicePayPadClient.ActualizarEstadoTransaccion(item, CLSEstadoEstadoTransaction.Aprobada);
        }

        public int BilletesBaul(int idCorrespo)
        {
            return servicePayPadClient.BilletesBaul(idCorrespo);
        }

        internal void InsertarControlBillete(int idCorrespo, int iDDenominacion, bool v1, bool v2)
        {
            servicePayPadClient.InsertarControlBillete(idCorrespo, iDDenominacion, v1, v2);
        }
    }
}
