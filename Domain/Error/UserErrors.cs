namespace yeni.Domain.Error;

public class UserErrors
{
    public static Error InvalidCurrentPassword() => Error.BadRequest("400", "Hatalı Şifre girdiniz");
    
    
    

}