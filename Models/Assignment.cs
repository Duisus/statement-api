using System;
using Dapper.Contrib.Extensions;

namespace Server.Models 
{
    [Table("Assignment")]
    public class Assignment 
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public double? AttemptGrade { get; set; }

        public double? GradeAfterOverride { get; set; }

        public bool IsAttemptPassed { get; set; }

        public DateTime? AttemptTimestamp { get; set; }

        public int ItemAttemptOrderNumber { get; set; }
        
        public string CourseName { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Assignment);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
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
                   && DoubleEquals(assignment.AttemptGrade, AttemptGrade)
                   && DoubleEquals(assignment.GradeAfterOverride, GradeAfterOverride)
                   && assignment.IsAttemptPassed == IsAttemptPassed
                   && assignment.ItemAttemptOrderNumber == ItemAttemptOrderNumber
                   && assignment.CourseName == CourseName;
        }

        private static bool DoubleEquals(double? first, double? second)
        {
            if (!first.HasValue && !second.HasValue)
                return true;
            if (first.HasValue != second.HasValue)
                return false;

            return Math.Abs(first.Value - second.Value) < 0.5;
        }
    }
}