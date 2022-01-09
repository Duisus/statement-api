using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;

namespace Server.Managers.Interfaces
{
    public interface IUnloadManager
    {
        /*public Task<OperationResult> UnloadBySpecializationAsync(string specializaiton);

        public Task<OperationResult> UnloadByCoursesAsync(List<string> courses);*/
        
        public Task<OperationResult> UnloadAsync(List<string> courses, List<string> specializationTitles);
    }
}