using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Serilog;
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

        public CsvParserManager(IStudentRepository studentRepository, ISpecializationRepository specializationRepository, ICourseRepository courseRepository, IAssignmentRepository assignmentRepository)
        {
            _studentRepository = studentRepository;
            _specializationRepository = specializationRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
        }

        public Task<List<string[]>> ParseCsvFileAsync(string delimeter, string base64String)
        {
            var result = new List<string[]>();
            var bytes = Convert.FromBase64String(base64String);
            var csv = Encoding.UTF8.GetString(bytes);
            foreach (string row in csv.Split('\n').Skip(1))
            {
                string[] values = row.Split($"\"{delimeter}\"");
                result.Add(values);
            }

            return Task.FromResult(result);
        }

        public async Task<List<Student>> ParseMembershipCsvToStudents(string base64String)
        {
            var students = new Dictionary<string, Student>();
            var rows = await ParseCsvFileAsync(",", base64String);
            foreach (var row in rows)
            {
                var name = row[0].Trim('\"');
                var group = row[2].Trim('\"');
                if (name == "ANONYMIZED_NAME" || string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var student = new Student
                {
                    FullName = name,
                    Group = group
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

            return students.Values.GroupBy(stud => stud.FullName).Select(stud => stud.First()).ToList();
        }

        public async Task<List<Specialization>> ParseSpecializationCsvToSpecializations(string base64String)
        {
            var result = new Dictionary<string, List<Specialization>>();
            var students = await _studentRepository.GetAllAsync();
            var rows = await ParseCsvFileAsync(",", base64String);
            foreach (var row in rows)
            {
                var name = row[0].Trim('\"');
                if (name == "ANONYMIZED_NAME" || string.IsNullOrEmpty(name))
                {
                    continue;
                }
                if (result.ContainsKey(name))
                {
                    var specialization = CreateSpecializationWithoutStudentId(row);
                    specialization.StudentId = result[name][0].StudentId;
                    result[name].Add(specialization);
                }
                else
                {
                    var student = students.FirstOrDefault(student => student.FullName == name);
                    if (student == null)
                    {
                        Serilog.Log.Warning($"There is specialization without student with name {name}. Skipping");
                        continue;
                    }

                    var specialization = CreateSpecializationWithoutStudentId(row);
                    specialization.StudentId = student.Id;
                    result.Add(name, new List<Specialization>()
                    {
                        specialization
                    });
                }
            }

            return result.Values.SelectMany(specs => specs).ToList();
        }

        public async Task<List<Course>> ParseCourseCsvToSpecializations(string base64String)
        {
            var result = new Dictionary<string, List<Course>>();
            var students = await _studentRepository.GetAllAsync();
            var specializations = await _specializationRepository.GetAllAsync();
            var rows = await ParseCsvFileAsync(",", base64String);
            foreach (var row in rows)
            {
                var name = row[0].Trim('\"');
                var course = CreateCourseWithoutStudentIdSpecId(row);
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

            return result.SelectMany(courses => courses.Value).ToList();
        }

        public async Task<List<Assignment>> ParseAssignmentCsvToAssignments(string base64String)
        {
            var rows = await ParseCsvFileAsync(",",base64String);
            var students = await _studentRepository.GetAllAsync();
            var courses = await _courseRepository.GetAllAsync();
            var assignments = await _assignmentRepository.GetAllAsync();
            var result = new Dictionary<string, List<Assignment>>();
            foreach (var row in rows)
            {
                var studentName = row[3].Trim('\"');
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

            return result.Values.SelectMany(value => value).ToList();
        }

        private Specialization CreateSpecializationWithoutStudentId(string[] row)
        {
            return new Specialization
            {
                Title = row[3].Trim('\"'),
                CourseCount = int.Parse(row[9].Trim('\"')),
                CompletedCourseCount = int.Parse(row[8].Trim('\"')),
                IsCompleted = row[10].Trim('\"') == "Yes",
                University = row[5].Trim('\"')
            };
        }

        private Course CreateCourseWithoutStudentIdSpecId(string[] row)
        {
            var trimmedRow = row.ToList().Select(item => item.Trim('\"')).ToArray();
            var endTimeParsed = DateTime.TryParse(trimmedRow[9], out DateTime endTime);
            var enrollmentTimeParsed = DateTime.TryParse(trimmedRow[7], out DateTime enrollmentTime);
            var startTimeParsed = DateTime.TryParse(trimmedRow[9], out DateTime startTime);
            var lastActivityTimeParsed = DateTime.TryParse(trimmedRow[9], out DateTime lastActivityTime);
            var completionTimeParsed = DateTime.TryParse(trimmedRow[9], out DateTime completionTime);
            var gradePased = double.TryParse(trimmedRow[19].Replace('.',','), out var grade);
            var progressParsed = double.TryParse(trimmedRow[11].Replace('.',','), out var progress);
            var learningHours = double.TryParse(trimmedRow[12].Replace('.',','), out var hours);
            return new Course
            {
                Title = trimmedRow[3],
                ClassEndTime = endTimeParsed ? endTime : null,
                EnrollmentTime = enrollmentTimeParsed ? enrollmentTime : null,
                ClassStartTime = startTimeParsed ? startTime : null,
                LastCourseActivityTime = lastActivityTimeParsed ? lastActivityTime : null,
                CompletionTime = completionTimeParsed ? completionTime : null,
                Grade = grade,
                IsCompleted = trimmedRow[13] == "Yes",
                Progress = progress,
                SertificateUrl = trimmedRow[20],
                LearningHours = hours,
                University = trimmedRow[6]
            };
        }

        private Assignment CreateAssignmentWithoutStudentId(string[] row)
        {
            var trimmedRow = row.ToList().Select(item => item.Trim('\"')).ToArray();   
            var attemptTimestampParsed = DateTime.TryParse(trimmedRow[13], out DateTime attemptTimestampt);
            var gradeAfterOverrideParsed = double.TryParse(trimmedRow[11].Replace('.',','), out double gradeAfterOvveride);
            return new Assignment
            {
                Title = row[8],
                AttemptGrade = double.Parse(trimmedRow[10].Replace('.',',')),
                AttemptTimestampt = attemptTimestampParsed ? attemptTimestampt : null,
                GradeAfterOverride = gradeAfterOverrideParsed ? gradeAfterOvveride : null,
                IsAttemptPassed = trimmedRow[12] == "Yes",
                ItemAttemptOrderNumber = int.Parse(trimmedRow[14]),
                Order = int.Parse(trimmedRow[9])
            };
        }
    }
}