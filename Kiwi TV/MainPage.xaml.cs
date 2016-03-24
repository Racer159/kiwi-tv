using Kiwi_TV.Models;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Kiwi_TV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
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
            AddChannelButton.Visibility = Visibility.Collapsed;
            LanguagesBox.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Collapsed;
            NavPane.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            NavPane.IsPaneOpen = false;

            if (e.SourcePageType == typeof(Views.Player))
            {
                NavPane.DisplayMode = SplitViewDisplayMode.Overlay;

                ChannelsButton.IsChecked = false;
                FavoritesButton.IsChecked = false;
                FeedbackButton.IsChecked = false;
                SettingsButton.IsChecked = false;

                if (e.Parameter is Channel)
                {
                    TitleText.Text = ((Channel)e.Parameter).Name;
                } else
                {
                    TitleText.Text = "Now Playing";
                }
            }
            else if (e.SourcePageType == typeof(Views.Channels))
            {
                SearchBox.Visibility = Visibility.Visible;
                if (e.Parameter is bool && (bool)e.Parameter)
                {
                    TitleText.Text = "Favorites";
                }
                else
                {
                    TitleText.Text = "All Channels";
                    AddChannelButton.Visibility = Visibility.Visible;
                    LanguagesBox.Visibility = Visibility.Visible;
                }
            }
            else if (e.SourcePageType == typeof(Views.Feedback))
            {
                TitleText.Text = "Feedback";
            }
            else if (e.SourcePageType == typeof(Views.Settings))
            {
                TitleText.Text = "Settings";
            }
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
