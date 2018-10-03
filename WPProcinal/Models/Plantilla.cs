using System.Collections.Generic;
using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "PLANILLA")]
    public class Plantilla
    {
        [XmlElement(ElementName = "tarifas")]
        public Tarifas2 Tarifas { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
    }

    

    [XmlRoot(ElementName = "tarifas")]
    public class Tarifas2
    {
        [XmlElement(ElementName = "codigo")]
        public Codigo Codigo { get; set; }       

        [XmlElement(ElementName = "descripcion")]
        public string Descripcion { get; set; }

        [XmlElement(ElementName = "valor")]
        public string Valor { get; set; }

        [XmlElement(ElementName = "tipo")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "documento")]
        public string Documento { get; set; }
    }

    [XmlRoot(ElementName = "codigo")]
    public class Codigo
    {
        [XmlAttribute(AttributeName = "Error_en_Sistema")]
        public string Error_en_Sistema { get; set; }
    }

    [XmlRoot(ElementName = "PLANILLA")]
    public class Plantilla2
    {
        [XmlElement(ElementName = "tarifas")]
        public List<Tarifas3> Tarifas { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
    }

    [XmlRoot(ElementName = "tarifas")]
    public class Tarifas3
    {       
        [XmlElement(ElementName = "codigo")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "descripcion")]
        public string Descripcion { get; set; }

        [XmlElement(ElementName = "valor")]
        public string Valor { get; set; }

        [XmlElement(ElementName = "tipo")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "documento")]
        public string Documento { get; set; }
    }
}
