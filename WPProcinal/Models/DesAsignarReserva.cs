using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "DESASIGNAR_RESERVA")]
    public class DesAsignarReserva
    {
        [XmlElement(ElementName = "Respuesta")]
        public string Respuesta { get; set; }
        [XmlElement(ElementName = "Error_en_proceso")]
        public string Error_en_proceso { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
    }

}
