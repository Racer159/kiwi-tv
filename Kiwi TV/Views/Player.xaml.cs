using Kiwi_TV.Logic;
using Kiwi_TV.Models;
using Kiwi_TV.Models.TwitchAPI;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Player : Page
    {
        public Player()
        {
            this.InitializeComponent();
            
            MainPlayer.AreTransportControlsEnabled = true;
            MainPlayer.PosterSource = new BitmapImage(new Uri("ms-appx:///Assets/bars.png"));

            // DW MainPlayer.Source = new Uri("http://dwstream4-lh.akamaihd.net/i/dwstream4_live@131329/master.m3u8");
            // DW (Latinoamerica) MainPlayer.Source = new Uri("http://dwstream3-lh.akamaihd.net/i/dwstream3_live@124409/master.m3u8");
            // DW (Arabia) MainPlayer.Source = new Uri("http://dwstream2-lh.akamaihd.net/i/dwstream2_live@124400/master.m3u8");
            // DW (Amerika) MainPlayer.Source = new Uri("http://dwstream5-lh.akamaihd.net/i/dwstream5_live@124540/master.m3u8");
            // DW (Deutsch) MainPlayer.Source = new Uri("http://dwstream6-lh.akamaihd.net/i/dwstream6_live@123962/master.m3u8");
            // ABC MainPlayer.Source = new Uri("http://abclive.abcnews.com/i/abc_live4@136330/index_1200_av-b.m3u8?sd=10&b=1200&rebase=on");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Channel)
            {
                Channel c = (Channel)e.Parameter;
                if (c.Type == "twitch")
                {
                    AccessToken token = await TwitchAPI.RetireveAccessToken(TwitchAPI.GetChannelNameFromURL(c.Source.AbsolutePath));
                    MainPlayer.Source = new Uri(c.Source, "?allow_source=true&token=" + Uri.EscapeDataString(token.Token.Replace("\\", "")) + "&sig=" + Uri.EscapeDataString(token.Signature));
                }
                else
                {
                    MainPlayer.Source = c.Source;
                }
            }

            MainPlayer.Play();
            base.OnNavigatedTo(e);
        }
    }
}
