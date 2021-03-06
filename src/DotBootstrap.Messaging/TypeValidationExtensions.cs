using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Queries.QueryPipelines;

namespace DotBootstrap.Messaging;

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
    
    internal static void IsQueryPreprocessor(this Type type)
    {
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(IQueryPreprocessor<,>)))
        {
            throw new ArgumentException($"{type.Name} is not a query preprocessor");
        }
    }
    
    internal static void IsQueryPostprocessor(this Type type)
    {
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(IQueryPostprocessor<,>)))
        {
            throw new ArgumentException($"{type.Name} is not a query postprocessor");
        }
    }
    
    internal static void IsQueryMiddleware(this Type type)
    {
        if (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .All(i => i.GetGenericTypeDefinition() != typeof(IQueryMiddleware<,>)))
        {
            throw new ArgumentException($"{type.Name} is not a query middleware");
        }
    }
}