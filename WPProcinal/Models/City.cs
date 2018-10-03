using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models
{
    class City
    {
        public List<Cinema> Cinemas { get; set; }

        public int CityId { get; set; }

        public string Name { get; set; }
    }
}
