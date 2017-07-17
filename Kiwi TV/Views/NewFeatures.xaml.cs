using Kiwi_TV.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Kiwi_TV.Views
{
    /// <summary>
    /// A page that displays the new features tutorial
    /// </summary>
    public sealed partial class NewFeatures : Page
    {
        DeviceFormFactorType DeviceType;
        int currentPage = 0;

        /* Instantiate the page, and setup device specific options */
        public NewFeatures()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TutorialWrap.Margin = new Thickness(20, 20, 20, 20);
                SubText.Margin = new Thickness(10, 220, 10, 10);
                TitleText.FontSize = 20;
                SubText.FontSize = 14;
                ExampleImage.Height = 150;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                ExampleImage.Margin = new Thickness(10, -100, 10, 10);
                TitleText.Margin = new Thickness(10, 100, 10, 10);
                SubText.Margin = new Thickness(10, 170, 10, 10);
                ButtonWrap.Margin = new Thickness(10, 250, 10, 10);
                TitleText.FontSize = 20;
                SubText.FontSize = 14;
                ExampleImage.Height = 150;
                GreenWrap.Margin = new Thickness(-48, -75, -48, -27);
            }

            ChannelManager.MigrateChannelList();
        }

        /* Skip the new features tutorial */
        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Channels), false);
        }

        /* Go to the next page of the tutorial */
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            if (currentPage == 1)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/EPG.png"));
                TitleText.Text = "Electronic Progam Guide";
                SubText.Text = "*Experimental: Toggle it on in Settings.";
            }
            else if (currentPage == 2)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Feedback.png"));
                TitleText.Text = "New Feedback Mechanism";
                SubText.Text = "Now using Feedback Hub to get better Feedback.";
            }
            else if (currentPage == 3)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Welcome.png"));
                TitleText.Text = "Updated Channel URLs and Bug Fixes";
                SubText.Text = "More stability for certain channels";
            }
            else if (currentPage == 4)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Welcome.png"));
                TitleText.Text = "Lets Get Started";
                SubText.Text = "You're all set to begin.";
                Next.Content = "Start";
            }
            else if (currentPage == 5)
            {
                Frame.Navigate(typeof(Views.Channels), false);
            }
        }

        /* Focus on the Next button when loaded on Xbox */
        private void Next_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                Next.Focus(FocusState.Keyboard);
            }
        }
    }
}
