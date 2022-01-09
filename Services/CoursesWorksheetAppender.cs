using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aspose.Cells;
using Server.Logic;
using Server.Repository.Interfaces;
using Server.Settings;

namespace Server.Services
{
    public class CoursesWorksheetAppender : IWorksheetAppender<List<string>>
    {
        private readonly IAppSettings _appSettings;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;

        public CoursesWorksheetAppender(ICourseRepository courseRepository, IStudentRepository studentRepository,
            IAppSettings appSettings)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _appSettings = appSettings;
        }

        public async Task<OperationResult> Append(Workbook workbook, List<string> coursesTitles)
        {
            var sheet = workbook.Worksheets[0];
            var row = 0;
            var column = 0;
            foreach (var courseTitle in coursesTitles)
            {
                var getCourses = await _courseRepository.GetByTitleAsync(courseTitle);
                if (!getCourses.IsSuccess)
                    return OperationResult.Error("Ошибка при получении курса с названием " + courseTitle);

                var courses = getCourses.Data;
                sheet.Cells[row, column].PutValue("ФИО студента");
                sheet.Cells[row, column + 1].PutValue(courseTitle);

                foreach (var course in courses)
                {
                    var student = await _studentRepository.GetAsync(course.StudentId);
                    row += 1;
                    column = 0;
                    sheet.Cells[row, column].PutValue(student.Data.FullName);
                    sheet.Cells[row, column + 1].PutValue(Math.Round(course.Grade, 2));
                }

                row += 2;
            }

            sheet.AutoFitColumns();
            workbook.Save(Path.Combine(_appSettings.Path, _appSettings.UnloadCoursesFileName) + ".xlsx");
            return OperationResult.Success();
        }
    }
}