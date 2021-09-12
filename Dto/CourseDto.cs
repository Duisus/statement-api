using System;
using Newtonsoft.Json;

namespace Server.Dto
{
    public class CourseDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("progress")]
        public double Progress {get;set;}

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty("grade")]
        public double Grade {get;set;}

        [JsonProperty("sertificateUrl")]
        public string SertificateUrl { get; set; }

        [JsonProperty("learningHours")]
        public double LearningHours { get; set; }

        [JsonProperty("university")]
        public string University { get; set; }

        [JsonProperty("enrollmentTime")]
        public DateTime? EnrollmentTime { get; set; }

        [JsonProperty("classStartTime")]
        public DateTime? ClassStartTime { get; set; } 

        [JsonProperty("classEndTime")]
        public DateTime? ClassEndTime { get; set; }

        [JsonProperty("lastCourseActivityTime")]
        public DateTime? LastCourseActivityTime { get; set; }

        [JsonProperty("completionTime")]
        public DateTime? CompletionTime { get; set; }
    }
}