using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.API.UStream.Models;

namespace Kiwi_TV.Views.States
{
    [DataContract]
    class UStreamViewModel : INotifyPropertyChanged
    {
        private string _ustreamSearchText;
        private string _ustreamCategoryText;
        private UStreamChannel[] _searchChannels;
        private UStreamChannel _selectedSearch;
        private UStreamChannel[] _liveChannels;
        private UStreamChannel _selectedLive;
        private string _logoPath;

        public UStreamViewModel()
        {
            _ustreamCategoryText = "Other";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DataMember]
        public string UStreamSearchText
        {
            get { return _ustreamSearchText; }
            set
            {
                _ustreamSearchText = value;
                NotifyPropertyChanged("UStreamSearchText");
            }
        }

        [DataMember]
        public string UStreamCategoryText
        {
            get { return _ustreamCategoryText; }
            set
            {
                _ustreamCategoryText = value;
                NotifyPropertyChanged("UStreamCategoryText");
            }
        }

        [DataMember]
        public UStreamChannel[] SearchChannels
        {
            get { return _searchChannels; }
            set
            {
                _searchChannels = value;
                NotifyPropertyChanged("SearchChannels");
            }
        }

        [DataMember]
        public UStreamChannel SelectedSearch
        {
            get { return _selectedSearch; }
            set
            {
                _selectedSearch = value;
                NotifyPropertyChanged("SelectedSearch");
            }
        }

        [DataMember]
        public UStreamChannel[] LiveChannels
        {
            get { return _liveChannels; }
            set
            {
                _liveChannels = value;
                NotifyPropertyChanged("LiveChannels");
            }
        }

        [DataMember]
        public UStreamChannel SelectedLive
        {
            get { return _selectedLive; }
            set
            {
                _selectedLive = value;
                NotifyPropertyChanged("SelectedLive");
            }
        }

        [DataMember]
        public string LogoPath
        {
            get { return _logoPath; }
            set
            {
                _logoPath = value;
                NotifyPropertyChanged("LogoPath");
            }
        }
    }
}
