using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("[Student]")]
    public class Student 
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public int EnrolledCourses { get; set; }
        public int CompletedCourses { get; set; }
        public string MemberState { get; set; }
    }
}