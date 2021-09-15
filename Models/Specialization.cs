using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("[Specialization]")]
    public class Specialization 
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public string University { get; set; }
        
        public int CourseCount { get; set; }
        
        public int CompletedCourseCount { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Specialization);
        }

        private bool Equals(Specialization specialization)
        {
            if (specialization is null)
            {
                return false;
            }

            return Title == specialization.Title &&
                   IsCompleted == specialization.IsCompleted &&
                   University == specialization.University &&
                   CourseCount == specialization.CourseCount &&
                   CompletedCourseCount == specialization.CompletedCourseCount &&
                   StudentId == specialization.StudentId;
        }
    }
}