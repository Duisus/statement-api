using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cells;
using Server.Logic;
using Server.Models;
using Server.Repository.Interfaces;
using Server.Settings;

namespace Server.Services
{
    public class SpecializationWorksheetAppender : IWorksheetAppender<string>
    {
        private readonly IAppSettings _appSettings;
        private readonly ICourseRepository _courseRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IStudentRepository _studentRepository;

        public SpecializationWorksheetAppender(ICourseRepository courseRepository, IStudentRepository studentRepository,
            ISpecializationRepository specializationRepository, IAppSettings appSettings)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _specializationRepository = specializationRepository;
            _appSettings = appSettings;
        }

        public async Task<OperationResult> Append(Workbook workbook, string specializationTitle)
        {
            var getSpecializationsResult = await _specializationRepository.GetByTitleAsync(specializationTitle);
            if (!getSpecializationsResult.IsSuccess)
                return OperationResult.Error(getSpecializationsResult.ErrorText);

            var specializations = getSpecializationsResult.Data;
            var possibleCoursesCount = await GetPossibleCoursesCount(specializations);
            List<string> possibleCourses;

            if (specializations.Count > 5)
                possibleCourses = possibleCoursesCount.Where(keyValue => keyValue.Value > 5)
                    .Select(keyValue => keyValue.Key).ToList();
            else
                possibleCourses = possibleCoursesCount.Select(keyValue => keyValue.Key).ToList();
            
            var sheetName = specializationTitle
                .Replace(":", "")
                .Replace("/","")
                .Replace("\\","")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("[", "")
                .Replace("]", ""); 
            sheetName = sheetName.Length < 31 ? sheetName : sheetName.Substring(0, 31);
            var sheet = workbook.Worksheets.Add(sheetName);

            //appending header
            var cell = sheet.Cells[0, 0];
            cell.PutValue("ФИО студента");
            for (var i = 0; i < possibleCourses.Count; i++)
            {
                cell = sheet.Cells[0, i + 1];
                cell.PutValue(possibleCourses.ElementAt(i));
            }

            var row = 1;
            var column = 0;
            foreach (var specialization in specializations)
            {
                var student = await _studentRepository.GetAsync(specialization.StudentId);
                var courses = await _courseRepository.GetBySpecializationIdAsync(specialization.Id);
                if (!courses.IsSuccess)
                    continue;
                cell = sheet.Cells[row, column];
                cell.PutValue(student.Data.FullName);
                column++;
                for (var i = 0; i < possibleCourses.Count; i++)
                {
                    var course = courses.Data.FirstOrDefault(crs => crs.Title == possibleCourses.ElementAt(i));
                    cell = sheet.Cells[row, column];
                    if (course is not null)
                        cell.PutValue(Math.Round(course.Grade, 2));
                    else
                        cell.PutValue(0);

                    column++;
                }

                column = 0;
                row++;
            }

            sheet.AutoFitColumns();
            return OperationResult.Success();
        }

        private async Task<Dictionary<string, int>> GetPossibleCoursesCount(List<Specialization> specializations)
        {
            var possibleCoursesCount = new Dictionary<string, int>();
            foreach (var specialization in specializations)
            {
                var getCourses = await _courseRepository.GetBySpecializationIdAsync(specialization.Id);
                if (getCourses.IsSuccess)
                    foreach (var course in getCourses.Data)
                    {
                        if (!possibleCoursesCount.ContainsKey(course.Title))
                            possibleCoursesCount.Add(course.Title, 0);

                        possibleCoursesCount[course.Title]++;
                    }
            }

            return possibleCoursesCount;
        }
    }
}