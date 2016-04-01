using Kiwi_TV.Models;
using Kiwi_TV.Models.TwitchAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kiwi_TV.Logic
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

        public async static Task<bool> IsLive(string channelName)
        {
            TwitchStreamDesc streamDesc = await RetreiveStreamDescription(channelName);
            return !(streamDesc.Stream == null);
        }

        public static string GetChannelNameFromURL(string videoUrl)
        {
            return videoUrl.Split('/').Last().Split('.').First();
        }
    }
}