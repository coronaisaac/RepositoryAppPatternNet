using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.Dto.Auth
{
    public record LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Psw { get; set; }
        public string? NameSocial { get; set; }
    }
}
