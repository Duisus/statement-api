using Microsoft.Extensions.Configuration;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository 
{
    public class SpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}