using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto.ModelDto
{
    public class StudentDto
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }
        
        [JsonProperty("memberState")]
        public string MemberState { get; set; }

        [JsonProperty("specializations")]
        public List<SpecializationDto> Specializations { get; set; } = new List<SpecializationDto>();

        [JsonProperty("coursesWithoutSpecialization")]
        public List<CourseDto> CoursesWithoutSpecialization { get; set; } = new List<CourseDto>();
        
        
    }
}