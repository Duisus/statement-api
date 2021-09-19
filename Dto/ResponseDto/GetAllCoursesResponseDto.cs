using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto.ResponseDto
{
    public class GetAllCoursesResponseDto
    {
        [JsonProperty("courses")]
        public List<string> Courses { get; set; }
    }
}