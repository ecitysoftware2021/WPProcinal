﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPProcinal.WCFTarifas {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WCFTarifas.tarifasSoap")]
    public interface tarifasSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/commit", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode commit(double tea_ext_l, string fec_ext_l, double pel_ext_l, double sal_ext_l, double ini_ext_l);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/commit", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Xml.XmlNode> commitAsync(double tea_ext_l, string fec_ext_l, double pel_ext_l, double sal_ext_l, double ini_ext_l);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface tarifasSoapChannel : WPProcinal.WCFTarifas.tarifasSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class tarifasSoapClient : System.ServiceModel.ClientBase<WPProcinal.WCFTarifas.tarifasSoap>, WPProcinal.WCFTarifas.tarifasSoap {
        
        public tarifasSoapClient() {
        }
        
        public tarifasSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public tarifasSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public tarifasSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public tarifasSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Xml.XmlNode commit(double tea_ext_l, string fec_ext_l, double pel_ext_l, double sal_ext_l, double ini_ext_l) {
            return base.Channel.commit(tea_ext_l, fec_ext_l, pel_ext_l, sal_ext_l, ini_ext_l);
        }
        
        public System.Threading.Tasks.Task<System.Xml.XmlNode> commitAsync(double tea_ext_l, string fec_ext_l, double pel_ext_l, double sal_ext_l, double ini_ext_l) {
            return base.Channel.commitAsync(tea_ext_l, fec_ext_l, pel_ext_l, sal_ext_l, ini_ext_l);
        }
    }
}
