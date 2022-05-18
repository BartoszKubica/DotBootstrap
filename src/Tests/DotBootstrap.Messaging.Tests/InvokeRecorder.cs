using System.Collections.Generic;

namespace DotBootstrap.Messaging.Tests;

public class InvokeRecorder
{
    public IList<string> Messages { get; } = new List<string>();
}