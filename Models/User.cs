using Dapper.Contrib.Extensions;

namespace server.Models
{
    [Table("Users")]
    public class User
    {
        public string Email;
        public string Password;
        public string Salt;
        public int Id;
    }
}
