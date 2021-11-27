using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Server.Logic;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Repository 
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<OperationResult<List<Course>>> GetBySpecializationIdAsync(int id)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var sql = $"SELECT * FROM {nameof(Course)} WHERE {nameof(Course.SpecializationId)} = {id}";
            try
            {
                var result = await connection.QueryAsync<Course>(sql);
                return OperationResult<List<Course>>.Success(result.ToList());
            }
            catch (Exception e)
            {
                return OperationResult<List<Course>>.Error(e.Message);
            }
        }

        public async Task<OperationResult<List<Course>>> GetByTitleAsync(string title)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var sql = $"SELECT * FROM {nameof(Course)} WHERE {nameof(Course.Title)} = '{title}'";
            try
            {
                var result = await connection.QueryAsync<Course>(sql);
                return OperationResult<List<Course>>.Success(result.ToList());
            }
            catch (Exception e)
            {
                return OperationResult<List<Course>>.Error(e.Message);
            }
        }
    }
}