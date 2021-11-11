using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Server.Logic;
using Server.Repository.Interfaces;

namespace Server.Repository 
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly string _dbConnection;

        public BaseRepository(IConfiguration configuration)
        {
            _dbConnection = configuration.GetValue<string>("ConnectionString");
        }

        public async Task<OperationResult<T>> GetAsync(int id)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var result = await connection.GetAsync<T>(id);
            if (result is not null)
            {
                return OperationResult<T>.Success(result);
            }

            return OperationResult<T>.Error($"Не удалось найти запись с id {id} в таблице {nameof(T)}");
        }

        public async Task<long> AddAsync(T item)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var addResult = await connection.InsertAsync(item);
            return addResult;
        }

        public async Task<OperationResult> AddMultipleAsync(IEnumerable<T> items)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var propNames = typeof(T)
                .GetProperties()
                // Не уверен, что это правильное решение
                //.Where(prop => prop.Name != "Id")
                .Select(prop => $"@{prop.Name}");

            var sql = $"INSERT INTO {typeof(T).Name} VALUES ({string.Join(',',propNames)}) ";
            foreach (var item in items)
            {
                try
                {
                    await connection.ExecuteAsync(sql, item);
                }
                catch (Exception e)
                {
                    return OperationResult.Error(e.Message);
                }
                
            }

            return OperationResult.Success();
        }

        public async Task<List<T>> GetByStudentIdColumnAsync(int studentId)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var sql = $"SELECT * FROM {typeof(T).Name} WHERE StudentId = {studentId}";
            var result = await connection.QueryAsync<T>(sql);
            return result.ToList();
        }

        public async Task<bool> UpdateAsync(T item)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var result = await connection.UpdateAsync(item);
            return result;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {typeof(T).Name}";
            await using var connection = new MySqlConnection(_dbConnection);
            var items = await connection.QueryAsync<T>(sql);
            return items.ToList();
        }
    }
}