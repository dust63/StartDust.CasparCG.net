using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.RestApi.Applications.Commands;
using StarDust.CasparCG.net.RestApi.Applications.Queries;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Controllers;

[ApiController]
[Route("api/servers")]
public class ServersController : BaseCasparCGController
{ 
    public ServersController(IMediator mediator, CasparCGConnectionManager serverConnectionManager) : base(mediator, serverConnectionManager)
    {
    }

    /// <summary>
    /// Get a list of server connections
    /// </summary>
    /// <param name="pageIndex">Page index to retrieve</param>
    /// <param name="pageSize">Number of elements per page</param>
    /// <param name="cancellationToken">to cancel running task</param>
    /// <returns></returns>
    [HttpGet("connection-info")]
    public async Task<IEnumerable<CasparCGServerDto>> ListServers([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var servers = await _mediator.Send(new GetServersQuery(pageIndex, pageSize), cancellationToken);
        return servers.Select(e => new CasparCGServerDto { Hostname = e.Hostname, Id = e.Id, Name = e.Name });
    }

    /// <summary>
    /// Get the status of all the registered servers 
    /// </summary>
    /// <returns></returns>
    [HttpGet("status")]
    public async Task<IEnumerable<CasparCGServerStatusDto>> GetServersStatus()
    {
        return await _mediator.Send(new GetServersStatusQuery());
    }

    /// <summary>
    /// Add a server connection
    /// </summary>
    /// <param name="request">information to call CasparCG server</param>
    /// <param name="cancellationToken">to cancel running task</param>
    /// <returns></returns>
    [HttpPost("connection-info")]
    public async Task CreateServer(CreateCasparCGServerRequestDto request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateCasparCGServerRequest(request.Id, request.Hostname, request.Name), cancellationToken);
    }

    /// <summary>
    /// Remove server connection information from database
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="cancellationToken">to cancel running task</param>
    /// <returns></returns>
    [HttpDelete("{serverId}/connection-info")]
    public async Task DeleteServer(Guid serverId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCasparCgServerRequest(serverId), cancellationToken);
    }    
}