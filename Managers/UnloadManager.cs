using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Repository.Interfaces;
using Server.Settings;
using Aspose.Cells;
using Server.Services;

namespace Server.Managers
{
    public class UnloadManager : IUnloadManager
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAppSettings _appSettings;
        
        public UnloadManager(ISpecializationRepository specializationRepository,
            ICourseRepository courseRepository, IStudentRepository studentRepository, IAppSettings appSettings)
        {
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _appSettings = appSettings;
        }
        
        public async Task<OperationResult> UnloadAsync(List<string> courses, List<string> specializationTitles)
        {
            var coursesWorksheetAppender =
                new CoursesWorksheetAppender(_courseRepository, _studentRepository, _appSettings);
            var specializationWorksheetAppender = new SpecializationWorksheetAppender(_courseRepository,
                _studentRepository, _specializationRepository, _appSettings);
            var workbook = new Workbook();
            await coursesWorksheetAppender.Append(workbook, courses);
            foreach (var specializationTitle in specializationTitles)
                await specializationWorksheetAppender.Append(workbook, specializationTitle);
            workbook.Save(Path.Combine(_appSettings.Path, _appSettings.UnloadCoursesFileName) + ".xlsx");
            return OperationResult.Success();
        }
    }
}