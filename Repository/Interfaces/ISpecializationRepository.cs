using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;
using Server.Models;

namespace Server.Repository.Interfaces 
{
    public interface ISpecializationRepository : IBaseRepository<Specialization> 
    {
        public Task<OperationResult<List<Specialization>>> GetByTitleAsync(string title); 
    }
}