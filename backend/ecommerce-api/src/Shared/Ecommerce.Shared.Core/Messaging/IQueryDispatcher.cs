namespace Ecommerce.Shared.Core.Messaging
{
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
    }
}
