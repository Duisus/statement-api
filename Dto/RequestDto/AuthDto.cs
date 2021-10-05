using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace server.Dto.ModelDto
{
    public class AuthDto
    {
        [Required(ErrorMessage = "email required")]
        [FromHeader]//should be from body
        public string Email { get; set; }

        [Required(ErrorMessage = "password required")]
        //should be from body
        [FromHeader]
        public string Password { get; set; }
    }
}
