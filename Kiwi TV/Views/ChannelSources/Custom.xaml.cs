using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// A page that allows a custom channel to be added to the channel list
    /// </summary>
    public sealed partial class Custom : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        CustomViewModel _viewModel;
        
        /* Instantiate the page and setup device specific options */
        public Custom()
        {
            this.InitializeComponent();
            CustomCategory.ItemsSource = categories;
            CustomLanguage.ItemsSource = languages;
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconSize.Tag = 115;
                GridViewWrapSize.Tag = 120;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                GridViewIconSize.Tag = 115;
                GridViewWrapSize.Tag = 120;
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

            if (e.Parameter is CustomViewModel)
            {
                _viewModel = (CustomViewModel)e.Parameter;
                this.DataContext = _viewModel;
            }
            else
            {
                _viewModel = new CustomViewModel();
            }

            // Switch UI elements if editting existing channel
            if (_viewModel.EditMode)
            {
                SuggestButton.Visibility = Visibility.Collapsed;
                FileButton.Visibility = Visibility.Collapsed;
                AddButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
                TitleText.Text = "Edit Channel";

                if (_viewModel.EditChannel != null && _viewModel.CustomNameText == null)
                {
                    _viewModel.CustomCategoryText = _viewModel.EditChannel.Genre;
                    _viewModel.CustomImageURLText = _viewModel.EditChannel.Icon;
                    _viewModel.CustomLanguageText = _viewModel.EditChannel.Languages.ElementAtOrDefault(0);
                    _viewModel.CustomNameText = _viewModel.EditChannel.Name;
                    _viewModel.CustomSourceURLText = _viewModel.EditChannel.Source.AbsoluteUri;
                }
            }
        }

        /* Add the provided channel information the the channel list */
        private async void AddButton_Click(object sender, RoutedEventArgs e)
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

        /* Test the provided channel information in the player */
        private async void TestButton_Click(object sender, RoutedEventArgs e)
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

        /* Create a channel object from the provided channel information */
        private Channel GenerateCustomChannel()
        {
            List<string> languages = new List<string>();
            languages.Add(CustomLanguage.SelectedItem == null ? "None" : CustomLanguage.SelectedItem.ToString());
            string category = CustomCategory.SelectedItem == null ? "None" : CustomCategory.SelectedItem.ToString();
            return new Channel(CustomName.Text, CustomImageURL.Text, CustomSourceURL.Text, languages, false, category, "iptv", true);
        }

        /* Send the provided channel information as a suggestion */
        private async void SuggestButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomSourceURL.Text != "")
            {
                MessageDialog dialog;
                if (CustomName.Text != "")
                {
                    dialog = new MessageDialog("Are you sure you want to suggest " + CustomName.Text + " to be added as a default channel?", "Suggest Channel?");
                }
                else
                {
                    dialog = new MessageDialog("Are you sure you want to suggest this to be added as a default channel?", "Suggest Channel?");
                }

                dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                dialog.Commands.Add(new UICommand("No") { Id = 1 });

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                if (await dialog.ShowAsync() == dialog.Commands[0])
                {
                    object output = await MailHelper.SendFeedbackEmail("", "Suggestion", "Hi, I want to suggest you add '" + CustomName.Text +
                    "' as a default channel.  Below are the sources I used:\n\nImage: " + CustomImageURL.Text + "\nVideo: " + CustomSourceURL.Text + "\n\nThank you!");

                    if (!(output is Exception))
                    {
                        await new MessageDialog("Successfully received your suggestion to add this channel as a default. Thank you!").ShowAsync();
                    }
                    else
                    {
                        await new MessageDialog("I'm sorry, but I encoutered an error.  Please try to send your suggestion later.").ShowAsync();
                    }
                }
            }
            else
            {
                await new MessageDialog("Please enter a valid source, and then try your suggestion again.").ShowAsync();
            }
        }

        /* Navigate to the Custom File page */
        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ChannelSources.File), new FileViewModel());
        }

        /* Save the provided channel information to the channel list */
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure you want to save this channel?", "Save " + _viewModel.CustomNameText + "?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                Uri source;
                Uri.TryCreate(CustomSourceURL.Text, UriKind.RelativeOrAbsolute, out source);
                if (source.IsAbsoluteUri)
                {
                    await ChannelManager.RemoveChannel(_viewModel.EditChannel);
                    Channel c = GenerateCustomChannel();
                    c.Favorite = _viewModel.EditChannel.Favorite;
                    await ChannelManager.AddChannel(c);
                    await new Windows.UI.Popups.MessageDialog("Successfully saved " + CustomName.Text).ShowAsync();
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("I'm sorry, but I cannot save that channel because the specified video URL is invalid.").ShowAsync();
                }
            }
        }
        
        /* Set the focus state of the channel preview gridview for Xbox */
        private void ChannelsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                ChannelsGridView.Focus(FocusState.Keyboard);
            }
        }
    }
}
