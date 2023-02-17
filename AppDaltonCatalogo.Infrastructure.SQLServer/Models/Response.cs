using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.Models
{
    public class Response<T>
    {
        public static Response<T> FromSuccess(T value) => new Response<T> { IsSucceed = true, Value = value };
        public static Response<T> FromError(string error) => new Response<T> { IsSucceed = false, ErrorMessage = error };

        public T? Value { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsSucceed { get; set; }
        public Response()
        {

        }
    }
}
