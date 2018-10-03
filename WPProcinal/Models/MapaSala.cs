using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "MAPA_SALA")]
    public class MapaSala
    {
        [XmlElement(ElementName = "FT")]
        public string FT { get; set; }
        [XmlElement(ElementName = "FR")]
        public string FR { get; set; }
        [XmlElement(ElementName = "CT")]
        public string CT { get; set; }
        [XmlElement(ElementName = "CR")]
        public string CR { get; set; }
        [XmlElement(ElementName = "TS")]
        public string TS { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation",
            Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
