using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("Credentials")]
    public class Credentials
    {
        [XmlElement("Login")]
        public string Login { get; set; }
        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
