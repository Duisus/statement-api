using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto
{
    public class GetStudentsResponseDto
    {
        [JsonProperty("students")]
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
    }
}