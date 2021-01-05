using System.Windows.Controls;
using WPProcinal.Forms;

namespace WPProcinal.Classes
{
    public class Switcher
    {
        public static frmConfigurate Navigator { get; set; }
        public static void Navigate(UserControl newPage)
        {
            Navigator.Navegar(newPage);
        }
    }
}
