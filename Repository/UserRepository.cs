using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using server.Models;
using Server.Repository;

namespace server.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
            
        }

        public async Task<List<User>> SearchAsync(string email)
        {
            var sql = $"SELECT * FROM Users WHERE email = \'{email}\'";
            await using var connection = new MySqlConnection(_dbConnection);
            
            var items = await connection.QueryAsync<User>(sql);
            return items.ToList();
        }
    }
}
