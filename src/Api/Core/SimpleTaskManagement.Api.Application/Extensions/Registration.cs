using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using SimpleTaskManagement.Common.Models.Commands;
using SimpleTaskManagement.Api.Application.Validators.Task;

namespace SimpleTaskManagement.Api.Application.Extensions;

public static class Registiration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
    {
        var assm = typeof(Registiration).GetTypeInfo().Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assm));

        services.AddAutoMapper(assm);

        services.AddValidatorsFromAssembly(assm);

        return services;
    }
}
