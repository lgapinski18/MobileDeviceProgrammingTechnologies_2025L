using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("AccountOwner")]
    public class AccountOwnerDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Surname")]
        public string Surname { get; set; }
        [XmlElement("Email")]
        public string Email { get; set; }
    }
}
