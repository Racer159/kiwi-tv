using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using Windows.UI;
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
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (localSettings.Values["freshInstall"] is bool)
            {
                FavoritesButton.IsChecked = true;
            }
            else
            {
                localSettings.Values["freshInstall"] = false;
                ContentView.Navigate(typeof(Views.StartTutorial), true);
            }
            //if (Microsoft.Services.Store.Engagement.Feedback.IsSupported)
            //{
            FeedbackButton.Visibility = Visibility.Visible;
            //}
            Windows.UI.ViewManagement.ApplicationView appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.TitleBar.ButtonBackgroundColor = Colors.White;
            appView.TitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 230, 230, 230);
            appView.TitleBar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(255, 204, 204, 204);
            appView.TitleBar.ButtonForegroundColor = Colors.Black;
            appView.TitleBar.ButtonHoverForegroundColor = Colors.Black;
            appView.TitleBar.ButtonPressedForegroundColor = Colors.Black;
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
            ContentView.Navigate(typeof(Views.AddChannel));
        }

        private void FeedbackButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Feedback));
            //await Microsoft.Services.Store.Engagement.Feedback.LaunchFeedbackAsync();
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

            if (e.SourcePageType == typeof(Views.Player) || e.SourcePageType == typeof(Views.StartTutorial))
            {
                NavPane.DisplayMode = SplitViewDisplayMode.Overlay;

                ChannelsButton.IsChecked = false;
                FavoritesButton.IsChecked = false;
                AddChannelButton.IsChecked = false;
                FeedbackButton.IsChecked = false;
                SettingsButton.IsChecked = false;
            }
            else if (e.SourcePageType == typeof(Views.Channels))
            {
                if (e.Parameter is bool && (bool)e.Parameter)
                {
                    FavoritesButton.IsChecked = true;
                }
                else
                {
                    ChannelsButton.IsChecked = true;
                }
            }
            else if (e.SourcePageType == typeof(Views.Feedback))
            {
                FeedbackButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Views.Settings))
            {
                SettingsButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Views.AddChannel))
            {
                AddChannelButton.IsChecked = true;
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
