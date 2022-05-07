using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface IPipelineInvoker
{
    IReadOnlyCollection<ICommandPreprocessor<TCommand>> InvokePreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<ICommandPostprocessor<TCommand>> InvokePostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<ICommandMiddleware<TCommand>> InvokeMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand;
}

internal class PipelineInvoker : IPipelineInvoker
{
    private readonly PipelineStore _pipelineStore;
    private readonly IServiceProvider _serviceProvider;

    public PipelineInvoker(PipelineStore pipelineStore, IServiceProvider serviceProvider)
    {
        _pipelineStore = pipelineStore;
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyCollection<ICommandPreprocessor<TCommand>> InvokePreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _pipelineStore.GetPreprocessors(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandPreprocessor<TCommand>>()
            .ToArray();
    }
    
    public IReadOnlyCollection<ICommandPostprocessor<TCommand>> InvokePostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _pipelineStore.GetPostprocessors(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandPostprocessor<TCommand>>()
            .ToArray();
    }
    
    public IReadOnlyCollection<ICommandMiddleware<TCommand>> InvokeMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _pipelineStore.GetMiddlewares(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandMiddleware<TCommand>>()
            .ToArray();
    }
}