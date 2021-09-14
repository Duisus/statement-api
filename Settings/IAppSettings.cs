namespace Server.Settings
{
    public interface IAppSettings
    {
        string StudentsFileName { get; }
        string SpecializationsFileName { get; }
        string CoursesFileName { get; }
        string AssignmentFileName { get; }
        
        /// <summary>
        /// Куда сохранять файлы
        /// </summary>
        string Path { get; }
    }
}