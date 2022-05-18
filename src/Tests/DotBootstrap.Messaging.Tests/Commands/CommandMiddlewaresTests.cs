using System;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Commands;

public class CommandMiddlewaresTests
{
    [Fact]
    public async Task Send_WithGlobalMiddlewaresSet_ShouldInvokeMiddlewares()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddGlobalMiddleware(typeof(TestMiddleware<>))
                .AddGlobalMiddleware(typeof(TestMiddleware2<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        await commandBus.Send(command);

        invokerRecorder.Messages[0].Should().Be($"{nameof(TestMiddleware<TestCommand>)} before");
        invokerRecorder.Messages[1].Should().Be($"{nameof(TestMiddleware2<TestCommand>)} before");
        invokerRecorder.Messages[2].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");
        invokerRecorder.Messages[3].Should().Be($"{nameof(TestMiddleware2<TestCommand>)} after");
        invokerRecorder.Messages[4].Should().Be($"{nameof(TestMiddleware<TestCommand>)} after");
        command.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithCommandMiddlewareSet_ShouldInvokeOnlySetMiddleware()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddMiddlewareForCommand<TestCommand>(typeof(TestMiddleware<>))
                .AddGlobalMiddleware(typeof(TestMiddleware2<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        await commandBus.Send(command);

        invokerRecorder.Messages[0].Should().Be($"{nameof(TestMiddleware<TestCommand>)} before");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");
        invokerRecorder.Messages[2].Should().Be($"{nameof(TestMiddleware<TestCommand>)} after");
        command.Handled.Should().BeTrue();
    }
    
    [Fact]
    public void AddNotMiddlewareType_AsMiddleware_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(commandConfiguration: c =>
            c.AddGlobalMiddleware(typeof(TestCommand)));

        act.Should().ThrowExactly<ArgumentException>();
    }
}