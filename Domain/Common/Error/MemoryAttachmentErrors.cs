namespace yeni.Domain.Error;

public class MemoryAttachmentErrors
{
    public static Error MemoryAttachmentNotFound(int memoryAttachmentId) => Error.BadRequest("400", $"Anıya bağlı belge '{memoryAttachmentId}' bulunamadı");
}