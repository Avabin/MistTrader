
using MediatR;

namespace MistTrader.Proxy.Commands;

public readonly record struct StartProxy(int Port) : IRequest;