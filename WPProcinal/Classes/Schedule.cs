using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPProcinal.Models;

namespace WPProcinal.Classes
{
    public class Schedule
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int FontS { get; set; }
        public string Language { get; set; }
        public string Gener { get; set; }
        public string Duration { get; set; }
        public string Room { get; set; }
        public int RoomId { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public List<HoraTMP> Hours { get; set; }
        public string UnivDate { get; set; }
        public int MilitarHour { get; set; }
        public int MovieId { get; set; }
        public string TypeZona { get; set; }
        public string TipoSala { get; set; }
        public string Formato { get; set; }
    }
}
