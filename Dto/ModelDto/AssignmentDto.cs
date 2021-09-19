using System;
using Newtonsoft.Json;

namespace server.Dto.ModelDto
{
    public class AssignmentDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("attemptGrade")]
        public double AttemptGrade { get; set; }

        [JsonProperty("gradeAfterOverride")]
        public double? GradeAfterOverride { get; set; }

        [JsonProperty("isAttemptPassed")]
        public bool IsAttemptPassed { get; set; }

        [JsonProperty("attemptTimestamp")]
        public DateTime? AttemptTimestamp { get; set; }

        [JsonProperty("itemAttemptOrderNumber")]
        public int ItemAttemptOrderNumber { get; set; }
        
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
    }
}