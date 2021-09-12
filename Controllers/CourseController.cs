using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("courses")]
    [ApiController]
    public class CourseController : Controller 
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;
        private readonly ICourseRepository _courseRepository;

        public CourseController(
            ISpecializationRepository specializationRepository,
            ICsvParserManager csvParserManager,
            IFileRepository fileRepository, 
            IAppSettings appSettings,
            ICourseRepository courseRepository)
        {
            _specializationRepository = specializationRepository;
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
            _courseRepository = courseRepository;
        }

        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromCoursesFileAsync()
        {
            var file = await _fileRepository.GetByFileNameAsync(_appSettings.CoursesFileName);
            if (file == null)
            {
                return BadRequest($"Невозможно загрузить курсы, так как файла {_appSettings.CoursesFileName} не существует");
            }
            
            var result = await _csvParserManager.ParseCourseCsvToSpecializations(file.Base64);
            if (result.Count == 0)
            {
                return BadRequest("Не удалось получить курсы из файла");
            }

            var addResult = await _courseRepository.AddMultipleAsync(result);
            if (addResult == 0)
            {
                return BadRequest("Не удалось добавить курсы");
            }

            return Ok();
        }
    }
}