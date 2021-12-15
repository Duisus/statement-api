using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Org.BouncyCastle.Tsp;
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
            var tag = $"{GetType()}.{nameof(UpdateAsync)}";
            try
            {
                await ClearAllTables();
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"{tag}: An error occured while clearing tables with message: {e.Message}");
                return OperationResult.Error("Произошла ошибка при удалении данных");
            }
            
            var uploadResult = await UploadUntilErrorAsync();
            return uploadResult;
        }

        private async Task<OperationResult> UploadUntilErrorAsync()
        {
            var operationResult = await AddFromFile(
                _appSettings.StudentsFileName,
                _studentRepository,
                _csvParserManager.ParseStudentsCsvToStudents);
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddFromFile(
                _appSettings.SpecializationsFileName,
                _specializationRepository,
                _csvParserManager.ParseSpecializationCsvToSpecializations);
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddFromFile(
                _appSettings.CoursesFileName,
                _courseRepository,
                _csvParserManager.ParseCourseCsvToSpecializations);
            if (!operationResult.IsSuccess)
                return operationResult;

            operationResult = await AddFromFile(
                _appSettings.AssignmentFileName,
                _assignmentRepository,
                _csvParserManager.ParseAssignmentCsvToAssignments);
            if (!operationResult.IsSuccess)
                return operationResult;
            
            return OperationResult.Success();
        }

        private async Task<OperationResult> AddFromFile<T>(
            string fileName,
            IBaseRepository<T> repository,
            Func<string, Task<OperationResult<List<T>>>> parser) 
            where T : class
        {
            var result = await _uploadManager.UploadFormFileAsync(
                fileName,
                repository,
                parser);

            return result;
        }

        private async Task ClearAllTables()
        {
            await _courseRepository.Clear();
            await _assignmentRepository.Clear();
            await _studentRepository.Clear();
            await _specializationRepository.Clear();
        }
    }
}