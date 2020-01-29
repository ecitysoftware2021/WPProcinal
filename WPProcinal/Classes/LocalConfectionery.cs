using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{

    public class LocalCombos
    {
        public string Nombre { get; set; }
        public int Precio { get; set; }
        public int Codigo { get; set; }
        public string Disponible { get; set; }
    }

    public class LocalConfectionery
    {
        public List<LocalCombos> Combos { get; set; }
    }

}
