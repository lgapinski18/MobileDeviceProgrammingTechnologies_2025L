using System.Xml.Serialization;

namespace ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures
{
    [XmlRoot("AccountOwnerCreationData")]
    internal class AccountOwnerCreationData
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
