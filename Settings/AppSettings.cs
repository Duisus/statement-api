using Microsoft.Extensions.Configuration;

namespace Server.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        private readonly string _fileExtension;
        private readonly string _fileSeparator;
        private const string FileSection = "Files";

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            _fileExtension = _configuration
                .GetSection(FileSection)
                .GetValue<string>("FileExtension");
            _fileSeparator = _configuration.GetSection("").GetValue<string>("FileSeparator");
        }

        public string StudentsFileName => _configuration
            .GetSection(FileSection)
            .GetValue<string>("Students") + _fileExtension;

        public string StudentsFilePath => Path + _fileSeparator + StudentsFileName;

        public string SpecializationsFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Specializations") + _fileExtension;

        public string SpecializationsFilePath => Path + _fileSeparator + SpecializationsFileName;

        public string CoursesFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Courses") + _fileExtension;

        public string CoursesFilePath => Path + _fileSeparator + CoursesFileName;

        public string AssignmentFileName => _configuration
            .GetSection(FileSection)
        .GetValue<string>("Assignments") + _fileExtension;

        public string AssignmentFilePath => Path + _fileSeparator + AssignmentFileName;

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