using Ecommerce.Shared.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Shared.Infrastructure.Messaging
{
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.DispatchAsync(query, ct);
        }
    }
}
