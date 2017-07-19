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

namespace Kiwi_TV.Views.Components
{
    /// <summary>
    /// A custom content dialog to retrieve a URL input.
    /// </summary>
    public sealed partial class InputDialog : ContentDialog
    {
        public Uri Result { get; set; }

        public InputDialog()
        {
            this.InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            this.Hide();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnteredURL();
        }

        private void URLTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ProcessEnteredURL();
            }
        }

        private void ProcessEnteredURL()
        {
            Uri entered;            
            if (Uri.TryCreate(URLTextBox.Text, UriKind.Absolute, out entered) || 
                Uri.TryCreate("http://" + URLTextBox.Text, UriKind.Absolute, out entered))
            {
                Result = entered;
                this.Hide();
            }
            else
            {
                InvalidURLText.Visibility = Visibility.Visible;
            }
        }
    }
}
