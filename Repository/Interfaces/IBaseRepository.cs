using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Logic;

namespace Server.Repository.Interfaces 
{
    public interface IBaseRepository<T>
    {
        public Task<T> GetAsync(int id);

        /// <summary>
        /// Добавить и получить Id
        ///</summary>
        public Task<long> AddAsync(T item);

        /// <summary>
        /// Получить все записи
        ///</summary>
        public Task<List<T>> GetAllAsync();

        /// <summary>
        /// Добавить несколько записей и получить их Id
        ///</summary>
        public Task<OperationResult<long>> AddMultipleAsync(IEnumerable<T> items);

        public Task<List<T>> GetByStudentIdColumnAsync(int studentId);

        public Task<bool> UpdateAsync(T item);
    }
}