using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface ICommandPreprocessorRunner
{
    Task Process<TCommand>(TCommand command) where TCommand : ICommand;
}

internal class CommandPreprocessorRunner : ICommandPreprocessorRunner
{
    private readonly ICommandPipelineInvoker _commandPipelineInvoker;

    public CommandPreprocessorRunner(ICommandPipelineInvoker commandPipelineInvoker)
    {
        _commandPipelineInvoker = commandPipelineInvoker;
    }

    public async Task Process<TCommand>(TCommand command) where TCommand : ICommand
    {
        var preprocessors = _commandPipelineInvoker.InvokePreprocessors(command);

        foreach (var preprocessor in preprocessors)
        {
            await preprocessor.Process(command);
        }
    }
}