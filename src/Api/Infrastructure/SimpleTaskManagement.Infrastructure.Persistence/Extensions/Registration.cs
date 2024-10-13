using Microsoft.Extensions.DependencyInjection;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Infrastructure.Persistence.Repositories;

namespace SimpleTaskManagement.Infrastructure.Persistence.Extensions;

public static class Registration
{
    public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services)
    {
        services.AddSingleton<ITaskRepository, TaskRepository>();

        return services;
    }
}
