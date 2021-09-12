using FluentMigrator;
using Server.Models;

namespace Migrations
{
    [Migration(12092021)]
    public class AddFileTable_12092021 : Migration
    {
        private const string TableName = nameof(FileModel);
        public override void Up()
        {
            CreateFileTable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
        
        public void CreateFileTable()
        {
            if (!Schema.Table(TableName).Exists())
            {
                Create.Table(TableName)
                    .WithColumn(nameof(FileModel.Id)).AsInt64().PrimaryKey().Identity()
                    .WithColumn(nameof(FileModel.FileName)).AsString()
                    .WithColumn(nameof(FileModel.Base64)).AsString(int.MaxValue);
            }
        }
    }
}