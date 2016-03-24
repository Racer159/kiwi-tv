using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class Channels : Page
    {
        public Channels()
        {
            this.InitializeComponent();
        }

        private void ChannelsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView)
            {
                Frame.Navigate(typeof(Views.Player), ((GridView)sender).SelectedItem);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<Channel> ChannelList = new List<Channel>();

            if (e.Parameter is bool)
            {
                ChannelList = await Library.LoadChannels((bool)e.Parameter);
                ChannelList.Sort();
            }
            
            if (ChannelList.Count == 0)
            {
                NoContentHeader.Visibility = Visibility.Visible;
            }

            // News
            List<Channel> NewsChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "News"; });
            NewsChannelsListView.ItemsSource = NewsChannelList;
            if (NewsChannelList.Count > 0) { NewsChannelsListView.Visibility = Visibility.Visible; NewsHeader.Visibility = Visibility.Visible; }

            // Sci/Tech
            List<Channel> SciTechChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "Sci/Tech"; });
            SciTechChannelsListView.ItemsSource = SciTechChannelList;
            if (SciTechChannelList.Count > 0) { SciTechChannelsListView.Visibility = Visibility.Visible; SciTechHeader.Visibility = Visibility.Visible; }

            // Sports
            List<Channel> SportsChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "Sports"; });
            SportsChannelsListView.ItemsSource = SportsChannelList;
            if (SportsChannelList.Count > 0) { SportsChannelsListView.Visibility = Visibility.Visible; SportsHeader.Visibility = Visibility.Visible; }

            // Entertainment
            List<Channel> EntertainmentChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "Entertainment"; });
            EntertainmentChannelsListView.ItemsSource = EntertainmentChannelList;
            if (EntertainmentChannelList.Count > 0) { EntertainmentChannelsListView.Visibility = Visibility.Visible; EntertainmentHeader.Visibility = Visibility.Visible; }

            // Kids
            List<Channel> KidsChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "Kids"; });
            KidsChannelsListView.ItemsSource = KidsChannelList;
            if (KidsChannelList.Count > 0) { KidsChannelsListView.Visibility = Visibility.Visible; KidsHeader.Visibility = Visibility.Visible; }

            // Other
            List<Channel> OtherChannelList = ChannelList.FindAll(delegate (Channel c) { return c.Genre == "Other"; });
            OtherChannelsListView.ItemsSource = OtherChannelList;
            if (OtherChannelList.Count > 0) { OtherChannelsListView.Visibility = Visibility.Visible; OtherHeader.Visibility = Visibility.Visible; }

            base.OnNavigatedTo(e);
        }

        private async void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                List<Channel> TempList = await Library.LoadChannels(false);
                
                foreach (Channel c in TempList)
                {
                    if (c.Name == (String)((CheckBox)sender).Tag)
                    {
                        c.Favorite = (bool)((CheckBox)sender).IsChecked;
                    }
                }

                await Library.SaveChannels(TempList);
            }
        }
    }
}
