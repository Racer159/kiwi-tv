using Kiwi_TV.Helpers;
using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Kiwi_TV
{
    /// <summary>
    /// The main page for the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceFormFactorType DeviceType;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        /* Instantiate the page and setup device specific options */
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            DeviceType = UWPHelper.GetDeviceFormFactorType();

            // Setup changes for the Xbox
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                NavPane.Margin = new Thickness(0, 48, -48, -27);
                MainAppBar.Margin = new Thickness(-48, -27, -48, 0);
                MainAppBar.Height = 75;
                HamburgerButton.Margin = new Thickness(48, 27, 0, 0);
                NavPane.CompactPaneLength = 48;
                ContentView.Margin = new Thickness(0, 0, 48, 27);
                NavButtons.Margin = new Thickness(0, 0, 0, 27);
                FavoritesButton.XYFocusUp = HamburgerButton;
                AddChannelButton.XYFocusDown = SettingsButton;
                SettingsButton.XYFocusUp = AddChannelButton;
                XboxNavBarPadding.Visibility = Visibility.Visible;

                HamburgerButton.UseSystemFocusVisuals = false;
                FavoritesButton.UseSystemFocusVisuals = false;
                ChannelsButton.UseSystemFocusVisuals = false;
                AddChannelButton.UseSystemFocusVisuals = false;
                FeedbackButton.UseSystemFocusVisuals = false;
                SettingsButton.UseSystemFocusVisuals = false;
            }

            // Start the start tutorial or new features tutorials
            if (localSettings.Values["freshInstall"] is bool)
            {
                FavoritesButton.IsChecked = true;
            }
            else
            {
                localSettings.Values["freshInstall"] = false;
                localSettings.Values["version15"] = false;
                ContentView.Navigate(typeof(Views.StartTutorial), true);
            }

            if (!(localSettings.Values["version15"] is bool))
            {
                localSettings.Values["version15"] = false;
                ContentView.Navigate(typeof(Views.NewFeatures), true);
            }

            // Enable the Feedback button if Feedback is supported on the device
            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                FeedbackButton.Visibility = Visibility.Visible;
            }

            // Setup the title bar based off of the theme
            Windows.UI.ViewManagement.ApplicationView appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (localSettings.Values["darkTheme"] is bool && (bool)localSettings.Values["darkTheme"])
            {
                appView.TitleBar.BackgroundColor = ColorHelper.FromArgb(255, 31, 31, 31);
                appView.TitleBar.InactiveBackgroundColor = ColorHelper.FromArgb(255, 31, 31, 31);
                appView.TitleBar.ForegroundColor = Colors.White;
                appView.TitleBar.InactiveForegroundColor = Colors.Gray;

                appView.TitleBar.ButtonBackgroundColor = ColorHelper.FromArgb(255, 32, 31, 31);
                appView.TitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 43, 43, 43);
                appView.TitleBar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(255, 64, 64, 64);
                appView.TitleBar.ButtonInactiveBackgroundColor = ColorHelper.FromArgb(255, 31, 31, 31);
                appView.TitleBar.ButtonForegroundColor = Colors.White;
                appView.TitleBar.ButtonHoverForegroundColor = Colors.White;
                appView.TitleBar.ButtonPressedForegroundColor = Colors.White;
                appView.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
            else
            {
                appView.TitleBar.BackgroundColor = ColorHelper.FromArgb(255, 240, 240, 240);
                appView.TitleBar.InactiveBackgroundColor = ColorHelper.FromArgb(255, 240, 240, 240);
                appView.TitleBar.ForegroundColor = Colors.Black;
                appView.TitleBar.InactiveForegroundColor = Colors.Gray;

                appView.TitleBar.ButtonBackgroundColor = ColorHelper.FromArgb(255, 240, 240, 240);
                appView.TitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 220, 220, 220);
                appView.TitleBar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(255, 200, 200, 200);
                appView.TitleBar.ButtonInactiveBackgroundColor = ColorHelper.FromArgb(255, 240, 240, 240);
                appView.TitleBar.ButtonForegroundColor = Colors.Black;
                appView.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                appView.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                appView.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
        }

        /* Open or close the nav pane when the hamburger button is clicked */
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavPane.IsPaneOpen = !NavPane.IsPaneOpen;
        }

        /* Navigate to the favorites page or guide when the favorites button is checked */
        private void FavoritesButton_Checked(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["electronicProgramGuide"] is bool && (bool)localSettings.Values["electronicProgramGuide"])
            {
                ContentView.Navigate(typeof(Views.Guide), true);
            }
            else
            {
                ContentView.Navigate(typeof(Views.Channels), true);
            }
        }

        /* Navigate to the all channels page when the channels button is checked */
        private void ChannelsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Channels), false);
        }

        /* Navigate to the add channels page when the add channel button is checked */
        private void AddChannelButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.AddChannel));
        }

        /* Navigate to the feedback page when the feedback button is checked */
        private async void FeedbackButton_Checked(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        /* Navigate to the settings page when the settings button is checked */
        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentView.Navigate(typeof(Views.Settings));
        }

        /* Change display based off of current page */
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

            if (e.SourcePageType == typeof(Views.Player) || 
                e.SourcePageType == typeof(Views.StartTutorial) || 
                e.SourcePageType == typeof(Views.NewFeatures))
            {
                NavPane.DisplayMode = SplitViewDisplayMode.Overlay;

                ChannelsButton.IsChecked = false;
                FavoritesButton.IsChecked = false;
                AddChannelButton.IsChecked = false;
                FeedbackButton.IsChecked = false;
                SettingsButton.IsChecked = false;
            }
            else if (e.SourcePageType == typeof(Views.Channels) || e.SourcePageType == typeof(Views.Guide))
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
            else if (e.SourcePageType == typeof(Views.Settings))
            {
                SettingsButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Views.AddChannel) || 
                e.SourcePageType == typeof(Views.ChannelSources.Custom) || 
                e.SourcePageType == typeof(Views.ChannelSources.Twitch) || 
                e.SourcePageType == typeof(Views.ChannelSources.UStream))
            {
                AddChannelButton.IsChecked = true;
            }
        }

        /* Navigate back when the back button is pressed */
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentView != null && ContentView.CanGoBack)
            {
                e.Handled = true;
                ContentView.GoBack();
            }
        }
        
        /* Open and close the nav pane when the view button is pressed */
        private void Grid_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox && (e.Key == Windows.System.VirtualKey.GamepadView || e.Key == Windows.System.VirtualKey.NavigationView))
            {
                NavPane.IsPaneOpen = !NavPane.IsPaneOpen;
            }
        }
    }
}
