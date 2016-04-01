using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models.TwitchAPI
{
    [DataContract]
    class TwitchAccessToken
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }
        [DataMember(Name = "sig")]
        public string Signature { get; set; }
        [DataMember(Name = "mobile_restricted")]
        public bool IsMobileRestricted { get; set; }
    }
}
