using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Server.Managers;
using Server.Models;
using Server.Repository.Interfaces;

namespace CourseraApiTests
{
    public class CsvParserManagerTests
    {
        private const string TestDataDirectory = "TestData";
        private const string StudentsCsvFileName = "membership-report.csv";
        private const string SpecializationsCsvFileName = "specialization-report.csv";
        private const string CourseCsvFileName = "usage-report.csv";
        private const string AssignmentsCsvFileName = "Ural Federal University Learning Program.csv";
        private const string TestStudentName = "Иванов Иван Иванович";
        private const string TestSpecializationTitle = "Test Specialization";
        private IAssignmentRepository assignmentRepository;
        private ICourseRepository courseRepository;

        private CsvParserManager csvParserManager;
        private ISpecializationRepository specializationRepository;
        private IStudentRepository studentRepository;

        [SetUp]
        public void Setup()
        {
            studentRepository = Substitute.For<IStudentRepository>();
            studentRepository.GetAllAsync().ReturnsForAnyArgs(new List<Student> {new() {FullName = TestStudentName}});
            specializationRepository = Substitute.For<ISpecializationRepository>();
            specializationRepository.GetAllAsync().ReturnsForAnyArgs(new List<Specialization>
                {new() {Title = TestSpecializationTitle}});
            courseRepository = Substitute.For<ICourseRepository>();
            courseRepository.GetAllAsync().ReturnsForAnyArgs(new List<Course>());
            assignmentRepository = Substitute.For<IAssignmentRepository>();
            assignmentRepository.GetAllAsync().ReturnsForAnyArgs(new List<Assignment>());
            csvParserManager = new CsvParserManager(studentRepository, specializationRepository, courseRepository,
                assignmentRepository);
        }

        [Test]
        public async Task Should_parse_student_csv_to_students()
        {
            var emptyStudentRepository = Substitute.For<IStudentRepository>();
            emptyStudentRepository.GetAllAsync().ReturnsForAnyArgs(new List<Student>());
            csvParserManager = new CsvParserManager(emptyStudentRepository, null, null,
                null);
            var fileData = await GetFileData(StudentsCsvFileName);
            var parsingResult = await csvParserManager.ParseStudentsCsvToStudents(fileData);
            parsingResult.IsSuccess.Should().BeTrue();
            parsingResult.StatusCode.Should().Be(200);
            parsingResult.Data.Count.Should().Be(2);
        }

        [Test]
        public async Task Should_parse_specialization_csv_to_specializations()
        {
            var fileData = await GetFileData(SpecializationsCsvFileName);
            var parsingResult = await csvParserManager.ParseSpecializationCsvToSpecializations(fileData);
            parsingResult.IsSuccess.Should().BeTrue();
            parsingResult.StatusCode.Should().Be(200);
            parsingResult.Data.Count.Should().Be(1);
        }

        [Test]
        public async Task Should_parse_course_csv_to_specializations()
        {
            var fileData = await GetFileData(CourseCsvFileName);
            var parsingResult = await csvParserManager.ParseCourseCsvToSpecializations(fileData);
            parsingResult.IsSuccess.Should().BeTrue();
            parsingResult.StatusCode.Should().Be(200);
            parsingResult.Data.Count.Should().Be(1);
        }

        [Test]
        public async Task Should_parse_assignment_csv_to_assignments()
        {
            var fileData = await GetFileData(AssignmentsCsvFileName);
            var parsingResult = await csvParserManager.ParseAssignmentCsvToAssignments(fileData);
            parsingResult.IsSuccess.Should().BeTrue();
            parsingResult.StatusCode.Should().Be(200);
            parsingResult.Data.Count.Should().Be(2);
        }

        private static async Task<string> GetFileData(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), TestDataDirectory, fileName);
            var fileData = await File.ReadAllTextAsync(filePath);
            return fileData;
        }
    }
}