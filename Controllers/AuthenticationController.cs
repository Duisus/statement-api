using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using server.Dto.ModelDto;
using server.Models;
using server.Repository;

namespace server.Controllers
{
    [Route("Auth")]
    public class AuthenticationController : Controller
    {
        private readonly UserRepository _userRepository;

        public AuthenticationController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("Login")]
        public ActionResult Login()
        {
            return View("Auth/index");
        }

        [HttpPost("LoginInner")]
        public async Task<ActionResult> LoginInner(AuthDto auth)
        {
            var user = await _userRepository.SearchAsync(auth.Email);
            if (user == null || user.Count == 0 || !IsDtoValid(auth))
                return NotFound("неверный логин или пароль");

            if (user.Count > 1)
                return Conflict("внутренняя ошибка сервиса");

            if (!IsPasswordsEqual(user.First(), auth.Password))
                return NotFound("неверный логин или пароль");

            await Authenticate(auth.Email);
            return Redirect("/index.html");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private static bool IsPasswordsEqual(User user, string enteredPassword)
        {
            var salt = Convert.FromBase64String(user.Salt);

            var hashedEnteredPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                enteredPassword,
                salt,
                KeyDerivationPrf.HMACSHA256,
                1000,
                256 / 8));
            return hashedEnteredPassword == user.Password;
        }

        private static bool IsDtoValid(AuthDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Password) && !string.IsNullOrWhiteSpace(dto.Email);
        }
    }
}