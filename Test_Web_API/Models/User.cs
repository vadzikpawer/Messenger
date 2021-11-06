using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Test_Web_API.Models
{
    [Table("Users")]
    [Serializable]
    public class User
    {

        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 3 символов")]

        //[Remote("IsUserExists","Home",ErrorMessage="User Name already in use")] TODO: create method to do it
        [Column("Name")]
        public string Name { get; set; }
        [Column("IsOnline")]
        public bool Online { get; set; }

        [Column("Pass")]
        public string Pass { get; set; }

        [Column("Color")]
        public string Color { get; set; }
        [Column("Salt")]
        public string Salt { get; set; }
        [Column("ConnectionID")]
        public string ConnectionID { get; set; }
    }
}
