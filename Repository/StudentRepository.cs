using Microsoft.Extensions.Configuration;
using Server.Repository.Interfaces;
using Server.Models;

namespace Server.Repository 
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}