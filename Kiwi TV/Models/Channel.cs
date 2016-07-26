using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models
{
    class Channel : IComparable<Channel>, INotifyPropertyChanged
    {
        public string _name;
        public string Name { get { return _name; } }
        public string _icon;
        public string Icon { get { return _icon; } }
        public Uri _source;
        public Uri Source { get { return _source; } }
        public List<String> _languages;
        public List<String> Languages { get { return _languages; } }
        public bool _favorite;
        public bool Favorite { get { return _favorite; } set { _favorite = value; NotifyPropertyChanged("Favorite"); } }
        public string _genre;
        public string Genre { get { return _genre; } set { _genre = value; } }
        public string _type;
        public string Type { get { return _type; } }
        public bool _live;
        public bool Live { get { return _live; } set { _live = value; NotifyPropertyChanged("Live"); } }

        public Channel(string name, string icon, string source, List<String> languages, bool favorite, string genre, string type, bool live)
        {
            this._name = name;
            this._icon = icon;
            Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out this._source);
            this._languages = languages;
            this._favorite = favorite;
            this._genre = genre;
            this._type = type;
            this._live = live;
        }

        public Channel(string name, string source)
        {
            this._name = name;
            this._icon = "";
            Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out this._source);
            this._languages = new List<String>();
            this._favorite = false;
            this._genre = "Other";
            this._type = "iptv";
            this._live = true;
        }

        public Channel()
        {
            this._name = "";
            this._icon = "";
            this._source = null;
            this._languages = new List<String>();
            this._favorite = false;
            this._genre = "";
            this._type = "";
            this._live = true;
        }

        public int CompareTo(Channel other)
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
