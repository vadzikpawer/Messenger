using System;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class Message
    {

        public int Id { get; set; }

        public int From { get; set; }
        public string FromName { get; set; }

        public int To { get; set; }

        public string Text { get; set; }

        public DateTime dateStapm { get; set; }

        public bool IsSticker { get; set; }

        public string PathToSticker { get; set; }

        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

    }
}
