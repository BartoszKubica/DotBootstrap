using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Tests.Commands;

public class TestCommand : ICommand
{
    public bool Handled { get; set; }
}

public class TestCommandHandler : ICommandHandler<TestCommand>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestCommandHandler(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public async Task Execute(TestCommand command, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _invokeRecorder.Messages.Add($"{command.GetType()} {nameof(TestCommandHandler)}");
        command.Handled = true;
    }
}

public class TestCommand2 : ICommand
{
    public bool Handled { get; set; }
}

public class TestCommandHandler2 : ICommandHandler<TestCommand2>
{
    public async Task Execute(TestCommand2 command, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        command.Handled = true;
    }
}

public class TestPreprocessor<TCommand> : ICommandPreprocessor<TCommand>
    where TCommand : ICommand
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestPreprocessor(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task Process(TCommand command)
    {
        _invokeRecorder.Messages.Add($"{command.GetType()} {nameof(TestPreprocessor<TCommand>)}");
        return Task.CompletedTask;
    }
}

public class TestPostprocessor<TCommand> : ICommandPostprocessor<TCommand>
    where TCommand : ICommand
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestPostprocessor(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task Process(TCommand command)
    {
        _invokeRecorder.Messages.Add($"{command.GetType()} {nameof(TestPostprocessor<TCommand>)}");
        return Task.CompletedTask;
    }
}

public class TestMiddleware<TCommand> : ICommandMiddleware<TCommand> where TCommand : ICommand
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestMiddleware(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task Process(TCommand command, Func<Task> next)
    {
        _invokeRecorder.Messages.Add($"{nameof(TestMiddleware<TCommand>)} before");
        next();
        _invokeRecorder.Messages.Add($"{nameof(TestMiddleware<TCommand>)} after");

        return Task.CompletedTask;
    }
}

public class TestMiddleware2<TCommand> : ICommandMiddleware<TCommand> where TCommand : ICommand
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestMiddleware2(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task Process(TCommand command, Func<Task> next)
    {
        _invokeRecorder.Messages.Add($"{nameof(TestMiddleware2<TCommand>)} before");
        next();
        _invokeRecorder.Messages.Add($"{nameof(TestMiddleware2<TCommand>)} after");

        return Task.CompletedTask;
    }
}
