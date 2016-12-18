using Kiwi_TV.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kiwi_TV.Views
{
    /// <summary>
    /// A page to send feedback about the application
    /// </summary>
    public sealed partial class Feedback : Page
    {
        DeviceFormFactorType DeviceType;

        /* Instantiate the page and setup device specific options */
        public Feedback()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                DislikeButton.Margin = new Thickness(5, 0, 0, 0);
                LikeButton.Margin = new Thickness(5,0,0,0);
                EmailBox.XYFocusDown = SuggestButton;
                SuggestButton.XYFocusDown = FeedbackBox;
                XboxCommandWrap.Visibility = Visibility.Visible;
            }
        }

        /* Submit the given feedback information */
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string type = "Dislike";
            if ((bool)SuggestButton.IsChecked) { type = "Suggestion"; }  else if ((bool)LikeButton.IsChecked) { type = "Like"; }
            object output = await MailHelper.SendFeedbackEmail(EmailBox.Text, type, FeedbackBox.Text);

            if (!(output is Exception))
            {
                EmailBox.Text = "";
                FeedbackBox.Text = "";
                SuggestButton.IsChecked = true;
                await new Windows.UI.Popups.MessageDialog("Successfully received your feedback. Thank you!").ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("I'm sorry, but I encoutered an error.  Please try to send your feedback later.").ShowAsync();
            }
        }

        /* Focus on the email box when on an Xbox */
        private void EmailBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                EmailBox.Focus(FocusState.Keyboard);
            }
        }
    }
}
