using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures
{
    [Flags]
    internal enum CreationAccountOwnerDataLayerFlags
    {
        EMPTY = 0,
        SUCCESS = 1,
        INCORRECT_NAME = 2,
        INCORRECT_SURNAME = 4,
        INCORRECT_EMAIL = 8,
        INCORRECT_PASSWORD = 16,
    }
}
