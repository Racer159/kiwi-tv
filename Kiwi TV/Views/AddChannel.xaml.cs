using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddChannel : Page
    {
        DeviceFormFactorType DeviceType;

        public AddChannel()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconHeight.Tag = 115;
                GridViewIconWidth.Tag = 230;
            }
        }

        private void CustomGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ChannelSources.Custom), new CustomViewModel());
        }

        private void TwitchGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ChannelSources.Twitch), new TwitchViewModel());
        }

        private void UStreamGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ChannelSources.UStream), new UStreamViewModel());
        }
    }
}
