using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchStreamDesc
    {
        [DataMember(Name = "stream")]
        public TwitchStream Stream { get; set; }
    }
}
