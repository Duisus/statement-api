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
        [FromForm]
        public string Email { get; set; }

        [FromForm]
        public string Password { get; set; }
    }
}
