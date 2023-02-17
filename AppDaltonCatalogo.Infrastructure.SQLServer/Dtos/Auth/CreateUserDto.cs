using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.Dtos.Auth
{
    public record CreateUserDto
    {
        public string email { set; get; }
        public string vchPswOrToken { set; get; }
        public string? SocialMedia { set; get; }
    }
}
