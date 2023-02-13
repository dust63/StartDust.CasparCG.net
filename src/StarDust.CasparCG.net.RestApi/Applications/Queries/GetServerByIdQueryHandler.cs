using System;
using MediatR;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public class GetServerByIdQueryHandler : IRequestHandler<GetServerByIdQuery, CasparCGServer>
{
    private readonly IDbConnectionFactory _dbConnection;

    public GetServerByIdQueryHandler(IDbConnectionFactory dbConnection)
    {
        _dbConnection = dbConnection;
    }
    
    public Task<CasparCGServer> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
    {
       using var db = _dbConnection.OpenDbConnection();
       return db.SingleByIdAsync<CasparCGServer>(request.ServerId, cancellationToken);
    }
}
