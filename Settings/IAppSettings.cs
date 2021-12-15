namespace Server.Settings
{
    public interface IAppSettings
    {
        string StudentsFileName { get; }
        string StudentsFilePath { get; }
        string SpecializationsFileName { get; }
        string SpecializationsFilePath { get; }
        string CoursesFileName { get; }
        string CoursesFilePath { get; }
        string AssignmentFileName { get; }
        string AssignmentFilePath { get; }
        string UnloadSpecializationFileName { get; }
        string UnloadCoursesFileName { get; }
        string FileEncoding { get; }

        /// <summary>
        /// Куда сохранять файлы
        /// </summary>
        string Path { get; }
    }
}