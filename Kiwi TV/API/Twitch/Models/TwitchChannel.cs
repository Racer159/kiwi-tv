using System.Runtime.Serialization;

namespace Kiwi_TV.API.Twitch.Models
{
    [DataContract]
    class TwitchChannel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "language")]
        public string Language { get; set; }
        [DataMember(Name = "broadcaster_language")]
        public string BroadcasterLanguage { get; set; }
        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }
        [DataMember(Name = "game")]
        public string Game { get; set; }
        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }
        [DataMember(Name = "updated_at")]
        public string UpdatedAt { get; set; }
        [DataMember(Name = "logo")]
        public string Logo { get; set; }
        [DataMember(Name = "banner")]
        public string Banner { get; set; }
        [DataMember(Name = "video_banner")]
        public string VideoBanner { get; set; }
        [DataMember(Name = "background")]
        public string Background { get; set; }
        [DataMember(Name = "profile_banner")]
        public string ProfileBanner { get; set; }
        [DataMember(Name = "profile_banner_background_color")]
        public string ProfileBannerBackgroundColor { get; set; }
        [DataMember(Name = "partner")]
        public bool? Partner { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "views")]
        public long? Views { get; set; }
        [DataMember(Name = "followers")]
        public long? Followers { get; set; }
    }
}
