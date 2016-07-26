using Kiwi_TV.Models;
using Kiwi_TV.API.Twitch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Kiwi_TV.API.UStream;

namespace Kiwi_TV.Helpers
{
    class ChannelManager
    {
        public async static Task<List<Channel>> LoadChannels(bool favorite, bool justDefault)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            if (localSettings.Values["syncData"] is bool && !(bool)localSettings.Values["syncData"])
            {
                currentFolder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                currentFolder = ApplicationData.Current.RoamingFolder;
            }

            
            IStorageItem channelsItem = await currentFolder.TryGetItemAsync("channels.txt");
            StorageFile channelsFile;

            if (channelsItem == null || !(channelsItem is StorageFile) || justDefault)
            {
                channelsFile = await currentFolder.CreateFileAsync("channels.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(channelsFile, System.IO.File.ReadAllLines("Data/channels.txt"));
            }
            else
            {
                channelsFile = (StorageFile)channelsItem;
            }

            List<Channel> allChannels = await LoadChannelFile(channelsFile, favorite);

            return allChannels;
        }

        public async static Task<List<Channel>> LoadChannelFile(StorageFile channelsFile, bool favorite)
        {
            List<Channel> allChannels = new List<Channel>();

            try
            {
                IList<string> lines = await FileIO.ReadLinesAsync(channelsFile);
            
                if (lines[0].Trim() == "#EXTM3U")
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].StartsWith("#EXTINF:"))
                        {
                            string[] data = lines[i].Split(',');
                            if (data.Length == 7 && i + 1 < lines.Count)
                            {
                                List<String> langs = new List<String>();
                                langs.AddRange(data[2].Trim().Split('-'));
                                if (!favorite || data[4].Trim() == "y")
                                {
                                    string type = data[6].Trim();
                                    allChannels.Add(new Channel(data[1].Trim(), data[3].Trim(), lines[i + 1].Trim(), langs, data[4].Trim() == "y", data[5].Trim(), type, false));
                                }
                            }
                            else if (data.Length > 0 && i + 1 < lines.Count)
                            {
                                Channel c = new Channel(data[1].Trim(), lines[i + 1].Trim());
                                if (c.Source != null)
                                {
                                    allChannels.Add(c);
                                }
                            }
                        }
                    }
                }

                return allChannels;
            }
            catch
            {
                return new List<Channel>();
            }
        }

        private async static Task SaveChannels(List<Channel> channels)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            if (localSettings.Values["syncData"] is bool && !(bool)localSettings.Values["syncData"])
            {
                currentFolder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                currentFolder = ApplicationData.Current.RoamingFolder;
            }

            String file = "#EXTM3U\n\n";

            foreach (Channel c in channels)
            {
                //Name
                file += "#EXTINF:0, " + c.Name + ", ";
                //Languages
                for (int i = 0; i < c.Languages.Count; i++)
                {
                    file += c.Languages[i];
                    if (i < c.Languages.Count - 1) { file += "-"; }
                }
                //Icon
                file += ", " + c.Icon + ", ";
                //Favorite
                if (c.Favorite) { file += "y";  } else { file += "n"; }
                //Genre
                file += ", " + c.Genre + ", ";
                //Type
                file += c.Type + "\n";
                //Source
                file += c.Source + "\n\n";
            }

            StorageFile channelsFile = await currentFolder.CreateFileAsync("channels.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(channelsFile, file);
        }

        public async static Task SaveFavorite(string channelName, bool favorited)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            foreach (Channel c in TempList)
            {
                if (c.Name == channelName)
                {
                    c.Favorite = favorited;
                }
            }

            await ChannelManager.SaveChannels(TempList);
        }

        public async static Task AddChannel(Channel channel)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.Add(channel);

            await ChannelManager.SaveChannels(TempList);
        }

        public async static Task AddChannels(List<Channel> channels)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.AddRange(channels);

            await ChannelManager.SaveChannels(TempList);
        }

        public async static Task RemoveChannel(Channel channel)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));

            await ChannelManager.SaveChannels(TempList);
        }

        public async static Task RestoreDefaultChannels()
        {
            List<Channel> CurrList = await LoadChannels(false, false);
            List<Channel> DefaultList = await LoadChannels(false, true);

            foreach (Channel channel in DefaultList)
            {
                CurrList.Remove(CurrList.Find(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));
            }

            CurrList.AddRange(DefaultList);

            await ChannelManager.SaveChannels(CurrList);
        }

        public async static Task ResetChannels()
        {
            List<Channel> DefaultList = await LoadChannels(false, true);

            await ChannelManager.SaveChannels(DefaultList);
        }

        public static void LoadCategories(List<Channel> channels, ObservableCollection<Category> categoryList)
        {

            foreach (Channel c in channels)
            {
                int i = -1;

                for (int j = 0; j < categoryList.Count; j++)
                {
                    if (categoryList[j].Name == c.Genre)
                    {
                        i = j;
                    }
                }

                if (i < 0)
                {
                    Category cat = new Category(c.Genre);
                    cat.Channels.Add(c);
                    categoryList.Add(cat);
                }
                else
                {
                    categoryList[i].Channels.Add(c);
                }
            }
        }

        public async static Task<List<Channel>> SetLive(List<Channel> channels)
        {
            foreach (Channel c in channels)
            {
                switch (c.Type)
                {
                    case "iptv":
                        c.Live = true; //await WebserviceHelper.Ping(c.Source);
                        break;
                    case "ustream":
                        c.Live = await UStreamAPI.IsLive(UStreamAPI.GetChannelIdFromURL(c.Source.AbsolutePath));
                        break;
                    case "twitch":
                        c.Live = await TwitchAPI.IsLive(TwitchAPI.GetChannelNameFromURL(c.Source.AbsolutePath));
                        break;
                    default:
                        break;
                }
            }

            return channels;
        }

        public async static void MigrateChannelList()
        {
            List<Channel> TempList = await LoadChannels(false, false);

            if (TempList.Find(delegate (Channel c) { return (c.Name == "NHK World") && (c.Icon == "ms-appx:///Data/ChannelIcons/nhkworld.png"); }) == null)
            {
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "NASA TV ISS") && (c.Icon == "ms-appx:///Data/ChannelIcons/nasatviss.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Weather Nation") && (c.Icon == "ms-appx:///Data/ChannelIcons/weathernation.png"); }));

                StorageFile updateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/updates.txt"));

                List<Channel> updates = await LoadChannelFile(updateFile, false);
                TempList.AddRange(updates);

                await ChannelManager.SaveChannels(TempList);
            }
        }
    }
}
