namespace Server.Logic
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorText { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }

        public static OperationResult<T> Success(int statusCode = 200)
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                ErrorText = null,
                StatusCode = statusCode
            };
        }
        
        public static OperationResult<T> Success(T data, int statusCode = 200)
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                ErrorText = null,
                StatusCode = statusCode,
                Data = data
            };
        }
        
        public static OperationResult<T> Error(string errorText, int statusCode = 400)
        {
            return new OperationResult<T>()
            {
                IsSuccess = false,
                ErrorText = errorText,
                StatusCode = statusCode
            };
        }
    }
}