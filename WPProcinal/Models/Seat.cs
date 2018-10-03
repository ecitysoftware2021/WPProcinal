using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models
{
   public class Seat
    {
        public int SeatId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int RoomId { get; set; }
        public object Room { get; set; }
        public decimal Price { get; set; }
        public string FieldNull { get; set; }
    }
}
