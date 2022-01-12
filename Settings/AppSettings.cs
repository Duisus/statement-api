using Microsoft.Extensions.Configuration;

namespace Server.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        private string _fileExtension;
        private const string FileSection = "Files";

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            _fileExtension = _configuration
                .GetSection(FileSection)
                .GetValue<string>("FileExtension");
        }

        public string StudentsFileName => _configuration
            .GetSection(FileSection)
            .GetValue<string>("Students") + _fileExtension;
        
        public string SpecializationsFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Specializations") + _fileExtension;
        
        public string CoursesFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Courses") + _fileExtension;
        
        public string AssignmentFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Assignments") + _fileExtension;

        public string Path => _configuration
            .GetSection(FileSection)
            .GetValue<string>("Path");
        
        public string UnloadFileName => _configuration
            .GetSection(FileSection)
            .GetValue<string>("UnloadFileName");

        public string FileEncoding => _configuration
            .GetSection(FileSection)
            .GetValue<string>("FileEncoding");

    }
}