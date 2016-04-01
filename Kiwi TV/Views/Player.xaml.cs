using Kiwi_TV.Logic;
using Kiwi_TV.Models;
using Kiwi_TV.Models.TwitchAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Player : Page
    {
        Channel nowPlaying = new Channel();

        public Player()
        {
            this.InitializeComponent();
            
            MainPlayer.AreTransportControlsEnabled = true;
            MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/bars.png"));
            
            // ABC MainPlayer.Source = new Uri("http://abclive.abcnews.com/i/abc_live4@136330/index_1200_av-b.m3u8?sd=10&b=1200&rebase=on");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Tuple<Channel, object>)
            {
                Tuple<Channel, object> parameter = (Tuple<Channel, object>)e.Parameter;
                nowPlaying = parameter.Item1;
                if (nowPlaying.Type == "twitch")
                {
                    TwitchAccessToken token = await TwitchAPI.RetireveAccessToken(TwitchAPI.GetChannelNameFromURL(nowPlaying.Source.AbsolutePath));
                    TwitchStreamDesc streamDesc = await TwitchAPI.RetreiveStreamDescription(TwitchAPI.GetChannelNameFromURL(nowPlaying.Source.AbsolutePath));
                    if (streamDesc.Stream != null)
                    {
                        MainPlayer.Source = new Uri(nowPlaying.Source, "?allow_source=true&token=" + Uri.EscapeDataString(token.Token.Replace("\\", "")) + "&sig=" + Uri.EscapeDataString(token.Signature));
                    }
                    else
                    {
                        TwitchChannel channelDesc = await TwitchAPI.RetreiveChannelDescription(TwitchAPI.GetChannelNameFromURL(nowPlaying.Source.AbsolutePath));
                        MainPlayer.PosterSource = new BitmapImage(new Uri(channelDesc.VideoBanner));
                    }
                }
                else
                {
                    MainPlayer.Source = nowPlaying.Source;
                }

                FavoriteCheckBox.IsChecked = nowPlaying.Favorite;
            }

            MainPlayer.Play();
            base.OnNavigatedTo(e);
        }

        private async void FavoriteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                await FileManager.SaveFavorite(nowPlaying.Name, (bool)((CheckBox)sender).IsChecked);
            }
        }

        private void LiveCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
