using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models.TwitchAPI
{
    [DataContract]
    class TwitchPreview
    {
        [DataMember(Name = "small")]
        public string Small { get; set; }
        [DataMember(Name = "medium")]
        public string Medium { get; set; }
        [DataMember(Name = "large")]
        public string Large { get; set; }
        [DataMember(Name = "template")]
        public string Template { get; set; }
    }
}
