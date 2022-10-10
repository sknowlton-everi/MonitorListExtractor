using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorListExtractor.Utility
{
    public class GuardException : Exception
    {
        public GuardException(string? message) : base(message)
        {
        }
    }
}
