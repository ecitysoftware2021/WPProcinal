using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models.ApiLocal
{
    public class RequestAuth
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
    }
}

