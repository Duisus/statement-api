using AutoMapper;
using Server.Dto;
using Server.Models;

namespace Server.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<Student, StudentDto>();
            CreateMap<Specialization, SpecializationDto>();
            CreateMap<Assignment, AssignmentDto>();
        }
    }
}