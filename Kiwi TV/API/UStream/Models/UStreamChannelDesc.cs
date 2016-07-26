using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{

    [DataContract]
    class UStreamChannelDesc
    {
        [DataMember(Name = "channel")]
        public UStreamChannel Channel { get; set; }
    }
}
