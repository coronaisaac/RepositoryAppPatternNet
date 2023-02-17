using AppDaltonCatalogo.Infrastructure.UOW;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.Adapter;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.SQL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.SQLCatalogos
{
    public class UnitOfWorkSQL : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private static readonly string? ConnectionStringExt = "Data Source=10.1.1.4;Initial Catalog=DaltonPAS;User ID=dltapicatalogo;Password=dltapicatalogo#2023";

        public UnitOfWorkSQL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IUnitOfWorkAdapter Create(string? ConnectionString = null)
        {
            var connectionString = String.IsNullOrEmpty(ConnectionString) ?  _configuration.GetConnectionString("SqlServer") : ConnectionString;
            connectionString = String.IsNullOrEmpty(ConnectionString) ? _configuration.GetSection("SqlServer").Value : connectionString;
            connectionString = String.IsNullOrEmpty(ConnectionString) ? ConnectionStringExt : connectionString;
            if (String.IsNullOrEmpty(connectionString))  throw new Exception("Sorry but tha connection string is not valid that connect to database, try againg with a new connection");
            return new UnitOfWorkSQLAdapter(connectionString);
        }


    }
}
