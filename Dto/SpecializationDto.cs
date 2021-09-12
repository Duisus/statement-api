using Newtonsoft.Json;

namespace Server.Dto
{
    public class SpecializationDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty("university")]
        public string University { get; set; }
        
        [JsonProperty("courseCount")]
        public int CourseCount { get; set; }
        
        [JsonProperty("completedCourseCount")]
        public int CompletedCourseCount { get; set; }
    }
}