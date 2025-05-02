using System.Net;

namespace Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public Error Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static Result<T> Success(T value)
            => new() { IsSuccess = true, Value = value, StatusCode = HttpStatusCode.OK};

        public static Result<T> Failure(HttpStatusCode code, string errorCode, string errorMessage)
            => Failure(code, Error.Create(errorCode, errorMessage));
        
        public static Result<T> Failure(HttpStatusCode code, Error error)
            => new() { IsSuccess = false, Error = error, StatusCode = code};

    }
}