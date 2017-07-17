using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kiwi_TV.API.EPG.Models
{
    [XmlSerializerFormat, XmlType(TypeName = "programme")]
    public class EPGProgramme
    {
        private const string format = "yyyyMMddHHmmss zzzz";

        [XmlAttribute(AttributeName = "start")]
        public string Start;
        [XmlAttribute(AttributeName = "stop")]
        public string Stop;
        [XmlAttribute(AttributeName = "channel")]
        public string Channel;
        [XmlElement(ElementName = "title")]
        public EPGTitle Title;

        public DateTime getStart()
        {
            try { return DateTime.ParseExact(this.Start, format, CultureInfo.InvariantCulture); } catch { return DateTime.Now; }
        }

        public DateTime getStop()
        {
            try { return DateTime.ParseExact(this.Stop, format, CultureInfo.InvariantCulture); } catch { return DateTime.Now; }
        }
    }
}
