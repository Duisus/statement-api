using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Dto.ModelDto
{
    public class AuthDto
    {
        [Required(ErrorMessage = "email required")]
        public string Email;
        [Required(ErrorMessage = "password required")]
        public string Password;
    }
}
