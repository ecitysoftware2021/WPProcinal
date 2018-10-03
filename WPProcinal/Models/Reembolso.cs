using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "TRANSACCION_COMPRA")]
    class Reembolso
    {
        public string Respuesta { get; set; }
        [XmlElement(ElementName = "Respuesta")]
        [XmlElement(ElementName = "Error_en_proceso")]
        public string Error_en_proceso { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchemainstance")]
        public string NoNamespaceSchemaLocation { get; set; }
    }

}