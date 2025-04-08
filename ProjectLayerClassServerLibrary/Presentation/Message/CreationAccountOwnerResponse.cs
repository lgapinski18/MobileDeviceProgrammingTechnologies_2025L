using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("CreationAccountOwnerResponse")]
    internal class CreationAccountOwnerResponse
    {
        [XmlElement("CreationFlags")]
        public CreationAccountOwnerDataLayerFlags CreationFlags { get; set; }
        [XmlElement("AccountOwner")]
        public AccountOwnerDto? AccountOwner { get; set; }
    }
}
