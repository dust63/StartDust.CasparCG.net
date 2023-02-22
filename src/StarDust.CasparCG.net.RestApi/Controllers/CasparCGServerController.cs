using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Exceptions;
using StarDust.CasparCG.net.RestApi.Services;

public abstract class CasparCGServerController : ControllerBase
{
    private readonly CasparCGConnectionManager _serverConnectionManager;
    protected readonly IMediator _mediator;

    protected CasparCGServerController(IMediator mediator, CasparCGConnectionManager serverConnectionManager)
    {
        _mediator = mediator;
        _serverConnectionManager = serverConnectionManager;
    }

    /// <summary>
    /// Get server connection. If no server found on db throw an exception
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <returns></returns>
    /// <exception cref="ServerNotFoundException"></exception>
    protected async Task<ICasparDevice> GetServer(Guid serverId)
    {
        var server = await _serverConnectionManager[serverId];
        if (server is null)
            throw new ServerNotFoundException(serverId);
        return server;
    }

    /// <summary>
    ///  Get channel by <paramref name="serverId"/>
    /// </summary>
    /// <param name="serverId">Id of the server</param>
    /// <param name="channelId">Id of the channel on the server</param>
    /// <returns>A CasparCG Channel</returns>
    /// <exception cref="ChannelNotFoundException">No channel found for the given <paramref name="channelId"/></exception>
    protected async Task<ChannelManager> GetChannel(Guid serverId, int channelId)
    {
        var server = await GetServer(serverId);
        var channel = server.Channels.FirstOrDefault(ch => ch.ID == channelId);
        if (channel is null)
            throw new ChannelNotFoundException(channelId);
        return channel;
    }
}

