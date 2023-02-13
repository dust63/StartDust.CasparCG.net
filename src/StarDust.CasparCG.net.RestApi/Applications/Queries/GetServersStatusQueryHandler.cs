using System.Runtime.CompilerServices;
using MediatR;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public class GetServersStatusQueryHandler : IRequestHandler<GetServersStatusQuery, IEnumerable<CasparCGServerStatusDto>>
{
    private readonly IMediator _mediator;
    private readonly ServerConnectionManager _connectionManager;

    public GetServersStatusQueryHandler(IMediator mediator, ServerConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
        _mediator = mediator;
    }

    public async Task<IEnumerable<CasparCGServerStatusDto>> Handle(GetServersStatusQuery request, CancellationToken cancellationToken)
    {
        return await Handleinternal(request).ToListAsync(cancellationToken);
    }

    public async IAsyncEnumerable<CasparCGServerStatusDto> Handleinternal(GetServersStatusQuery request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var servers = await _mediator.Send(new GetServersQuery(0, int.MaxValue), cancellationToken);
        foreach (var server in servers.Select(x => new CasparCGServerStatusDto { Id = x.Id, Hostname = x.Hostname, Name = x.Name }))
        {
            try
            {
                server.IsListening = CasparCGConnectivityControl.CheckAmqpResponse(server.Hostname!);
                server.IsInstantiated = _connectionManager.IsServerInstantiated(server.Id);
                if (server.IsInstantiated)
                    server.IsConnected = (await _connectionManager[server.Id])?.IsConnected ?? false;
            }
            catch
            {
                // Avoid connectivity error
            }
            yield return server;

        }
    }
}
