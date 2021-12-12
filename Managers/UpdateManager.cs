using System.Threading.Tasks;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Repository.Interfaces;
using Server.Settings;

namespace Server.Managers
{
    public class UpdateManager : IUpdateManager
    {
        public UpdateManager(
            IStudentRepository studentRepository, 
            ISpecializationRepository specializationRepository, 
            ICourseRepository courseRepository, 
            IAssignmentRepository assignmentRepository, 
            ICsvParserManager csvParserManager, 
            IAppSettings appSettings, 
            IUploadManager uploadManager)
        {
            _studentRepository = studentRepository;
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
            _csvParserManager = csvParserManager;
            _appSettings = appSettings;
            _uploadManager = uploadManager;
        }
        
        private readonly IStudentRepository _studentRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICsvParserManager _csvParserManager;
        private readonly IAppSettings _appSettings;
        private readonly IUploadManager _uploadManager;
        
        public async Task<OperationResult> UpdateAsync()
        {
            await TruncateAllTables();
            
            var uploadResult = await UploadUntilErrorAsync();
            return uploadResult;
        }

        private async Task<OperationResult> UploadUntilErrorAsync()
        {
            var operationResult = await AddStudents();
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddSpecializations();
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddCourses();
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddAssignments();
            if (!operationResult.IsSuccess)
                return operationResult;
            
            return OperationResult.Success();
        }

        private async Task<OperationResult> AddStudents()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.StudentsFileName,
                _studentRepository,
                _csvParserManager.ParseStudentsCsvToStudents);

            return result;
        }

        private async Task<OperationResult> AddSpecializations()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.SpecializationsFileName,
                _specializationRepository,
                _csvParserManager.ParseSpecializationCsvToSpecializations);

            return result;
        }

        private async Task<OperationResult> AddCourses()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.CoursesFileName,
                _courseRepository,
                _csvParserManager.ParseCourseCsvToSpecializations);

            return result;
        }

        private async Task<OperationResult> AddAssignments()
        {
            var result = await _uploadManager.UploadFormFileAsync(
                _appSettings.AssignmentFileName,
                _assignmentRepository,
                _csvParserManager.ParseAssignmentCsvToAssignments);

            return result;
        }

        private async Task TruncateAllTables()
        {
            await _studentRepository.TruncateAsync();
            await _specializationRepository.TruncateAsync();
            await _courseRepository.TruncateAsync();
            await _assignmentRepository.TruncateAsync();
        }
    }
}