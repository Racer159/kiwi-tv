using Kiwi_TV.Helpers;
using Kiwi_TV.API.Twitch.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Kiwi_TV.API.Twitch
{
    class TwitchAPI
    {
        public async static Task<TwitchAccessToken> RetireveAccessToken(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("http://api.twitch.tv/api/channels/" + channelName + "/access_token", typeof(TwitchAccessToken));
            TwitchAccessToken token = response as TwitchAccessToken;
            return token;
        }

        public async static Task<TwitchStreamDesc> RetreiveStreamDescription(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/streams/" + channelName, typeof(TwitchStreamDesc));
            TwitchStreamDesc streamDesc = response as TwitchStreamDesc;
            return streamDesc;
        }

        public async static Task<TwitchChannel> RetreiveChannelDescription(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/channels/" + channelName, typeof(TwitchChannel));
            TwitchChannel channelDesc = response as TwitchChannel;
            return channelDesc;
        }

        public async static Task<TwitchSearchResults> RetrieveSearchResults(string search)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/search/channels?q=" + search, typeof(TwitchSearchResults));
            TwitchSearchResults results = response as TwitchSearchResults;
            return results;
        }

        public async static Task<bool> IsLive(string channelName)
        {
            TwitchStreamDesc streamDesc = await RetreiveStreamDescription(channelName);
            if (streamDesc != null)
            {
                return !(streamDesc.Stream == null);
            }
            else
            {
                return false;
            }
        }

        public static string GetChannelNameFromURL(string videoUrl)
        {
            return videoUrl.Split('/').Last().Split('.').First();
        }
    }
}