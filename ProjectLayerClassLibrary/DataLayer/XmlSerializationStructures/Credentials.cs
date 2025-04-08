﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures
{
    [XmlRoot("Credentials")]
    internal class Credentials
    {
        [XmlElement("Login")]
        public string Login { get; set; }
        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
