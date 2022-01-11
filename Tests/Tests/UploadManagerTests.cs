using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Server.Logic;
using Server.Managers;
using Server.Models;
using Server.Repository.Interfaces;
using Server.Settings;

namespace CourseraApiTests
{
    public class UploadManagerTests
    {
        private const string TestFileName = "test.txt";

        [SetUp]
        public void Setup()
        {
            using (File.Create(TestFileName))
            {
            }
        }

        [Test]
        public async Task Should_return_error_when_file_does_not_exist()
        {
            var fileRepository = Substitute.For<IFileRepository>();
            fileRepository.GetByFileNameAsync(null).ReturnsForAnyArgs(Task.FromResult<FileModel>(null));
            var appSettings = Substitute.For<IAppSettings>();
            var uploadManager = new UploadManager(fileRepository, appSettings);

            var uploadResult = await uploadManager.UploadFormFileAsync<Specialization>(TestFileName, null, null);

            uploadResult.IsSuccess.Should().BeFalse();
            uploadResult.StatusCode.Should().Be(400);
            uploadResult.ErrorText.Should().Be($"Файла {TestFileName} не существует");
        }

        [Test]
        public async Task Should_return_error_when_parsing_failed()
        {
            var fileRepository = Substitute.For<IFileRepository>();
            fileRepository.GetByFileNameAsync(null).ReturnsForAnyArgs(new FileModel());
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.FileEncoding.Returns(Encoding.Unicode.EncodingName);
            appSettings.Path.Returns(Directory.GetCurrentDirectory());
            var uploadManager = new UploadManager(fileRepository, appSettings);

            var uploadResult = await uploadManager.UploadFormFileAsync(TestFileName, null,
                _ => Task.FromResult(OperationResult<List<Specialization>>.Error("")));

            uploadResult.IsSuccess.Should().BeFalse();
            uploadResult.StatusCode.Should().Be(400);
            uploadResult.ErrorText.Should().Be("Не удалось получить данные из файла");
        }

        [Test]
        public async Task Should_return_error_when_adding_to_repo_failed()
        {
            var fileRepository = Substitute.For<IFileRepository>();
            fileRepository.GetByFileNameAsync(null).ReturnsForAnyArgs(new FileModel());
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.FileEncoding.Returns(Encoding.Unicode.EncodingName);
            appSettings.Path.Returns(Directory.GetCurrentDirectory());
            var uploadManager = new UploadManager(fileRepository, appSettings);
            var repo = Substitute.For<IBaseRepository<Specialization>>();
            repo.AddMultipleAsync(null).ReturnsForAnyArgs(Task.FromResult(OperationResult.Error("")));

            var uploadResult = await uploadManager.UploadFormFileAsync(TestFileName, repo,
                _ => Task.FromResult(OperationResult<List<Specialization>>.Success()));

            uploadResult.IsSuccess.Should().BeFalse();
            uploadResult.StatusCode.Should().Be(400);
            uploadResult.ErrorText.Should().Be("Не удалось добавить данные в базу данных");
        }

        [Test]
        public async Task Should_return_success_when_all_operations_success()
        {
            var fileRepository = Substitute.For<IFileRepository>();
            fileRepository.GetByFileNameAsync(null).ReturnsForAnyArgs(new FileModel());
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.FileEncoding.Returns(Encoding.Unicode.EncodingName);
            appSettings.Path.Returns(Directory.GetCurrentDirectory());
            var uploadManager = new UploadManager(fileRepository, appSettings);
            var repo = Substitute.For<IBaseRepository<Specialization>>();
            repo.AddMultipleAsync(null).ReturnsForAnyArgs(Task.FromResult(OperationResult.Success()));

            var uploadResult = await uploadManager.UploadFormFileAsync(TestFileName, repo,
                _ => Task.FromResult(OperationResult<List<Specialization>>.Success()));

            uploadResult.IsSuccess.Should().BeTrue();
            uploadResult.StatusCode.Should().Be(200);
            uploadResult.ErrorText.Should().BeNull();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(TestFileName);
        }
    }
}