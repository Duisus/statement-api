using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Dto.ResponseDto;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
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
        private readonly IUploadManager _uploadManager;

        public CourseController(
            ISpecializationRepository specializationRepository,
            ICsvParserManager csvParserManager,
            IFileRepository fileRepository, 
            IAppSettings appSettings,
            ICourseRepository courseRepository,
            IUploadManager uploadManager)
        {
            _specializationRepository = specializationRepository;
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
            _courseRepository = courseRepository;
            _uploadManager = uploadManager;
        }

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
        
        [HttpGet]
        public async Task<ActionResult<GetAllCoursesResponseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            var result = courses
                .GroupBy(spec => spec.Title)
                .Select(spec => spec.FirstOrDefault()?.Title)
                .ToList();
            
            return Ok(new GetAllCoursesResponseDto() {Courses = result});
        }
    }
}