using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Dto;
using server.Dto.ModelDto;
using Server.Dto.ModelDto;
using Server.Dto.RequestDto;
using Server.Dto.ResponseDto;
using Server.Repository.Interfaces;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("students")]
    [ApiController]
    public class StudentController : Controller 
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IMapper _mapper;
        private readonly IUploadManager _uploadManager;
        private readonly IDataManager _dataManager;

        public StudentController(
            IStudentRepository studentRepository,
            ICsvParserManager csvParserManager, 
            IFileRepository fileRepository,
            IAppSettings appSettings,
            ISpecializationRepository specializationRepository,
            ICourseRepository courseRepository,
            IAssignmentRepository assignmentRepository, 
            IMapper mapper, IUploadManager uploadManager, 
            IDataManager dataManager)
        {
            _studentRepository = studentRepository;
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
            _uploadManager = uploadManager;
            _dataManager = dataManager;
        }

        [HttpPost]
        public async Task<ActionResult<GetStudentsResponseDto>> GetStudentAsync(GetStudentsRequestDto dto)
        {
            var result = await _dataManager.GetStudentsAsync(dto);
            return Ok(result);
        }
        
        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromStudentsFileAsync()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.StudentsFileName,
                _studentRepository,
                _csvParserManager.ParseStudentsCsvToStudents);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorText);
            }

            return Ok("Студенты успешно добавлены в базу данных");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDto>> GetStudentByIdWithInfoDto(int id)
        {
            var result = await _dataManager.GetStudentAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorText);
            }
            return Ok(result.Data);
        }
    }
}