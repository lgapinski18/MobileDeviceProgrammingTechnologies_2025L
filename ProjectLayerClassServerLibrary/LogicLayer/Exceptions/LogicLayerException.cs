using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer.Exceptions
{
    internal class LogicLayerException : Exception
    {
        public LogicLayerException() { }

        public LogicLayerException(string message) : base(message) { }

        public LogicLayerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
