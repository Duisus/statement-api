using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using server.Dto.ModelDto;
using Server.Logic;
using server.Models;
using Server.Repository;
using Server.Repository.Interfaces;

namespace server.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
            
        }

        public async Task<List<User>> SearchAsync(string email)
        {
            var sql = $"SELECT * FROM [Users] WHERE email = \'{email}\'";
            await using var connection = new SqlConnection(_dbConnection);
            
            var items = await connection.QueryAsync<User>(sql);
            return items.ToList();
        }
    }
}
