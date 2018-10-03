using System.Collections.Generic;

namespace WPProcinal.Models
{
    class Movies
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CinemaMovies> CinemaMovies { get; set; }
    }
}
