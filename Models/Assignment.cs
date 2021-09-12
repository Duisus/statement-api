using System;
using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("[Assignment]")]
    public class Assignment 
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public double AttemptGrade { get; set; }

        public double? GradeAfterOverride { get; set; }

        public bool IsAttemptPassed { get; set; }

        public DateTime? AttemptTimestampt { get; set; }

        public int ItemAttemptOrderNumber { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Assignment);
        }

        private bool Equals(Assignment assignment)
        {
            if (assignment is null) return false;
            if (assignment.GradeAfterOverride.HasValue && !GradeAfterOverride.HasValue
            || GradeAfterOverride.HasValue && !assignment.GradeAfterOverride.HasValue)
            {
                return false;
            }

            return assignment.IsAttemptPassed == IsAttemptPassed
                   && assignment.StudentId == StudentId
                   && assignment.Title == Title
                   && assignment.Order == Order
                   && Math.Abs(assignment.AttemptGrade - AttemptGrade) < 0.5
                   && ((!assignment.GradeAfterOverride.HasValue && !GradeAfterOverride.HasValue) ||
                       Math.Abs(assignment.GradeAfterOverride.Value - GradeAfterOverride.Value) < 0.5)
                   && assignment.IsAttemptPassed == IsAttemptPassed
                   && assignment.ItemAttemptOrderNumber == ItemAttemptOrderNumber;
        }
    }
}