using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddChannel : Page
    {
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();

        public AddChannel()
        {
            this.InitializeComponent();
            CustomCategory.ItemsSource = categories;
            CustomLanguage.ItemsSource = languages;
        }

        private async void AddCustomButton_Click(object sender, RoutedEventArgs e)
        {
            Uri source;
            Uri.TryCreate(CustomSourceURL.Text, UriKind.RelativeOrAbsolute, out source);
            if (source.IsAbsoluteUri)
            {
                List<string> languages = new List<string>();
                languages.Add(CustomLanguage.Text);
                Channel newChannel = new Channel(CustomName.Text, CustomImageURL.Text, CustomSourceURL.Text, languages, false, CustomCategory.Text, "iptv", true);
                await ChannelManager.AddChannel(newChannel);
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("I'm sorry, but I cannot add that channel because the specified video URL is invalid.").ShowAsync();
            }
        }

        private void CustomCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Sports");
            categories.Add("Kids");
            categories.Add("Gaming");
            categories.Add("Other");
        }

        private void CustomCategory_LostFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
        }

        private void CustomLanguage_GotFocus(object sender, RoutedEventArgs e)
        {
            languages.Clear();
            languages.Add("English");
            languages.Add("French");
            languages.Add("Spanish");
            languages.Add("German");
            languages.Add("Russian");
            languages.Add("Arabic");
        }

        private void CustomLanguage_LostFocus(object sender, RoutedEventArgs e)
        {
            languages.Clear();
        }

        private async void CustomChannelsGridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is GridView)
            {
                Uri source;
                Uri.TryCreate(CustomSourceURL.Text, UriKind.RelativeOrAbsolute, out source);
                if (source.IsAbsoluteUri)
                {
                    Channel c = new Channel(CustomName.Text, CustomImageURL.Text, CustomSourceURL.Text, new List<string>(), (bool)FavoriteCheckBox.IsChecked, CustomCategory.Text, "iptv", true);
                    Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(c, ""));
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("I'm sorry, but I cannot load that channel because the specified video URL is invalid.").ShowAsync();
                }
            }
        }
    }
}
