using FluentAssertions;
using NUnit.Framework;
using Server.Logic;

namespace CourseraApiTests
{
    public class OperationResultTests
    {
        private const string ErrorMessage = "Error message";
        private readonly object result = new { };
        
        [Test]
        public void Should_create_operation_result_success()
        {
            var operationResult = OperationResult.Success();
            
            operationResult.IsSuccess.Should().BeTrue();
            operationResult.StatusCode.Should().Be(200);
            operationResult.ErrorText.Should().BeNull();
        }
        
        [Test]
        public void Should_create_operation_result_with_error()
        {
            var operationResult = OperationResult.Error(ErrorMessage, 500);
            
            operationResult.IsSuccess.Should().BeFalse();
            operationResult.StatusCode.Should().Be(500);
            operationResult.ErrorText.Should().Be(ErrorMessage);
        }
        
        [Test]
        public void Should_create_generic_operation_result_success()
        {
            var operationResult = OperationResult<object>.Success(result);
            
            operationResult.IsSuccess.Should().BeTrue();
            operationResult.StatusCode.Should().Be(200);
            operationResult.ErrorText.Should().BeNull();
            operationResult.Data.Should().Be(result);
        }
        
        [Test]
        public void Should_create_generic_operation_result_with_error()
        {
            var operationResult = OperationResult<object>.Error(ErrorMessage, 500);
            
            operationResult.IsSuccess.Should().BeFalse();
            operationResult.StatusCode.Should().Be(500);
            operationResult.ErrorText.Should().Be(ErrorMessage);
            operationResult.Data.Should().BeNull();
        }
    }
}