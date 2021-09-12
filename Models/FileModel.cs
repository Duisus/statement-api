using Dapper.Contrib.Extensions;

namespace Server.Models
{
    [Table("[FileModel]")]
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        
        public string FileName { get; set; }
        
        public string Base64 { get; set; }
    }
}