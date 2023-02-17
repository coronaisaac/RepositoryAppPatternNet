using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.Interfaces.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.UOW
{
    public interface IUnitOfWork
    {
        public IUnitOfWorkAdapter Create(string? ConnectionString = null);
    }
}
