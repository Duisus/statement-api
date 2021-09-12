using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IFileRepository _fileRepository;
        public FileController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }
        
        [ProducesResponseType(typeof(int),200)]
        [HttpPost("upload")]
        public async Task<ActionResult> UploadFileAsync(IFormFile uploadedFile)
        {
            if (uploadedFile == null)
            {
                return BadRequest();
            }

            var existingFile = await _fileRepository.GetByFileNameAsync(uploadedFile.FileName);
            if (existingFile != null)
            {
                return BadRequest($"File with name {existingFile.FileName} already exists");
            }
            await using var fileStream = uploadedFile.OpenReadStream();
            byte[] bytes = new byte[uploadedFile.Length];
            await fileStream.ReadAsync(bytes, 0, (int)uploadedFile.Length);
            var newFile = new FileModel()
            {
                Base64 = Convert.ToBase64String(bytes),
                FileName = uploadedFile.FileName,
            };

            var id = await _fileRepository.AddAsync(newFile);
            if (id <= 0)
            {
                return BadRequest("Произошла ошибка при загрузке файла");
            }

            return Ok(id);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileModel>),200)]
        public async Task<ActionResult<IEnumerable<FileModel>>> GetAllFilesAsync()
        {
            var result = await _fileRepository.GetAllAsync();
            return Ok(result);
        }
        
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