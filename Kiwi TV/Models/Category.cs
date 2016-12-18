using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Kiwi_TV.Models
{
    /// <summary>
    /// Represents a category that contains many channels
    /// </summary>
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
