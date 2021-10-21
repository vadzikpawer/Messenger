
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class User
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        public string Color { get; set; }

        public ObservableCollection<Message> Messages { get; set; }

        public string LastMessage => Messages.Last().Text;
        //deserelize to db
    }
}
