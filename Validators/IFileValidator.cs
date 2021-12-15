using Microsoft.AspNetCore.Http;
using Server.Logic;

namespace server.Validators
{
    public interface IFileValidator
    {
        OperationResult IsValid(params IFormFile[] files);
        OperationResult IsValid(IFormFile file);
    }
}