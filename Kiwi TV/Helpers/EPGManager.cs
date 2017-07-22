using Kiwi_TV.API.EPG;
using Kiwi_TV.API.EPG.Models;
using Kiwi_TV.API.Twitch;
using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Kiwi_TV.Helpers
{
    class EPGManager
    {
        private const int MINS_TO_PIX = 3;

        public static async Task<List<Channel>> SetProgramInfo(List<Channel> channels)
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            Progress<ProgressTaskAsync> progress = new Progress<ProgressTaskAsync>();
            channels = await ChannelManager.SetLive(channels, false, progress);

            foreach (Channel c in channels)
            {
                c.Programs = await LoadProgramInfo(c, start);
            }

            return channels;
        }

        public static async Task<List<ProgramInfo>> LoadProgramInfo(Channel c, DateTime time)
        {
            List<ProgramInfo> programs = new List<ProgramInfo>();
            if (c.EPGSource != null)
            {
                // Download or Load XML EPG Data
                EPGTV data = await DownloadorLoadEPGXML(c, time);
                // Parse EPG File creating program infos along the way
                if (data != null)
                {
                    DateTime stop = time.AddHours(12);
                    for (int i = 0; i < data.Programmes.Length && time < stop; i ++)
                    {
                        // Load only each program in the future up to 12 hours of programs
                        EPGProgramme programme = data.Programmes[i];
                        DateTime programmeStop = programme.getStop();
                        if (programmeStop != null && programmeStop > time)
                        {
                            int width = ((int)(programmeStop - time).TotalMinutes * MINS_TO_PIX);
                            ProgramInfo info = new ProgramInfo(programme.Title.Text, width);
                            programs.Add(info);
                            time = programmeStop;
                        }
                    }
                }
                else
                {
                    ProgramInfo unset = new ProgramInfo("Unable to Load EPG", 540);
                }
            }
            else if (c.Type == "twitch")
            {
                // Load current game as 2 hour block
                string currentGameTitle = await TwitchAPI.GetCurrentGame(TwitchAPI.GetChannelNameFromURL(c.Source.AbsolutePath));
                ProgramInfo currentGame = new ProgramInfo(currentGameTitle, 360);
                programs.Add(currentGame);
            }
            else
            {
                // Set an unset one hour block
                ProgramInfo unset = new ProgramInfo("Channel EPG Not Set", 540);
                programs.Add(unset);
            }

            return programs;
        }

        private static async Task<EPGTV> DownloadorLoadEPGXML(Channel c, DateTime start)
        {
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            string filename = Uri.EscapeUriString(c.Name) + ".epg.xml";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(EPGTV));

            if (await localfolder.TryGetItemAsync(filename) != null)
            {
                StorageFile existing = await localfolder.GetFileAsync(filename);
                BasicProperties properties = await existing.GetBasicPropertiesAsync();
                if (properties.DateModified.AddDays(1) > start)
                {
                    try
                    {
                        EPGTV existingData = xmlSerializer.Deserialize(await existing.OpenStreamForReadAsync()) as EPGTV;
                        return existingData;
                    } catch { } //do nothing (try download again) 
                }
            }

            StorageFile file = await localfolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            EPGTV epgData = await EPGAPI.RetrieveEPGData(c.EPGSource.AbsoluteUri);
            if (epgData != null)
            {
                xmlSerializer.Serialize(await file.OpenStreamForWriteAsync(), epgData);
            }
            return epgData;
        } 
    }
}
