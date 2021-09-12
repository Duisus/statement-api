using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Managers.Interfaces
{
    public interface ICsvParserManager
    {
        public Task<List<string[]>> ParseCsvFileAsync(string delimeter, string base64String);

        public Task<List<Student>> ParseMembershipCsvToStudents(string base64String);

        public Task<List<Specialization>> ParseSpecializationCsvToSpecializations(string base64String);
        
        public Task<List<Course>> ParseCourseCsvToSpecializations(string base64String);

        public Task<List<Assignment>> ParseAssignmentCsvToAssignments(string base64String);
    }
}