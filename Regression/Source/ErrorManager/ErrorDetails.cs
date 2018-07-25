using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorManager
{
    public class ErrorDetails
    {
        public bool Occurred { get; set; }
        public LoggingDetails Details { get; set; }
    }
}
