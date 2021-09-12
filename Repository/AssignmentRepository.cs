using Microsoft.Extensions.Configuration;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository 
{
    public class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}