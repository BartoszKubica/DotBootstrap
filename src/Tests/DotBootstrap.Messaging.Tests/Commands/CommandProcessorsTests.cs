using System;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Commands;

public class CommandProcessorTests
{
    [Fact]
    public async Task Send_WithGlobalPreprocessorsSet_ShouldInvokePreprocessor()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddGlobalPreprocessor(typeof(TestPreprocessor<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();


        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        await commandBus.Send(command);

        invokerRecorder.Messages[0].Should().Be($"{command.GetType()} {nameof(TestPreprocessor<TestCommand>)}");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");
        command.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithCommandPreprocessorsSet_ShouldInvokePreprocessorOnlyForGivenCommand()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddPreprocessorForCommand<TestCommand>(typeof(TestPreprocessor<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        serviceCollection.RegisterCommandHandler<TestCommand2, TestCommandHandler2>();

        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();


        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        var command2 = new TestCommand2();
        await commandBus.Send(command);
        await commandBus.Send(command2);

        invokerRecorder.Messages[0].Should().Be($"{command.GetType()} {nameof(TestPreprocessor<TestCommand>)}");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");

        command.Handled.Should().BeTrue();
        command2.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithCommandPostprocessorsSet_ShouldInvokePostprocessorOnlyForGivenCommand()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddPostprocessorForCommand<TestCommand>(typeof(TestPostprocessor<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        serviceCollection.RegisterCommandHandler<TestCommand2, TestCommandHandler2>();

        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        var command2 = new TestCommand2();
        await commandBus.Send(command);
        await commandBus.Send(command2);

        invokerRecorder.Messages[0].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestPostprocessor<TestCommand>)}");

        command.Handled.Should().BeTrue();
        command2.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithGlobalPostprocessorsSet_ShouldInvokePreprocessor()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(c =>
            c.AddGlobalPostprocessor(typeof(TestPostprocessor<>)));
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        await commandBus.Send(command);

        invokerRecorder.Messages[0].Should().Be($"{command.GetType()} {nameof(TestCommandHandler)}");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestPostprocessor<TestCommand>)}");

        command.Handled.Should().BeTrue();
    }

    [Fact]
    public void AddNotPreprocessorType_AsPreprocessor_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(c =>
            c.AddGlobalPreprocessor(typeof(TestCommand)));

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void AddNotPostprocessorType_AsPostprocessor_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(c =>
            c.AddGlobalPostprocessor(typeof(TestCommand)));

        act.Should().ThrowExactly<ArgumentException>();
    }
}