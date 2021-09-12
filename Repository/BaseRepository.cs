using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        public async Task<long> AddAsync(T item)
        {
            await using var connection = new SqlConnection(_dbConnection);
            var addResult = await connection.InsertAsync(item);
            return addResult;
        }

        public async Task<long> AddMultipleAsync(IEnumerable<T> items)
        {
            await using var connection = new SqlConnection(_dbConnection);
            var propNames = typeof(T)
                .GetProperties()
                .Where(prop => prop.Name != "Id")
                .Select(prop => $"@{prop.Name}");

            var sql = $"INSERT INTO [{typeof(T).Name}] VALUES ({string.Join(',',propNames)}) ";
            foreach (var item in items)
            {
                await connection.ExecuteAsync(sql, item);
            }

            return 1;
        }

        public async Task<List<T>> GetByStudentIdColumnAsync(int studentId)
        {
            await using var connection = new SqlConnection(_dbConnection);
            var sql = $"SELECT * FROM {typeof(T).Name} WHERE StudentId = {studentId}";
            var result = await connection.QueryAsync<T>(sql);
            return result.ToList();
        }

        public async Task<List<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM [{typeof(T).Name}]";
            await using var connection = new SqlConnection(_dbConnection);
            var items = await connection.QueryAsync<T>(sql);
            return items.ToList();
        }

        public async Task<T> GetAsync(int id)
        {
            await using var connection = new SqlConnection(_dbConnection);
            var item = (await connection.QueryAsync<T>($"SELECT * FROM [{typeof(T).Name}] WHERE Id = {id}")).FirstOrDefault();
            return item;
        }
    }
}