using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// A page for loading in a custom M3U8 file
    /// </summary>
    public sealed partial class File : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        FileViewModel _viewModel;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        List<Channel> ChannelList = new List<Channel>();

        /* Instantiate the page, and setup device specific options */
        public File()
        {
            this.InitializeComponent();
            CustomCategory.ItemsSource = categories;
            CustomLanguage.ItemsSource = languages;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconSize.Tag = 115;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                GridViewIconSize.Tag = 115;
                ProgressWrap.Margin = new Thickness(0, 0, -48, 48);
                ProgressWrap.Padding = new Thickness(0, 0, 48, 0);
                ChannelsGridView.SingleSelectionFollowsFocus = false;
                ChannelsGridView.XYFocusDown = AddButton;
                XboxCommandWrap.Visibility = Visibility.Visible;
            }
        }

        /* Handle the data context (view model) provided when navigated to */
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Load category list from category file
            List<string> categoriesFile = await CategoryHelper.LoadCategories();
            foreach (string category in categoriesFile)
            {
                categories.Add(category);
            }
            CustomCategory.ItemsSource = categories;

            // Set languages list
            languages.Add("English");
            languages.Add("French");
            languages.Add("Spanish");
            languages.Add("German");
            languages.Add("Russian");
            languages.Add("Arabic");
            CustomLanguage.ItemsSource = languages;

            if (e.Parameter is FileViewModel)
            {
                _viewModel = (FileViewModel)e.Parameter;
                this.DataContext = _viewModel;

                if (_viewModel.FileChannels != null && _viewModel.FileChannels.Count > 0)
                {
                    NoChannels.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _viewModel = new FileViewModel();
            }
        }

        /* Add the provided channel information the the channel list */
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure you want to add all of the above " + (CustomLanguage.SelectedItem == null ? "None" : CustomLanguage.SelectedItem.ToString()) + 
                " channels to the " + (CustomCategory.SelectedItem == null ? "None" : CustomCategory.SelectedItem.ToString()) + " category?", "Add Channels?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                for (int i = 0; i < _viewModel.FileChannels.Count; i++)
                {
                    _viewModel.FileChannels[i].Languages.Add(CustomLanguage.SelectedItem == null ? "None" : CustomLanguage.SelectedItem.ToString());
                    _viewModel.FileChannels[i].Genre = CustomCategory.SelectedItem == null ? "None" : CustomCategory.SelectedItem.ToString();
                }
                await ChannelManager.AddChannels(_viewModel.FileChannels.ToList());
                await new Windows.UI.Popups.MessageDialog("Successfully added the above channels to your channel list.").ShowAsync();
            }
        }

        /* Open a custom M3U8 file */
        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".m3u");
            picker.FileTypeFilter.Add(".m3u8");
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                NoChannels.Visibility = Visibility.Collapsed;
                LoadingSpinner.Visibility = Visibility.Visible;
                ChannelList = await ChannelManager.LoadChannelFile(file, false);
                _viewModel.FileChannels = new ObservableCollection<Channel>(ChannelList);
                LoadingSpinner.Visibility = Visibility.Collapsed;

                if (_viewModel.FileChannels.Count == 0)
                {
                    NoChannels.Visibility = Visibility.Visible;
                }
            }

            if (localSettings.Values["m3u8LiveCheck"] is bool && (bool)localSettings.Values["m3u8LiveCheck"])
            {
                ProgressWrap.Visibility = Visibility.Visible;
                ChannelsScrollViewer.Margin = new Thickness(0, 0, 0, 104);
            }

            Progress<ProgressTaskAsync> progress = new Progress<ProgressTaskAsync>();

            progress.ProgressChanged += (s, p) =>
                {
                    LiveStatusProgressBar.Value = p.ProgressPercentage;
                    ProgressFraction.Text = p.Text;
                };

            ChannelList = await ChannelManager.SetLive(ChannelList, true, progress);

            if (localSettings.Values["m3u8LiveCheck"] is bool && (bool)localSettings.Values["m3u8LiveCheck"])
            {
                ProgressWrap.Visibility = Visibility.Collapsed;
                ChannelsScrollViewer.Margin = new Thickness(0, 0, 0, 48);
            }
        }

        /* Delete a channel from the import list */
        private async void DeleteChannel(Channel channel)
        {
            var dialog = new MessageDialog("This will remove this channel from the channel import list.", "Delete " + channel.Name + "?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                List<Channel> TempList = new List<Channel>(_viewModel.FileChannels);
                _viewModel.FileChannels.RemoveAt(TempList.FindIndex(delegate (Channel c) { return (c.Source == channel.Source) && (c.Name == channel.Name) && (c.Icon == channel.Icon) && (c.Genre == channel.Genre); }));

            }
        }

        /* Handle right click of channel */
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

        /* Submit channel list as a suggestion */
        private async void SuggestButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.FileChannels != null && _viewModel.FileChannels.Count > 0)
            {
                var dialog = new MessageDialog("Are you sure you want to suggest the above channels to be added as a default channels?", "Suggest Channels?");

                dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                dialog.Commands.Add(new UICommand("No") { Id = 1 });

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                if (await dialog.ShowAsync() == dialog.Commands[0])
                {
                    string body = "Hi, I want to suggest you add the following channels as defaults:\n\nName\t\tSource\n";
                    foreach (Channel c in _viewModel.FileChannels)
                    {
                        body += c.Name + "\t" + c.Source.AbsoluteUri + "\n";
                    }

                    object output = await MailHelper.SendFeedbackEmail("", "Suggestion", body);

                    if (!(output is Exception))
                    {
                        await new MessageDialog("Successfully received your suggestion to add these channel as a default. Thank you!").ShowAsync();
                    }
                    else
                    {
                        await new MessageDialog("I'm sorry, but I encoutered an error.  Please try to send your suggestion later.").ShowAsync();
                    }
                }
            }
            else
            {
                await new MessageDialog("Please load channels from a file, and try your suggestion again.").ShowAsync();
            }
        }

        /* Enable multi-select in the channels grid */
        private void MultiSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MultiSelectButton.IsChecked)
            {
                MultiDeleteButton.Visibility = Visibility.Visible;
                MultiSelectSeparator.Visibility = Visibility.Visible;
                ChannelsGridView.SelectionMode = ListViewSelectionMode.Multiple;
                ChannelsGridView.IsItemClickEnabled = false;

                if (DeviceType == DeviceFormFactorType.Phone)
                {
                    TitleText.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MultiDeleteButton.Visibility = Visibility.Collapsed;
                MultiSelectSeparator.Visibility = Visibility.Collapsed;
                ChannelsGridView.SelectionMode = ListViewSelectionMode.None;
                ChannelsGridView.SelectionMode = ListViewSelectionMode.Single;
                ChannelsGridView.IsItemClickEnabled = true;

                if (DeviceType == DeviceFormFactorType.Phone)
                {
                    TitleText.Visibility = Visibility.Visible;
                }
            }
        }

        /* Delete multiple channels at once */
        private async void MultiDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("This will remove any selected channels from the channel import list.", "Delete Channels?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                List<Channel> TempList = new List<Channel>(_viewModel.FileChannels);
                for (int i = 0; i < ChannelsGridView.SelectedItems.Count; i++)
                {
                    _viewModel.FileChannels.Remove((Channel)ChannelsGridView.SelectedItems.ElementAt(i));
                    i--;
                }
            }
        }

        /* Navigate to the player page when a channel is selected */
        private void ChannelsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(bool)MultiSelectButton.IsChecked)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(e.ClickedItem as Channel, ""));
            }
        }

        /* Handle right-click and other options for Xbox/Keyboard users */
        private async void ChannelsGridView_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Menu || e.Key == Windows.System.VirtualKey.Application ||
                (DeviceType == DeviceFormFactorType.Xbox && (e.Key == Windows.System.VirtualKey.GamepadMenu || e.Key == Windows.System.VirtualKey.NavigationMenu)))
            {
                foreach (Channel channel in _viewModel.FileChannels)
                {
                    GridViewItem g = (GridViewItem)ChannelsGridView.ContainerFromItem(channel);
                    FocusState f = g.FocusState;
                    if (f == FocusState.Keyboard)
                    {
                        PopupMenu popup = new PopupMenu();

                        UICommand delete = new UICommand("Delete", delegate (IUICommand command) { DeleteChannel(channel); });
                        popup.Commands.Add(delete);

                        GeneralTransform gt = g.TransformToVisual((FrameworkElement)((FrameworkElement)((FrameworkElement)((FrameworkElement)Frame.Parent).Parent).Parent).Parent);

                        await popup.ShowAsync(gt.TransformPoint(new Windows.Foundation.Point(40, 0)));
                    }
                }
            }
            else if (DeviceType == DeviceFormFactorType.Xbox && e.Key == Windows.System.VirtualKey.GamepadX)
            {
                foreach (Channel channel in _viewModel.FileChannels)
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
        
        /* Focus on the open button when on Xbox */
        private void OpenButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                OpenButton.Focus(FocusState.Keyboard);
            }
        }
    }
}
