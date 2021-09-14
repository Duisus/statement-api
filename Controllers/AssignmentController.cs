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
        private readonly IAppSettings _appSettings;
        private readonly IUploadManager _uploadManager;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICsvParserManager _csvParserManager;

        public AssignmentController(IAppSettings appSettings,
            IUploadManager uploadManager,
            IAssignmentRepository assignmentRepository,
            ICsvParserManager csvParserManager)
        {
            _appSettings = appSettings;
            _uploadManager = uploadManager;
            _assignmentRepository = assignmentRepository;
            _csvParserManager = csvParserManager;
        }

        [HttpGet("uploadfromfile")]
        public async Task<ActionResult> AddFromAssignmentFileAsync()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.AssignmentFileName,
                _assignmentRepository, _csvParserManager.ParseAssignmentCsvToAssignments);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorText);
            }

            return Ok("Задания успешно добавлены в базу данных");
        }
    }
}