using yeni.Domain.Entities.Base;

namespace yeni.Data.Repository;

public interface IBaseRepository<TEntity, TType> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(TType id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}