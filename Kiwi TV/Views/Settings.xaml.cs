using Kiwi_TV.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        DeviceFormFactorType DeviceType;

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public Settings()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
            }

            if (localSettings.Values["syncData"] is bool)
            {
                SyncToggleSwitch.IsOn = (bool)localSettings.Values["syncData"];
            }
            else
            {
                SyncToggleSwitch.IsOn = true;
            }
        }

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

        private void SyncToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["syncData"] = SyncToggleSwitch.IsOn;
        }
    }
}
