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
    [DataContract]
    class AddChannelViewModel : INotifyPropertyChanged
    {
        private int _mainPivotSelectedIndex;
        private string _customNameText;
        private string _customCategoryText;
        private string _customLanguageText;
        private string _customSourceURLText;
        private string _customImageURLText;
        private string _twitchSearchText;
        private string _twitchCategoryText;
        private string _twitchLanguageText;
        private TwitchChannel[] _twitchChannels;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DataMember]
        public int MainPivotSelectedIndex
        {
            get { return _mainPivotSelectedIndex; }
            set
            {
                _mainPivotSelectedIndex = value;
                NotifyPropertyChanged("MainPivotSelectedIndex");
            }
        }

        [DataMember]
        public string CustomNameText
        {
            get { return _customNameText; }
            set
            {
                _customNameText = value;
                NotifyPropertyChanged("CustomNameText");
            }
        }

        [DataMember]
        public string CustomCategoryText
        {
            get { return _customCategoryText; }
            set
            {
                _customCategoryText = value;
                NotifyPropertyChanged("CustomCategoryText");
            }
        }

        [DataMember]
        public string CustomLanguageText
        {
            get { return _customLanguageText; }
            set
            {
                _customLanguageText = value;
                NotifyPropertyChanged("CustomLanguageText");
            }
        }

        [DataMember]
        public string CustomSourceURLText
        {
            get { return _customSourceURLText; }
            set
            {
                _customSourceURLText = value;
                NotifyPropertyChanged("CustomSourceURLText");
            }
        }

        [DataMember]
        public string CustomImageURLText
        {
            get { return _customImageURLText; }
            set
            {
                _customImageURLText = value;
                NotifyPropertyChanged("CustomImageURLText");
            }
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
        public string TwitchLanguageText
        {
            get { return _twitchLanguageText; }
            set
            {
                _twitchLanguageText = value;
                NotifyPropertyChanged("TwitchLanguageText");
            }
        }

        [DataMember]
        public TwitchChannel[] TwitchChannels
        {
            get { return _twitchChannels; }
            set
            {
                _twitchChannels = value;
                NotifyPropertyChanged("TwitchChannels");
            }
        }
    }
}
