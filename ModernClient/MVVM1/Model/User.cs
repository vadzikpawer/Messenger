
using ModernClient.Core;
using System;
using System.Collections.ObjectModel;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class User : ObservableObject
    {

        private int _id;
        private bool _haveUnseen = false;
        private int _unseen = 0;
        private string _name;
        private string _pass;
        private string _color;
        private bool _online;
        private DateTime _lastmessage = DateTime.UtcNow.ToLocalTime();

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("User");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("User");
            }
        }

        public string Pass
        {
            get
            {
                return _pass;
            }
            set
            {
                _pass = value;
                OnPropertyChanged("User");
            }
        }

        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnPropertyChanged("User");
            }
        }

        public bool Online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
                OnPropertyChanged("Online");
            }
        }

        public ObservableCollection<Message> Messages { get; set; }
        public DateTime LastMessage
        {
            get
            {
                return _lastmessage;
            }
            set
            {
                _lastmessage = value;
                OnPropertyChanged("LastMessage");
            }
        }

        public bool HaveUnseen
        {
            get
            {
                return _haveUnseen;
            }
            set
            {
                _haveUnseen = value;
                OnPropertyChanged("HaveUnseen");
            }
        }

        public int Unseen
        {
            get
            {
                return _unseen;
            }
            set
            {
                _unseen = value;
                OnPropertyChanged("Unseen");
            }
        }
    }
}
