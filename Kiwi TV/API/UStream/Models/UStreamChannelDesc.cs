using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{
    /// <summary>
    /// Wrapper for a UStream channel
    /// </summary>
    [DataContract]
    class UStreamChannelDesc
    {
        [DataMember(Name = "channel")]
        public UStreamChannel Channel { get; set; }
    }
}
