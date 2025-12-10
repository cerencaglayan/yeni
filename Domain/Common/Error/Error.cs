namespace yeni.Domain.Error;

public class Error
{
    public string Code { get; set; }
    public string Message { get; set; }
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
    public static Error Failure(string code, string message) => new(code, message); 

    public static Error NotFound(string code, string message) => new(code, message);
    
    public static Error BadRequest(string code, string message) => new(code, message);

    public static Error Timeout(string code, string message) => new(code, message);

}