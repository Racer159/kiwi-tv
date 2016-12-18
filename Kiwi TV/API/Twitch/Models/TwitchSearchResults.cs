using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    /// <summary>
    /// Set of Twitch.tv channels returned from a search
    /// </summary>
    [DataContract]
    class TwitchSearchResults
    {
        [DataMember(Name = "channels")]
        public TwitchChannel[] Channels { get; set; }
    }
}
