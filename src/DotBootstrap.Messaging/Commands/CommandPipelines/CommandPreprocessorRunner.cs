using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface ICommandPreprocessorRunner
{
    Task Process<TCommand>(TCommand command) where TCommand : ICommand;
}

internal class CommandPreprocessorRunner : ICommandPreprocessorRunner
{
    private readonly IPipelineInvoker _pipelineInvoker;

    public CommandPreprocessorRunner(IPipelineInvoker pipelineInvoker)
    {
        _pipelineInvoker = pipelineInvoker;
    }

    public async Task Process<TCommand>(TCommand command) where TCommand : ICommand
    {
        var preprocessors = _pipelineInvoker.InvokePreprocessors(command);

        foreach (var preprocessor in preprocessors)
        {
            await preprocessor.Process(command);
        }
    }
}