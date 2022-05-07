using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands;

public static class ServiceCollectionExtensions
{
    public static void AddMessaging(this IServiceCollection serviceCollection, Action<IPipelineConfigurator>?
        action = null)
    {
        serviceCollection.AddScoped<ICommandBus, CommandBus>();
        serviceCollection.AddScoped<ICommandPreprocessorRunner, CommandPreprocessorRunner>();
        serviceCollection.AddScoped<ICommandPostProcessorRunner, CommandPostProcessorRunner>();
        serviceCollection.AddScoped<ICommandPipelineRunner, CommandPipelineRunner>();
        serviceCollection.AddScoped<IPipelineInvoker, PipelineInvoker>();

        PipelineStore? pipelineProvider = null;
        if (action is not null)
        {
            var configurator = new PipelineConfigurator(serviceCollection);
            action(configurator);
            pipelineProvider = configurator.Configure();
        }

        serviceCollection.AddSingleton(pipelineProvider ?? new PipelineStore());
    }

    public static IServiceCollection RegisterCommandHandler<TCommand, THandler>(this IServiceCollection services)
        where TCommand : class, ICommand where THandler : class, ICommandHandler<TCommand>
    {
        services.AddTransient<ICommandHandler<TCommand>, THandler>();
        return services;
    }

    private static IServiceCollection RegisterAllCommandHandlersFromAssembly<TType>(this IServiceCollection services)
    {
        var handlerType = typeof(ICommandHandler<>);
        return services.Scan(scan => scan
            .FromAssemblyOf<TType>()
            .AddClasses(c => c.Where(t => !t.ContainsGenericParameters)
                .AssignableTo(handlerType))
            .As(t => t.GetInterfaces().Where(i => i.IsGenericType &&
                                                  i.GetGenericTypeDefinition() == handlerType))
            .WithTransientLifetime());
    }
}