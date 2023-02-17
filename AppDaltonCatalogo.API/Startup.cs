namespace AppDaltonCatalogo.API
{
    public class Startup
    {
        public void Configuration(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }
    }
}
