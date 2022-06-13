using System.Collections.Generic;

namespace DotBootstrap.Persistence.IntegrationTests;

public class InvokeRecorder
{
    public IList<string> Messages { get; } = new List<string>();
}