using Kiwi_TV.Helpers;
using Kiwi_TV.Views.States;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kiwi_TV.Views
{
    /// <summary>
    /// A page to link to individual add channel pages
    /// </summary>
    public sealed partial class AddChannel : Page
    {
        DeviceFormFactorType DeviceType;

        /* Instantiate the page and setup device specific options */
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
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                GridViewIconHeight.Tag = 115;
                GridViewIconWidth.Tag = 230;
                SourcesGridView.SingleSelectionFollowsFocus = false;
            }
        }

        /* Navigate to a specific add channel page based on the item that was selected */
        private void SourcesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = SourcesGridView.Items.IndexOf(e.ClickedItem);
            if (index == 0)
            {
                Frame.Navigate(typeof(Views.ChannelSources.Custom), new CustomViewModel());
            }
            else if (index == 1)
            {
                Frame.Navigate(typeof(Views.ChannelSources.File), new FileViewModel());
            }
            else if (index == 2)
            {
                Frame.Navigate(typeof(Views.ChannelSources.Twitch), new TwitchViewModel());
            }
            else if (index == 3)
            {
                Frame.Navigate(typeof(Views.ChannelSources.UStream), new UStreamViewModel());
            }
        }
        
        /* Focus on the sources grid view when on Xbox */
        private void SourcesGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                SourcesGridView.Focus(FocusState.Keyboard);
            }
        }
    }
}
