﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Repository.Interfaces;
using Server.Settings;
using Aspose.Cells;

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
        
        public async Task<OperationResult> UnloadBySpecializationAsync(string specializationTitle)
        {
            var getSpecializationsResult = await _specializationRepository.GetByTitleAsync(specializationTitle);
            if (!getSpecializationsResult.IsSuccess)
            {
                return OperationResult.Error(getSpecializationsResult.ErrorText);
            }

            var specializations = getSpecializationsResult.Data;
            var possibleCoursesCount = new Dictionary<string, int>();
            var possibleCourses = new List<string>();
            foreach (var specialization in specializations)
            {
                var getCourses = await _courseRepository.GetBySpecializationIdAsync(specialization.Id);
                if (getCourses.IsSuccess)
                {
                    foreach (var course in getCourses.Data)
                    {
                        if (!possibleCoursesCount.ContainsKey(course.Title))
                        {
                            possibleCoursesCount.Add(course.Title, 0);
                        }
                        possibleCoursesCount[course.Title]++;
                    }
                }
            }

            if (specializations.Count > 5)
                possibleCourses = possibleCoursesCount.Where(keyValue => keyValue.Value > 5)
                    .Select(keyValue => keyValue.Key).ToList();
            else
            {
                possibleCourses = possibleCoursesCount.Select(keyValue => keyValue.Key).ToList();
            }

            var workbook = new Workbook();
            var sheet = workbook.Worksheets[0];
            
            //appending header
            var cell = sheet.Cells[0,0];
            cell.PutValue("ФИО студента");
            for (int i = 0; i < possibleCourses.Count; i++)
            {
                cell = sheet.Cells[0,i + 1];
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
                for (int i = 0; i < possibleCourses.Count; i++)
                {
                    var course = courses.Data.FirstOrDefault(crs => crs.Title == possibleCourses.ElementAt(i));
                    cell = sheet.Cells[row, column];
                    if (course is not null)
                    {
                        cell.PutValue(course.IsCompleted ? 100 : 0);
                    }
                    else
                    {
                        cell.PutValue(0);
                    }

                    column++;
                }

                column = 0;
                row++;
            }
            sheet.AutoFitColumns();
            workbook.Save(Path.Combine(_appSettings.Path, _appSettings.UnloadSpecializationFileName) + ".xlsx");
            return OperationResult.Success();
        }

        public async Task<OperationResult> UnloadByCoursesAsync(List<string> courses)
        {
            throw new System.NotImplementedException();
        }
    }
}