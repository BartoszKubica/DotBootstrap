using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Commands;

public class CommandMessagingTests
{
    [Fact]
    public async Task Send_ShouldInvokeCommandHandler()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging();
        serviceCollection.RegisterCommandHandler<TestCommand, TestCommandHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var commandBus = sp.GetRequiredService<ICommandBus>();

        var command = new TestCommand();
        await commandBus.Send(command, CancellationToken.None);

        command.Handled.Should().BeTrue();
    }
}