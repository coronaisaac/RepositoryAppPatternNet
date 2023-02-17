
using AppDaltonCatalogo.Infrastructure.SQL.Dtos.Auth;
using AppDaltonCatalogo.Infrastructure.SQLCatalogos.Dtos.Auth;

namespace AppDaltonCatalogo.Infrastructure.SQL.Interfaces.Auth
{
    public interface IAuth
    {
        public Response<string> Login(LoginDto command);
        
    }
}
