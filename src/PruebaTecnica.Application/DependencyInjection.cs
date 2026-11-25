using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PruebaTecnica.Application.Common.Cqrs;

namespace PruebaTecnica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        Type[] handlerTypes = [typeof(ICommandHandler<>), typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

        var registrations = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && handlerTypes.Contains(i.GetGenericTypeDefinition()))
                .Select(i => (Interface: i, Implementation: t)));

        foreach (var (serviceType, implementationType) in registrations)
            services.AddScoped(serviceType, implementationType);

        return services;
    }
}