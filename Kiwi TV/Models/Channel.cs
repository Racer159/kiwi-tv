using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwi_TV.Models
{
    class Channel : IComparable<Channel>
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
        public bool Favorite { get { return _favorite; } set { _favorite = value; } }
        public string _genre;
        public string Genre { get { return _genre; } }
        public string _type;
        public string Type { get { return _type; } }

        public Channel(string name, string icon, string source, List<String> languages, bool favorite, string genre, string type)
        {
            this._name = name;
            this._icon = icon;
            this._source = new Uri(source);
            this._languages = languages;
            this._favorite = favorite;
            this._genre = genre;
            this._type = type;
        }

        public Channel(string name, string source)
        {
            this._name = name;
            this._icon = "";
            this._source = new Uri(source);
            this._languages = new List<String>();
            this._favorite = false;
            this._genre = "Other";
            this._type = "iptv";
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
        }

        public int CompareTo(Channel other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
