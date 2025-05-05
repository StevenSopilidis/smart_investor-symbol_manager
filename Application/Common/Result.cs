using System.Net;

namespace Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public Error Error { get; set; }
        public ErrorType Type { get; set; }

        public static Result<T> Success(T value)
            => new() { IsSuccess = true, Value = value};

        public static Result<T> Failure(string errorCode, string errorMessage, ErrorType type)
            => Failure(Error.Create(errorCode, errorMessage), type);
        
        public static Result<T> Failure(Error error, ErrorType type)
            => new() { IsSuccess = false, Error = error, Type = type};

    }
}