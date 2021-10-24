﻿
using System;
using System.Collections.ObjectModel;

namespace ModernClient.MVVM1.Model
{
    [Serializable]
    public class User
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        public string Color { get; set; }

        public bool Online { get; set; }

        public ObservableCollection<Message> Messages { get; set; }

        //public string LastMessage => Messages.Last().Text;
        //deserelize to db
    }
}
