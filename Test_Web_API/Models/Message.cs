using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Web_API.Models
{
    [Serializable]
    [Table("Messages")]
    public class Message
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("From")]
        [Required]
        public string From { get; set; }

        [Required]
        [Column("To")]
        public string To { get; set; }

        [Column("Text")]
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

        public DateTime dateStapm { get; set; }
        // TODO: add datetime stamp
    }
}
