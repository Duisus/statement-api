using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Server.Logic;
using Server.Managers.Interfaces;
using Server.Repository;
using Server.Repository.Interfaces;
using Server.Settings;

namespace Server.Managers
{
    public class UploadManager: IUploadManager
    {
        private readonly IFileRepository _fileRepository;
        private readonly IAppSettings _appSettings;

        public UploadManager(IFileRepository fileRepository, ICsvParserManager csvParserManager, IAppSettings appSettings)
        {
            _fileRepository = fileRepository;
            _appSettings = appSettings;
        }

        /// <inheritdoc />
        public async Task<OperationResult<T>> UploadFormFileAsync<T>(
            string fileName,
            IBaseRepository<T> repo, 
            Func<string, Task<OperationResult<List<T>>>> parser) where T : class
        {
            var tag = $"{GetType()}.{nameof(UploadFormFileAsync)}";
            var file = await _fileRepository.GetByFileNameAsync(fileName);
            if (file is null)
            {
                Serilog.Log.Error($"{tag}: File {fileName} doesn't exists");
                return OperationResult<T>.Error($"Файла {fileName} не существует");
            }

            var encoding = _appSettings.FileEncoding;
            var fileText = await File.ReadAllTextAsync($"{_appSettings.Path}\\{fileName}", Encoding.GetEncoding(encoding));
            var parsingResult = await parser(fileText);
            if (!parsingResult.IsSuccess)
            {
                Serilog.Log.Error($"{tag}: An error occured while parsing csv");
                return OperationResult<T>.Error("Не удалось получить данные из файла");
            }

            var addResult = await repo.AddMultipleAsync(parsingResult.Data);
            if (!addResult.IsSuccess)
            {
                Serilog.Log.Error($"{tag}: An error occured while adding multiple items to the database: " + addResult.ErrorText);
                return OperationResult<T>.Error("Не удалось добавить данные в базу данных");
            }

            return OperationResult<T>.Success();
        }
    }
}