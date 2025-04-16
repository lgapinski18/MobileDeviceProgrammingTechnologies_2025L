using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComunicationApiXmlDto
{
    public enum TransferResultCodes
    {
        SUCCESS,
        OWNER_ACCOUNT_DOESNT_EXISTS,
        TARGET_BANK_ACCOUNT_DOESNT_EXISTS,
        INSUFICIENT_BANK_ACCOUNT_FUNDS,
        TIMEOUT,
        TRANSFER_HAS_BEEN_INTERUPTED
    }
}
