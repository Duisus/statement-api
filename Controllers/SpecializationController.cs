using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Dto.ResponseDto;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("specializations")]
    [ApiController]
    public class SpecializationController : Controller 
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IAppSettings _appSettings;
        private readonly IUploadManager _uploadManager;

        public SpecializationController(
            ISpecializationRepository specializationRepository,
            ICsvParserManager csvParserManager, 
            IAppSettings appSettings, 
            IUploadManager uploadManager)
        {
            _specializationRepository = specializationRepository;
            _csvParserManager = csvParserManager;
            _appSettings = appSettings;
            _uploadManager = uploadManager;
        }

        /// <summary>
        /// Загрузить все специализации учеников из файла в базу данных в таблицу Specialization (по умолчанию файл specialization-report.csv
        /// </summary>
        /// <returns></returns>
        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromSpeciazilationsFileAsync()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.SpecializationsFileName,
                _specializationRepository,
                _csvParserManager.ParseSpecializationCsvToSpecializations);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorText);
            }

            return Ok("Специализации успешно добавлены в базу данных");
        }

        /// <summary>
        /// Получить все названия специализаций из таблицы Specialization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<GetAllSpecializationsResponseDto>> GetAllSpecializationsAsync()
        {
            var specializations = await _specializationRepository.GetAllAsync();
            var result = specializations
                .GroupBy(spec => spec.Title)
                .Select(spec => spec.FirstOrDefault()?.Title)
                .ToList();
            
            return Ok(new GetAllSpecializationsResponseDto() {Specializations = result});
        }
    }
}