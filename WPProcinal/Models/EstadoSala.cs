using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName = "ESTADO_SALA")]
    public class EstadoSala
    {
        [XmlElement(ElementName = "FILA")]
        public List<Fila> FILA { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "FILA")]
    public class Fila
    {
        [XmlElement(ElementName = "Max_Columna")]
        public string Max_Columna { get; set; }
        [XmlElement(ElementName = "Max_Fila")]
        public string Max_Fila { get; set; }
        [XmlElement(ElementName = "Rel_Fila")]
        public string Rel_Fila { get; set; }
        [XmlElement(ElementName = "Des_Fila")]
        public string Des_Fila { get; set; }
        [XmlElement(ElementName = "Num_Col")]
        public string Num_Col { get; set; }
    }
   
}
