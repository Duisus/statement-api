using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Serilog;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Managers
{
    public class CsvParserManager : ICsvParserManager
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private readonly DateTimeStyles dateTimeStyle = DateTimeStyles.None;

        public CsvParserManager(IStudentRepository studentRepository, ISpecializationRepository specializationRepository, ICourseRepository courseRepository, IAssignmentRepository assignmentRepository)
        {
            _studentRepository = studentRepository;
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
        }

        public Task<List<string[]>> ParseCsvFileAsync(string delimeter, string file)
        {
            var result = new List<string[]>();
            foreach (string row in file.Split('\n').Skip(1))
            {
                string[] values = row
                    .Split($"\"{delimeter}\"")
                    .Select(str => str.Trim('\"'))
                    .ToArray();
                result.Add(values);
            }

            return Task.FromResult(result);
        }

        public async Task<OperationResult<List<Student>>> ParseStudentsCsvToStudents(string file)
        {
            var existingStudents = await _studentRepository.GetAllAsync();
            var students = new Dictionary<string, Student>();
            var rows = await ParseCsvFileAsync(",", file);
            foreach (var row in rows.Where(x => x.Length > 1))
            {
                var name = row[0];
                if (existingStudents.Any(student => student.FullName == name))
                {
                    continue;
                }
                var group = row[2];
                var enrolledCourses = int.Parse(row[5]);
                var completedCourses = int.Parse(row[6]);
                var memberState = row[7];
                if (name == "ANONYMIZED_NAME" || string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var student = new Student
                {
                    FullName = name,
                    Group = group,
                    EnrolledCourses = enrolledCourses,
                    CompletedCourses = completedCourses,
                    MemberState = memberState
                };

                if (!students.ContainsKey(name))
                {
                    students.Add(name, student);
                }
                else
                {
                    students[row[0]] = student;
                }
            }
            var result = students.Values.GroupBy(stud => stud.FullName).Select(stud => stud.First()).ToList();
            return OperationResult<List<Student>>.Success(result);
        }

        public async Task<OperationResult<List<Specialization>>> ParseSpecializationCsvToSpecializations(string file)
        {
            var result = new Dictionary<string, List<Specialization>>();
            var students = await _studentRepository.GetAllAsync();
            var rows = await ParseCsvFileAsync(",", file);
            var existingSpecializations = await _specializationRepository.GetAllAsync();
            foreach (var row in rows.Where(row=>row.Length>1))
            {
                var name = row[0];
                if (name == "ANONYMIZED_NAME" || string.IsNullOrEmpty(name))
                {
                    continue;
                }
                if (result.ContainsKey(name))
                {
                    var specialization = CreateSpecializationWithoutStudentId(row);
                    specialization.StudentId = result[name][0].StudentId;
                    if (existingSpecializations.Any(existingSpecialization =>
                        existingSpecialization.Equals(specialization)))
                    {
                        continue;
                    }
                    result[name].Add(specialization);
                }
                else
                {
                    var student = students.FirstOrDefault(student => student.FullName == name);
                    if (student == null)
                    {
                        Serilog.Log.Warning($"There is no student {name}. Skipping");
                        continue;
                    }

                    var specialization = CreateSpecializationWithoutStudentId(row);
                    specialization.StudentId = student.Id;
                    if (existingSpecializations.Any(existingSpecialization =>
                        existingSpecialization.Equals(specialization)))
                    {
                        continue;
                    }
                    result.Add(name, new List<Specialization>()
                    {
                        specialization
                    });
                }
            }
            var res = result.Values.SelectMany(specs => specs).ToList();
            return OperationResult<List<Specialization>>.Success(res);
        }

        public async Task<OperationResult<List<Course>>> ParseCourseCsvToSpecializations(string file)
        {
            var existingCourses = await _courseRepository.GetAllAsync();
            var result = new Dictionary<string, List<Course>>();
            var students = await _studentRepository.GetAllAsync();
            var specializations = await _specializationRepository.GetAllAsync();
            var rows = await ParseCsvFileAsync(",", file);
            foreach (var row in rows.Where(row=>row.Length>1))
            {
                var name = row[0];
                var course = CreateCourseWithoutStudentIdSpecId(row);
                if (existingCourses.Any(existingCourse => existingCourse.Equals(course)))
                {
                    continue;
                }
                if (result.ContainsKey(name))
                {
                    course.StudentId = result[name][0].StudentId;
                    
                    var specialization = specializations.FirstOrDefault(spec =>
                        spec.University == course.University && spec.StudentId == course.StudentId);
                    
                    if (specialization != null)
                        course.SpecializationId = specialization.Id;

                    result[name].Add(course);
                }
                else
                {
                    var student = students.FirstOrDefault(student => student.FullName == name);
                    if (student == null)
                    {
                        Serilog.Log.Warning($"There was no student with name {name} db. Skipping this course");
                        continue;
                    }

                    var specialization = specializations.FirstOrDefault(spec =>
                        spec.University == course.University && spec.StudentId == student.Id);
                    
                    course.StudentId = student.Id;
                    if (specialization == null)
                    {
                        
                        result.Add(name, new List<Course>() {course});
                    }
                    else
                    {
                        course.SpecializationId = specialization.Id;
                        result.Add(name, new List<Course>() {course});
                    }
                }
            }
            
            var res = result.SelectMany(courses => courses.Value).ToList();
            return OperationResult<List<Course>>.Success(res);
        }

        public async Task<OperationResult<List<Assignment>>> ParseAssignmentCsvToAssignments(string file)
        {
            var rows = await ParseCsvFileAsync(",",file);
            var students = await _studentRepository.GetAllAsync();
            var courses = await _courseRepository.GetAllAsync();
            var assignments = await _assignmentRepository.GetAllAsync();
            var result = new Dictionary<string, List<Assignment>>();
            foreach (var row in rows.Where(row=>row.Length>1))
            {
                var studentName = row[3];
                var student = students.FirstOrDefault(student => student.FullName == studentName);
                if (student is null)
                {
                    Serilog.Log.Warning($"There was no student with name {studentName}. Skipping this assignment");
                    continue;
                }

                var assignment = CreateAssignmentWithoutStudentId(row);
                assignment.StudentId = student.Id;
                if (assignments.Exists(assignm => assignm.Equals(assignment)))
                {
                    continue;
                }
                if (result.ContainsKey(studentName))
                {
                    result[studentName].Add(assignment);
                }
                else
                {
                    result[studentName] = new List<Assignment>() {assignment};
                }
            }
            var res = result.Values.SelectMany(value => value).ToList();

            return OperationResult<List<Assignment>>.Success(res);
        }

        private Specialization CreateSpecializationWithoutStudentId(string[] row)
        {
            return new Specialization
            {
                Title = row[3],
                CourseCount = int.TryParse(row[9], out var courseCount)? courseCount : -1,
                CompletedCourseCount = int.TryParse(row[8], out var completedCourseCount) ? completedCourseCount : -1,
                IsCompleted = row[10] == "Yes",
                University = row[5]
            };
        }

        private Course CreateCourseWithoutStudentIdSpecId(string[] row)
        {
            var endTimeParsed = DateTime.TryParse(row[9], culture, dateTimeStyle, out var endTime);
            var enrollmentTimeParsed = DateTime.TryParse(row[7], culture, dateTimeStyle, out var enrollmentTime);
            var startTimeParsed = DateTime.TryParse(row[8], culture, dateTimeStyle, out var startTime);
            var lastActivityTimeParsed = DateTime.TryParse(row[10], culture, dateTimeStyle, out var lastActivityTime);
            var completionTimeParsed = DateTime.TryParse(row[18], culture, dateTimeStyle, out var completionTime);
            double.TryParse(row[19].Replace('.',','), out var grade);
            double.TryParse(row[11].Replace('.',','), out var progress);
            double.TryParse(row[12].Replace('.',','), out var hours);
            return new Course
            {
                Title = row[3],
                ClassEndTime = endTimeParsed ? endTime : null,
                EnrollmentTime = enrollmentTimeParsed ? enrollmentTime : null,
                ClassStartTime = startTimeParsed ? startTime : null,
                LastCourseActivityTime = lastActivityTimeParsed ? lastActivityTime : null,
                CompletionTime = completionTimeParsed ? completionTime : null,
                Grade = grade,
                IsCompleted = row[13] == "Yes",
                Progress = progress,
                CertificateUrl = row[20],
                LearningHours = hours,
                University = row[6]
            };
        }

        private Assignment CreateAssignmentWithoutStudentId(string[] row)
        {
            var attemptTimestampParsed = DateTime.TryParse(row[13], culture, dateTimeStyle, out var attemptTimestamp);
            var gradeAfterOverrideParsed = double.TryParse(row[11].Replace('.',','), out var gradeAfterOverride);
            return new Assignment
            {
                Title = row[8],
                AttemptGrade = double.Parse(row[10].Replace('.',',')),
                AttemptTimestamp = attemptTimestampParsed ? attemptTimestamp : null,
                GradeAfterOverride = gradeAfterOverrideParsed ? gradeAfterOverride : null,
                IsAttemptPassed = row[12] == "Yes",
                ItemAttemptOrderNumber = int.Parse(row[14]),
                Order = int.Parse(row[9]),
                CourseName = row[5]
            };
        }
    }
}