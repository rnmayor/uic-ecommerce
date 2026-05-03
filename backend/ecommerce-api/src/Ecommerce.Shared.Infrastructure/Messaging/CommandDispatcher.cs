using Ecommerce.Shared.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Shared.Infrastructure.Messaging
{
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.HandleAsync((dynamic)command, ct);
        }
    }
}
