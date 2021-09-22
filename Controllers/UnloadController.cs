using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Managers.Interfaces;
using Server.Settings;

namespace Server.Controllers 
{
    [Route("unload")]
    [ApiController]
    public class UnloadController : Controller
    {
        private readonly IUnloadManager _unloadManager;
        private readonly IAppSettings _appSettings;
        
        public UnloadController(IUnloadManager unloadManager, IAppSettings appSettings)
        {
            _unloadManager = unloadManager;
            _appSettings = appSettings;
        }   

        /// <summary>
        /// Выгрузить из таблицы Specialization, Course в виде файла формата xlsx всех студентов и их оценки за курсы на данной специализации
        /// </summary>
        /// <param name="specializationName">Название специализации</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [HttpPost("specialization")]
        public async Task<ActionResult> UnloadBySpecializationAsync([FromBody] string specializationName)
        {
            var result = await _unloadManager.UnloadBySpecializationAsync(specializationName);
            if (result.IsSuccess)
            {
                var filePath = Path.Combine(_appSettings.Path, _appSettings.UnloadSpecializationFileName) + ".xlsx";
                var fileName = _appSettings.UnloadSpecializationFileName + ".xlsx";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/force-download", fileName);
            }

            return BadRequest();
        }
        
        /// <summary>
        /// Выгрузить из таблицы Course в виде файла формата xlsx всех студентов и их оценки, выбравшие данные курсы
        /// </summary>
        /// <param name="courses">Название курсов</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [HttpPost("courses")]
        public async Task<ActionResult> UnloadByCoursesAsync([FromBody] List<string> courses)
        {
            var result = await _unloadManager.UnloadByCoursesAsync(courses);
            if (result.IsSuccess)
            {
                var filePath = Path.Combine(_appSettings.Path, _appSettings.UnloadCoursesFileName) + ".xlsx";
                var fileName = _appSettings.UnloadCoursesFileName + ".xlsx";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/force-download", fileName);
            }

            return BadRequest();
        }
    }
}