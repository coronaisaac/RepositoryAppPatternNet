using AppDaltonCatalogo.Infrastructure.SQL.Interfaces.Auth;
using AppDaltonCatalogo.Infrastructure.SQL.Repositories;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.SQL;
using DaltORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.SQLCatalogos
{
    public class UnitOfWorkSQLRepository : IUnitOfWorkSQLRepository
    {
        private readonly Database database;
        public IAuth auth { get; }

        public UnitOfWorkSQLRepository(Database database)
        {
            auth = new AuthRepositories(database);
            this.database = database;
        }
    }
}
