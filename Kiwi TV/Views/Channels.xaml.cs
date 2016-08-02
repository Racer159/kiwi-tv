using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Channels : Page
    {
        DeviceFormFactorType DeviceType;
        List<Channel> ChannelList = new List<Channel>();
        ObservableCollection<Category> CategoryList = new ObservableCollection<Category>();
        List<StorageFile> Sharefiles = new List<StorageFile>();
        Channel FocusedChannel = new Channel();

        public Channels()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                ChannelFilters.Visibility = Visibility.Collapsed;
                ShortSearchButton.Visibility = Visibility.Visible;
                GridViewIconSize.Tag = 115;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                ChannelsGridView.SingleSelectionFollowsFocus = false;
                GridViewIconSize.Tag = 115;
                MainScrollViewer.Margin = new Thickness(0, 0, 0, -27);
                ChannelsGridView.Margin = new Thickness(20, 0, 20, 27);
                MultiShareButton.Visibility = Visibility.Collapsed;
                GamepadButtons.Visibility = Visibility.Visible;
                ChannelsGridView.XYFocusUp = SearchBox;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is bool)
            {
                ChannelList = await ChannelManager.LoadChannels((bool)e.Parameter, false);
                ChannelList.Sort();
                if ((bool)e.Parameter)
                {
                    TitleText.Text = "Favorites";
                }
                else
                {
                    TitleText.Text = "All Channels";
                }
            }

            RefreshChannelList(ChannelList, "");
            LoadingSpinner.Visibility = Visibility.Collapsed;
            base.OnNavigatedTo(e);

            Progress<ProgressTaskAsync> progress = new Progress<ProgressTaskAsync>();
            ChannelList = await ChannelManager.SetLive(ChannelList, false, progress);
        }

        private void RefreshChannelList(List<Channel> channelList, string search)
        {
            if (channelList.Count == 0)
            {
                NoContentHeader.Visibility = Visibility.Visible;
            }
            else
            {
                NoContentHeader.Visibility = Visibility.Collapsed;
            }

            CategoryList.Clear();
            List<Channel> searchResults = new List<Channel>();
            string[] searchTerms = search.Split(' ');
            foreach (string term in searchTerms)
            {
                searchResults.AddRange(channelList.FindAll(delegate (Channel c) {
                    foreach (string lang in c.Languages) {
                        if (lang.ToLower().Contains(term)) {
                            return true;
                        }
                    }
                    return c.Name.ToLower().Contains(term);
                }));
            }

            channelList = searchResults;
            
            if (channelList.Count == 0 && NoContentHeader.Visibility == Visibility.Collapsed)
            {
                NoSearchHeader.Visibility = Visibility.Visible;
            }
            else
            {
                NoSearchHeader.Visibility = Visibility.Collapsed;
            }

            ChannelManager.LoadCategories(channelList, CategoryList);
        }

        private void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox)
            {
                FavoriteChannel((Channel)((CheckBox)sender).Tag, (bool)((CheckBox)sender).IsChecked);
            }
        }

        private void MainChannelsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainChannelsGrid.ActualWidth < 550 && ChannelFilters.Visibility == Visibility.Visible)
            {
                ChannelFilters.Visibility = Visibility.Collapsed;
                ShortSearchButton.Visibility = Visibility.Visible;
                TitleText.Visibility = Visibility.Visible;
            }
            else if (MainChannelsGrid.ActualWidth > 550 && ChannelFilters.Visibility == Visibility.Collapsed)
            {
                ChannelFilters.Visibility = Visibility.Visible;
                ShortSearchButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ShortSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                ChannelFilters.Width = Frame.ActualWidth;
            }

            if ((bool)MultiSelectButton.IsChecked)
            {
                MultiSelectButton.IsChecked = false;
                HideMultiSelectOptions();
            }

            ChannelFilters.Visibility = Visibility.Visible;
            ShortSearchButton.Visibility = Visibility.Collapsed;
            TitleText.Visibility = Visibility.Collapsed;
            SearchBox.Focus(FocusState.Pointer);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.FocusState == FocusState.Unfocused && TitleText.Visibility == Visibility.Collapsed)
            {
                ShortSearchButton.Visibility = Visibility.Visible;
                TitleText.Visibility = Visibility.Visible;
                ChannelFilters.Visibility = Visibility.Collapsed;

                if (DeviceType == DeviceFormFactorType.Phone)
                {
                    ChannelFilters.Width = double.NaN;
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox != null)
            {
                RefreshChannelList(ChannelList, SearchBox.Text.ToLower());
            }
        }

        private void SearchBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                RefreshChannelList(ChannelList, SearchBox.Text.ToLower());
            }
        }

        private async void Border_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is Border && ((Border)sender).Tag is Channel)
            {
                Channel channel = (Channel)((Border)sender).Tag;
                PopupMenu popup = new PopupMenu();

                if (channel.Favorite)
                {
                    UICommand favorite = new UICommand("Unfavorite", delegate (IUICommand command) { FavoriteChannel(channel, false); });
                    popup.Commands.Add(favorite);
                }
                else
                {
                    UICommand favorite = new UICommand("Favorite", delegate (IUICommand command) { FavoriteChannel(channel, true); });
                    popup.Commands.Add(favorite);
                }

                UICommand edit = new UICommand("Edit", delegate (IUICommand command) { EditChannel(channel); });
                popup.Commands.Add(edit);

                UICommand delete = new UICommand("Delete", delegate (IUICommand command) { DeleteChannel(channel); });
                popup.Commands.Add(delete);

                await popup.ShowAsync(e.GetPosition((FrameworkElement)((FrameworkElement)((FrameworkElement)((FrameworkElement)Frame.Parent).Parent).Parent).Parent));
            }
        }

        private async void DeleteChannel(Channel channel)
        {
            var dialog = new MessageDialog("This will remove this channel from all channel lists.", "Delete " + channel.Name + "?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                await ChannelManager.RemoveChannel(channel);

                ChannelList.Remove(ChannelList.Find(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));

                for ( int i = 0; i < CategoryList.Count; i++)
                {
                    int j = CategoryList[i].Channels.IndexOf(channel);
                    if (j > -1) { CategoryList[i].Channels.RemoveAt(j); }

                    if (CategoryList[i].Channels.Count == 0)
                    {
                        CategoryList.Remove(CategoryList[i]);
                        i--;
                    }
                }
            }
        }

        private void EditChannel(Channel channel)
        {
            if (DeviceType == DeviceFormFactorType.Xbox) { ElementSoundPlayer.Play(ElementSoundKind.Invoke); }
            CustomViewModel cvm = new CustomViewModel();
            cvm.EditMode = true;
            cvm.EditChannel = channel;
            Frame.Navigate(typeof(Views.ChannelSources.Custom), cvm);
        }

        private async void FavoriteChannel(Channel channel, bool favorite)
        {
            if (DeviceType == DeviceFormFactorType.Xbox) { ElementSoundPlayer.Play(ElementSoundKind.Invoke); }
            channel.Favorite = favorite;
            await ChannelManager.SaveFavorite(channel.Name, favorite);
        }

        private void MultiSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MultiSelectButton.IsChecked)
            {
                MultiSelectOptions.Visibility = Visibility.Visible;

                if (DeviceType == DeviceFormFactorType.Phone)
                {
                    TitleText.Visibility = Visibility.Collapsed;
                }

                ChannelsGridView.SelectionMode = ListViewSelectionMode.Multiple;
                ChannelsGridView.IsItemClickEnabled = false;
            }
            else
            {
                HideMultiSelectOptions();
            }
        }

        private void HideMultiSelectOptions()
        {
            MultiSelectOptions.Visibility = Visibility.Collapsed;

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Visibility = Visibility.Visible;
            }

            ChannelsGridView.SelectionMode = ListViewSelectionMode.None;
            ChannelsGridView.IsItemClickEnabled = true;
        }

        private async void MultiDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("This will remove any selected channels from all channel lists.", "Delete Channels?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                List<Channel> removed = new List<Channel>();

                for ( int i = 0; i < ChannelsGridView.SelectedItems.Count; i++)
                {
                    Channel c = (Channel)ChannelsGridView.SelectedItems[i];
                    ChannelList.Remove(c);
                    removed.Add(c);

                    for (int j = 0; j < CategoryList.Count; j++)
                    {
                        int k = CategoryList[j].Channels.IndexOf(c);
                        if (k > -1) { CategoryList[j].Channels.RemoveAt(k); }

                        if (CategoryList[j].Channels.Count == 0)
                        {
                            CategoryList.Remove(CategoryList[j]);
                            j--;
                        }
                    }

                    i--;
                }

                await ChannelManager.RemoveChannels(removed);
            }
        }

        private async void MultiShareButton_Click(object sender, RoutedEventArgs e)
        {
            List<Channel> shareChannels = new List<Channel>();

            foreach (Channel c in ChannelsGridView.SelectedItems)
            {
                shareChannels.Add(c);
            }

            Sharefiles.Clear();
            Sharefiles.Add(await ChannelManager.GetFileToShare(shareChannels));

            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName + " Channel List";
            request.Data.Properties.Description = "Extended M3U8 Channel List File";
            request.Data.SetStorageItems(Sharefiles);
        }

        private void ChannelsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(bool)MultiSelectButton.IsChecked)
            {
                Channel c = e.ClickedItem as Channel;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(c, ""));
            }
        }

        private async void ChannelsGridView_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Menu || e.Key == Windows.System.VirtualKey.Application ||
                (DeviceType == DeviceFormFactorType.Xbox && (e.Key == Windows.System.VirtualKey.GamepadMenu || e.Key == Windows.System.VirtualKey.NavigationMenu)))
            {
                foreach (Channel channel in ChannelList)
                {
                    GridViewItem g = (GridViewItem)ChannelsGridView.ContainerFromItem(channel);
                    FocusState f = g.FocusState;
                    if (f == FocusState.Keyboard)
                    {
                        PopupMenu popup = new PopupMenu();
                        
                        if (channel.Favorite)
                        {
                            UICommand favorite = new UICommand("Unfavorite", delegate (IUICommand command) { FavoriteChannel(channel, false); });
                            popup.Commands.Add(favorite);
                        }
                        else
                        {
                            UICommand favorite = new UICommand("Favorite", delegate (IUICommand command) { FavoriteChannel(channel, true); });
                            popup.Commands.Add(favorite);
                        }

                        UICommand edit = new UICommand("Edit", delegate (IUICommand command) { EditChannel(channel); });
                        popup.Commands.Add(edit);

                        UICommand delete = new UICommand("Delete", delegate (IUICommand command) { DeleteChannel(channel); });
                        popup.Commands.Add(delete);

                        GeneralTransform gt = g.TransformToVisual((FrameworkElement)((FrameworkElement)((FrameworkElement)((FrameworkElement)Frame.Parent).Parent).Parent).Parent);

                        if (DeviceType == DeviceFormFactorType.Xbox) { ElementSoundPlayer.Play(ElementSoundKind.Show); }
                        await popup.ShowAsync(gt.TransformPoint(new Windows.Foundation.Point(40, 0)));
                    }
                }
            }
            else if (DeviceType == DeviceFormFactorType.Xbox && e.Key == Windows.System.VirtualKey.GamepadY)
            {
                foreach (Channel channel in ChannelList)
                {
                    GridViewItem g = (GridViewItem)ChannelsGridView.ContainerFromItem(channel);
                    FocusState f = g.FocusState;
                    if (f == FocusState.Keyboard)
                    {
                        if (channel.Favorite) {  FavoriteChannel(channel, false);  } else { FavoriteChannel(channel, true); }
                    }
                }
            }
            else if (DeviceType == DeviceFormFactorType.Xbox && e.Key == Windows.System.VirtualKey.GamepadX)
            {
                foreach (Channel channel in ChannelList)
                {
                    GridViewItem g = (GridViewItem)ChannelsGridView.ContainerFromItem(channel);
                    FocusState f = g.FocusState;
                    if (f == FocusState.Keyboard)
                    {
                        DeleteChannel(channel);
                    }
                }
            }
        }

        private void ChannelsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox && ChannelList.Count > 0)
            {
                ((GridViewItem)ChannelsGridView.ContainerFromItem(ChannelList[0])).Focus(FocusState.Keyboard);
            }
        }
    }
}
