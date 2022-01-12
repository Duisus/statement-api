using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;

namespace Server.Managers.Interfaces
{
    public interface IUnloadManager
    {
        public Task<OperationResult> UnloadAsync(List<string> courses, List<string> specializationTitles);
    }
}