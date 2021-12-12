using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Logic;

namespace server.Managers.Interfaces
{
    public interface IFileUpdater
    {
        Task<OperationResult> Update(IFormFile students, IFormFile specialization, IFormFile course, IFormFile assignmetns);
    }
}