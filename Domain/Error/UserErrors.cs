namespace yeni.Domain.Error;

public class UserErrors
{
    public static Error InvalidCurrentPassword() => Error.BadRequest("400", "Hatalı Şifre girdiniz");
    public static Error UserNotfound(string name) => Error.BadRequest("400", $"Kullanıcı '{name}' bulunamadı");
    
    
    

}