using System.Collections.Generic;


namespace ConsoleApp
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public List<int> Friends_IDs { get; set; }
    }
}
