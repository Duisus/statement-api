using AutoMapper;
using Server.Dto;
using server.Dto.ModelDto;
using Server.Dto.ModelDto;
using Server.Models;

namespace Server.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<Specialization, SpecializationDto>()
                .ForMember(dest => dest.Courses, opt => opt.Ignore());
            CreateMap<Course, CourseDto>();
            CreateMap<Assignment, AssignmentDto>();
        }
    }
}