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
    public class SpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<OperationResult<List<Specialization>>> GetByTitleAsync(string title)
        {
            await using var connection = new MySqlConnection(_dbConnection);
            var sql = $"SELECT * FROM {nameof(Specialization)} WHERE {nameof(Specialization.Title)} = '{title}'";
            try
            {
                var result = await connection.QueryAsync<Specialization>(sql);
                return OperationResult<List<Specialization>>.Success(result.ToList());
            }
            catch (Exception e)
            {
                return OperationResult<List<Specialization>>.Error(e.Message);
            }
        }
    }
}