using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto.RequestDto
{
    public class GetStudentsRequestDto
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        
        [JsonProperty("specializations")]
        public List<string> Specializations { get; set; }
        
        [JsonProperty("courses")]
        public List<string> Courses { get; set; }
        
        [JsonProperty("orderBy")]
        public string OrderBy { get; set; }
        
        [JsonProperty("isDescending")]
        public bool IsDescending { get; set; }
    }
}