using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures
{
    [XmlRoot("TransferData")]
    internal class TransferData
    {
        [XmlElement("SourceAccountNumber")]
        public string SourceAccountNumber { get; set; }

        [XmlElement("TargetAccountNumber")]
        public string TargetAccountNumber { get; set; }

        [XmlElement("Amount")]
        public float Amount { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }
}
