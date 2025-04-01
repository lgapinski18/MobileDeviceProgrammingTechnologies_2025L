using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Exceptions
{
    internal class ThereIsNoSuchOwnerException : LogicLayerException
    {
        public ThereIsNoSuchOwnerException() { }

        public ThereIsNoSuchOwnerException(string message) : base(message) { }

        public ThereIsNoSuchOwnerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
