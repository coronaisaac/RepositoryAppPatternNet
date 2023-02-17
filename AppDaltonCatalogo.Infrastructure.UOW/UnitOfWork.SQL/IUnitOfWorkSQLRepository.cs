using AppDaltonCatalogo.Infrastructure.SQL.Interfaces.Auth;

namespace AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.SQL
{
    public interface IUnitOfWorkSQLRepository
    {
        public IAuth auth { get; }
    }
}
