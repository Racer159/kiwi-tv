using System.Runtime.Serialization;

namespace Kiwi_TV.API.UStream.Models
{
    /// <summary>
    /// UStream channel image information
    /// </summary>
    [DataContract]
    class UStreamPicture
    {
        [DataMember(Name = "90x90")]
        public string Large { get; set; }
        [DataMember(Name = "66x66")]
        public string Medium { get; set; }
        [DataMember(Name = "48x48")]
        public string Small { get; set; }

        public string ExtraLarge
        {
            get
            {
                return Large.Split(',')[0] + ".jpg";
            }
        }
    }
}
