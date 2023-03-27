
using Microsoft.EntityFrameworkCore;
using Serilog;
using VakilPors.Business.Extensions;
using VakilPors.Core.Authentication.Extensions;
using VakilPors.Core.Mapper;
using VakilPors.Data.Context;
using VakilPors.Data.Extensions;
using VakilPors.Web.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString=Environment.GetEnvironmentVariable("ConnectionString") ?? configuration.GetConnectionString("AppDbContext");
// Add services to the container.
builder.Services.RegisterServices();
builder.Services.RegisterAppDbContext(connectionString);
builder.Services.RegisterIdentity<AppDbContext>();
builder.Services.RegisterAuthentication(configuration);
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithJWTSupport();
var app = builder.Build();
try
{
    await using var scope = app.Services.CreateAsyncScope();
    using var db = scope.ServiceProvider.GetService<AppDbContext>();
    await db.Database.MigrateAsync();
}
catch (System.Exception)
{
    System.Console.WriteLine("An error occurred while migrating or seeding the database.");
}


// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
