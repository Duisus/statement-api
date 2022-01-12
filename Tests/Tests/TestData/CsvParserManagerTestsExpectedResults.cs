using System;
using server.Models;
using Server.Models;

namespace CourseraApiTests.TestData
{
    public static class CsvParserManagerTestsExpectedResults
    {
        public static Student Student = new()
        {
            CompletedCourses = 5, EnrolledCourses = 6, FullName = "TestName", Group = "TestGroup", Id = 0,
            MemberState = "DELETED_MEMBER"
        };

        public static Specialization Specialization = new()
        {
            CompletedCourseCount = 3, CourseCount = 4, Id = 0, IsCompleted = false, StudentId = 0, Title = "TestSpec",
            University = "TestUniversity"
        };

        public static Course Course = new()
        {
            CertificateUrl = "",
            ClassEndTime = new DateTime(2021, 12, 13, 12, 59, 59),
            ClassStartTime = new DateTime(2021, 10, 25, 12, 0, 0),
            CompletionTime = null,
            EnrollmentTime = new DateTime(2021, 10, 07, 0, 56, 02),
            Grade = 0,
            Id = 0,
            IsCompleted = false,
            LastCourseActivityTime = new DateTime(2021, 10, 7, 2, 8, 0),
            LearningHours = 0,
            Progress = 0,
            SpecializationId = null,
            StudentId = 0,
            Title = "TestSpec",
            University = "TestUniversity"
        };

        public static Assignment Assignment = new()
        {
            AttemptGrade = 0.8,
            AttemptTimestamp = new DateTime(2021, 10, 17, 10, 59, 15),
            CourseName = "TestCourse",
            GradeAfterOverride = null,
            Id = 0,
            IsAttemptPassed = true,
            ItemAttemptOrderNumber = 1,
            Order = 1,
            StudentId = 0,
            Title = "TestItem"
        };
    }
}