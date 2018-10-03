using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models
{
    class Room
    {
        public int RoomId { get; set; }

        public string Name { get; set; }

        public int NumSeatH { get; set; }

        public int NumSeatV { get; set; }

        public int CinemaId { get; set; }

        public List<Seat> Seats { get; set; }
    }
}
