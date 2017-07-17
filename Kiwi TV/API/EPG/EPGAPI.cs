using Kiwi_TV.API.EPG.Models;
using Kiwi_TV.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.API.EPG
{
    /// <summary>
    /// The driver for the API interactions with EPG XML Urls
    /// </summary>
    class EPGAPI
    {
        /* Gets the XML data from an EPG XML Url */
        public async static Task<EPGTV> RetrieveEPGData(string url)
        {
            object response = await WebserviceHelper.MakeXMLRequest(url, typeof(EPGTV));
            EPGTV tv = response as EPGTV;
            return tv;
        }
    }
}
