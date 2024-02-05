using Api.Authorization;
using Api.Routes.Weather;
using Application;
using FluentValidation;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add other layers
builder.AddApplicaton();
builder.AddInfrastructure();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT-token authentication + our custom authorization policies
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization(options => { options.AddAuthorizationPolicies(); });

// FluentValidation register all validators present in this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Api.Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    if (app.Environment.IsDevelopment())
    {
        options.EnableTryItOutByDefault();
        options.EnablePersistAuthorization();
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapWeatherGroup();

app.Run();

// To make it visible for E2E-tests:
namespace Api
{
    public partial class Program
    {
    }
}