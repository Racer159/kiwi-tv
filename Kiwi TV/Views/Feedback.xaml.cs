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
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Feedback : Page
    {
        DeviceFormFactorType DeviceType;

        public Feedback()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleText.Margin = new Thickness(48, 0, 0, 0);
            }
        }

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
    }
}
