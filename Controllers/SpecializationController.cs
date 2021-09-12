using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("specializations")]
    [ApiController]
    public class SpecializationController : Controller 
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;

        public SpecializationController(ISpecializationRepository specializationRepository,
            ICsvParserManager csvParserManager, IFileRepository fileRepository, IAppSettings appSettings)
        {
            _specializationRepository = specializationRepository;
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
        }

        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromSpeciazilationFileAsync()
        {
            var file = await _fileRepository.GetByFileNameAsync(_appSettings.SpecializationsFileName);
            if (file == null)
            {
                return BadRequest($"Невозможно загрузить специализации, так как файла {_appSettings.SpecializationsFileName} не существует");
            }

            var result = await _csvParserManager.ParseSpecializationCsvToSpecializations(file.Base64);
            if (result.Count == 0)
            {
                return BadRequest("Не удалось получить специализации из файла");
            }

            var addResult = await _specializationRepository.AddMultipleAsync(result);
            if (addResult == 0)
            {
                return BadRequest("Не удалось добавить специализации");
            }

            return Ok();
        }
    }
}