using MediatR;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Applications.Events;
using StarDust.CasparCG.net.RestApi.Exceptions;
using StarDust.CasparCG.net.RestApi.Models;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Applications.Commands;

public class CreateCasparCGServerHandler : IRequestHandler<CreateCasparCGServerRequest, Guid>
{
    private readonly IDbConnectionFactory _dbConnection;
    private readonly IMediator _mediator;

    public CreateCasparCGServerHandler(IDbConnectionFactory dbConnection, IMediator mediator)
    {
        _dbConnection = dbConnection;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateCasparCGServerRequest request, CancellationToken cancellationToken)
    {       
        using var db = _dbConnection.OpenDbConnection();
        var server = new CasparCGServer(request.Hostname, request.Name);
        await db.InsertAsync<CasparCGServer>(server, token: cancellationToken);
        await _mediator.Publish(new CasparCGServerCreated(request.ServerId), cancellationToken);
        return server.Id;
    }
}