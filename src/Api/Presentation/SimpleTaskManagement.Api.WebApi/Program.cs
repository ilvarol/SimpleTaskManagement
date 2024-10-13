using FluentValidation.AspNetCore;
using SimpleTaskManagement.Api.Application.Extensions;
using SimpleTaskManagement.Api.WebApi.Infrastructure.Extensions;
using SimpleTaskManagement.Infrastructure.Persistence.Extensions;
using static SimpleTaskManagement.Api.WebApi.Infrastructure.ActionFilters.ValidationFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(opt => opt.Filters.Add<ValidateModelStateFilter>())
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services
    .AddFluentValidationAutoValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureRegistration();
builder.Services.AddApplicationRegistration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.ConfigureExceptionHandling(app.Environment.IsDevelopment());

app.UseAuthorization();

app.MapControllers();

app.Run();
