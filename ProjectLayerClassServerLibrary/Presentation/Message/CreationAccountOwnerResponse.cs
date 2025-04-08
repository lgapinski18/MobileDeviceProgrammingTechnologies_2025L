using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("CreationAccountOwnerResponse")]
    public class CreationAccountOwnerResponse
    {
        [XmlElement("ResponseCodes")]
        public List<string> ResponseCodes { get; set; }
        [XmlElement("AccountOwner")]
        public AccountOwnerDto? AccountOwner { get; set; }
    }
}
