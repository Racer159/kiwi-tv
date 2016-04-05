using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        bool favorites = false;

        public Channels()
        {
            this.InitializeComponent();
            CategoriesItemsControl.ItemsSource = CategoryList;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                SearchBox.Visibility = Visibility.Collapsed;
                LanguagesBox.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
                GridViewIconSize.Tag = 115;
                SearchBox.Width = 125;
                LanguagesBox.Width = 125;
            }
        }

        private void ChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView && ((GridView)sender).SelectedItem is Channel)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>((Channel)((GridView)sender).SelectedItem, ""));
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
                    LanguagesBox.Visibility = Visibility.Collapsed;
                    TitleText.Text = "Favorites";
                    favorites = true;
                }
            }

            RefreshChannelList(ChannelList, "", "All Languages");
            LoadingSpinner.Visibility = Visibility.Collapsed;
            base.OnNavigatedTo(e);
            ChannelList = await ChannelManager.SetLive(ChannelList);
        }

        private void RefreshChannelList(List<Channel> channelList, string search, string language)
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
            channelList = channelList.FindAll(delegate (Channel c) { return c.Name.ToLower().Contains(search); });
            if (!(language == "All Languages" || language == ""))
            {
                channelList = channelList.FindAll(delegate (Channel c) { return c.Languages.Contains(language); });
            }



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

        private async void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox)
            {
                await ChannelManager.SaveFavorite((String)((CheckBox)sender).Tag, (bool)((CheckBox)sender).IsChecked);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LanguagesBox != null && SearchBox != null)
            {
                RefreshChannelList(ChannelList, SearchBox.Text.ToLower(), ((ComboBoxItem)LanguagesBox.SelectedValue).Content.ToString());
            }
        }

        private void LanguagesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguagesBox != null && SearchBox != null)
            {
                RefreshChannelList(ChannelList, SearchBox.Text.ToLower(), ((ComboBoxItem)LanguagesBox.SelectedValue).Content.ToString());
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchButton.Visibility = Visibility.Collapsed;
            TitleText.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Visible;
            SearchBox.Focus(FocusState.Pointer);

            if (!favorites)
            {
                LanguagesBox.Visibility = Visibility.Visible;
            }
        }

        private void MainChannelsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainChannelsGrid.ActualWidth < 550 && SearchBox.Visibility == Visibility.Visible)
            {
                SearchBox.Visibility = Visibility.Collapsed;
                LanguagesBox.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
                TitleText.Visibility = Visibility.Visible;
            }
            else if (MainChannelsGrid.ActualWidth > 550 && SearchBox.Visibility == Visibility.Collapsed)
            {
                SearchBox.Visibility = Visibility.Visible;
                if (!favorites)
                {
                    LanguagesBox.Visibility = Visibility.Visible;
                }

                SearchButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.FocusState == FocusState.Unfocused && LanguagesBox.FocusState == FocusState.Unfocused && TitleText.Visibility == Visibility.Collapsed && !LanguagesBox.IsSelectionBoxHighlighted)
            {
                SearchButton.Visibility = Visibility.Visible;
                TitleText.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Collapsed;
                LanguagesBox.Visibility = Visibility.Collapsed;
            }
        }

        private async void Border_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            if (sender is Border && ((Border)sender).Tag is Channel)
            {
                Channel channel = (Channel)((Border)sender).Tag;
                PopupMenu popup = new PopupMenu();
                UICommand delete = new UICommand("Delete", delegate (IUICommand command) { DeleteChannel(channel); });
                popup.Commands.Add(delete);
                await popup.ShowAsync(e.GetPosition((FrameworkElement)((FrameworkElement)((FrameworkElement)((FrameworkElement)Frame.Parent).Parent).Parent).Parent));
            }
        }

        private async void Border_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is Border && ((Border)sender).Tag is Channel)
            {
                Channel channel = (Channel)((Border)sender).Tag;
                PopupMenu popup = new PopupMenu();
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
                RefreshChannelList(ChannelList, SearchBox.Text.ToLower(), ((ComboBoxItem)LanguagesBox.SelectedValue).Content.ToString());
            }
        }
    }
}
