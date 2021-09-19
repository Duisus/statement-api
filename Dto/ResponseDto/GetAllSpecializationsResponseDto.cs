using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Dto.ResponseDto
{
    public class GetAllSpecializationsResponseDto
    {
        [JsonProperty("specializations")]
        public List<string> Specializations { get; set; }
    }
}