using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface ICommandPostProcessorRunner
{
    Task Process<TCommand>(TCommand command) where TCommand : ICommand;
}

internal class CommandPostProcessorRunner : ICommandPostProcessorRunner
{
    private readonly IPipelineInvoker _pipelineInvoker;

    public CommandPostProcessorRunner(IPipelineInvoker pipelineInvoker)
    {
        _pipelineInvoker = pipelineInvoker;
    }

    public async Task Process<TCommand>(TCommand command) where TCommand : ICommand
    {
        var postprocessors = _pipelineInvoker.InvokePostprocessors(command);

        foreach (var postprocessor in postprocessors)
        {
            await postprocessor.Process(command);
        }
    }
}