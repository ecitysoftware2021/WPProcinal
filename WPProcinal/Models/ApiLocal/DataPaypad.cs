using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models.ApiLocal
{
    public class DataPaypad
    {
        public bool State { get; set; }
        public bool StateAceptance { get; set; }
        public bool StateDispenser { get; set; }
        public string Message { get; set; }
        public object ListImages { get; set; }
        public object Invoicedata { get; set; }
        public bool StateUpdate { get; set; }
        public PaypadConfiguration PaypadConfiguration { get; set; }
    }
    public class PaypadConfiguration
    {
        public int paypaD_ID { get; set; }
        public string bilL_PORT { get; set; }
        public string coiN_PORT { get; set; }
        public object unifieD_PORT { get; set; }
        public string printeR_PORT { get; set; }
        public int printeR_BAUD_RATE { get; set; }
        public string dispenseR_CONFIGURATION { get; set; }
        public string localdB_PATH { get; set; }
        public int publicitY_TIMER { get; set; }
        public int generiC_TIMER { get; set; }
        public int modaL_TIMER { get; set; }
        public int inactivitY_TIMER { get; set; }
        public int renotifY_TIMER { get; set; }
        public object extrA_DATA { get; set; }
        public bool enablE_CARD { get; set; }
        public bool enablE_VALIDATE_PERIPHERALS { get; set; }
        public int peripheralS_TO_CHECK { get; set; }
    }
}
