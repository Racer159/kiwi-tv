using Kiwi_TV.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kiwi_TV.API.UStream.Models;
using System.Text.RegularExpressions;

namespace Kiwi_TV.API.UStream
{
    class UStreamAPI
    {
        public async static Task<UStreamChannel> RetreiveChannelDescription(string channelId)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.ustream.tv/channels/" + channelId + ".json", typeof(UStreamChannelDesc));
            UStreamChannelDesc channelDesc = response as UStreamChannelDesc;
            return channelDesc.Channel;
        }

        public async static Task<UStreamChannel[]> RetrieveSearchResults(string search)
        {
            object response = await WebserviceHelper.MakeRequest("https://www.ustream.tv/ajax/search.json?q=" + search + "&type=live&category=all&location=anywhere", typeof(UStreamPageData));
            UStreamPageData pageData = response as UStreamPageData;
            
            UStreamChannel[] results = await ParsePageContent(pageData.PageContent, 10);

            return results;
        }

        public async static Task<UStreamChannel[]> RetrieveLiveStreams()
        {
            object response = await WebserviceHelper.MakeRequest("https://www.ustream.tv/ajax-alwayscache/explore/all/all.json?subCategory=&type=no-offline&location=anywhere", typeof(UStreamPageData));
            UStreamPageData pageData = response as UStreamPageData;

            UStreamChannel[] results = await ParsePageContent(pageData.PageContent, 5);

            return results;
        }

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

        private async static Task<UStreamChannel[]> ParsePageContent(string pageContent, int limit)
        {
            List<string> channelIds = new List<string>();

            Regex regex = new Regex("data-mediaid=\\\"([0-9]*)\\\"");
            MatchCollection matches = regex.Matches(pageContent);

            for (int i = 0; i < matches.Count; i++)
            {
                channelIds.Add(matches[i].Groups[1].Value);
            }

            List<UStreamChannel> channels = new List<UStreamChannel>();

            for (int i = 0; i < limit && i < channelIds.Count; i++)
            {
                UStreamChannel c = await UStreamAPI.RetreiveChannelDescription(channelIds[i]);
                channels.Add(c);
            }

            return channels.ToArray();
        }

        public static string GetChannelIdFromURL(string videoUrl)
        {
            return videoUrl.Split('/')[2];
        }

        public static string ConvertLanguageCode(string code)
        {
            switch (code)
            {
                case "en":
                    return "English";
                case "fr":
                    return "French";
                case "ar":
                    return "Arabic";
                case "ru":
                    return "Russian";
                case "de":
                    return "German";
                default:
                    return "Other";
            }
        }
    }
}