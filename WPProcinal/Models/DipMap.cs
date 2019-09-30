using System.Collections.Generic;

namespace WPProcinal.Models
{
    public class DipMap
    {
        public int CinemaId { get; set; }//Id del cinema ex:303

        public int RoomId { get; set; }//Id sala ex: 1

        public string Date { get; set; } //fecha de la funcion ex:aaaammddd

        public string DateFormat { get; set; } //fecha de la funcion con formato ex:ddmmaaaa

        public int Hour { get; set; }// horario(hora) ex:22

        public int HourFormat { get; set; } //horario ex:2230

        public string Letter { get; set; }

        public string Login { get; set; }

        public int MovieId { get; set; } //Id película ex:5150

        public int PointOfSale { get; set; }

        public int Group { get; set; }

        public int Secuence { get; set; }

        public string IsCard { get; set; }

        public string Email { get; set; }


        public double Total { get; set; }


        public string PaymentMethod { get; set; }

        #region MovieData
        public string MovieName { get; set; }//Titulo original ex: Deadpool 2

        public string Language { get; set; }

        public string Gener { get; set; }

        public string Duration { get; set; }

        public string Day { get; set; }

        public string HourFunction { get; set; }

        public string RoomName { get; set; }//Nombre sala ex: Sala 1

        public string Category { get; set; }

        public List<TipoZona> TypeZona { get; set; }

        public int DipMapId { get; set; }
        public int IDFuncion { get; set; }
        #endregion
    }
}
