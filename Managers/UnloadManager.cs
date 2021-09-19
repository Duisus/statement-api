using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Repository.Interfaces;
using Server.Settings;

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
            var possibleCourses = new HashSet<string>();
            var csv = new StringBuilder();
            foreach (var specialization in specializations)
            {
                var getCourses = await _courseRepository.GetBySpecializationIdAsync(specialization.Id);
                if (getCourses.IsSuccess)
                {
                    foreach (var course in getCourses.Data)
                    {
                        possibleCourses.Add(course.Title);
                    }
                }
            }
            //appending header
            csv.AppendLine("ФИО," + string.Join(',', possibleCourses.Select(crs => crs.Replace(',',' '))));
            var format = string.Join(',', possibleCourses.Select((crs, i) => "{" + i + "}"));
            foreach (var specialization in specializations)
            {
                var student = await _studentRepository.GetAsync(specialization.StudentId);
                var courses = await _courseRepository.GetBySpecializationIdAsync(specialization.Id);
                if (!courses.IsSuccess)
                    continue;
                var grades = new string[possibleCourses.Count];
                for (int i = 0; i < possibleCourses.Count; i++)
                {
                    var course = courses.Data.FirstOrDefault(crs => crs.Title == possibleCourses.ElementAt(i));
                    if (course is not null)
                    {
                        grades[i] = course.IsCompleted ? "100" : "0";
                    }
                    else
                    {
                        grades[i] = "0";
                    }
                }

                var gradesRow = $"{student.Data.FullName}, {string.Format(format, grades)}";
                csv.AppendLine(gradesRow);
            }
            using (StreamWriter sw = File.CreateText(_appSettings.Path + $"/{_appSettings.UnloadSpecializationFileName}.csv"))
            {
                await sw.WriteAsync(csv);
            }
            return OperationResult.Success();
        }

        public async Task<OperationResult> UnloadByCoursesAsync(List<string> courses)
        {
            throw new System.NotImplementedException();
        }
    }
}