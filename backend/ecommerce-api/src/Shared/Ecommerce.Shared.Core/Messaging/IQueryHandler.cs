namespace Ecommerce.Shared.Core.Messaging
{
    public interface IQuery<TResponse> { }
    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query, CancellationToken ct);
    }
}
