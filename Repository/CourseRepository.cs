using Microsoft.Extensions.Configuration;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository 
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}