using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchStream
    {
        [DataMember(Name = "game")]
        public string Game { get; set; }
        [DataMember(Name = "viewers")]
        public int? Viewers { get; set; }
        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }
        [DataMember(Name = "video_height")]
        public int? VideoHeight { get; set; }
        [DataMember(Name = "average_fps")]
        public double? AverageFps { get; set; }
        [DataMember(Name = "delay")]
        public int? Delay { get; set; }
        [DataMember(Name = "is_playlist")]
        public bool? IsPlaylist { get; set; }
        [DataMember(Name = "preview")]
        public TwitchPreview Preview { get; set; }
        [DataMember(Name = "channel")]
        public TwitchChannel Channel { get; set; }
    }
}
