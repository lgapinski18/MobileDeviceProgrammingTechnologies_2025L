using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer.Exceptions
{
    internal class InvalidBankAccountOperationException : LogicLayerException
    {
        public InvalidBankAccountOperationException() { }

        public InvalidBankAccountOperationException(string message) : base(message) { }

        public InvalidBankAccountOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
