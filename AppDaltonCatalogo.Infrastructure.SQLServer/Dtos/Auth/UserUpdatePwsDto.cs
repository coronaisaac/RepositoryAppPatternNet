using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQLCatalogos.Dtos.Auth
{
    public class UserUpdatePwsDto
    {
        public string Email { set; get; }
        public string PwsTmp { set; get; }
        public string PwsNew { set; get; }
    }
}
