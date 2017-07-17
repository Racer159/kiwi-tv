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
    /// <summary>
    /// A helper to deal with the managing channel information
    /// </summary>
    class ChannelManager
    {
        /* Helper to load a list of channels from the channels file */
        public async static Task<List<Channel>> LoadChannels(bool favorite, bool justDefault)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            // Determines the correct folder based on a user's sync settings
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

            // Determines if a channels file exists, and if not copies in the defaults
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
        
        /* Helper to parse a channels file into a channels list */
        public async static Task<List<Channel>> LoadChannelFile(StorageFile channelsFile, bool favorite)
        {
            List<Channel> allChannels = new List<Channel>();

            try
            {
                IList<string> lines = await FileIO.ReadLinesAsync(channelsFile);
            
                // File starts with '#EXTM3U'
                if (lines[0].Trim() == "#EXTM3U")
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        // Channel line starts with '#EXTINF:'
                        if (lines[i].StartsWith("#EXTINF:"))
                        {
                            string[] data = lines[i].Split(',');
                            // Kiwi TV channels have seven pieces of data
                            if (data.Length == 7 && i + 1 < lines.Count)
                            {
                                List<String> langs = new List<String>();
                                langs.AddRange(data[2].Trim().Split('-'));
                                if (!favorite || data[4].Trim() == "y")
                                {
                                    string type = data[6].Trim();
                                    Channel c = new Channel(data[1].Trim(), data[3].Trim(), lines[i + 1].Trim(), langs, data[4].Trim() == "y", data[5].Trim(), type, false);

                                    // See if there is an EPG XML URI below the source URL
                                    if (i+2 < lines.Count && Uri.IsWellFormedUriString(lines[i + 2].Trim(), UriKind.Absolute))
                                    {
                                        c.EPGSource = new Uri(lines[i + 2].Trim());
                                    }

                                    allChannels.Add(c);
                                }
                            }
                            // Other channels have different ammounts of data
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

        /* Helper to save a list of channels to the channels file */
        private async static Task SaveChannels(List<Channel> channels)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            // Determines the correct folder based on a user's sync settings
            if (localSettings.Values["syncData"] is bool && !(bool)localSettings.Values["syncData"])
            {
                currentFolder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                currentFolder = ApplicationData.Current.RoamingFolder;
            }
            
            StorageFile channelsFile = await currentFolder.CreateFileAsync("channels.txt", CreationCollisionOption.ReplaceExisting);

            await SaveChannelFile(channels, channelsFile);
        }

        /* Helper to create a channels file from a channels list */
        private async static Task SaveChannelFile(List<Channel> channels, StorageFile channelsFile)
        {
            String file = "#EXTM3U\n\n";

            foreach (Channel c in channels)
            {
                // Name
                file += "#EXTINF:0, " + c.Name + ", ";
                // Languages
                for (int i = 0; i < c.Languages.Count; i++)
                {
                    file += c.Languages[i];
                    if (i < c.Languages.Count - 1) { file += "-"; }
                }
                // Icon
                file += ", " + c.Icon + ", ";
                // Favorite
                if (c.Favorite) { file += "y"; } else { file += "n"; }
                // Genre
                file += ", " + c.Genre + ", ";
                // Type
                file += c.Type + "\n";
                // Source
                file += c.Source + "\n";
                if (c.EPGSource != null)
                {
                    file += c.EPGSource + "\n";
                }
                file += "\n";
            }

            // Kiwi TV Version
            file += "version1.5";

            await FileIO.WriteTextAsync(channelsFile, file);
        }
        
        /* Helper to return the current version of the channels file */
        private async static Task<string> GetVersionInfo()
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

            if (lines.Count > 0) { return lines[lines.Count - 1]; } else { return ""; }
        }

        /* Helper to save a channel as a favorite */
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
        
        /* Helper to add a single channel to the channel list */
        public async static Task AddChannel(Channel channel)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.Add(channel);

            await ChannelManager.SaveChannels(TempList);
        }

        /* Helper to add multiple channels to the channel list */
        public async static Task AddChannels(List<Channel> channels)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.AddRange(channels);

            await ChannelManager.SaveChannels(TempList);
        }
        
        /* Helper to remove a single channel from the channel list */
        public async static Task RemoveChannel(Channel channel)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));

            await ChannelManager.SaveChannels(TempList);
        }

        /* Helper to remove multiple channels from the channel list */
        public async static Task RemoveChannels(List<Channel> channels)
        {
            List<Channel> TempList = await LoadChannels(false, false);

            foreach (Channel channel in channels)
            {
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));
            }

            await ChannelManager.SaveChannels(TempList);
        }

        /* Helper to restore the default channels in case they were deleted */
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

        /* Helper to reset a corrupted channel list to the defaults */
        public async static Task ResetChannels()
        {
            List<Channel> DefaultList = await LoadChannels(false, true);

            await ChannelManager.SaveChannels(DefaultList);
        }

        /* Helper to parse loaded channels into a category key map object */
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

        /* Helper to set the live status for a given list of channels */
        public async static Task<List<Channel>> SetLive(List<Channel> channels, bool m3u8LiveCheck, IProgress<ProgressTaskAsync> progress)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            
            ProgressTaskAsync p = new ProgressTaskAsync();
            p.ProgressPercentage = 0;
            p.Text = 0 + "/" + channels.Count;
            progress.Report(p);

            for ( int i = 0; i < channels.Count; i++)
            {
                Channel c = channels[i];

                switch (c.Type)
                {
                    case "iptv":
                        if (m3u8LiveCheck && localSettings.Values["m3u8LiveCheck"] is bool && (bool)localSettings.Values["m3u8LiveCheck"])
                        {
                            c.Live = await WebserviceHelper.Ping(c.Source);
                        }
                        else
                        {
                            c.Live = true;
                        }
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

                p = new ProgressTaskAsync();
                p.ProgressPercentage = (channels.Count < 0 ? 0 : (100 * (i+1)) / channels.Count);
                p.Text = (i+1) + "/" + channels.Count;
                progress.Report(p);
            }

            return channels;
        }

        /* Helper to create a shareable channel list file */
        public async static Task<StorageFile> GetFileToShare(List<Channel> shareChannels)
        {
            StorageFile shareFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("share.m3u8", CreationCollisionOption.ReplaceExisting);
            await SaveChannelFile(shareChannels, shareFile);
            return shareFile;
        }

        /* Helper to migrate the channel list between Kiwi TV versions */
        public async static void MigrateChannelList()
        {
            List<Channel> TempList = await LoadChannels(false, false);
            string version = await GetVersionInfo();

            if (version != "version1.5")
            {
                // Removed to slim channel lineup
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Euro News") && (c.Icon == "ms-appx:///Data/ChannelIcons/euronews.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Beatz TV") && (c.Icon == "ms-appx:///Data/ChannelIcons/beatztv.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Weather Nation") && (c.Icon == "ms-appx:///Data/ChannelIcons/weathernation.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Toonami Aftermath") && (c.Icon == "ms-appx:///Data/ChannelIcons/toonami.png"); }));

                // Upgraded / Replaced
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Bloomberg") && (c.Icon == "ms-appx:///Data/ChannelIcons/bloomberg.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "TV Shopping Network") && (c.Icon == "ms-appx:///Data/ChannelIcons/tvshoppingnetwork.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "360 North") && (c.Icon == "ms-appx:///Data/ChannelIcons/360north.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Jupiter Broadcasting") && (c.Icon == "ms-appx:///Data/ChannelIcons/jupiterbroadcasting.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "RadioU TV") && (c.Icon == "ms-appx:///Data/ChannelIcons/radiou.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "Spirit Television") && (c.Icon == "ms-appx:///Data/ChannelIcons/spirittelevision.png"); }));
                TempList.Remove(TempList.Find(delegate (Channel c) { return (c.Name == "NASA TV") && (c.Icon == "ms-appx:///Data/ChannelIcons/nasatv.png"); }));

                StorageFile updateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/updates.txt"));

                List<Channel> updates = await LoadChannelFile(updateFile, false);
                TempList.AddRange(updates);
            }

            await ChannelManager.SaveChannels(TempList);
        }
    }
}
