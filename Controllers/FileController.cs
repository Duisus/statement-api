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
using Server.Models;
using Server.Repository.Interfaces;
using Server.Settings;

namespace Server.Controllers
{
    [Route("files")]
    [ApiController]
    [Authorize]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class FileController : Controller
    {
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;
        public FileController(IFileRepository fileRepository, IAppSettings appSettings)
        {
            _fileRepository = fileRepository;
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
            
            var newFile = new FileModel()
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
    }
}