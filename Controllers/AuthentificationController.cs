using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dto.ModelDto;
using Server.Logic;
using Server.Models;
using server.Repository;

namespace server.Controllers
{
    [Route("Auth")]
    public class AuthentificationController : Controller
    {
        private readonly UserRepository _userRepository;

        public AuthentificationController(UserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [HttpGet("IsAuthorized")]
        public bool IsAuthorized()
        {
            var identity = HttpContext.User.Identity;
            return identity != null && identity.IsAuthenticated;
        }

        [HttpGet("Login")]
        public async Task<ActionResult> Login()
        {
            return Redirect("http://localhost:3000/Auth");
        }

        [HttpPost("LoginInner")]
        public async Task<ActionResult> LoginInner([FromHeader] AuthDto auth, [FromQuery] string returnUrl)
        {
            var user =await _userRepository.SearchAsync(auth.Email, auth.Password);
            if (user == null || user.Count==0)
                return Ok("неверный логин или пароль");

            if (user.Count != 1)
                return Conflict("внутренняя ошибка сервиса, обратитесь в тех поддержку");

            await Authenticate(auth.Email);

            return Redirect(returnUrl);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
