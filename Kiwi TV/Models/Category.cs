using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models
{
    class Category : IComparable<Category>
    {
        public string _name;
        public string Name { get { return _name; } }
        public ObservableCollection<Channel> _channels;
        public ObservableCollection<Channel> Channels { get { return _channels; } }

        public Category(string name)
        {
            this._name = name;
            this._channels = new ObservableCollection<Channel>();
        }

        public int CompareTo(Category other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
