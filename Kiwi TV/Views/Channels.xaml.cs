using Kiwi_TV.Logic;
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
        List<Channel> ChannelList = new List<Channel>();

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
            ChannelList = new List<Channel>();

            if (e.Parameter is bool)
            {
                ChannelList = await FileManager.LoadChannels((bool)e.Parameter);
                ChannelList.Sort();
                if ((bool)e.Parameter)
                {
                    LanguagesBox.Visibility = Visibility.Collapsed;
                    AddChannelButton.Visibility = Visibility.Collapsed;
                }
            }

            RefreshChannelList(ChannelList, "", "All Languages");
            base.OnNavigatedTo(e);
        }

        private void RefreshChannelList(List<Channel> channelList, string search, string language)
        {
            channelList = channelList.FindAll(delegate (Channel c) { return c.Name.ToLower().Contains(search); });
            if (!(language == "All Languages" || language == ""))
            {
                channelList = channelList.FindAll(delegate (Channel c) { return c.Languages.Contains(language); });
            }

            NewsChannelsListView.Visibility = Visibility.Collapsed; NewsHeader.Visibility = Visibility.Collapsed;
            SciTechChannelsListView.Visibility = Visibility.Collapsed; SciTechHeader.Visibility = Visibility.Collapsed;
            SportsChannelsListView.Visibility = Visibility.Collapsed; SportsHeader.Visibility = Visibility.Collapsed;
            EntertainmentChannelsListView.Visibility = Visibility.Collapsed; EntertainmentHeader.Visibility = Visibility.Collapsed;
            KidsChannelsListView.Visibility = Visibility.Collapsed; KidsHeader.Visibility = Visibility.Collapsed;
            OtherChannelsListView.Visibility = Visibility.Collapsed; OtherHeader.Visibility = Visibility.Collapsed;

            if (channelList.Count == 0)
            {
                NoContentHeader.Visibility = Visibility.Visible;
            } 
            else
            {
                NoContentHeader.Visibility = Visibility.Collapsed;
            }

            // News
            List<Channel> NewsChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "News"; });
            NewsChannelsListView.ItemsSource = NewsChannelList;
            if (NewsChannelList.Count > 0) { NewsChannelsListView.Visibility = Visibility.Visible; NewsHeader.Visibility = Visibility.Visible; }

            // Sci/Tech
            List<Channel> SciTechChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "Sci/Tech"; });
            SciTechChannelsListView.ItemsSource = SciTechChannelList;
            if (SciTechChannelList.Count > 0) { SciTechChannelsListView.Visibility = Visibility.Visible; SciTechHeader.Visibility = Visibility.Visible; }

            // Sports
            List<Channel> SportsChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "Sports"; });
            SportsChannelsListView.ItemsSource = SportsChannelList;
            if (SportsChannelList.Count > 0) { SportsChannelsListView.Visibility = Visibility.Visible; SportsHeader.Visibility = Visibility.Visible; }

            // Entertainment
            List<Channel> EntertainmentChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "Entertainment"; });
            EntertainmentChannelsListView.ItemsSource = EntertainmentChannelList;
            if (EntertainmentChannelList.Count > 0) { EntertainmentChannelsListView.Visibility = Visibility.Visible; EntertainmentHeader.Visibility = Visibility.Visible; }

            // Kids
            List<Channel> KidsChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "Kids"; });
            KidsChannelsListView.ItemsSource = KidsChannelList;
            if (KidsChannelList.Count > 0) { KidsChannelsListView.Visibility = Visibility.Visible; KidsHeader.Visibility = Visibility.Visible; }

            // Other
            List<Channel> OtherChannelList = channelList.FindAll(delegate (Channel c) { return c.Genre == "Other"; });
            OtherChannelsListView.ItemsSource = OtherChannelList;
            if (OtherChannelList.Count > 0) { OtherChannelsListView.Visibility = Visibility.Visible; OtherHeader.Visibility = Visibility.Visible; }
        }

        private async void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                List<Channel> TempList = await FileManager.LoadChannels(false);
                
                foreach (Channel c in TempList)
                {
                    if (c.Name == (String)((CheckBox)sender).Tag)
                    {
                        c.Favorite = (bool)((CheckBox)sender).IsChecked;
                    }
                }

                await FileManager.SaveChannels(TempList);
            }
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
