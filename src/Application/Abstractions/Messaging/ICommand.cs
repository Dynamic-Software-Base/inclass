using MediatR;

namespace Application.Abstractions.Messaging;

// Marker interfaces (no MediatR inheritance)
public interface ICommand { }
public interface IPublicEndpoint { }

// Commands with response - inherit from marker + MediatR
public interface ICommand<out TResponse> : ICommand, IRequest<TResponse> { }
public interface IPublicCommand<out TResponse> : IPublicEndpoint, ICommand, IRequest<TResponse> { }

// Queries
public interface IQuery<out TResponse> : IRequest<TResponse> { }
public interface IPublicQuery<out TResponse> : IPublicEndpoint, IRequest<TResponse> { }
