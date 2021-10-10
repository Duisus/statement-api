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
        //[FromQuery]//should be from body
        [FromForm]
        public string Email { get; set; }

        [Required(ErrorMessage = "password required")]
        //should be from body
        //[FromQuery]
        [FromForm]
        public string Password { get; set; }
    }
}
