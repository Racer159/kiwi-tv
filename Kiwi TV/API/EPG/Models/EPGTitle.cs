using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kiwi_TV.API.EPG.Models
{
    [XmlSerializerFormat, XmlType(TypeName = "title")]
    public class EPGTitle
    {
        [XmlAttribute(AttributeName = "lang")]
        public string Language;

        [XmlText]
        public string Text;
    }
}
