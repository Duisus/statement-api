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
        private readonly CoursesWorksheetAppender _coursesWorksheetAppender;
        private readonly SpecializationWorksheetAppender _specializationWorksheetAppender;
        
        public UnloadManager(ISpecializationRepository specializationRepository,
            ICourseRepository courseRepository, IStudentRepository studentRepository, IAppSettings appSettings)
        {
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _appSettings = appSettings;
            _coursesWorksheetAppender = new CoursesWorksheetAppender(courseRepository, studentRepository, appSettings);
            _specializationWorksheetAppender = new SpecializationWorksheetAppender(courseRepository, studentRepository,
                specializationRepository, appSettings);
        }
        
        public async Task<OperationResult> UnloadAsync(List<string> courses, List<string> specializationTitles)
        {
            var workbook = new Workbook();
            if(courses.Count > 0)
                await _coursesWorksheetAppender.Append(workbook, courses);
            foreach (var specializationTitle in specializationTitles)
                await _specializationWorksheetAppender.Append(workbook, specializationTitle);
            
            workbook.Worksheets.RemoveAt(0);
            workbook.Save(Path.Combine(_appSettings.Path, _appSettings.UnloadFileName) + ".xlsx");
            return OperationResult.Success();
        }
    }
}