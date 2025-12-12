using yeni.Domain.Entities.Base;
using yeni.Domain.Repositories;

namespace yeni.Domain.Repositories;

public interface IRefreshTokenRepository:IBaseRepository<RefreshToken, int>
{
    
}