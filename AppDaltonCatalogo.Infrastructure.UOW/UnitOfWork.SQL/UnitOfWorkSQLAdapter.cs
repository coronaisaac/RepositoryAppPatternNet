using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.Adapter;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.SQL;
using DaltORM;

namespace AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.SQLCatalogos
{
    public class UnitOfWorkSQLAdapter : IUnitOfWorkAdapter
    {
        private Database database { get; set; }
        private DataConnection dataConnectiondata { get; set; }

        public IUnitOfWorkSQLRepository CatalogosRepository { get; set; }

        public UnitOfWorkSQLAdapter(string ConnectionString)
        {
            database = Database.CreateConnection(ConnectionString, DaltORM.Enumerics.DatabaseTypes.SQLServer);
            database.Open();
            if (database.GetStateConnection() == "closed") database.Open();
            CatalogosRepository = new UnitOfWorkSQLRepository(database);
        }

        public void Dispose()
        {
            var p = database.GetStateConnection();
            if (database.GetStateConnection().ToUpper() == "OPEN") database.Close();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
