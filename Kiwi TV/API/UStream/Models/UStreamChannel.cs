using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{

    [DataContract]
    class UStreamChannel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        
        [DataMember(Name = "title")]
        public string Title { get; set; }
        
        [DataMember(Name = "picture")]
        public UStreamPicture Picture { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "last_broadcast_at")]
        public long LastBroadcastAt { get; set; }

        [DataMember(Name = "tinyurl")]
        public string TinyUrl { get; set; }

        [DataMember(Name = "stats")]
        public UStreamStats Stats { get; set; }
    }
}
