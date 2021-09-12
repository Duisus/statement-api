using System;
using Newtonsoft.Json;

namespace Server.Dto
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

        [JsonProperty("attemptTimestampt")]
        public DateTime? AttemptTimestampt { get; set; }

        [JsonProperty("itemAttemptOrderNumber")]
        public int ItemAttemptOrderNumber { get; set; }
    }
}