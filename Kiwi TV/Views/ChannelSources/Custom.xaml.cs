﻿using Kiwi_TV.Helpers;
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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Custom : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        CustomViewModel _viewModel;

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
                CustomButtons.Orientation = Orientation.Vertical;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is CustomViewModel)
            {
                _viewModel = (CustomViewModel)e.Parameter;
                this.DataContext = _viewModel;
            }
            else
            {
                _viewModel = new CustomViewModel();
            }
        }

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

        private Channel GenerateCustomChannel()
        {
            List<string> languages = new List<string>();
            languages.Add(CustomLanguage.Text);
            return new Channel(CustomName.Text, CustomImageURL.Text, CustomSourceURL.Text, languages, false, CustomCategory.Text, "iptv", true);
        }

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

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ChannelSources.File), new FileViewModel());
        }
    }
}