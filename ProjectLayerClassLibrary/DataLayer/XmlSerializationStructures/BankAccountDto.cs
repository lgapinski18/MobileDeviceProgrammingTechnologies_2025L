using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures
{
    [XmlRoot("BankAccount")]
    internal class BankAccountDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }
        [XmlElement("AccountNumber")]
        public string AccountNumber { get; set; }
        [XmlElement("OwnerId")]
        public int OwnerId { get; set; }
        [XmlElement("Balance")]
        public float Balance { get; set; }
    }
}
