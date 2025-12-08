using yeni.Domain.Entities.Base;
using yeni.Domain.Repository;

namespace yeni.Data.Repository;

public interface IRefreshTokenRepository:IBaseRepository<RefreshToken, int>
{
    
}