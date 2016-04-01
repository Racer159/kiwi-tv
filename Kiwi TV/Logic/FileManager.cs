using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kiwi_TV.Logic
{
    class FileManager
    {
        public async static Task<List<Channel>> LoadChannels(bool favorite)
        {
            List<Channel> allChannels = new List<Channel>();
            StorageFolder currentFolder = ApplicationData.Current.LocalFolder;
            
            IStorageItem channelsItem = await currentFolder.TryGetItemAsync("channels.txt");
            StorageFile channelsFile;

            if (channelsItem == null || !(channelsItem is StorageFile))
            {
                channelsFile = await currentFolder.CreateFileAsync("channels.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(channelsFile, System.IO.File.ReadAllLines("Data/channels.txt"));
            }
            else
            {
                channelsFile = (StorageFile)channelsItem;
            }
            
            IList<string> lines = await FileIO.ReadLinesAsync(channelsFile);
            if (lines[0].Trim() == "#EXTM3U")
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("#EXTINF:"))
                    {
                        string[] data = lines[i].Split(',');
                        if (data.Length == 7)
                        {
                            List<String> langs = new List<String>();
                            langs.AddRange(data[2].Trim().Split('-'));
                            if (!favorite || data[4].Trim() == "y")
                            {
                                string type = data[6].Trim();
                                allChannels.Add(new Channel(data[1].Trim(), data[3].Trim(), lines[i + 1].Trim(), langs, data[4].Trim() == "y", data[5].Trim(), type, false));
                            }
                        }
                        else if (data.Length > 0)
                        {
                            allChannels.Add(new Channel(data[1].Trim(), lines[i + 1].Trim()));
                        }
                    }
                }
            }

            return allChannels;
        }

        private async static Task SaveChannels(List<Channel> channels)
        {
            StorageFolder currentFolder = ApplicationData.Current.LocalFolder;
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
            List<Channel> TempList = await LoadChannels(false);

            foreach (Channel c in TempList)
            {
                if (c.Name == channelName)
                {
                    c.Favorite = favorited;
                }
            }

            await FileManager.SaveChannels(TempList);
        }

        public async static Task AddChannel(Channel channel)
        {
            List<Channel> TempList = await LoadChannels(false);

            TempList.Add(channel);

            await FileManager.SaveChannels(TempList);
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
                        c.Live = true;
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
    }
}
