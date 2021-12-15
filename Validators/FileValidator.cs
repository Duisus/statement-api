using Microsoft.AspNetCore.Http;
using Server.Logic;

namespace server.Validators
{
    public class FileValidator : IFileValidator
    {
        public OperationResult IsValid(params IFormFile[] files)
        {
            foreach (var file in files)
            {
                var isFileValid = IsFileValid(file);
                if (!isFileValid.IsSuccess)
                    return isFileValid;
            }

            return OperationResult.Success();
        }

        public OperationResult IsValid(IFormFile file)
        {
            return IsFileValid(file);
        }

        private static OperationResult IsFileValid(IFormFile file)
        {
            if (file == null)
                return OperationResult.Error("file can not be null");
            if (file.Length == 0)
                return OperationResult.Error($"file can not be empty {file.FileName}");
            return OperationResult.Success();
        }
    }
}