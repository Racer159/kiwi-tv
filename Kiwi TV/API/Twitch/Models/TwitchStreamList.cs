using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchStreamList
    {
        [DataMember(Name = "streams")]
        public TwitchStream[] Streams { get; set; }
    }
}
