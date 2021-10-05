using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class StudentController : Controller 
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IAppSettings _appSettings;
        private readonly IUploadManager _uploadManager;
        private readonly IDataManager _dataManager;

        public StudentController(
            IStudentRepository studentRepository,
            ICsvParserManager csvParserManager,
            IAppSettings appSettings,
            IUploadManager uploadManager, 
            IDataManager dataManager)
        {
            _studentRepository = studentRepository;
            _csvParserManager = csvParserManager;
            _appSettings = appSettings;
            _uploadManager = uploadManager;
            _dataManager = dataManager;
        }
        
        /// <summary>
        /// Получить студентов со следующей информацией:
        /// ФИО, id в таблице Student, среднее кол-во часов обучения, средняя оценка за курс
        /// </summary>
        [ProducesResponseType(typeof(GetStudentsResponseDto), 200)]
        [HttpPost]
        public async Task<ActionResult<GetStudentsResponseDto>> GetStudentAsync(GetStudentsRequestDto dto)
        {
            var result = await _dataManager.GetStudentsAsync(dto);
            return Ok(result);
        }
        
        /// <summary>
        /// Загрузить студентов в таблицу Student из файла (по умолчанию membership-report.csv)
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получить информацию о студенте, его специализациях, курсах, заданиях
        /// </summary>
        /// <param name="id">id в таблице Studdent</param>
        [ProducesResponseType(typeof(StudentDto), 200)]
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