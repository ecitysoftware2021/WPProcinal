using System.Collections.Generic;

namespace WPProcinal.Classes
{
    public class Dictionaries
    {

        public static Dictionary<string, string> Cinemas = new Dictionary<string, string>()
        {
            {"301","Aves María"},
            {"302","Plaza del Rio"},
            {"303","Mayorca"},
            {"304","Puerta del Norte"},
            {"305","Monterrey"},
            {"306","Américas"},
            {"310","San Nicolás"},
            {"313","Florida"},
            {"315 ","Santa Marta"},
            {"319","Aventura"},
            {"321","La Ceja"},
            {"323","Guacarí"},
            { "325","La Central"},
        };
        public enum ECinemas
        {
            AvesMaria = 301,
            PlazadelRio = 302,
            Mayorca = 303,
            PuertadelNorte = 304,
            Monterrey = 305,
            Americas = 306,
            SanNicolás = 310,
            Florida = 313,
            SantaMarta = 315,
            Aventura = 319,
            LaCeja = 321,
            Guacari = 323,
            LaCentral = 325,
        }
    }
}
