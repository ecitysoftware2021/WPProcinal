using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes
{
    public class LogError
    {
        public string Method { get; set; }

        public string Message { get; set; }

        public object Error { get; set; }

        public Exception Exception { get; set; }
    }
}
