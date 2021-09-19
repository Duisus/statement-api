using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using server.Dto.ModelDto;
using Server.Dto.ModelDto;
using Server.Dto.RequestDto;
using Server.Dto.ResponseDto;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Managers
{
    public class DataManager : IDataManager
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IMapper _mapper;
        
        public DataManager(IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            ISpecializationRepository specializationRepository, IMapper mapper, IAssignmentRepository assignmentRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _specializationRepository = specializationRepository;
            _mapper = mapper;
            _assignmentRepository = assignmentRepository;
        }
        
        public async Task<GetStudentsResponseDto> GetStudentsAsync(GetStudentsRequestDto dto)
        {
            var result = new GetStudentsResponseDto();
            var students = await _studentRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(dto.FullName))
            {
                students = students.Where(student => student.FullName.ToLower().Contains(dto.FullName.ToLower())).ToList();
            }

            foreach (var student in students)
            {
                var studentCourses = await _courseRepository.GetByStudentIdColumnAsync(student.Id);
                var haveCoursesInFilters = studentCourses.Any(course => dto.Courses.Any(filterCourse =>
                    course.Title.Equals(filterCourse, StringComparison.InvariantCultureIgnoreCase)));
                
                if (haveCoursesInFilters || dto.Courses.Count == 0 && dto.Specializations.Count == 0)
                {
                    var studentDto = CreateStudentDto(student, studentCourses);
                    result.Students.Add(studentDto);
                    continue;
                }

                var studentSpecializations = await _specializationRepository.GetByStudentIdColumnAsync(student.Id);
                var haveSpecializationsInFilters = studentSpecializations.Any(spec => dto.Specializations.Any(filterSpec =>
                    spec.Title.Equals(filterSpec, StringComparison.InvariantCultureIgnoreCase)));
                
                if (haveSpecializationsInFilters || dto.Courses.Count == 0 && dto.Specializations.Count == 0)
                {
                    var studentDto = CreateStudentDto(student, studentCourses);
                    result.Students.Add(studentDto);
                }
            }

            result.Students = OrderBy(dto.OrderBy, dto.IsDescending, result.Students);
            return result;
        }

        public async Task<OperationResult<StudentDto>> GetStudentAsync(int id)
        {
            var getStudent = await _studentRepository.GetAsync(id);
            if (!getStudent.IsSuccess)
            {
                return OperationResult<StudentDto>.Error(getStudent.ErrorText);
            }

            var student = getStudent.Data;
            var specializations = await _specializationRepository.GetByStudentIdColumnAsync(id);
            var courses = await _courseRepository.GetByStudentIdColumnAsync(id);
            var assignments = await _assignmentRepository.GetByStudentIdColumnAsync(id);
            var result = new StudentDto
            {
                FullName = student.FullName,
                Group = student.Group,
                Specializations = _mapper.Map<List<SpecializationDto>>(specializations),
                MemberState = student.MemberState
            };
            var addedCourses = new List<CourseDto>();
            foreach (var course in _mapper.Map<List<CourseDto>>(courses))
            {
                var specialization =
                    result.Specializations.FirstOrDefault(spec => spec.University == course.University);
                
                if (specialization is null)
                {
                    result.CoursesWithoutSpecialization.Add(course);
                }
                else
                {
                    specialization.Courses.Add(course);
                }
                addedCourses.Add(course);
            }

            foreach (var assignment in _mapper.Map<List<AssignmentDto>>(assignments))
            {
                var course = addedCourses.FirstOrDefault(crs => crs.Title == assignment.CourseName);
                if (course is null)
                {
                    Serilog.Log.Warning($"Course not found for assignment {assignment.Title} of student {student.FullName}");
                }
                else
                {
                    course.Assignments.Add(assignment);
                }
            }
            return OperationResult<StudentDto>.Success(result);
        }

        private StudentResponseDto CreateStudentDto(Student student, List<Course> courses)
        {
            var averageGrade = courses.Count != 0
                ? courses.Select(course => course.Grade).Sum() / courses.Count
                : 0;
            var averageHours = courses.Count != 0
                ? courses.Select(course => course.LearningHours).Sum() / courses.Count
                : 0;
            var studentDto = new StudentResponseDto()
            {
                FullName = student.FullName,
                AverageGrade = (float)averageGrade,
                AverageHours = (float)averageHours,
                Id = student.Id
            };

            return studentDto;
        }

        private List<StudentResponseDto> OrderBy(string orderBy, bool isDescending, List<StudentResponseDto> students)
        {
            switch (orderBy) {
                case "hours":
                    if (!isDescending)
                        return students.OrderBy(student => student.AverageHours).ToList();
                    return students.OrderByDescending(student => student.AverageHours).ToList();
                case "grade":
                    if (!isDescending)
                        return students.OrderBy(student => student.AverageGrade).ToList();
                    return students.OrderByDescending(student => student.AverageGrade).ToList();
                case "name":
                    if (!isDescending)
                        return students.OrderBy(student => student.FullName).ToList();
                    return students.OrderByDescending(student => student.FullName).ToList();
                default: return students;
            }
        }
    }
}