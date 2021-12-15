using System.Threading.Tasks;
using Server.Logic;

namespace Server.Managers.Interfaces
{
    public interface IUpdateManager
    {
        public Task<OperationResult> UpdateAsync();
    }
}