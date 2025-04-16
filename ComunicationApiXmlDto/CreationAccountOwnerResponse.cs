using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ComunicationApiXmlDto
{
    [XmlRoot("CreationAccountOwnerResponse")]
    public class CreationAccountOwnerResponse
    {

        [XmlElement("CreationFlags")]
        public CreationAccountOwnerFlags CreationFlags { get; set; }
        [XmlElement("AccountOwner")]
        public AccountOwnerDto AccountOwner { get; set; }
    }

    [Flags]
    public enum CreationAccountOwnerFlags
    {
        EMPTY = 0,
        SUCCESS = 1,
        INCORRECT_NAME = 2,
        INCORRECT_SURNAME = 4,
        INCORRECT_EMAIL = 8,
        INCORRECT_PASSWORD = 16,
    }
}
