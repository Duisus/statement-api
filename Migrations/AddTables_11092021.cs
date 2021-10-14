using FluentMigrator;

namespace Migrations 
{
    [Migration(11092021)]
    public class AddTables_11092021 : Migration
    {
        public override void Up()
        {
            CreateStudentTable();
            CreateSpecializationTable();
            CreateCourseTable();
            CreateAssignmentTable();
            CreateUsersTable();
        }

        public override void Down()
        {
            Delete.Table("Assignment");
            Delete.Table("Course");
            Delete.Table("Specialization");
            Delete.Table("Student");
            Delete.Table("Users");
        }

        private void CreateStudentTable() 
        {
            if (!Schema.Table("Student").Exists()) 
            {
                Create.Table("Student")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("FullName").AsString().Unique()
                    .WithColumn("Group").AsString()
                    .WithColumn("EntrolledCourses").AsInt64()
                    .WithColumn("CompletedCourses").AsInt64()
                    .WithColumn("MemberState").AsString();
            }
        }

        private void CreateSpecializationTable() 
        {
            if (!Schema.Table("Specialization").Exists())
            {
                Create.Table("Specialization")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("StudentId").AsInt64()
                    .WithColumn("Title").AsString()
                    .WithColumn("IsCompleted").AsBoolean()
                    .WithColumn("University").AsString()
                    .WithColumn("CourseCount").AsInt64()
                    .WithColumn("CompletedCourseCount").AsInt64();
            }
        }

        private void CreateCourseTable() 
        {
            if (!Schema.Table("Course").Exists()) 
            {
                Create.Table("Course")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("SpecializationId").AsInt64().ForeignKey("Specialization", "Id").Nullable()
                    .WithColumn("StudentId").AsInt64().ForeignKey("Student", "Id")
                    .WithColumn("Title").AsString()
                    .WithColumn("IsCompleted").AsBoolean()
                    .WithColumn("Progress").AsDouble()
                    .WithColumn("Grade").AsFloat()
                    .WithColumn("CertificateUrl").AsString().Nullable()
                    .WithColumn("LearningHours").AsFloat()
                    .WithColumn("University").AsString()
                    .WithColumn("EnrollmentTime").AsDateTime().Nullable()
                    .WithColumn("ClassStartTime").AsDateTime().Nullable()
                    .WithColumn("ClassEndTime").AsDateTime().Nullable()
                    .WithColumn("LastCourseActivityTime").AsDateTime().Nullable()
                    .WithColumn("CompletionTime").AsDateTime().Nullable();
            }
        }

        private void CreateAssignmentTable() 
        {
            if (!Schema.Table("Assignment").Exists())
            {
                Create.Table("Assignment")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("StudentId").AsInt64().ForeignKey("Student", "Id")
                    .WithColumn("Title").AsString()
                    .WithColumn("Order").AsInt64()
                    .WithColumn("AttemptGrade").AsFloat()
                    .WithColumn("GradeAfterOverride").AsFloat().Nullable()
                    .WithColumn("IsAttemptPassed").AsBoolean()
                    .WithColumn("AttemptTimestamp").AsDateTime()
                    .WithColumn("ItemAttemptOrderNumber").AsInt64()
                    .WithColumn("CourseName").AsString();
            }
        }

        private void CreateUsersTable()
        {
            if (!Schema.Table("Users").Exists())
            {
                Create.Table("Users")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Email").AsString()
                    .WithColumn("Password").AsString()
                    .WithColumn("Salt").AsString();
            }
        }
    }
}