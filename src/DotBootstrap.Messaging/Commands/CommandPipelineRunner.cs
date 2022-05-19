using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

internal interface ICommandPipelineRunner
{
    Task RunPipeline<TCommand>(TCommand command, CancellationToken cancellationToken, CommandWrapperBase commandWrapper)
        where TCommand : ICommand;
}

internal class CommandPipelineRunner : ICommandPipelineRunner
{
    private readonly ICommandPostProcessorRunner _commandPostprocessorRunner;
    private readonly ICommandPreprocessorRunner _commandPreprocessorRunner;
    private readonly ICommandPipelineInvoker _commandPipelineInvoker;

    public CommandPipelineRunner(ICommandPostProcessorRunner commandPostprocessorRunner, 
        ICommandPreprocessorRunner commandPreprocessorRunner, ICommandPipelineInvoker commandPipelineInvoker)
    {
        _commandPostprocessorRunner = commandPostprocessorRunner;
        _commandPreprocessorRunner = commandPreprocessorRunner;
        _commandPipelineInvoker = commandPipelineInvoker;
    }

    public async Task RunPipeline<TCommand>(TCommand command, CancellationToken cancellationToken,
        CommandWrapperBase commandWrapper)
        where TCommand : ICommand
    {
        async Task Invoke()
        {
            await commandWrapper.Process(command, cancellationToken);
        }

        await _commandPreprocessorRunner.Process(command);
        await _commandPipelineInvoker.InvokeMiddlewares(command).Reverse().Aggregate((Func<Task>)Invoke, 
            (next, middleware) =>
            () => middleware.Process(command, next))();
        await _commandPostprocessorRunner.Process(command);
    }
}