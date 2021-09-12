using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("[Student]")]
    public class Student 
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Group { get; set; }
    }
}