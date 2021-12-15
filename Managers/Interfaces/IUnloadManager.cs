using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;

namespace Server.Managers.Interfaces
{
    public interface IUnloadManager
    {
        public Task<OperationResult> UnloadBySpecializationAsync(List<string> specializaiton);

        public Task<OperationResult> UnloadByCoursesAsync(List<string> courses);
    }
}