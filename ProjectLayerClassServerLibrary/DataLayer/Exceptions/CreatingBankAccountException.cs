using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Exceptions
{
    public class CreatingBankAccountException : DataLayerException
    {
        public CreatingBankAccountException() { }

        public CreatingBankAccountException(string message) : base(message) { }

        public CreatingBankAccountException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
