
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using VakilPors.Business.Extensions;
using VakilPors.Core.Authentication.Extensions;
using VakilPors.Core.Exceptions.Extensions;
using VakilPors.Core.Hubs;
using VakilPors.Core.Mapper;
using VakilPors.Data.Context;
using VakilPors.Data.Extensions;
using VakilPors.Web.Configuration.Extensions;
using ZarinSharp.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? configuration.GetConnectionString("AppDbContext");
// Add services to the container.
builder.Services.RegisterServices();
builder.Services.RegisterAppDbContext(connectionString);
builder.Services.RegisterIdentity<AppDbContext>();
builder.Services.RegisterAuthentication(configuration);
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddZarinSharp(op =>
{
    op.MerchantId = "831ql8a0-31ja-ms82-1e30-pzla92kd145s";//dummy merchant id
    op.IsSandbox = true;
});
builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithJWTSupport();


var app = builder.Build();

await app.MigrateDb();


// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseGlobalExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
