﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPProcinal.WCFPcrint {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WCFPcrint.ServiceSoap")]
    public interface ServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/comint", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode comint(double teatro, double pun_ext_l, double secuencia, double can_entradas, double tarifa, string tiene_tarjeta, string correo, string autorizacion_visa, double referencia, double sec_papaya, string monedapago, string fecha_funcion, double total_compra, string med_pago);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/comint", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Xml.XmlNode> comintAsync(double teatro, double pun_ext_l, double secuencia, double can_entradas, double tarifa, string tiene_tarjeta, string correo, string autorizacion_visa, double referencia, double sec_papaya, string monedapago, string fecha_funcion, double total_compra, string med_pago);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceSoapChannel : WPProcinal.WCFPcrint.ServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceSoapClient : System.ServiceModel.ClientBase<WPProcinal.WCFPcrint.ServiceSoap>, WPProcinal.WCFPcrint.ServiceSoap {
        
        public ServiceSoapClient() {
        }
        
        public ServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Xml.XmlNode comint(double teatro, double pun_ext_l, double secuencia, double can_entradas, double tarifa, string tiene_tarjeta, string correo, string autorizacion_visa, double referencia, double sec_papaya, string monedapago, string fecha_funcion, double total_compra, string med_pago) {
            return base.Channel.comint(teatro, pun_ext_l, secuencia, can_entradas, tarifa, tiene_tarjeta, correo, autorizacion_visa, referencia, sec_papaya, monedapago, fecha_funcion, total_compra, med_pago);
        }
        
        public System.Threading.Tasks.Task<System.Xml.XmlNode> comintAsync(double teatro, double pun_ext_l, double secuencia, double can_entradas, double tarifa, string tiene_tarjeta, string correo, string autorizacion_visa, double referencia, double sec_papaya, string monedapago, string fecha_funcion, double total_compra, string med_pago) {
            return base.Channel.comintAsync(teatro, pun_ext_l, secuencia, can_entradas, tarifa, tiene_tarjeta, correo, autorizacion_visa, referencia, sec_papaya, monedapago, fecha_funcion, total_compra, med_pago);
        }
    }
}
