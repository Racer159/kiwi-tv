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
using Windows.UI.Xaml.Media.Animation;
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

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleImage.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconSize.Tag = 115;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is UStreamViewModel)
            {
                _viewModel = (UStreamViewModel)e.Parameter;
                this.DataContext = _viewModel;
                if (localSettings.Values["darkTheme"] is bool && (bool)localSettings.Values["darkTheme"])
                {
                    _viewModel.LogoPath = "ms-appx:///Data/ChannelSources/ustream-logo-dark.png";
                }
                else
                {
                    _viewModel.LogoPath = "ms-appx:///Data/ChannelSources/ustream-logo-light.png";
                }

                if (this._viewModel.SearchChannels != null && this._viewModel.SearchChannels.Length > 0)
                {
                    CategoryWrap.Visibility = Visibility.Visible;
                    LiveWrap.Visibility = Visibility.Collapsed;
                }
                else if (this._viewModel.LiveChannels != null && this._viewModel.LiveChannels.Length > 0)
                {
                    CategoryWrap.Visibility = Visibility.Visible;
                    SearchWrap.Visibility = Visibility.Collapsed;
                }
                else
                {
                    await GetLiveNow();
                }

                
            }
            else
            {
                _viewModel = new UStreamViewModel();
                this.DataContext = _viewModel;
            }
        }

        private void CategoryBox_GotFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Entertainment");
            categories.Add("Sports");
            categories.Add("Gaming");
            categories.Add("Other");
        }

        private void CategoryBox_LostFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible && SearchChannelsGridView.SelectedItem is UStreamChannel)
            {
                UStreamChannel selected = (UStreamChannel)SearchChannelsGridView.SelectedItem;
                await ChannelManager.AddChannel(GenerateUStreamChannel(selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + selected.Title).ShowAsync();
            }
            else if (LiveWrap.Visibility == Visibility.Visible && LiveNowGridView.SelectedItem is UStreamChannel)
            {
                UStreamChannel selected = (UStreamChannel)LiveNowGridView.SelectedItem;
                await ChannelManager.AddChannel(GenerateUStreamChannel(selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + selected.Title).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a UStream channel to add.").ShowAsync();
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible && SearchChannelsGridView.SelectedItem is UStreamChannel)
            {
                UStreamChannel selected = (UStreamChannel)SearchChannelsGridView.SelectedItem;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateUStreamChannel(selected), ""));
            }
            else if (LiveWrap.Visibility == Visibility.Visible && LiveNowGridView.SelectedItem is UStreamChannel)
            {
                UStreamChannel selected = (UStreamChannel)LiveNowGridView.SelectedItem;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateUStreamChannel(selected), ""));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a UStream channel to test.").ShowAsync();
            }
        }

        private Channel GenerateUStreamChannel(UStreamChannel selected)
        {
            List<string> languages = new List<string>();
            languages.Add("English");
            return new Channel(selected.Title, selected.Picture.ExtraLarge, "http://iphone-streaming.ustream.tv/uhls/" + selected.Id + "/streams/live/iphone/playlist.m3u8", languages, false, CategoryBox.Text, "ustream", true);
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

        private void SearchChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedSearch = (UStreamChannel)SearchChannelsGridView.SelectedItem;
        }

        private void LiveNowGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible)
            {
                Frame.BackStack.Add(new PageStackEntry(typeof(Views.ChannelSources.UStream), new UStreamViewModel(), new DrillInNavigationTransitionInfo()));
                CategoryWrap.Visibility = Visibility.Visible;
                SearchWrap.Visibility = Visibility.Collapsed;
            }
        }

        private async Task RunSearch()
        {
            _viewModel.SearchChannels = new UStreamChannel[0];

            if (SearchBox.Text != "")
            {
                if (LiveWrap.Visibility == Visibility.Visible)
                {
                    Frame.BackStack.Add(new PageStackEntry(typeof(Views.ChannelSources.UStream), new UStreamViewModel(), new DrillInNavigationTransitionInfo()));
                    CategoryWrap.Visibility = Visibility.Visible;
                    LiveWrap.Visibility = Visibility.Collapsed;
                }

                SearchLoadingSpinner.Visibility = Visibility.Visible;
                _viewModel.SearchChannels = await UStreamAPI.RetrieveSearchResults(Uri.EscapeDataString(SearchBox.Text));
                SearchLoadingSpinner.Visibility = Visibility.Collapsed;

                if (SearchChannelsGridView.Items.Count > 0)
                {
                    SearchChannelsGridView.SelectedIndex = 0;
                }
            }
        }

        private async Task GetLiveNow()
        {
            LiveLoadingSpinner.Visibility = Visibility.Visible;
            _viewModel.LiveChannels = await UStreamAPI.RetrieveLiveStreams();
            LiveLoadingSpinner.Visibility = Visibility.Collapsed;
        }
    }
}
