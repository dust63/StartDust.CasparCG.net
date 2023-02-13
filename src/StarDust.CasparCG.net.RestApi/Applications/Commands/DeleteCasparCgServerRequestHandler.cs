using MediatR;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Applications.Events;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Applications.Commands;

public class DeleteCasparCgServerRequestHandler : IRequestHandler<DeleteCasparCgServerRequest, Unit>
{
    private readonly IDbConnectionFactory _dbConnection;
    private readonly IMediator _mediator;
    public DeleteCasparCgServerRequestHandler(IDbConnectionFactory dbConnection, IMediator mediator)
    {
        _mediator = mediator;
        _dbConnection = dbConnection;
    }

    public async Task<Unit> Handle(DeleteCasparCgServerRequest request, CancellationToken cancellationToken)
    {
        using var db = _dbConnection.OpenDbConnection();
        await db.DeleteAsync<CasparCGServer>(request.ServerId, token: cancellationToken);
        await _mediator.Publish(new CasparCGServerDeleted(request.ServerId), cancellationToken);
        return Unit.Value;
    }
}
