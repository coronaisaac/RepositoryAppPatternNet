using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.SQL;

namespace AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.Adapter
{
    public interface IUnitOfWorkAdapter : IDisposable
    {
        public IUnitOfWorkSQLRepository CatalogosRepository { get; }
    }
}
