using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Exceptions
{
    public class CreatingAccountOwnerException : DataLayerException
    {
        public CreatingAccountOwnerException() { }

        public CreatingAccountOwnerException(string message) : base(message) { }

        public CreatingAccountOwnerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
