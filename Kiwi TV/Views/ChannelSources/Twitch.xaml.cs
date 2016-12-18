using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// A page that allows adding a Twitch.tv channel
    /// </summary>
    public sealed partial class Twitch : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        TwitchViewModel _viewModel;

        /* Instantiate the page and setup device specific options */
        public Twitch()
        {
            this.InitializeComponent();
            CategoryBox.ItemsSource = categories;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            
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

        /* Handle the data context (view model) provided when navigated to */
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Load category list from category file
            List<string> categoriesFile = await CategoryHelper.LoadCategories();
            foreach (string category in categoriesFile)
            {
                categories.Add(category);
            }
            CategoryBox.ItemsSource = categories;
            if (categories.Contains("Gaming")) { CategoryBox.SelectedItem = "Gaming"; }

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

        /* Add the provided channel information the the channel list */
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

        /* Test the provided channel information in the player */
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

        /* Create a channel object from the provided channel information */
        private Channel GenerateTwitchChannel(TwitchChannel selected)
        {
            List<string> languages = new List<string>();
            languages.Add(TwitchAPI.ConvertLanguageCode(selected.Language));
            string category = CategoryBox.SelectedItem == null ? "None" : CategoryBox.SelectedItem.ToString();
            return new Channel(selected.DisplayName, selected.Logo, "http://usher.ttvnw.net/api/channel/hls/" + selected.Name + ".m3u8", languages, false, category, "twitch", true);
        }

        /* Run the search when the search button is pressed */
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await RunSearch();
        }

        /* Run the search when the enter key is pressed */
        private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await RunSearch();
            }
        }

        /* Show additional options when a channel is selected */
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

        /* Run a search on Twitch.tv */
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

        /* Get the currently live channels */
        private async Task GetLiveNow()
        {
            _viewModel.Channels = new TwitchChannel[0];
            LoadingSpinner.Visibility = Visibility.Visible;
            TwitchSearchResults results = await TwitchAPI.RetrieveLiveStreams();
            LoadingSpinner.Visibility = Visibility.Collapsed;
            _viewModel.Channels = results.Channels;
        }

        /* Show/hide UI elements when the size of the page changes */
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

        /* Expand the search options when the short search button is clicked */
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

        /* Collapse the search options when the short search box loses focus */
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
