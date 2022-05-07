using DotBootstrap.Messaging.Commands.CommandPipelines;

namespace DotBootstrap.Messaging.Commands;

internal static class TypeValidationExtensions
{
    internal static void IsCommandPreprocessor(this Type type)
    {
        var ina = type.GetInterfaces().Single();
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(ICommandPreprocessor<>)))
        {
            throw new ArgumentException($"{type.Name} is not a command preprocessor");
        }
    }
    
    internal static void IsCommandPostprocessor(this Type type)
    {
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(ICommandPostprocessor<>)))
        {
            throw new ArgumentException($"{type.Name} is not a command postprocessor");
        }
    }
    
    internal static void IsCommandMiddleware(this Type type)
    {
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(ICommandMiddleware<>)))
        {
            throw new ArgumentException($"{type.Name} is not a command postprocessor");
        }
    }
}