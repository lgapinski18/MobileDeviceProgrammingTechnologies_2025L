using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Exceptions
{
    public class DataLayerException : Exception
    {
        public DataLayerException() { }

        public DataLayerException(string message) : base(message) { }

        public DataLayerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
