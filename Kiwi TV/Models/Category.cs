using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Kiwi_TV.Models
{
    [DataContract]
    class Category : IComparable<Category>, INotifyPropertyChanged
    {
        public string _name;
        public string Name { get { return _name; } }
        public ObservableCollection<Channel> _channels = new ObservableCollection<Channel>();
        public ObservableCollection<Channel> Channels { get { return _channels; } }
        public ListViewSelectionMode _selectionMode;
        [DataMember]
        public ListViewSelectionMode SelectionMode { get { return _selectionMode; } set { _selectionMode = value; NotifyPropertyChanged("SelectionMode");} }
        public List<Channel> _selected = new List<Channel>();
        [DataMember]
        public List<Channel> Selected { get { return _selected; } set { _selected = value; NotifyPropertyChanged("Selected"); } }

        public Category(string name)
        {
            this._name = name;
            this._selectionMode = ListViewSelectionMode.Single;
        }

        public int CompareTo(Category other)
        {
            return this.Name.CompareTo(other.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
