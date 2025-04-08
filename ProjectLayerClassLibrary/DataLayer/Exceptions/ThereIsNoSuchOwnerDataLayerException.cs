using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Exceptions
{
    internal class ThereIsNoSuchOwnerDataLayerException : DataLayerException
    {
        public ThereIsNoSuchOwnerDataLayerException()
        {
        }

        public ThereIsNoSuchOwnerDataLayerException(string message) : base(message)
        {
        }

        public ThereIsNoSuchOwnerDataLayerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
