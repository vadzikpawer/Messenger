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
        public int From { get; set; }
        [Column("FromName")]
        [Required]
        public string FromName { get; set; }

        [Required]
        [Column("To")]
        public int To { get; set; }

        [Column("Text")]
        public string Text { get; set; }

        [Column("Color")]
        public string Color { get; set; }

        [Column("dateStamp")]
        public DateTime dateStapm { get; set; }
        [Column("IsSticker")]
        public bool IsSticker { get; set; }
        [Column("PathToSticker")]
        public string PathToSticker { get; set; }
        [Column("IsSeen")]
        public bool IsSeen { get; set; }

        public override string ToString()
        {
            return $"{From} {To} {Text} {Id}";
        }

    }
}
