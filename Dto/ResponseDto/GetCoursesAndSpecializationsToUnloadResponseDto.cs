using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Server.Dto.ResponseDto
{
    public class GetCoursesAndSpecializationsToUnloadResponseDto
    {
        [JsonProperty("courses")]
        /*[FromBody]*/
        public List<string> Courses { get; set; }
        
        [JsonProperty("specializations")]
        /*[FromBody]*/
        public List<string> Specializations { get; set; }
    }
}