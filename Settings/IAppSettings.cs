namespace Server.Settings
{
    public interface IAppSettings
    {
        string StudentsFileName { get; }
        string SpecializationsFileName { get; }
        string CoursesFileName { get; }
        string AssignmentFileName { get; }
        string UnloadSpecializationFileName { get; }
        string UnloadCoursesFileName { get; }
        string UnloadFileName { get; }
        string FileEncoding { get; }

        /// <summary>
        /// Куда сохранять файлы
        /// </summary>
        string Path { get; }
    }
}