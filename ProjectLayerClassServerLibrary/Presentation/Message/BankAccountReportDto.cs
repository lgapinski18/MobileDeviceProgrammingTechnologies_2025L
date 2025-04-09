using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Message
{
    [XmlRoot("BankAccountReport")]
    public class BankAccountReportDto
    {
        [XmlElement("TimeOfReportCreation")]
        public DateTime TimeOfReportCreation { get; set; }

        [XmlElement("CurrentAccountBalance")]
        public float CurrentAccountBalance { get; set; }

        [XmlElement("PreviousAccountBalance")]
        public float PreviousAccountBalance { get; set; }

        [XmlElement("OwnerName")]
        public string OwnerName { get; set; }

        [XmlElement("OwnerSurname")]
        public string OwnerSurname { get; set; }

        [XmlElement("OwnerEmail")]
        public string OwnerEmail { get; set; }
    }
}
