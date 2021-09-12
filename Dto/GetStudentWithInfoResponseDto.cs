using System.Collections.Generic;
using Newtonsoft.Json;
using Server.Models;

namespace Server.Dto
{
    public class GetStudentWithInfoRequestDto
    {
        [JsonProperty("student")]
        public StudentDto Student { get; set; }
        
        [JsonProperty("specializations")]
        public List<SpecializationDto> Specializations { get; set; } 
        
        [JsonProperty("courses")]
        public List<CourseDto> Courses { get; set; } 
        
        [JsonProperty("assignments")]
        public List<AssignmentDto> Assignments { get; set; } 
    }
}