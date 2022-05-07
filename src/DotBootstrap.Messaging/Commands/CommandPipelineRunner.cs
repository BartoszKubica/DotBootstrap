using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

internal interface ICommandPipelineRunner
{
    Task RunPipeline<TCommand>(TCommand command, CommandWrapperBase commandWrapper)
        where TCommand : ICommand;
}

internal class CommandPipelineRunner : ICommandPipelineRunner
{
    private readonly ICommandPostProcessorRunner _commandPostprocessorRunner;
    private readonly ICommandPreprocessorRunner _commandPreprocessorRunner;
    private readonly IPipelineInvoker _pipelineInvoker;

    public CommandPipelineRunner(ICommandPostProcessorRunner commandPostprocessorRunner, 
        ICommandPreprocessorRunner commandPreprocessorRunner, IPipelineInvoker pipelineInvoker)
    {
        _commandPostprocessorRunner = commandPostprocessorRunner;
        _commandPreprocessorRunner = commandPreprocessorRunner;
        _pipelineInvoker = pipelineInvoker;
    }

    public async Task RunPipeline<TCommand>(TCommand command, CommandWrapperBase commandWrapper)
        where TCommand : ICommand
    {
        async Task Invoke()
        {
            await commandWrapper.Process(command);
        }

        await _commandPreprocessorRunner.Process(command);
        await _pipelineInvoker.InvokeMiddlewares(command).Reverse().Aggregate((Func<Task>)Invoke, 
            (next, middleware) =>
            () => middleware.Process(command, next))();
        await _commandPostprocessorRunner.Process(command);
    }
}