using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Exceptions
{
    public class CreatingBankAccountException : Exception
    {
        public CreatingBankAccountException() { }

        public CreatingBankAccountException(string message) : base(message) { }

        public CreatingBankAccountException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
