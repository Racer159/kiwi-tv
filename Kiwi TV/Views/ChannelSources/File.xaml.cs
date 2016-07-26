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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class File : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        FileViewModel _viewModel;

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
                CustomButtons.Orientation = Orientation.Vertical;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        private void Category_GotFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Entertainment");
            categories.Add("Sports");
            categories.Add("Gaming");
            categories.Add("Other");
        }

        private void Category_LostFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
        }

        private void Language_GotFocus(object sender, RoutedEventArgs e)
        {
            languages.Clear();
            languages.Add("English");
            languages.Add("French");
            languages.Add("Spanish");
            languages.Add("German");
            languages.Add("Russian");
            languages.Add("Arabic");
        }

        private void Language_LostFocus(object sender, RoutedEventArgs e)
        {
            languages.Clear();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure you want to add all of the above " + CustomLanguage.Text + " channels to the " + CustomCategory.Text + " category?", "Add Channels?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                for (int i = 0; i < _viewModel.FileChannels.Count; i++)
                {
                    _viewModel.FileChannels[i].Languages.Add(CustomLanguage.Text);
                    _viewModel.FileChannels[i].Genre = CustomCategory.Text;
                }
                await ChannelManager.AddChannels(_viewModel.FileChannels.ToList());
                await new Windows.UI.Popups.MessageDialog("Successfully added the above channels to your channel list.").ShowAsync();
            }
        }

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
                _viewModel.FileChannels = new ObservableCollection<Channel>(await ChannelManager.LoadChannelFile(file, false));
                LoadingSpinner.Visibility = Visibility.Collapsed;

                if (_viewModel.FileChannels.Count == 0)
                {
                    NoChannels.Visibility = Visibility.Visible;
                }
            }
        }

        private void ChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView && ((GridView)sender).SelectedItem is Channel)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>((Channel)((GridView)sender).SelectedItem, ""));
            }
        }

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
                ChannelsGridView.Focus(FocusState.Programmatic);
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
    }
}
