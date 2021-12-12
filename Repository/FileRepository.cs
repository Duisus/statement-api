using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Server.Logic;
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

        public Task<OperationResult> AddMultipleAsync(params string[] fileName)
        {
            return AddMultipleAsync(fileName.Select(x => new FileModel { FileName = x }));
        }
    }
}