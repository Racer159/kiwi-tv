using Kiwi_TV.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kiwi_TV.API.UStream.Models;
using System.Text.RegularExpressions;

namespace Kiwi_TV.API.UStream
{
    /// <summary>
    /// The driver for the API interactions with UStream
    /// </summary>
    class UStreamAPI
    {
        // A regex to find the channel id from the HTML page results
        private static Regex ustreamRegex = new Regex("data-mediaid=\\\"([0-9]+)\\\"", RegexOptions.Compiled);

        /* Gets the channel description wrapper for a UStream channel */
        public async static Task<UStreamChannel> RetreiveChannelDescription(string channelId)
        {
            object response = await WebserviceHelper.MakeJSONRequest("https://api.ustream.tv/channels/" + channelId + ".json", typeof(UStreamChannelDesc));
            UStreamChannelDesc channelDesc = response as UStreamChannelDesc;
            return channelDesc.Channel;
        }

        /* Gets the channel search results for a given search */
        public async static Task<UStreamChannel[]> RetrieveSearchResults(string search)
        {
            object response = await WebserviceHelper.MakeJSONRequest("https://www.ustream.tv/ajax/search.json?q=" + search + "&type=live&category=all&location=anywhere", typeof(UStreamPageData));
            UStreamPageData pageData = response as UStreamPageData;

            if (pageData != null)
            {
                UStreamChannel[] results = await ParsePageContent(pageData.PageContent);
                return results;
            }
            else
            {
                return new UStreamChannel[0];
            }
        }

        /* Gets the currently live streams on UStream */
        public async static Task<UStreamChannel[]> RetrieveLiveStreams()
        {
            object response = await WebserviceHelper.MakeJSONRequest("https://www.ustream.tv/ajax-alwayscache/explore/all/all.json?subCategory=&type=no-offline&location=anywhere", typeof(UStreamPageData));
            UStreamPageData pageData = response as UStreamPageData;

            if (pageData != null)
            {
                UStreamChannel[] results = await ParsePageContent(pageData.PageContent);
                return results;
            }
            else
            {
                return new UStreamChannel[0];
            }
        }

        /* Determines if a given channel is currently live */
        public async static Task<bool> IsLive(string channelId)
        {
            try
            {
                UStreamChannel streamDesc = await RetreiveChannelDescription(channelId);
                if (streamDesc != null)
                {
                    return !(streamDesc.Status == "offair");
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /* Helper to find channels on given HTML page content */
        private async static Task<UStreamChannel[]> ParsePageContent(string pageContent)
        {
            MatchCollection matches = ustreamRegex.Matches(pageContent);
            
            List<UStreamChannel> channels = new List<UStreamChannel>();

            for (int i = 0; i < matches.Count; i++)
            {
                UStreamChannel c = await UStreamAPI.RetreiveChannelDescription(matches[i].Groups[1].Value);
                channels.Add(c);
            }

            return channels.ToArray();
        }

        /* Helper to get the channel id from a provided URL */
        public static string GetChannelIdFromURL(string videoUrl)
        {
            return videoUrl.Split('/')[2];
        }
    }
}