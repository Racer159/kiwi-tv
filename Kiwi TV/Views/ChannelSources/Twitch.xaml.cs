using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Twitch : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        TwitchViewModel _viewModel;

        public Twitch()
        {
            this.InitializeComponent();
            CategoryBox.ItemsSource = categories;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Entertainment");
            categories.Add("Sports");
            categories.Add("Gaming");
            categories.Add("Other");
            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectedItem = "Gaming";

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleImage.Margin = new Thickness(48, 0, 0, 0);
                ChannelFilters.Visibility = Visibility.Collapsed;
                ShortSearchButton.Visibility = Visibility.Visible;
                GridViewIconSize.Tag = 115;
                CategoryTextWrap.Orientation = Orientation.Vertical;
                CategoryWrap.Height = 83;
                CategoryText.Margin = new Thickness(0, 5, 0, 0);
                CategoryTextWrap.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                CategoryWrap.Margin = new Thickness(0, 0, -48, 48);
                CategoryWrap.Padding = new Thickness(0, 0, 48, 0);
                GridViewIconSize.Tag = 115;
                ChannelsGridView.SingleSelectionFollowsFocus = false;
                ChannelsGridView.XYFocusDown = AddButton;
                ChannelsGridView.XYFocusUp = SearchBox;
                XboxCommandWrap.Visibility = Visibility.Visible;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TwitchViewModel)
            {
                _viewModel = (TwitchViewModel)e.Parameter;
            }
            else
            {
                _viewModel = new TwitchViewModel();
            }

            this.DataContext = _viewModel;

            if (this._viewModel.Channels == null || this._viewModel.Channels.Length == 0)
            {
                await GetLiveNow();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Selected != null)
            {
                await ChannelManager.AddChannel(GenerateTwitchChannel(_viewModel.Selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + _viewModel.Selected.DisplayName).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a Twitch.tv channel to add.").ShowAsync();
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Selected != null)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateTwitchChannel(_viewModel.Selected), ""));
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
            string category = CategoryBox.SelectedItem == null ? "None" : CategoryBox.SelectedItem.ToString();
            return new Channel(selected.DisplayName, selected.Logo, "http://usher.ttvnw.net/api/channel/hls/" + selected.Name + ".m3u8", languages, false, category, "twitch", true);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await RunSearch();
        }

        private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await RunSearch();
            }
        }

        private void ChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.Selected != null)
            {
                CategoryWrap.Visibility = Visibility.Visible;
                ChannelsScrollViewer.Margin = new Thickness(0, 0, 0, 104);
            }
            else
            {
                CategoryWrap.Visibility = Visibility.Collapsed;
                ChannelsScrollViewer.Margin = new Thickness(0, 0, 0, 48);
            }
        }

        private async Task RunSearch()
        {
            _viewModel.Channels = new TwitchChannel[0];

            if (SearchBox.Text != "")
            {
                Header.Text = "Search Results";
                LoadingSpinner.Visibility = Visibility.Visible;
                TwitchSearchResults results = await TwitchAPI.RetrieveSearchResults(Uri.EscapeDataString(SearchBox.Text));
                LoadingSpinner.Visibility = Visibility.Collapsed;
                if (results != null)
                {
                    _viewModel.Channels = results.Channels;
                }

                if (ChannelsGridView.Items.Count > 0)
                {
                    ChannelsGridView.SelectedIndex = 0;
                }
                else
                {
                    NoResults.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task GetLiveNow()
        {
            _viewModel.Channels = new TwitchChannel[0];
            LoadingSpinner.Visibility = Visibility.Visible;
            TwitchSearchResults results = await TwitchAPI.RetrieveLiveStreams();
            LoadingSpinner.Visibility = Visibility.Collapsed;
            _viewModel.Channels = results.Channels;
        }

        private void MainChannelsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainChannelsGrid.ActualWidth < 550 && ChannelFilters.Visibility == Visibility.Visible)
            {
                ChannelFilters.Visibility = Visibility.Collapsed;
                ShortSearchButton.Visibility = Visibility.Visible;
                TitleImage.Visibility = Visibility.Visible;
                CategoryTextWrap.Orientation = Orientation.Vertical;
                CategoryWrap.Height = 83;
                CategoryText.Margin = new Thickness(0, 5, 0, 0);
                CategoryTextWrap.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (MainChannelsGrid.ActualWidth > 550 && ChannelFilters.Visibility == Visibility.Collapsed)
            {
                ChannelFilters.Visibility = Visibility.Visible;
                ShortSearchButton.Visibility = Visibility.Collapsed;
                CategoryTextWrap.Orientation = Orientation.Horizontal;
                CategoryWrap.Height = 60;
                CategoryText.Margin = new Thickness(0, 12, 0, 12);
                CategoryTextWrap.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }

        private void ShortSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                ChannelFilters.Width = Frame.ActualWidth;
            }

            ChannelFilters.Visibility = Visibility.Visible;
            ShortSearchButton.Visibility = Visibility.Collapsed;
            TitleImage.Visibility = Visibility.Collapsed;
            SearchBox.Focus(FocusState.Pointer);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.FocusState == FocusState.Unfocused && TitleImage.Visibility == Visibility.Collapsed)
            {
                ShortSearchButton.Visibility = Visibility.Visible;
                TitleImage.Visibility = Visibility.Visible;
                ChannelFilters.Visibility = Visibility.Collapsed;

                if (DeviceType == DeviceFormFactorType.Phone)
                {
                    ChannelFilters.Width = double.NaN;
                }
            }
        }
    }
}
