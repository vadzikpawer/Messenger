using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp
{
   
    public class Message
    {
        
        public int Id { get; set; }
        
        public string From { get; set; }

        public string To { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

        // TODO: add datetime stamp
    }
}
