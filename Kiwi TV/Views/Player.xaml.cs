using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.System.Display;
using System.Diagnostics;
using Windows.Media;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Player : Page
    {
        Channel nowPlaying = new Channel();
        private DisplayRequest dispRequest = null;
        SystemMediaTransportControls systemControls;

        public Player()
        {
            this.InitializeComponent();
            this.InitializeTransportControls();

            MainPlayer.AreTransportControlsEnabled = true;
            MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/Bars.png"));
            
            // ABC MainPlayer.Source = new Uri("http://abclive.abcnews.com/i/abc_live4@136330/index_1200_av-b.m3u8?sd=10&b=1200&rebase=on");
        }

        void InitializeTransportControls()
        {
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += systemControls_ButtonPressed;
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;
        }

        void systemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Pause:
                    PauseMedia();
                    break;
                case SystemMediaTransportControlsButton.Play:
                    PlayMedia();
                    break;
                default:
                    break;
            }
        }

        private async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPlayer.Play();
            });
        }

        private async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPlayer.Pause();
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Tuple<Channel, object>)
            {
                Tuple<Channel, object> parameter = (Tuple<Channel, object>)e.Parameter;
                nowPlaying = parameter.Item1;
                SetLiveCheckBoxValue(nowPlaying.Live);
                FavoriteCheckBox.IsChecked = nowPlaying.Favorite;
                TitleText.Text = nowPlaying.Name;

                if (nowPlaying.Type == "twitch")
                {
                    string channelName = TwitchAPI.GetChannelNameFromURL(nowPlaying.Source.AbsolutePath);
                    TwitchAccessToken token = await TwitchAPI.RetireveAccessToken(channelName);
                    if (await TwitchAPI.IsLive(channelName))
                    {
                        MainPlayer.Source = new Uri(nowPlaying.Source, "?allow_source=true&token=" + Uri.EscapeDataString(token.Token.Replace("\\", "")) + "&sig=" + Uri.EscapeDataString(token.Signature));
                        SetLiveCheckBoxValue(true);
                    }
                    else if (token != null)
                    {
                        TwitchChannel channelDesc = await TwitchAPI.RetreiveChannelDescription(channelName);

                        Uri source;
                        Uri.TryCreate(channelDesc.VideoBanner, UriKind.RelativeOrAbsolute, out source);
                        if (source != null)
                        {
                            MainPlayer.PosterSource = new BitmapImage(source);
                        }
                        else
                        {
                            MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/Black.png"));
                        }

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
                OfflineText.Visibility = Visibility.Visible;
            }
        }

        private void MainPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (dispRequest == null)
            {
                dispRequest = new DisplayRequest();
                dispRequest.RequestActive();
            }
            Debug.Write(MainPlayer.AudioCategory);
        }

        private void MainPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (dispRequest != null)
            {
                dispRequest.RequestRelease();
                dispRequest = null;
            }
        }

        private void MainPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            SetLiveCheckBoxValue(false);
        }

        private void MainPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (MainPlayer.CurrentState)
            {
                case MediaElementState.Closed:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                case MediaElementState.Paused:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Playing:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Stopped:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                default:
                    break;
            }
        }
    }
}
