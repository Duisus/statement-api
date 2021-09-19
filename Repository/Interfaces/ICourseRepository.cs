using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository.Interfaces 
{
    public interface ICourseRepository : IBaseRepository<Course>
    {
        public Task<OperationResult<List<Course>>> GetBySpecializationIdAsync(int id);

        public Task<OperationResult<List<Course>>> GetByTitleAsync(string title);
    }
}