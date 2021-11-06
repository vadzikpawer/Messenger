using ModernClient.Core;
using System;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class Message : ObservableObject
    {

        public int Id { get; set; }

        public int From { get; set; }

        public string FromName { get; set; }

        public int To { get; set; }

        public string Text { get; set; }
        public string Color { get; set; }

        public DateTime dateStapm { get; set; }

        public bool IsSticker { get; set; }
        public bool IsSeen { get; set; }

        public string _PathToSticker { get; set; }
        public string PathToSticker
        {
            get
            {
                return _PathToSticker;
            }
            set
            {
                _PathToSticker = value;
                OnPropertyChanged("PathToSticker");
            }
        }


        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

    }
}
