using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Exceptions
{
    internal class CreatingAccountOwnerException : Exception
    {
        public CreatingAccountOwnerException() { }

        public CreatingAccountOwnerException(string message) : base(message) { }

        public CreatingAccountOwnerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
