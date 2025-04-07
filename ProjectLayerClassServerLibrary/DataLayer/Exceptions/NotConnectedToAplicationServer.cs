using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Exceptions
{
    internal class NotConnectedToAplicationServer : DataLayerException
    {
        public NotConnectedToAplicationServer()
        {
        }

        public NotConnectedToAplicationServer(string message) : base(message)
        {
        }

        public NotConnectedToAplicationServer(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
