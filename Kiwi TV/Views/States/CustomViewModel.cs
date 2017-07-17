using System.Runtime.Serialization;
using System.ComponentModel;
using Kiwi_TV.Models;

namespace Kiwi_TV.Views.States
{
    /// <summary>
    /// Represents the data backend for the Custom Channel page
    /// </summary>
    [DataContract]
    class CustomViewModel : INotifyPropertyChanged
    {
        private string _customNameText;
        private string _customCategoryText;
        private string _customLanguageText;
        private string _customSourceURLText;
        private string _customEPGURLText;
        private string _customImageURLText;
        private bool _editMode = false;
        private Channel _editChannel;

        public CustomViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
        public string CustomEPGURLText
        {
            get { return _customEPGURLText; }
            set
            {
                _customEPGURLText = value;
                NotifyPropertyChanged("CustomEPGURLText");
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
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                NotifyPropertyChanged("EditMode");
            }
        }

        [DataMember]
        public Channel EditChannel
        {
            get { return _editChannel; }
            set
            {
                _editChannel = value;
                NotifyPropertyChanged("EditChannel");
            }
        }
    }
}
