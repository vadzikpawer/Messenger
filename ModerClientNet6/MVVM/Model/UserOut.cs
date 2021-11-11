
using System;

namespace ModernClientNet6.MVVM.Model
{
    [Serializable]
    public class UserOut
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        public string Color { get; set; }
        //public string LastMessage => Messages.Last().Text;
        //deserelize to db
    }
}
