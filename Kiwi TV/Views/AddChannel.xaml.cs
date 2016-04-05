using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddChannel : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        AddChannelViewModel _viewModel;

        public AddChannel()
        {
            this.InitializeComponent();
            CustomCategory.ItemsSource = categories;
            CustomLanguage.ItemsSource = languages;
            TwitchCategory.ItemsSource = categories;
            DeviceType = UWPHelper.GetDeviceFormFactorType();

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconSize.Tag = 115;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           if (e.Parameter is AddChannelViewModel)
            {
                _viewModel = (AddChannelViewModel)e.Parameter;
                this.DataContext = _viewModel;
            }
            else
            {
                _viewModel = new AddChannelViewModel();
            }
        }

        private async void AddCustomButton_Click(object sender, RoutedEventArgs e)
        {
            Uri source;
            Uri.TryCreate(CustomSourceURL.Text, UriKind.RelativeOrAbsolute, out source);
            if (source.IsAbsoluteUri)
            {
                await ChannelManager.AddChannel(GenerateCustomChannel());
                await new Windows.UI.Popups.MessageDialog("Successfully added " + CustomName.Text).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("I'm sorry, but I cannot add that channel because the specified video URL is invalid.").ShowAsync();
            }
        }

        private async void TestCustomButton_Click(object sender, RoutedEventArgs e)
        {
            Uri source;
            Uri.TryCreate(CustomSourceURL.Text, UriKind.RelativeOrAbsolute, out source);
            if (source.IsAbsoluteUri)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateCustomChannel(), ""));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("I'm sorry, but I cannot load that channel because the specified video URL is invalid.").ShowAsync();
            }
        }

        private Channel GenerateCustomChannel()
        {
            List<string> languages = new List<string>();
            languages.Add(CustomLanguage.Text);
            return new Channel(CustomName.Text, CustomImageURL.Text, CustomSourceURL.Text, languages, false, CustomCategory.Text, "iptv", true);
        }

        private async void SuggestCustomButton_Click(object sender, RoutedEventArgs e)
        {
            object output = await MailHelper.SendFeedbackEmail("", "Suggestion", "Hi, I want to suggest you add '" + CustomName.Text +
                "' as a default channel.  Below are the sources I used:\n\nImage: " + CustomImageURL.Text + "\nVideo: " + CustomSourceURL.Text + "\n\nThank you!");

            if (!(output is Exception))
            {
                await new Windows.UI.Popups.MessageDialog("Successfully received your suggestion. Thank you!").ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("I'm sorry, but I encoutered an error.  Please try to send your suggestion later.").ShowAsync();
            }
        }

        private void CustomCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Entertainment");
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

        private async void AddTwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (TwitchChannelsGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)TwitchChannelsGridView.SelectedItem;
                await ChannelManager.AddChannel(GenerateTwitchChannel(selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + selected.DisplayName).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a Twitch.tv channel to add.").ShowAsync();
            }
        }

        private async void TestTwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (TwitchChannelsGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)TwitchChannelsGridView.SelectedItem;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateTwitchChannel(selected), ""));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a Twitch.tv channel to test.").ShowAsync();
            }
        }

        private Channel GenerateTwitchChannel(TwitchChannel selected)
        {
            List<string> languages = new List<string>();
            languages.Add(TwitchAPI.ConvertLanguageCode(selected.Language));
            return new Channel(selected.DisplayName, selected.Logo, "http://usher.ttvnw.net/api/channel/hls/" + selected.Name + ".m3u8", languages, false, TwitchCategory.Text, "twitch", true);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await RunTwitchSearch();
        }

        private async void TwitchSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await RunTwitchSearch();
            }
        }

        private async Task RunTwitchSearch()
        {
            _viewModel.TwitchChannels = new TwitchChannel[0];

            if (TwitchSearch.Text != "")
            {
                TwitchLoadingSpinner.Visibility = Visibility.Visible;
                TwitchSearchResults results = await TwitchAPI.RetrieveSearchResults(Uri.EscapeDataString(TwitchSearch.Text));
                TwitchLoadingSpinner.Visibility = Visibility.Collapsed;
                _viewModel.TwitchChannels = results.Channels;
            }
        }
    }
}
