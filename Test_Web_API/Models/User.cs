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
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 3 символов")]
        [Key]
        //[Remote("IsUserExists","Home",ErrorMessage="User Name already in use")] TODO: create method to do it
        [Column("Name")]
        public string Name { get; set; }
        [Column("Pass")]
        public string Pass { get; set; }

    }
}
