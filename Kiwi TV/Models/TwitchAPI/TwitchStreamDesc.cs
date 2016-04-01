using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models.TwitchAPI
{
    [DataContract]
    class TwitchStreamDesc
    {
        [DataMember(Name = "stream")]
        public TwitchStream Stream { get; set; }
    }
}
