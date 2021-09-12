using System;
using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("[Course]")]
    public class Course 
    {
        public int Id { get; set; }

        public int? SpecializationId { get; set; }

        public int StudentId { get; set; }

        public string Title { get; set; }

        public double Progress {get;set;}

        public bool IsCompleted { get; set; }

        public double Grade {get;set;}

        public string SertificateUrl { get; set; }

        public double LearningHours { get; set; }

        public string University { get; set; }

        public DateTime? EnrollmentTime { get; set; }

        public DateTime? ClassStartTime { get; set; } 

        public DateTime? ClassEndTime { get; set; }

        public DateTime? LastCourseActivityTime { get; set; }

        public DateTime? CompletionTime { get; set; }
    }
}