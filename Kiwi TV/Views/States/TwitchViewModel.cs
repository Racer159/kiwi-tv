using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kiwi_TV.API.Twitch.Models;

namespace Kiwi_TV.Views.States
{
    /// <summary>
    /// Represents the data backend for the Twitch.tv page
    /// </summary>
    [DataContract]
    class TwitchViewModel : INotifyPropertyChanged
    {
        private string _twitchSearchText;
        private string _twitchCategoryText;
        private TwitchChannel[] _channels;
        private TwitchChannel _selected;

        public TwitchViewModel()
        {
            _twitchCategoryText = "Gaming";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DataMember]
        public string TwitchSearchText
        {
            get { return _twitchSearchText; }
            set
            {
                _twitchSearchText = value;
                NotifyPropertyChanged("TwitchSearchText");
            }
        }

        [DataMember]
        public string TwitchCategoryText
        {
            get { return _twitchCategoryText; }
            set
            {
                _twitchCategoryText = value;
                NotifyPropertyChanged("TwitchCategoryText");
            }
        }

        [DataMember]
        public TwitchChannel[] Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                NotifyPropertyChanged("Channels");
            }
        }

        [DataMember]
        public TwitchChannel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged("Selected");
            }
        }
    }
}
