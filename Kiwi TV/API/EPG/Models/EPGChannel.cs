using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kiwi_TV.API.EPG.Models
{
    [XmlSerializerFormat, XmlType(TypeName = "channel")]
    public class EPGChannel
    {
        [XmlAttribute(AttributeName ="id")]
        public string Id;
    }
}
