using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Dto;
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

        public StudentController(
            IStudentRepository studentRepository,
            ICsvParserManager csvParserManager, 
            IFileRepository fileRepository,
            IAppSettings appSettings,
            ISpecializationRepository specializationRepository,
            ICourseRepository courseRepository,
            IAssignmentRepository assignmentRepository, 
            IMapper mapper)
        {
            _studentRepository = studentRepository;
            _csvParserManager = csvParserManager;
            _fileRepository = fileRepository;
            _appSettings = appSettings;
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Student>> GetStudentAsync(int id) 
        {
            var result = await _studentRepository.GetAsync(id);
            if (result != null) 
            {
                return Ok(result);
            }
            return NotFound();
        }
        
        [HttpGet("addfromfile")]
        public async Task<ActionResult> AddFromMembershipFile()
        {
            var file = await _fileRepository.GetByFileNameAsync(_appSettings.StudentsFileName);
            if (file == null)
            {
                return BadRequest($"Невозможно загрузить студентов, так как файла {_appSettings.StudentsFileName} не существует");
            }

            var result = await _csvParserManager.ParseMembershipCsvToStudents(file.Base64);
            if (result.Count == 0)
            {
                return BadRequest("Не удалось получить студентов");
            }

            var addResult = await _studentRepository.AddMultipleAsync(result);
            if (addResult == 0)
            {
                return BadRequest("Не удалось добавить студентов");
            }

            return Ok();
        }
        
        [HttpGet]
        public async Task<ActionResult<StudentDto>> GetAllStudentsAsync()
        {
            var result = await _studentRepository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("withinfo/{id:int}")]
        public async Task<ActionResult<GetStudentWithInfoRequestDto>> GetStudentByIdWithInfoDto(int id)
        {
            var student = await _studentRepository.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var specializations = await _specializationRepository.GetByStudentIdColumnAsync(student.Id);
            var courses = await _courseRepository.GetByStudentIdColumnAsync(student.Id);
            var assignments = await _assignmentRepository.GetByStudentIdColumnAsync(student.Id);
            return Ok(new GetStudentWithInfoRequestDto()
            {
                Student = _mapper.Map<StudentDto>(student),
                Assignments = _mapper.Map<List<AssignmentDto>>(assignments),
                Courses = _mapper.Map<List<CourseDto>>(courses),
                Specializations = _mapper.Map<List<SpecializationDto>>(specializations)
            });
        }
    }
}