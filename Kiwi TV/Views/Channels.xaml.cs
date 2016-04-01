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
        ObservableCollection<Category> CategoryList = new ObservableCollection<Category>();

        public Channels()
        {
            this.InitializeComponent();
            CategoriesItemsControl.ItemsSource = CategoryList;
        }

        private void ChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView && ((GridView)sender).SelectedItem is Channel)
            {
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>((Channel)((GridView)sender).SelectedItem, ""));
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            RefreshLiveStatus(await FileManager.SetLive(ChannelList));
        }

        private void RefreshLiveStatus(List<Channel> channelList)
        {
            for (int i = 0; i < CategoryList.Count; i++)
            {
                for(int j = 0; j < CategoryList[i].Channels.Count; j++)
                {
                    for (int k = 0; k < channelList.Count; k++)
                    {
                        if (CategoryList[i].Channels[j].Name == channelList[k].Name)
                        {
                            Channel updated = CategoryList[i].Channels[j];
                            updated.Live = channelList[k].Live;
                            CategoryList[i].Channels.RemoveAt(j);
                            CategoryList[i].Channels.Insert(j, updated);
                        }
                    }
                }
            }
        }

        private void RefreshChannelList(List<Channel> channelList, string search, string language)
        {
            channelList = channelList.FindAll(delegate (Channel c) { return c.Name.ToLower().Contains(search); });
            if (!(language == "All Languages" || language == ""))
            {
                channelList = channelList.FindAll(delegate (Channel c) { return c.Languages.Contains(language); });
            }

            if (channelList.Count == 0)
            {
                NoContentHeader.Visibility = Visibility.Visible;
            }
            else
            {
                NoContentHeader.Visibility = Visibility.Collapsed;
            }

            FileManager.LoadCategories(channelList, CategoryList);
        }

        private async void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox)
            {
                await FileManager.SaveFavorite((String)((CheckBox)sender).Tag, (bool)((CheckBox)sender).IsChecked);
            }
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton)
            {
                Frame.Navigate(typeof(Views.AddChannel));
            }
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
