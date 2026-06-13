namespace Ecommerce.Shared.Core.Messaging;

public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
}
