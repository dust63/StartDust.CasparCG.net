using System;
using MediatR;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public class GetServersQueryHandler : IRequestHandler<GetServersQuery, List<CasparCGServer>>
{
    private readonly IDbConnectionFactory _dbConnection;

    public GetServersQueryHandler(IDbConnectionFactory dbConnection)
    {
        _dbConnection = dbConnection;
    }
    
    public Task<List<CasparCGServer>> Handle(GetServersQuery request, CancellationToken cancellationToken)
    {
       using var db = _dbConnection.OpenDbConnection();
       return db.SelectAsync<CasparCGServer>(db.From<CasparCGServer>(q=> q.Limit(skip: request.pageIndex * request.pageSize, rows: request.pageSize)));
    }
}
