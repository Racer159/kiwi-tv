using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
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

            if (true)//channelsItem == null || !(channelsItem is StorageFile))
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
                            if (!favorite || data[4] == "y")
                            {
                                allChannels.Add(new Channel(data[1].Trim(), data[3].Trim(), lines[i + 1].Trim(), langs, data[4].Trim() == "y", data[5].Trim(), data[6].Trim()));
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

        public async static Task SaveChannels(List<Channel> channels)
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
    }
}
