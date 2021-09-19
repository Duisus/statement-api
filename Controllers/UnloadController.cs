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
    }
}