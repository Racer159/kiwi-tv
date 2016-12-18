using Kiwi_TV.Helpers;
using Kiwi_TV.API.Twitch.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kiwi_TV.API.Twitch
{
    /// <summary>
    /// The driver for the API interactions with Twitch.tv
    /// </summary>
    class TwitchAPI
    {
        /* Gets the access token to allow the player to play a Twitch.tv stream */
        public async static Task<TwitchAccessToken> RetireveAccessToken(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("http://api.twitch.tv/api/channels/" + channelName + "/access_token?client_id=kgomr6iz3wk1c7n5z504sryi3j4tklc", typeof(TwitchAccessToken));
            TwitchAccessToken token = response as TwitchAccessToken;
            return token;
        }

        /* Gets the stream description wrapper for a Twitch.tv stream */
        public async static Task<TwitchStreamDesc> RetreiveStreamDescription(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/streams/" + channelName + "?client_id=kgomr6iz3wk1c7n5z504sryi3j4tklc", typeof(TwitchStreamDesc));
            TwitchStreamDesc streamDesc = response as TwitchStreamDesc;
            return streamDesc;
        }

        /* Gets the description for a Twitch.tv channel */
        public async static Task<TwitchChannel> RetreiveChannelDescription(string channelName)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/channels/" + channelName + "?client_id=kgomr6iz3wk1c7n5z504sryi3j4tklc", typeof(TwitchChannel));
            TwitchChannel channelDesc = response as TwitchChannel;
            return channelDesc;
        }

        /* Gets the channel search results for a given search */
        public async static Task<TwitchSearchResults> RetrieveSearchResults(string search)
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/search/channels?q=" + search + "&client_id=kgomr6iz3wk1c7n5z504sryi3j4tklc", typeof(TwitchSearchResults));
            TwitchSearchResults results = response as TwitchSearchResults;
            return results;
        }

        /* Gets the currently live streams on Twitch.tv */
        public async static Task<TwitchSearchResults> RetrieveLiveStreams()
        {
            object response = await WebserviceHelper.MakeRequest("https://api.twitch.tv/kraken/streams?client_id=kgomr6iz3wk1c7n5z504sryi3j4tklc", typeof(TwitchStreamList));
            TwitchStreamList streamList = response as TwitchStreamList;

            List<TwitchChannel> channels = new List<TwitchChannel>();
            if (streamList != null)
            {
                foreach (TwitchStream t in streamList.Streams)
                {
                    channels.Add(t.Channel);
                }
            }
            TwitchSearchResults results = new TwitchSearchResults();
            results.Channels = channels.ToArray();

            return results;
        }

        /* Determines if a given channel is currently live */
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

        /* Helper to get the channel name from a provided URL */
        public static string GetChannelNameFromURL(string videoUrl)
        {
            return videoUrl.Split('/').Last().Split('.').First();
        }

        /* Helper to convert a Twitch.tv language code to the language strings used by the app */
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