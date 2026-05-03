namespace Ecommerce.Shared.Core.Messaging
{
    public interface ICommand<TResponse> { }
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
    }
}
