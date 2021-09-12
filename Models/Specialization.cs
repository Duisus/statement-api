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
    }
}