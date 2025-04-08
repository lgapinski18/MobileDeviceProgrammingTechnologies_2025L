using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("AccountOwnerCreationData")]
    public class AccountOwnerCreationData
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Surname")]
        public string Surname { get; set; }
        [XmlElement("Email")]
        public string Email { get; set; }
        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
