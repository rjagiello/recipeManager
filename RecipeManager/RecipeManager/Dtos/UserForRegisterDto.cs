using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Hasło musi się składać od 6 do 20 znaków")]
        public string Password { get; set; }
    }
}
