using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchSearchResults
    {
        [DataMember(Name = "channels")]
        public TwitchChannel[] Channels { get; set; }
    }
}
