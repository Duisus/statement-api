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

        [HttpGet("specialization/{title}")]
        public async Task<ActionResult> UnloadBySpecializationAsync(string title)
        {
            var result = await _unloadManager.UnloadBySpecializationAsync(title);
            if (result.IsSuccess)
            {
                var filePath = Path.Combine(_appSettings.Path, _appSettings.UnloadSpecializationFileName) + ".csv";
                var fileName = _appSettings.UnloadSpecializationFileName + ".csv";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                return File(fileBytes, "application/force-download", fileName);
            }

            return BadRequest();
        }
    }
}