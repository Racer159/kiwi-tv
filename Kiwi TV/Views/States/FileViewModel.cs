using Kiwi_TV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Views.States
{
    [DataContract]
    class FileViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Channel> _fileChannels = new ObservableCollection<Channel>();
        private string _customCategoryText;
        private string _customLanguageText;

        public FileViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DataMember]
        public ObservableCollection<Channel> FileChannels
        {
            get { return _fileChannels; }
            set
            {
                _fileChannels = value;
                NotifyPropertyChanged("FileChannels");
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
    }
}
