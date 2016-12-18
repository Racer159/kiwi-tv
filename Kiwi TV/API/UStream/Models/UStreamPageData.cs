using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{
    /// <summary>
    /// UStream HTML website page data
    /// </summary>
    [DataContract]
    class UStreamPageData
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "pageContent")]
        public string PageContent { get; set; }
    }
}
