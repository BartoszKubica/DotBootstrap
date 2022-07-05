# DotBootstrap

Project contains infrastructure code for CQRS, DDD and persistance of data.

## Messaging
`IServiceCollection.AddMessaging(ICommandPipelineConfigurator commandConfiguration, IQueryPipelineConfigurator queryConfiguration)` extension method allows to register all required services. Method accepts delegates which allows to configure pipeline items. Pipeline items can be add globally to every command/query or it can be add to specific command or query. It is also required to use `IServiceCollection.AddDotBootstrap` method to register necessary services. Commands and query can be dispatched by `ICommandQueryDispatcher`


### Commands
All commands should implement `ICommand` interface. When command is dispatched, proper class with implementation of interface `ICommandHandler<TCommand>` will handle correlated command.
Library provides building blocks for building pipelines which handles command flow. 
There are 3 types of pipeline item:
- `ICommandPreprocessor` which is triggered before concrete command handler
- `ICommandPostprocessor` which is triggered after concrete command handler
- `ICommandMiddleware` which is triggered before and after concrete command handler

It can be only for command handler for specific command.
### Queries
All queries should implement `IQuery<TResponse>` interface. When query is dispatched, proper class with implementation of interface `IQuery<TQuery, TResponse>` will handle correlated query.
Library provides building blocks for building pipelines which handles command flow. 
There are 3 types of pipeline item:
- `IQueryPreprocessor` which is triggered before concrete command handler
- `IQueryPostprocessor` which is triggered after concrete command handler
- `IQueryMiddleware` which is triggered before and after concrete command handler

It can be only one query handler for specific query.

### Events
All events should implement `IEvent` interface. Event can be dispatched by `IEventBus`. When event is dispatched, proper class with implementation of interface `IEventHandler<TEvent>` will handle correlated event.

There can be many event handlers for specific event.

