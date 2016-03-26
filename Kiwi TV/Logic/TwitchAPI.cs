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
        public async static Task<AccessToken> RetireveAccessToken(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("http://api.twitch.tv/api/channels/" + channelName + "/access_token");
            AccessToken token = response as AccessToken;
            return token;
        }

        public static string GetChannelNameFromURL(string videoUrl)
        {
            return videoUrl.Split('/').Last().Split('.').First();
        }
    }
}