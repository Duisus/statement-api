using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;
using Server.Repository.Interfaces;

namespace Server.Managers.Interfaces
{
    public interface IUploadManager
    {
        /// <summary>
        /// Загрузить из файла в базу данных
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <typeparam name="T">Модель данных</typeparam>
        public Task<OperationResult<T>> UploadFormFileAsync<T>(
            string fileName,
            IBaseRepository<T> repo, 
            Func<string, Task<OperationResult<List<T>>>> parser);
    }
}