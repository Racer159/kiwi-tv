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

namespace Kiwi_TV.Views
{
    /// <summary>
    /// A page that handles the playing the selected channel
    /// </summary>
    public sealed partial class Player : Page
    {
        DeviceFormFactorType DeviceType;
        Channel nowPlaying = new Channel();
        private DisplayRequest dispRequest = null;
        SystemMediaTransportControls systemControls;

        /* Instantiate the page and setup device specific options */
        public Player()
        {
            this.InitializeComponent();
            this.InitializeTransportControls();
            DeviceType = UWPHelper.GetDeviceFormFactorType();

            MainPlayer.AreTransportControlsEnabled = true;
            MainPlayer.PosterSource = null;
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                Background.Margin = new Thickness(-48, 0, -48, -27);
                Background.Padding = new Thickness(48, 0, 48, 27);
            }
        }

        /* Initialize the system player controls for background operation */
        void InitializeTransportControls()
        {
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += systemControls_ButtonPressed;
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;
        }

        /* Handle the system player controls for background operation */
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

        /* Tell the player to play the media */
        private async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPlayer.Play();
            });
        }

        /* Tell the player to pause the media */
        private async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPlayer.Pause();
            });
        }

        /* Handle the channel information given when navigated to */
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
                            MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/Bars.png"));
                        }

                        SetLiveCheckBoxValue(false);
                    }
                }
                else
                {
                    try
                    {
                        MainPlayer.Source = nowPlaying.Source;
                        SetLiveCheckBoxValue(true);
                    }
                    catch
                    {
                        SetLiveCheckBoxValue(false);
                    }
                }
            }

            MainPlayer.Play();
            base.OnNavigatedTo(e);
        }

        /* Handle favoriting a channel when the favorite checkbox is clicked */
        private async void FavoriteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                await ChannelManager.SaveFavorite(nowPlaying.Name, (bool)((CheckBox)sender).IsChecked);
            }
        }

        /* Show/hide UI elements related to live status */
        private void SetLiveCheckBoxValue(bool live)
        {
            LiveCheckBox.IsChecked = live;
            if (live)
            {
                ToolTipService.SetToolTip(LiveCheckBox, "Live Now");
                OfflineText.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (MainPlayer.PosterSource == null)
                {
                    MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/Bars.png"));
                }
                ToolTipService.SetToolTip(LiveCheckBox, "Offline");
                OfflineText.Visibility = Visibility.Visible;
            }
        }

        /* Deal with background audio when media is opened in the player */
        private void MainPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (dispRequest == null)
            {
                dispRequest = new DisplayRequest();
                dispRequest.RequestActive();
            }
            Debug.Write(MainPlayer.AudioCategory);
        }

        /* Deal with background audio when media is ended in the player */
        private void MainPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (dispRequest != null)
            {
                dispRequest.RequestRelease();
                dispRequest = null;
            }
        }

        /* Set live status to false when the media fails */
        private void MainPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            SetLiveCheckBoxValue(false);
        }

        /* Update the system controls when the player state changes */
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

        /* Fullscreen the player when on an Xbox */
        private void MainPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                MainPlayer.IsFullWindow = true;
            }
        }
    }
}
