using Newtonsoft.Json;

namespace Server.Dto
{
    public class StudentDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }
    }
}