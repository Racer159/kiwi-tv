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
        private UStreamChannel[] _channels;
        private UStreamChannel _selected;
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
        public UStreamChannel[] Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                NotifyPropertyChanged("Channels");
            }
        }

        [DataMember]
        public UStreamChannel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged("Selected");
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
