using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface ICommandPostProcessorRunner
{
    Task Process<TCommand>(TCommand command) where TCommand : ICommand;
}

internal class CommandPostProcessorRunner : ICommandPostProcessorRunner
{
    private readonly ICommandPipelineInvoker _commandPipelineInvoker;

    public CommandPostProcessorRunner(ICommandPipelineInvoker commandPipelineInvoker)
    {
        _commandPipelineInvoker = commandPipelineInvoker;
    }

    public async Task Process<TCommand>(TCommand command) where TCommand : ICommand
    {
        var postprocessors = _commandPipelineInvoker.InvokePostprocessors(command);

        foreach (var postprocessor in postprocessors)
        {
            await postprocessor.Process(command);
        }
    }
}