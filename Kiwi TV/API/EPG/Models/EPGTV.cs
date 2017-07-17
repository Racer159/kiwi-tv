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
    [XmlSerializerFormat, XmlRoot(ElementName = "tv")]
    public class EPGTV
    {
        [XmlAttribute(AttributeName = "source-info-url")]
        public string SourceInfoUrl;

        [XmlAttribute(AttributeName = "source-info-name")]
        public string SourceInfoName;

        [XmlAttribute(AttributeName = "source-data-url")]
        public string SourceDataUrl;

        [XmlAttribute(AttributeName = "generator-info-name")]
        public string GeneratorInfoName;

        [XmlAttribute(AttributeName = "generator-info-url")]
        public string GeneratorInfoUrl;

        [XmlElement(ElementName = "channel")]
        public EPGChannel[] Channels;

        [XmlElement(ElementName = "programme")]
        public EPGProgramme[] Programmes;
    }
}
