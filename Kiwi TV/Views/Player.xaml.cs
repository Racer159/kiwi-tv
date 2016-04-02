using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

                FavoriteCheckBox.IsChecked = nowPlaying.Favorite;

                if (nowPlaying.Type == "twitch")
                {
                    string channelName = TwitchAPI.GetChannelNameFromURL(nowPlaying.Source.AbsolutePath);
                    TwitchAccessToken token = await TwitchAPI.RetireveAccessToken(channelName);
                    if (await TwitchAPI.IsLive(channelName))
                    {
                        MainPlayer.Source = new Uri(nowPlaying.Source, "?allow_source=true&token=" + Uri.EscapeDataString(token.Token.Replace("\\", "")) + "&sig=" + Uri.EscapeDataString(token.Signature));
                        SetLiveCheckBoxValue(true);
                    }
                    else
                    {
                        TwitchChannel channelDesc = await TwitchAPI.RetreiveChannelDescription(channelName);
                        MainPlayer.PosterSource = new BitmapImage(new Uri(channelDesc.VideoBanner));
                        SetLiveCheckBoxValue(false);
                    }
                }
                else
                {
                    MainPlayer.Source = nowPlaying.Source;
                    SetLiveCheckBoxValue(true);
                }
            }

            MainPlayer.Play();
            base.OnNavigatedTo(e);
        }

        private async void FavoriteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                await ChannelManager.SaveFavorite(nowPlaying.Name, (bool)((CheckBox)sender).IsChecked);
            }
        }

        private void SetLiveCheckBoxValue(bool live)
        {
            LiveCheckBox.IsChecked = live;
            if (live)
            {
                ToolTipService.SetToolTip(LiveCheckBox, "Live Now");
            }
            else
            {
                ToolTipService.SetToolTip(LiveCheckBox, "Offline");
            }
        }
    }
}
