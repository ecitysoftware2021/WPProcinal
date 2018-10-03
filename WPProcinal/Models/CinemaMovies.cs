using System;

namespace WPProcinal.Models
{
    class CinemaMovies
    {
        public int CinemaMoviesId { get; set; }

        public int CinemaId { get; set; }

        public int MovieId { get; set; }

        public DateTime Date { get; set; }
     
        public string Room { get; set; }
      
        public string Hour { get; set; }
    }
}
