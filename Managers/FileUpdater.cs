using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Server.Logic;
using server.Managers.Interfaces;
using Server.Repository.Interfaces;
using Server.Settings;

namespace server.Managers
{
    internal class FileUpdater : IFileUpdater
    {
        private readonly IAppSettings appSettings;
        private readonly IFileRepository fileRepository;

        public FileUpdater(IAppSettings appSettings, IFileRepository fileRepository)
        {
            this.appSettings = appSettings;
            this.fileRepository = fileRepository;
        }

        public async Task<OperationResult> Update(IFormFile students, IFormFile specialization, IFormFile course, IFormFile assignmetns)
        {
            await fileRepository.Clear();

            var fileAddedOperationResult =
                await AddAllFileName(
                    appSettings.StudentsFileName,
                    appSettings.SpecializationsFileName,
                    appSettings.CoursesFileName,
                    appSettings.AssignmentFileName);

            if (!fileAddedOperationResult.IsSuccess)
            {
                Log.Error("failed add fileNames to DB, message: {0}", fileAddedOperationResult.ErrorText);
                return fileAddedOperationResult;
            }

            await Task.WhenAll(RewriteAll(students, specialization, course, assignmetns));

            //return IUpdateManager.Update();

            return OperationResult.Success();
        }

        private async Task<OperationResult> AddAllFileName(params string[] fileNames)
        {
            return await fileRepository.AddMultipleAsync(fileNames);
        }

        private List<Task> RewriteAll(IFormFile students, IFormFile specialization, IFormFile course, IFormFile assignmetns)
        {
            return new List<Task>
            {
                WriteToFile(appSettings.StudentsFilePath, students),
                WriteToFile(appSettings.SpecializationsFilePath, specialization),
                WriteToFile(appSettings.CoursesFilePath, course),
                WriteToFile(appSettings.AssignmentFilePath, assignmetns)
            };
        }

        private static Task WriteToFile(string destinationFilePath, IFormFile sourceFile)
        {
            var destinationWriteStream = File.Create(destinationFilePath);
            return sourceFile.CopyToAsync(destinationWriteStream).ContinueWith(_ => destinationWriteStream.Close());
        }
    }
}