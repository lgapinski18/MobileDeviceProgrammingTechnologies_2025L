using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Exceptions
{
    internal class UnableToConnectToAplicationServer : DataLayerException
    {
        public UnableToConnectToAplicationServer()
        {
        }

        public UnableToConnectToAplicationServer(string message) : base(message)
        {
        }

        public UnableToConnectToAplicationServer(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
