using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchAccessToken
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }
        [DataMember(Name = "sig")]
        public string Signature { get; set; }
        [DataMember(Name = "mobile_restricted")]
        public bool? IsMobileRestricted { get; set; }
    }
}
