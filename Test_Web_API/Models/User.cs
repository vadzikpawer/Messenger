using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Test_Web_API.Models
{
    [Table("Users")]
    [Serializable]
    public class User
    {

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 3 символов")]
        //[Remote("IsUserExists","Home",ErrorMessage="User Name already in use")] TODO: create method to do it
        [Column("Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 символов")]
        [Column("Pass")]
        public string Pass { get; set; }

        [Column("Age")]
        //[Range(1, 110, ErrorMessage = "Недопустимый возраст")] TODO: сделать поле не обязательным для проверки
        public int Age { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Remote("CheckEmailExist", "UsersController", ErrorMessage = "The Email Exists")]
        [Column("Email")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Friends_IDs { get; set; }//deserelize to db
    }
}
