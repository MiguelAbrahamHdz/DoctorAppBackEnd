using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ClsRegistroDTO
    {
        [Required(ErrorMessage = "Username es requerido")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password es requerido")]
        public string Password { get; set; }
    }
}
