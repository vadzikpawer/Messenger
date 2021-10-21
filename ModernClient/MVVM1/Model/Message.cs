using System;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class Message
    {

        public int Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Text { get; set; }

        public DateTime dateStapm { get; set; }

        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

    }
}
