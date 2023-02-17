using AppDaltonCatalogo.API;
using AppDaltonCatalogo.Infrastructure.UOW;
using AppDaltonCatalogo.Infrastructure.UOW.UnitOfWork.SQLCatalogos;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
new Startup().Configuration(builder);
builder.Services.AddTransient<IUnitOfWork, UnitOfWorkSQL>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

