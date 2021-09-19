using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto.ResponseDto
{
    public class GetStudentsResponseDto
    {
        [JsonProperty("students")]
        public List<StudentResponseDto> Students { get; set; } = new List<StudentResponseDto>();
    }

    public class StudentResponseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        
        [JsonProperty("averageHours")]
        public float AverageHours { get; set; }
        
        [JsonProperty("averageGrade")]
        public float AverageGrade { get; set; }
    }
}