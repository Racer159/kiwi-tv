﻿using System;
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
    class TwitchViewModel : INotifyPropertyChanged
    {
        private string _twitchSearchText;
        private string _twitchCategoryText;
        private TwitchChannel[] _searchChannels;
        private TwitchChannel _selectedSearch;
        private TwitchChannel[] _liveChannels;
        private TwitchChannel _selectedLive;

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
        public TwitchChannel[] SearchChannels
        {
            get { return _searchChannels; }
            set
            {
                _searchChannels = value;
                NotifyPropertyChanged("SearchChannels");
            }
        }

        [DataMember]
        public TwitchChannel SelectedSearch
        {
            get { return _selectedSearch; }
            set
            {
                _selectedSearch = value;
                NotifyPropertyChanged("SelectedSearch");
            }
        }

        [DataMember]
        public TwitchChannel[] LiveChannels
        {
            get { return _liveChannels; }
            set
            {
                _liveChannels = value;
                NotifyPropertyChanged("LiveChannels");
            }
        }

        [DataMember]
        public TwitchChannel SelectedLive
        {
            get { return _selectedLive; }
            set
            {
                _selectedLive = value;
                NotifyPropertyChanged("SelectedLive");
            }
        }
    }
}
