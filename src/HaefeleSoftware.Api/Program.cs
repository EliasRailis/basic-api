using System.Reflection;
using FluentValidation;
using HaefeleSoftware.Api.Application.Configurations;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Infrastructure.Services;
using HaefeleSoftware.Api.Infrastructure.Services.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersioning();
builder.Services.AddSerilog();
builder.Services.AddBehaviors();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddTransient<IDateTimeService, DateTimeService>();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapEndpoints();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseJwtMiddleware();

app.Run();