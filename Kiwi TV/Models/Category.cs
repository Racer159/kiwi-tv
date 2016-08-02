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
    class Category : IComparable<Category>
    {
        public string _name;
        public string Name { get { return _name; } }
        public ObservableCollection<Channel> _channels = new ObservableCollection<Channel>();
        public ObservableCollection<Channel> Channels { get { return _channels; } }

        public Category(string name)
        {
            this._name = name;
        }

        public int CompareTo(Category other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
