
using VakilPors.Business.Extensions;
using VakilPors.Core.Authentication.Extensions;
using VakilPors.Data.Context;
using VakilPors.Data.Extensions;
using VakilPors.Web.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Add services to the container.
builder.Services.RegisterServices();
builder.Services.RegisterAppDbContext(configuration);
builder.Services.RegisterIdentity<AppDbContext>();
builder.Services.RegisterAuthentication(configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithJWTSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
