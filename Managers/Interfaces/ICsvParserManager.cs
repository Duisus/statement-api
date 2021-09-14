using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;
using Server.Models;

namespace Server.Managers.Interfaces
{
    public interface ICsvParserManager
    {
        public Task<OperationResult<List<Student>>> ParseStudentsCsvToStudents(string file);

        public Task<OperationResult<List<Specialization>>> ParseSpecializationCsvToSpecializations(string file);
        
        public Task<OperationResult<List<Course>>> ParseCourseCsvToSpecializations(string file);

        public Task<OperationResult<List<Assignment>>> ParseAssignmentCsvToAssignments(string file);
    }
}