using System.ComponentModel;

namespace Kiwi_TV.Models
{
    class ProgramInfo : INotifyPropertyChanged
    {
        public string _title;
        public string Title { get { return _title; } set { _title = value; NotifyPropertyChanged("Title"); } }
        public int _width;
        public int Width { get { return _width; } set { _width = value; NotifyPropertyChanged("Width"); } }

        public ProgramInfo(string title, int width)
        {
            this._title = title;
            this._width = width;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
