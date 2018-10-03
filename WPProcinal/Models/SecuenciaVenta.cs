using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "Secuencia_venta")]
    public class SecuenciaVenta
    {
        [XmlElement(ElementName = "Secuencia")]
        public string Secuencia { get; set; }

        [XmlElement(ElementName = "Error")]
        public string Error { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
    }

}
