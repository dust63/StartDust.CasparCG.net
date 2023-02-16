using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Models.Media;
using StarDust.CasparCG.net.RestApi.Applications.Commands;
using StarDust.CasparCG.net.RestApi.Applications.Queries;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Exceptions;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Controllers;

[ApiController]
[Route("api/servers")]
public class ServersController : ControllerBase
{
    private readonly CasparCGConnectionManager _serverConnectionManager;
    private readonly IMediator _mediator;

    public ServersController(CasparCGConnectionManager serverConnectionManager, IMediator mediator)
    {
        _mediator = mediator;
        _serverConnectionManager = serverConnectionManager;
    }

    /// <summary>
    /// Get a list of server connections
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">to cancel running task</param>
    /// <returns></returns>
    [HttpGet("connection-info")]
    public async Task<IEnumerable<CasparCGServerDto>> ListServers([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var servers = await _mediator.Send(new GetServersQuery(pageIndex, pageSize), cancellationToken);
        return servers.Select(e => new CasparCGServerDto { Hostname = e.Hostname, Id = e.Id, Name = e.Name });
    }

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
    /// <param name="serverId"></param>
    /// <param name="cancellationToken">to cancel running task</param>
    /// <returns></returns>
    [HttpDelete("{serverId}/connection-info")]
    public async Task DeleteServer(Guid serverId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCasparCgServerRequest(serverId), cancellationToken);
    }

    /// <summary>
    /// Loads a producer in the background and prepares it for playout. If no layer is specified the default layer index will be used.
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{serverId}/channels/{channelId}/load-background")]
    public async Task LoadBg(
        Guid serverId,
        int channelId,
        [FromBody] LoadBGRequestDto request
        )
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.LoadBGAsync(new CasparPlayingInfoItem(request.LayerId, request.ClipName, request.Transition), request.Auto);
    }

    /// <summary>
    /// Moves clip from background to foreground and starts playing it.
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="layerId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("{serverId}/channels/{channelId}/layers/{layerId}/play")]
    public async Task Play(Guid serverId, int channelId, uint layerId, CancellationToken token)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.PlayAsync(layerId);
    }

    /// <summary>
    /// Moves clip from background to foreground and starts playing it. If a transition is prepared, it will be executed.
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{serverId}/channels/{channelId}/play")]
    public async Task Play(Guid serverId, int channelId, PlayClipRequestDto request)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.PlayAsync(new CasparPlayingInfoItem(request.LayerId, request.ClipName, request.Transition));
    }

    /// <summary>
    /// Calls method on the specified producer with the provided param string.
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="layerId"></param>
    /// <param name="loop"></param>
    /// <param name="seek"></param>
    /// <returns></returns>
    [HttpGet("{serverId}/channels/{channelId}/layers/{layerId}/call")]
    public async Task Call(Guid serverId, int channelId, uint layerId, bool? loop, int? seek)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.CallAsync(layerId, loop, seek);
    }

    /// <summary>
    /// Adds a consumer to the specified <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{serverId}/channels/{channelId}/consumers/")]
    public async Task AddConsumer(Guid serverId, int channelId, AddConsumerRequestDto request)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.AddAsync(request.Consumer, request.Id, request.Parameters);
    }

    /// <summary>
    /// Removes an existing consumer from <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="channelId"></param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpDelete("{serverId}/channels/{channelId}/consumers")]
    public async Task AddConsumer(Guid serverId, int channelId, RemoveConsumerRequestDto requestDto)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.RemoveAsync(requestDto.Consumer, requestDto.Parameters);
    }



    /// <summary>
    /// Get server connection. If no server found on db throw an exception
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
    /// <exception cref="ServerNotFoundException"></exception>
    private async Task<ICasparDevice> GetServer(Guid serverId)
    {
        var server = await _serverConnectionManager[serverId];
        if (server is null)
            throw new ServerNotFoundException(serverId);
        return server;
    }

    private async Task<ChannelManager> GetChannel(Guid serverId, int channelId)
    {
        var server = await GetServer(serverId);
        var channel = server.Channels.FirstOrDefault(ch => ch.ID == channelId);
        if (channel is null)
            throw new ChannelNotFoundException(channelId);
        return channel;
    }
}