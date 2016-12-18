using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    /// <summary>
    /// Set of Twitch.tv streams
    /// </summary>
    [DataContract]
    class TwitchStreamList
    {
        [DataMember(Name = "streams")]
        public TwitchStream[] Streams { get; set; }
    }
}
