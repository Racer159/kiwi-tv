using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{
    /// <summary>
    /// UStream channel statistics
    /// </summary>
    [DataContract]
    class UStreamStats
    {
        [DataMember(Name = "follower")]
        public long Follower { get; set; }
        [DataMember(Name = "viewer_total")]
        public long ViewerTotal { get; set; }
    }
}
