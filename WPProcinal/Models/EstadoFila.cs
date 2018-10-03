using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "ESTADO_FILA")]
    public class EstadoFila
    {
        [XmlElement(ElementName = "Max_Columna")]
        public string Max_Columna { get; set; }
        [XmlElement(ElementName = "Max_Fila")]
        public string Max_Fila { get; set; }
        [XmlElement(ElementName = "Rel_Fila")]
        public string Rel_Fila { get; set; }
        [XmlElement(ElementName = "Des_Fila")]
        public string Des_Fila { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
