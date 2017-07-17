using Kiwi_TV.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An page that handles setting the application settings
    /// </summary>
    public sealed partial class Settings : Page
    {
        DeviceFormFactorType DeviceType;

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        bool showEPGWarning = true;

        /* Instantiate the page, application settings, and setup device specific options */
        public Settings()
        {
            this.InitializeComponent();
            CategoryList.ItemsSource = categories;

            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                ResetButtonStackPanel.Orientation = Orientation.Vertical;
            }

            if (localSettings.Values["syncData"] is bool)
            {
                SyncToggleSwitch.IsOn = (bool)localSettings.Values["syncData"];
            }
            else
            {
                SyncToggleSwitch.IsOn = true;
            }
            
            if (localSettings.Values["darkTheme"] is bool)
            {
                DarkThemeToggleSwitch.IsOn = (bool)localSettings.Values["darkTheme"];
            }
            else
            {
                DarkThemeToggleSwitch.IsOn = false;
            }

            if (localSettings.Values["m3u8LiveCheck"] is bool)
            {
                M3U8LiveCheckToggleSwitch.IsOn = (bool)localSettings.Values["m3u8LiveCheck"];
            }
            else
            {
                M3U8LiveCheckToggleSwitch.IsOn = false;
            }

            if (localSettings.Values["electronicProgramGuide"] is bool)
            {
                showEPGWarning = false;
                EPGToggleSwitch.IsOn = (bool)localSettings.Values["electronicProgramGuide"];
            }
            else
            {
                EPGToggleSwitch.IsOn = false;
            }
        }

        /* Load the category information when navigated to */
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<string> categoriesFile = await CategoryHelper.LoadCategories();
            foreach (string category in categoriesFile)
            {
                categories.Add(category);
            }
            CategoryList.ItemsSource = categories;
        }

        /* Restore the default channels in the channel list */
        private async void RestoreDefaultChannels_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog("This will add back all of the default channels, and unfavorite any that you have favorited.", "Restore the Default Channels?");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if(await dialog.ShowAsync() == dialog.Commands[0] )
            {
                await ChannelManager.RestoreDefaultChannels();
                await new Windows.UI.Popups.MessageDialog("Successfully Restored Default Channels.").ShowAsync();
            }

        }

        /* Reset a corrupted channel list */
        private async void ResetChannels_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog("This will remove all of the channels you have added, and will reset your favorites.", "Reset the Channel List?");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            if (await dialog.ShowAsync() == dialog.Commands[0])
            {
                await ChannelManager.ResetChannels();
                await new Windows.UI.Popups.MessageDialog("Successfully Reset Channel List.").ShowAsync();
            }
        }
        
        /* Toggle the sync data setting */
        private void SyncToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["syncData"] = SyncToggleSwitch.IsOn;
        }

        /* Toggle the dark theme setting */
        private void DarkThemeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["darkTheme"] = DarkThemeToggleSwitch.IsOn;
        }

        /* Toggle the live check setting */
        private void M3U8LiveCheckToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["m3u8LiveCheck"] = M3U8LiveCheckToggleSwitch.IsOn;
        }

        /* Remove a category from the category list */
        private async void RemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button btn = (Button)sender;
                categories.Remove((string)btn.Tag);
                await CategoryHelper.SaveCategories(categories.ToList());
            }
        }

        /* Add a category to the category list when the add button is pressed */
        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddCategoryBox.Text != "")
            {
                categories.Add(AddCategoryBox.Text);
                AddCategoryBox.Text = "";
                await CategoryHelper.SaveCategories(categories.ToList());
            }
        }

        /* Add a category to the category list when the enter key is pressed */
        private async void AddCategoryBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (AddCategoryBox.Text != "")
                {
                    categories.Add(AddCategoryBox.Text);
                    AddCategoryBox.Text = "";
                    await CategoryHelper.SaveCategories(categories.ToList());
                }
            }
        }

        private async void EPGToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (EPGToggleSwitch.IsOn && showEPGWarning)
            {
                await new Windows.UI.Popups.MessageDialog("This feature will replace the favorites page with an Electronic Program Guide.  Please note that this feature is EXPERIMENTAL and will only work out of the box on Bloomberg and in a limited way for Twitch.tv channels. EPG XML links must be added to channels before program information can be pulled.").ShowAsync();
            }
            localSettings.Values["electronicProgramGuide"] = EPGToggleSwitch.IsOn;
            showEPGWarning = true;
        }
    }
}
