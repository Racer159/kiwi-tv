using Kiwi_TV.API.UStream;
using Kiwi_TV.API.UStream.Models;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UStream : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        UStreamViewModel _viewModel;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public UStream()
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
            CategoryBox.SelectedItem = "Other";

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
            if (e.Parameter is UStreamViewModel)
            {
                _viewModel = (UStreamViewModel)e.Parameter;
            }
            else
            {
                _viewModel = new UStreamViewModel();
            }

            this.DataContext = _viewModel;

            if (localSettings.Values["darkTheme"] is bool && (bool)localSettings.Values["darkTheme"])
            {
                _viewModel.LogoPath = "ms-appx:///Data/ChannelSources/ustream-logo-dark.png";
            }
            else
            {
                _viewModel.LogoPath = "ms-appx:///Data/ChannelSources/ustream-logo-light.png";
            }

            if (this._viewModel.Channels == null || this._viewModel.Channels.Length == 0)
            {
                await GetLiveNow();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Selected != null)
            {
                await ChannelManager.AddChannel(GenerateUStreamChannel(_viewModel.Selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + _viewModel.Selected.Title).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a UStream.tv channel to add.").ShowAsync();
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Selected != null)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateUStreamChannel(_viewModel.Selected), ""));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a UStream.tv channel to test.").ShowAsync();
            }
        }

        private Channel GenerateUStreamChannel(UStreamChannel selected)
        {
            List<string> languages = new List<string>();
            languages.Add("English");
            string category = CategoryBox.SelectedItem == null ? "None" : CategoryBox.SelectedItem.ToString();
            return new Channel(selected.Title, selected.Picture.ExtraLarge, "http://iphone-streaming.ustream.tv/uhls/" + selected.Id + "/streams/live/iphone/playlist.m3u8", languages, false, category, "ustream", true);
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
            _viewModel.Channels = new UStreamChannel[0];

            if (SearchBox.Text != "")
            {
                Header.Text = "Search Results";
                LoadingSpinner.Visibility = Visibility.Visible;
                _viewModel.Channels = await UStreamAPI.RetrieveSearchResults(Uri.EscapeDataString(SearchBox.Text));
                LoadingSpinner.Visibility = Visibility.Collapsed;

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
            LoadingSpinner.Visibility = Visibility.Visible;
            _viewModel.Channels = await UStreamAPI.RetrieveLiveStreams();
            LoadingSpinner.Visibility = Visibility.Collapsed;
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
