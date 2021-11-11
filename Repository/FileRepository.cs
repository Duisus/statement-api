using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Repository
{
    public class FileRepository : BaseRepository<FileModel>, IFileRepository
    {
        public FileRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<FileModel> GetByFileNameAsync(string fileName)
        {
            var sql = $"SELECT * FROM {nameof(FileModel)} WHERE {nameof(FileModel.FileName)} = \'{fileName}\'";
            await using var connection = new MySqlConnection(_dbConnection);
            var result = (await connection.QueryAsync<FileModel>(sql)).FirstOrDefault();
            return result;
        }
    }
}