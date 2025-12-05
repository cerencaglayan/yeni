namespace yeni.Domain.Requests;

public class LoginRequest
{
    public string Name { get; set; } = null!; // username 
    public string Password { get; set; } = null!;
}