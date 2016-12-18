using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    /// <summary>
    /// Wrapper for a Twitch.tv stream
    /// </summary>
    [DataContract]
    class TwitchStreamDesc
    {
        [DataMember(Name = "stream")]
        public TwitchStream Stream { get; set; }
    }
}
