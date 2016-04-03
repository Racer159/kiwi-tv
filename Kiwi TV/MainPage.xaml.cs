using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Kiwi_TV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceFormFactorType DeviceType;

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            FavoritesButton.IsChecked = true;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavPane.IsPaneOpen = !NavPane.IsPaneOpen;
        }

        private void FavoritesButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Channels), true);
        }

        private void ChannelsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Channels), false);
        }

        private void AddChannelButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.AddChannel), new AddChannelViewModel());
        }

        private void FeedbackButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Feedback));
        }

        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Settings));
        }

        private void ContentView_Navigated(object sender, NavigationEventArgs e)
        {
            

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                NavPane.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                NavPane.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
            NavPane.IsPaneOpen = false;

            if (e.SourcePageType == typeof(Views.Player))
            {
                NavPane.DisplayMode = SplitViewDisplayMode.Overlay;

                ChannelsButton.IsChecked = false;
                FavoritesButton.IsChecked = false;
                AddChannelButton.IsChecked = false;
                FeedbackButton.IsChecked = false;
                SettingsButton.IsChecked = false;

                if (e.Parameter is Tuple<Channel,object>)
                {
                    TitleText.Text = ((Tuple<Channel, object>)e.Parameter).Item1.Name;
                } else
                {
                    TitleText.Text = "Now Playing";
                }
            }
            else if (e.SourcePageType == typeof(Views.Channels))
            {
                if (e.Parameter is bool && (bool)e.Parameter)
                {
                    TitleText.Text = "Favorites";
                    FavoritesButton.IsChecked = true;
                }
                else
                {
                    TitleText.Text = "All Channels";
                    ChannelsButton.IsChecked = true;
                }
            }
            else if (e.SourcePageType == typeof(Views.Feedback))
            {
                TitleText.Text = "Feedback";
                FeedbackButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Views.Settings))
            {
                TitleText.Text = "Settings";
                SettingsButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Views.AddChannel))
            {
                TitleText.Text = "Add Channel";
                AddChannelButton.IsChecked = true;
            }
            else
            {
                TitleText.Text = "";
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentView != null && ContentView.CanGoBack)
            {
                e.Handled = true;
                ContentView.GoBack();
            }
        }
    }
}
