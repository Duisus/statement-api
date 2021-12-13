using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Logic;
using server.Managers.Interfaces;
using Server.Models;
using Server.Repository.Interfaces;
using Server.Settings;
using server.Validators;

namespace Server.Controllers
{
    [Route("files")]
    [ApiController]
    [Authorize]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class FileController : Controller
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileUpdater _fileUpdater;
        private readonly IFileValidator _fileValidator;
        private readonly IAppSettings _appSettings;

        public FileController(IFileRepository fileRepository,IFileUpdater fileUpdater,IFileValidator fileValidator, IAppSettings appSettings)
        {
            _fileRepository = fileRepository;
            _fileUpdater = fileUpdater;
            _fileValidator = fileValidator;
            _appSettings = appSettings;
        }
        
        /// <summary>
        /// Загрузить файл в папку, указанную в настройках. Также вставляется новая запись в базу данных с названием файла
        /// </summary>
        /// <returns>id вставленной записи в базе</returns>
        [ProducesResponseType(typeof(int),200)]
        [HttpPost("upload")]
        public async Task<ActionResult> UploadFileAsync(IFormFile uploadedFile)
        {
            if (uploadedFile == null)
            {
                return BadRequest("empty file");
            }
            var existingFile = await _fileRepository.GetByFileNameAsync(uploadedFile.FileName);
            if (existingFile != null)
            {
                Serilog.Log.Warning($"File with name {existingFile.FileName} already exists, rewriting...");
            }

            var fileStream = new FileStream(_appSettings.Path + $"\\{uploadedFile.FileName}", FileMode.Create, FileAccess.Write);
            await uploadedFile.CopyToAsync(fileStream);
            await fileStream.DisposeAsync();
            
            var newFile = new FileModel
            {
                FileName = uploadedFile.FileName,
            };

            var id = await _fileRepository.AddAsync(newFile);
            if (id == 0)
            {
                return BadRequest("Произошла ошибка при загрузке файла");
            }

            return Ok(id);
        }

        /// <summary>
        /// Получить все записи в таблице FileModel
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileModel>),200)]
        public async Task<ActionResult<IEnumerable<FileModel>>> GetAllFilesAsync()
        {
            var result = await _fileRepository.GetAllAsync();
            return Ok(result);
        }
        
        /// <summary>
        /// Получить запись в таблице FileModel по id
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FileModel>> GetFileAsync(int id) 
        {
            var result = await _fileRepository.GetAsync(id);
            if (result != null) 
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost("uploadNew")]
        public async Task<OperationResult> UploadNewData(
            [FromForm(Name = "students")] IFormFile students,
            [FromForm(Name = "specializations")] IFormFile specializations,
            [FromForm(Name = "courses")] IFormFile courses,
            [FromForm(Name = "assignments")] IFormFile assignments)
        {
            var isValid = _fileValidator.IsValid(students, specializations, courses, assignments);

            if (!isValid.IsSuccess)
                return isValid;

            await _fileUpdater.Update(students, specializations, courses, assignments);
            return OperationResult.Success();
        }
    }
}