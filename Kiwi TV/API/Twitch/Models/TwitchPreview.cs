using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    /// <summary>
    /// Twitch.tv video preview image
    /// </summary>
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
