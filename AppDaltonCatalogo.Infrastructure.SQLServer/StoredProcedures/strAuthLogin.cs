using DaltORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.StoredProcedures
{
    [StoredProcedure("str_Dalton_Llantas_GENClienteAuth_LoginCustomer")]
    public class strAuthLogin : DatabaseStoredProcedure<strAuthLogin,string>
    {
        private string _email;
        private string _psw;
        private string _NameSocial;

        [Field("email",255,ParameterDirection.Input)]
        public string Email { get => _email; set => _email = value; }
        [Field("Psw", 255, ParameterDirection.Input)]
        public string Psw { get => _psw; set => _psw = value; }
        [Field("NameSocial", 255, ParameterDirection.Input)]
        public string NameSocial { get => _NameSocial; set => _NameSocial = value; }
    }
}
