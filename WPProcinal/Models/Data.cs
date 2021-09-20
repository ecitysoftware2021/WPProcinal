using System.Collections.Generic;
using System.Xml.Serialization;

namespace WPProcinal.Models
{
	[XmlRoot(ElementName = "data")]
	public class Data
	{
		//[XmlAttribute(AttributeName = "TituloOriginal")]
		[XmlAttribute(AttributeName = "Titulo")]
		public string TituloOriginal { get; set; }
		[XmlAttribute(AttributeName = "Imagen")]
		public string Imagen { get; set; }
		[XmlAttribute(AttributeName = "Censura")]
		public string Censura { get; set; }
		[XmlAttribute(AttributeName = "idioma")]
		public string Idioma { get; set; }
		[XmlAttribute(AttributeName = "genero")]
		public string Genero { get; set; }
		[XmlAttribute(AttributeName = "pais")]
		public string Pais { get; set; }
		[XmlAttribute(AttributeName = "duracion")]
		public string Duracion { get; set; }
		[XmlAttribute(AttributeName = "medio")]
		public string Medio { get; set; }
		[XmlAttribute(AttributeName = "formato")]
		public string Formato { get; set; }
		[XmlAttribute(AttributeName = "versión")]
		public string Versión { get; set; }
		[XmlAttribute(AttributeName = "trailer1")]
		public string Trailer1 { get; set; }
		[XmlAttribute(AttributeName = "trailer2")]
		public string Trailer2 { get; set; }
		[XmlAttribute(AttributeName = "fechaEstreno")]
		public string FechaEstreno { get; set; }
		[XmlAttribute(AttributeName = "reparto")]
		public string Reparto { get; set; }
		[XmlAttribute(AttributeName = "director")]
		public string Director { get; set; }
		[XmlAttribute(AttributeName = "distribuidor")]
		public string Distribuidor { get; set; }
	}

	[XmlRoot(ElementName = "dia")]
	public class Dia
	{
		[XmlAttribute(AttributeName = "fecha")]
		public string Fecha { get; set; }
		[XmlAttribute(AttributeName = "univ")]
		public string Univ { get; set; }
	}

	[XmlRoot(ElementName = "DiasDisponiblesTodosCinemas")]
	public class DiasDisponiblesTodosCinemas
	{
		[XmlElement(ElementName = "dia")]
		public List<Dia> Dia { get; set; }
	}

	[XmlRoot(ElementName = "DiasDisponiblesCinema")]
	public class DiasDisponiblesCinema
	{
		[XmlElement(ElementName = "dia")]
		public List<Dia> Dia { get; set; }
	}

	[XmlRoot(ElementName = "Tarifa")]
	public class Tarifa
	{
		[XmlAttribute(AttributeName = "codigoTarifa")]
		public string CodigoTarifa { get; set; }
		[XmlAttribute(AttributeName = "nombreTarifa")]
		public string NombreTarifa { get; set; }
		[XmlAttribute(AttributeName = "documentoEnCaja")]
		public string DocumentoEnCaja { get; set; }
		[XmlAttribute(AttributeName = "medioPago")]
		public string MedioPago { get; set; }
		[XmlAttribute(AttributeName = "validokiosko")]
		public string Validokiosko { get; set; }
		[XmlAttribute(AttributeName = "validoTeceros")]
		public string ValidoTeceros { get; set; }
		[XmlAttribute(AttributeName = "validoInternet")]
		public string ValidoInternet { get; set; }
		[XmlAttribute(AttributeName = "validoPos")]
		public string ValidoPos { get; set; }
		[XmlAttribute(AttributeName = "validoApp")]
		public string ValidoApp { get; set; }
		[XmlAttribute(AttributeName = "validoPosMovil")]
		public string ValidoPosMovil { get; set; }
		[XmlAttribute(AttributeName = "clienteFrecuente")]
		public string ClienteFrecuente { get; set; }
		[XmlAttribute(AttributeName = "listaEspecial")]
		public string ListaEspecial { get; set; }
		[XmlAttribute(AttributeName = "valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName = "TipoSilla")]
	public class TipoSilla
	{
		[XmlElement(ElementName = "Tarifa")]
		public List<Tarifa> Tarifa { get; set; }
		[XmlAttribute(AttributeName = "nombreTipoSilla")]
		public string NombreTipoSilla { get; set; }
		[XmlAttribute(AttributeName = "idTipoSilla")]
		public string IdTipoSilla { get; set; }
	}

	[XmlRoot(ElementName = "TipoZona")]
	public class TipoZona
	{
		[XmlElement(ElementName = "TipoSilla")]
		public List<TipoSilla> TipoSilla { get; set; }
		[XmlAttribute(AttributeName = "nombreZona")]
		public string NombreZona { get; set; }
		[XmlAttribute(AttributeName = "idZona")]
		public string IdZona { get; set; }
	}

	[XmlRoot(ElementName = "hora")]
	public class Hora
	{
		[XmlElement(ElementName = "TipoZona")]
		public List<TipoZona> TipoZona { get; set; }
		[XmlAttribute(AttributeName = "idFuncion")]
		public string IdFuncion { get; set; }
		[XmlAttribute(AttributeName = "horario")]
		public string Horario { get; set; }
		[XmlAttribute(AttributeName = "reservasONline")]
		public string ReservasONline { get; set; }
		[XmlAttribute(AttributeName = "ventasONline")]
		public string VentasONline { get; set; }
		[XmlAttribute(AttributeName = "militar")]
		public string Militar { get; set; }
		[XmlAttribute(AttributeName = "validaciones")]
		public string Validaciones { get; set; }
		[XmlAttribute(AttributeName = "cortoNacional")]
		public string CortoNacional { get; set; }
		[XmlAttribute(AttributeName = "capacidad")]
		public string Capacidad { get; set; }
		//[XmlAttribute(AttributeName = "disponibilidad")]
		[XmlAttribute(AttributeName = "porcentajeOcupación")]
		public string Disponibilidad { get; set; }
		
		[XmlAttribute(AttributeName = "KioscoVentas")]
		public string KioscoVentas { get; set; }

		[XmlAttribute(AttributeName = "posMovilVentas")]
		public string posMovilVentas { get; set; }

		[XmlAttribute(AttributeName = "posVentas")]
		public string posVentas { get; set; }

		[XmlAttribute(AttributeName = "posReservas")]
		public string posReservas { get; set; }

		[XmlAttribute(AttributeName = "webserviceReservas")]
		public string webserviceReservas { get; set; }

		[XmlAttribute(AttributeName = "kioskoReservas")]
		public string kioskoReservas { get; set; }

		[XmlAttribute(AttributeName = "posMovilReservas")]
		public string posMovilReservas { get; set; }

		[XmlAttribute(AttributeName = "webserviceVentas")]
		public string webserviceVentas { get; set; }

	}

	[XmlRoot(ElementName = "Fecha")]
	public class Fecha
	{
		[XmlElement(ElementName = "hora")]
		public List<Hora> Hora { get; set; }
		[XmlAttribute(AttributeName = "dia")]
		public string Dia { get; set; }
		[XmlAttribute(AttributeName = "univ")]
		public string Univ { get; set; }
	}

	[XmlRoot(ElementName = "sala")]
	public class Sala
	{
		[XmlElement(ElementName = "Fecha")]
		public List<Fecha> Fecha { get; set; }
		[XmlAttribute(AttributeName = "numeroSala")]
		public string NumeroSala { get; set; }
		[XmlAttribute(AttributeName = "tipoSala")]
		public string TipoSala { get; set; }
	}

	[XmlRoot(ElementName = "salas")]
	public class Salas
	{
		[XmlElement(ElementName = "sala")]
		public List<Sala> Sala { get; set; }
	}

	[XmlRoot(ElementName = "cinema")]
	public class Cinema
	{
		[XmlElement(ElementName = "DiasDisponiblesCinema")]
		public DiasDisponiblesCinema DiasDisponiblesCinema { get; set; }
		[XmlElement(ElementName = "salas")]
		public Salas Salas { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "nombre")]
		public string Nombre { get; set; }
		[XmlAttribute(AttributeName = "ciudad")]
		public string Ciudad { get; set; }
	}

	[XmlRoot(ElementName = "cinemas")]
	public class Cinemas
	{
		[XmlElement(ElementName = "cinema")]
		public List<Cinema> Cinema { get; set; }
	}

	[XmlRoot(ElementName = "pelicula")]
	public class Pelicula
	{
		[XmlElement(ElementName = "sinopsis")]
		public string Sinopsis { get; set; }
		[XmlElement(ElementName = "data")]
		public Data Data { get; set; }
		[XmlElement(ElementName = "DiasDisponiblesTodosCinemas")]
		public DiasDisponiblesTodosCinemas DiasDisponiblesTodosCinemas { get; set; }
		[XmlElement(ElementName = "cinemas")]
		public Cinemas Cinemas { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "nombre")]
		public string Nombre { get; set; }
		[XmlAttribute(AttributeName = "tipo")]
		public string Tipo { get; set; }
	}

	[XmlRoot(ElementName = "Peliculas")]
	public class Peliculas
	{
		[XmlElement(ElementName = "pelicula")]
		public List<Pelicula> Pelicula { get; set; }
	}
}
