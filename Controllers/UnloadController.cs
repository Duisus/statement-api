using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Dto.ResponseDto;
using Server.Managers.Interfaces;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("unload")]
    [ApiController]
    [Authorize]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UnloadController : Controller
    {
        private readonly IUnloadManager _unloadManager;
        private readonly IAppSettings _appSettings;
        
        public UnloadController(IUnloadManager unloadManager, IAppSettings appSettings)
        {
            _unloadManager = unloadManager;
            _appSettings = appSettings;
        }
        
        [ProducesResponseType(typeof(GetCoursesAndSpecializationsToUnloadResponseDto), 200)]
        [HttpPost("unload")]
        public async Task<ActionResult> UnloadAsync([FromBody] GetCoursesAndSpecializationsToUnloadResponseDto unloadResponseDto)
        {
            var courses = unloadResponseDto.Courses;
            var specializationName = unloadResponseDto.Specializations;
            var result = await _unloadManager.UnloadAsync(courses, specializationName);
            if (result.IsSuccess)
            {
                var filePath = Path.Combine(_appSettings.Path, _appSettings.UnloadFileName) + ".xlsx";
                var fileName = _appSettings.UnloadFileName + ".xlsx";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/force-download", fileName);
            }

            return BadRequest();
        }
    }
}