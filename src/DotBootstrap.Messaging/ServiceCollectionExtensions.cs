using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Queries.QueryPipelines;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging;

public static class ServiceCollectionExtensions
{
    public static void AddMessaging(this IServiceCollection serviceCollection, Action<ICommandPipelineConfigurator>?
        commandConfiguration = null, Action<IQueryPipelineConfigurator>? queryConfiguration = null)
    {
        serviceCollection.AddScoped<ICommandBus, CommandBus>();
        serviceCollection.AddScoped<ICommandPreprocessorRunner, CommandPreprocessorRunner>();
        serviceCollection.AddScoped<ICommandPostProcessorRunner, CommandPostProcessorRunner>();
        serviceCollection.AddScoped<ICommandPipelineRunner, CommandPipelineRunner>();
        serviceCollection.AddScoped<ICommandPipelineInvoker, CommandPipelineInvoker>();

        serviceCollection.AddScoped<IQueryBus, QueryBus>();
        serviceCollection.AddScoped<IQueryPreprocessorRunner, QueryPreprocessorRunner>();
        serviceCollection.AddScoped<IQueryPostprocessorRunner, QueryPostprocessorRunner>();
        serviceCollection.AddScoped<IQueryPipelineRunner, QueryPipelineRunner>();
        serviceCollection.AddScoped<IQueryPipelineInvoker, QueryPipelineInvoker>();
        
        CommandPipelineStore? commandPipelineProvider = null;
        QueryPipelineStore? queryPipelineProvider = null;
        if (commandConfiguration is not null)
        {
            var configurator = new CommandPipelineConfigurator(serviceCollection);
            commandConfiguration(configurator);
            commandPipelineProvider = configurator.Configure();
        }
        
        if (queryConfiguration is not null)
        {
            var configurator = new QueryPipelineConfigurator(serviceCollection);
            queryConfiguration(configurator);
            queryPipelineProvider = configurator.Configure();
        }

        serviceCollection.AddSingleton(commandPipelineProvider ?? new CommandPipelineStore());
        serviceCollection.AddSingleton(queryPipelineProvider ?? new QueryPipelineStore());
    }

    public static IServiceCollection RegisterCommandHandler<TCommand, THandler>(this IServiceCollection services)
        where TCommand : class, ICommand where THandler : class, ICommandHandler<TCommand>
    {
        services.AddTransient<ICommandHandler<TCommand>, THandler>();
        return services;
    }
    
    public static IServiceCollection RegisterQueryHandler<TQuery, TResponse, THandler>(this IServiceCollection services)
        where TQuery : class, IQuery<TResponse> where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        services.AddTransient<IQueryHandler<TQuery, TResponse>, THandler>();
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
    
    private static IServiceCollection RegisterAllQueryHandlersFromAssembly<TType>(this IServiceCollection services)
    {
        var handlerType = typeof(IQueryHandler<,>);
        return services.Scan(scan => scan
            .FromAssemblyOf<TType>()
            .AddClasses(c => c.Where(t => !t.ContainsGenericParameters)
                .AssignableTo(handlerType))
            .As(t => t.GetInterfaces().Where(i => i.IsGenericType &&
                                                  i.GetGenericTypeDefinition() == handlerType))
            .WithTransientLifetime());
    }
}