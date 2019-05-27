using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPProcinal.Classes;

namespace WPProcinal.Models
{
    [XmlRoot(ElementName="peliculas")]
    public class Data {
		[XmlAttribute(AttributeName="TituloOriginal")]
		public string TituloOriginal { get; set; }
		[XmlAttribute(AttributeName="censura")]
		public string Censura { get; set; }
		[XmlAttribute(AttributeName="idioma")]
		public string Idioma { get; set; }
		[XmlAttribute(AttributeName="genero")]
		public string Genero { get; set; }
		[XmlAttribute(AttributeName="pais")]
		public string Pais { get; set; }
		[XmlAttribute(AttributeName="duracion")]
		public string Duracion { get; set; }
		[XmlAttribute(AttributeName="formato")]
		public string Formato { get; set; }
		[XmlAttribute(AttributeName="thiller")]
		public string Thiller { get; set; }
		[XmlAttribute(AttributeName="img1")]
		public string Img1 { get; set; }
		[XmlAttribute(AttributeName="img2")]
		public string Img2 { get; set; }
		[XmlAttribute(AttributeName="img3")]
		public string Img3 { get; set; }
		[XmlAttribute(AttributeName="img4")]
		public string Img4 { get; set; }
		[XmlAttribute(AttributeName="img5")]
		public string Img5 { get; set; }
	}

	[XmlRoot(ElementName="dia")]
	public class Dia {
		[XmlAttribute(AttributeName="fecha")]
		public string Fecha { get; set; }
		[XmlAttribute(AttributeName="univ")]
		public string Univ { get; set; }
	}

	[XmlRoot(ElementName="DiasDisponiblesTodosCinemas")]
	public class DiasDisponiblesTodosCinemas {
		[XmlElement(ElementName="dia")]
		public List<Dia> Dia { get; set; }
	}

	[XmlRoot(ElementName="DiasDisponiblesCinema")]
	public class DiasDisponiblesCinema {
		[XmlElement(ElementName="dia")]
		public List<Dia> Dia { get; set; }
	}

	[XmlRoot(ElementName="hora")]
	public class Hora {
		[XmlAttribute(AttributeName="idFuncion")]
		public string IdFuncion { get; set; }
		[XmlAttribute(AttributeName="horario")]
		public string Horario { get; set; }
		[XmlAttribute(AttributeName="reservas")]
		public string Reservas { get; set; }
		[XmlAttribute(AttributeName="militar")]
		public string Militar { get; set; }
	}

    public class HoraTMP
    {        
        public string IdFuncion { get; set; }        
        public string Horario { get; set; }        
        public string Reservas { get; set; }        
        public int Militar { get; set; }
        public string TipoZona { get; set; }
        public Schedule DatosPelicula { get; set; }
    }

    [XmlRoot(ElementName="GeneralDia")]
	public class GeneralDia {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="PreferencialDia")]
	public class PreferencialDia {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="TP-General")]
	public class TPGeneral {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="TP-Preferencial")]
	public class TPPreferencial {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="TP-General-OMP")]
	public class TPGeneralOMP {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="TP-Preferencial-OMP")]
	public class TPPreferencialOMP {
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="Descripcion")]
		public string Descripcion { get; set; }
		[XmlAttribute(AttributeName="Valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName="Tarifas")]
	public class Tarifas {
		[XmlElement(ElementName="GeneralDia")]
		public GeneralDia GeneralDia { get; set; }
		[XmlElement(ElementName="PreferencialDia")]
		public PreferencialDia PreferencialDia { get; set; }
		[XmlElement(ElementName="TP-General")]
		public TPGeneral TPGeneral { get; set; }
		[XmlElement(ElementName="TP-Preferencial")]
		public TPPreferencial TPPreferencial { get; set; }
		[XmlElement(ElementName="TP-General-OMP")]
		public TPGeneralOMP TPGeneralOMP { get; set; }
		[XmlElement(ElementName="TP-Preferencial-OMP")]
		public TPPreferencialOMP TPPreferencialOMP { get; set; }
	}

	[XmlRoot(ElementName="Fecha")]
	public class Fecha {
		[XmlElement(ElementName="hora")]
		public List<Hora> Hora { get; set; }
		[XmlElement(ElementName="TipoZona")]
		public List<string> TipoZona { get; set; }
		[XmlElement(ElementName="Tarifas")]
		public List<Tarifas> Tarifas { get; set; }
		[XmlAttribute(AttributeName="dia")]
		public string Dia { get; set; }
		[XmlAttribute(AttributeName="univ")]
		public string Univ { get; set; }
	}

	[XmlRoot(ElementName="sala")]
	public class Sala {
		[XmlElement(ElementName="Fecha")]
		public List<Fecha> Fecha { get; set; }
		[XmlAttribute(AttributeName="numeroSala")]
		public string NumeroSala { get; set; }
		[XmlAttribute(AttributeName="TipoSala")]
		public string TipoSala { get; set; }
	}

	[XmlRoot(ElementName="salas")]
	public class Salas {
		[XmlElement(ElementName="sala")]
		public List<Sala> Sala { get; set; }
	}

	[XmlRoot(ElementName="cinema")]
	public class Cinema {
		[XmlElement(ElementName="nombreCinema")]
		public string NombreCinema { get; set; }
		[XmlElement(ElementName="DiasDisponiblesCinema")]
		public DiasDisponiblesCinema DiasDisponiblesCinema { get; set; }
		[XmlElement(ElementName="salas")]
		public Salas Salas { get; set; }
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName="cinemas")]
	public class Cinemas {
		[XmlElement(ElementName="cinema")]
		public List<Cinema> Cinema { get; set; }
	}

	[XmlRoot(ElementName="pelicula")]
	public class Pelicula {
		[XmlElement(ElementName="sinopsis")]
		public string Sinopsis { get; set; }
		[XmlElement(ElementName="data")]
		public Data Data { get; set; }
		[XmlElement(ElementName="DiasDisponiblesTodosCinemas")]
		public DiasDisponiblesTodosCinemas DiasDisponiblesTodosCinemas { get; set; }
		[XmlElement(ElementName="cinemas")]
		public Cinemas Cinemas { get; set; }
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="nombre")]
		public string Nombre { get; set; }
	}

	[XmlRoot(ElementName="peliculas")]
	public class Peliculas {
		[XmlElement(ElementName="pelicula")]
		public List<Pelicula> Pelicula { get; set; }
	}
}
