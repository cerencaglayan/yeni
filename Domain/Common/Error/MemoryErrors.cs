namespace yeni.Domain.Error;

public class MemoryErrors
{
    public static Error MemoryNotfound(int id) => Error.BadRequest("400", $"Anı '{id}' bulunamadı");

    
}