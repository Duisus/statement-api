using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("assignments")]
    [ApiController]
    public class AssignmentController : Controller 
    {
        private readonly ICsvParserManager _csvParserManager;
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentController(
            ICsvParserManager csvParserManager, 
            IFileRepository fileRepository,
            IAppSettings appSettings, 
            IAssignmentRepository assignmentRepository)
        {
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
            _assignmentRepository = assignmentRepository;
        }

        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromAssignmentFileAsync()
        {
            var file = await _fileRepository.GetByFileNameAsync(_appSettings.AssignmentFileName);
            if (file == null)
            {
                return BadRequest($"Невозможно загрузить специализации, так как файла {_appSettings.AssignmentFileName} не существует");
            }

            var result = await _csvParserManager.ParseAssignmentCsvToAssignments(file.Base64);
            if (result.Count == 0)
            {
                return BadRequest("Не удалось получить специализации из файла");
            }

            var addResult = await _assignmentRepository.AddMultipleAsync(result);
            if (addResult == 0)
            {
                return BadRequest("Не удалось добавить специализации");
            }

            return Ok();
        }
    }
}