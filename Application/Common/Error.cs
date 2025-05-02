namespace Application.Common
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public static Error Create(string code, string message) => 
            new Error{Code = code, Message = message};
    }
}