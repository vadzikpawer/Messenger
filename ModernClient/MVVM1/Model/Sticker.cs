namespace ModernClient.MVVM1.Model
{
    class Sticker
    {
        private string _name;
        private string _nameSent;

        public string NameForSent
        {
            get 
            { 
                return _nameSent; 
            }
            set 
            { 
                _nameSent = value;
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
            }
        }

    }
}
