using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Server.Dto.ResponseDto
{
    public class GetCoursesAndSpecializationsToUnloadResponseDto
    {
        [JsonProperty("courses")]
        public List<string> Courses { get; set; }
        
        [JsonProperty("specializations")]
        public List<string> Specializations { get; set; }
    }
}