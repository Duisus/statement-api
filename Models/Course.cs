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
        
        public bool IsCompleted { get; set; }

        public double Progress {get;set;}
        
        public double Grade {get;set;}

        public string CertificateUrl { get; set; }

        public double LearningHours { get; set; }

        public string University { get; set; }

        public DateTime? EnrollmentTime { get; set; }

        public DateTime? ClassStartTime { get; set; } 

        public DateTime? ClassEndTime { get; set; }

        public DateTime? LastCourseActivityTime { get; set; }

        public DateTime? CompletionTime { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Course);
        }

        private bool Equals(Course course)
        {
            if (course is null)
            {
                return false;
            }

            return
                Title == course.Title &&
                Math.Abs(Progress - course.Progress) < 0.15 &&
                IsCompleted == course.IsCompleted &&
                Math.Abs(Grade - course.Grade) < 0.15 &&
                CertificateUrl == course.CertificateUrl &&
                Math.Abs(LearningHours - course.LearningHours) < 0.15 &&
                University == course.University &&
                EnrollmentTime == course.EnrollmentTime &&
                ClassEndTime == course.ClassEndTime &&
                ClassStartTime == course.ClassStartTime &&
                LastCourseActivityTime == course.LastCourseActivityTime &&
                CompletionTime == course.CompletionTime;
        }
    }
}