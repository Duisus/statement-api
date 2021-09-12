using System.Threading.Tasks;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository.Interfaces
{
    public interface IFileRepository : IBaseRepository<FileModel>
    {
        public Task<FileModel> GetByFileNameAsync(string fileName);
    }
}