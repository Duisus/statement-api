using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Server.Dto.ResponseDto;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("courses")]
    [ApiController]
    [Authorize]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CourseController : Controller 
    {
        private readonly ICsvParserManager _csvParserManager;
        private readonly IAppSettings _appSettings;
        private readonly ICourseRepository _courseRepository;
        private readonly IUploadManager _uploadManager;

        public CourseController(
            ICsvParserManager csvParserManager,
            IAppSettings appSettings,
            ICourseRepository courseRepository,
            IUploadManager uploadManager)
        {
            _csvParserManager = csvParserManager;
            _appSettings = appSettings;
            _courseRepository = courseRepository;
            _uploadManager = uploadManager;
        }
        
        /// <summary>
        /// Загрузить в базу данных курсы из файла (по умолчанию файл usage-report.csv)
        /// </summary>
        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromCoursesFileAsync()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.CoursesFileName,
                _courseRepository,
                _csvParserManager.ParseCourseCsvToSpecializations);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorText);
            }

            return Ok("Курсы успешно добавлены в базу данных");
        }
        
        /// <summary>
        /// Получить все названия курсов
        /// </summary>
        [ProducesResponseType(typeof(GetAllCoursesResponseDto), 200)]
        [HttpGet]
        public async Task<ActionResult<GetAllCoursesResponseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            var result = courses
                .GroupBy(spec => spec.Title)
                .Select(spec => spec.FirstOrDefault()?.Title)
                .ToList();
            return  Ok(new GetAllCoursesResponseDto() {Courses = result});
        }
    }
}