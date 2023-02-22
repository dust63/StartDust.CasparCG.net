using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Media;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Controllers;

[ApiController]
[Route("api/channels")]
public class ChannelsController : BaseCasparCGController
{
    public ChannelsController(IMediator mediator, CasparCGConnectionManager serverConnectionManager) : base(mediator, serverConnectionManager)
    {
    }

    /// <summary>
    /// Loads a producer in the background and prepares it for playout. If no layer is specified the default layer index will be used.
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/servers/{serverId}/channels/{channelId}/load-background")]
    public async Task LoadBg(
        Guid serverId,
        int channelId,
        [FromBody] LoadBgRequestDto request
        )
    {
        var channel = await GetChannel(serverId, channelId);
        var clipInfo = new CasparPlayingInfoItem(request.LayerId, request.ClipName, request.Transition)
        {
            Seek = request.StartAtMs is null || request.FrameRate is null ? null : (uint)((request.StartAtMs / 1000) * request.FrameRate),
            Length = request.DurationInMs is null || request.FrameRate is null ? null : (uint)((request.DurationInMs / 1000) * request.FrameRate)
        };
        await channel.LoadBGAsync(clipInfo, request.Auto);
    }

    /// <summary>
    /// Loads a producer in the background and prepares it for playout. If no layer is specified the default layer index will be used.
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/servers/{serverId}/channels/{channelId}/load")]
    public async Task Load(
        Guid serverId,
        int channelId,
        [FromBody] LoadRequestDto request
        )
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.LoadAsync(new CasparPlayingInfoItem(request.LayerId, request.ClipName, request.Transition));
    }

    /// <summary>
    /// Moves clip from background to foreground and starts playing it.
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="layerId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/play")]
    public async Task Play(Guid serverId, int channelId, uint layerId, CancellationToken token)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.PlayAsync(layerId);
    }

    /// <summary>
    /// Pauses playback of the foreground clip on the specified layer. The RESUME command can be used to resume playback again.
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <param name="layerId">Id of the layer to pause</param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/pause")]
    public async Task Pause(Guid serverId, int channelId, uint layerId)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.PauseAsync(layerId);
    }

    /// <summary>
    /// Resumes playback of a foreground clip previously paused with the PAUSE command.
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <param name="layerId">Id of the layer to resume</param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/resume")]
    public async Task Resume(Guid serverId, int channelId, uint layerId)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.ResumeAsync(layerId);
    }

    /// <summary>
    /// Moves clip from background to foreground and starts playing it. If a transition is prepared, it will be executed.
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/servers/{serverId}/channels/{channelId}/play")]
    public async Task Play(Guid serverId, int channelId, PlayRequestDto request)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.PlayAsync(new CasparPlayingInfoItem(request.LayerId, request.ClipName, request.Transition));
    }

    /// <summary>
    /// Removes the foreground clip of the specified layer
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <param name="layerId">Id of the layer to stop</param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/stop")]
    public async Task Stop(Guid serverId, int channelId, int layerId)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.StopAsync(layerId);
    }

    /// <summary>
    /// Removes all clips (both foreground and background) on all layer of the channel
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/clear")]
    public async Task Clear(Guid serverId, int channelId)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.ClearAsync();
    }

    /// <summary>
    /// Removes all clips (both foreground and background) of the specified layer
    /// </summary>
    /// <param name="serverId">Id of the server connection</param>
    /// <param name="channelId">Id of the channel on the CasparCG server</param>
    /// <param name="layerId">Id of the layer to stop</param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/clear")]
    public async Task Clear(Guid serverId, int channelId, int layerId)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.ClearAsync(layerId);
    }

    /// <summary>
    /// Calls method on the specified producer with the provided param string.
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="layerId"></param>
    /// <param name="loop"></param>
    /// <param name="seek"></param>
    /// <returns></returns>
    [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{layerId}/call")]
    public async Task Call(Guid serverId, int channelId, uint layerId, bool? loop, int? seek)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.CallAsync(layerId, loop, seek);
    }

    /// <summary>
    /// Adds a consumer to the specified <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/servers/{serverId}/channels/{channelId}/consumers/")]
    public async Task AddConsumer(Guid serverId, int channelId, AddConsumerRequestDto request)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.AddAsync(request.Consumer, request.Id, request.Parameters);
    }

    /// <summary>
    /// Removes an existing consumer from <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpDelete("/servers/{serverId}/channels/{channelId}/consumers")]
    public async Task AddConsumer(Guid serverId, int channelId, RemoveConsumerRequestDto requestDto)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.RemoveAsync(requestDto.Consumer, requestDto.Parameters);
    }

    /// <summary>
    /// Set the video mode for the specified channel <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="videoMode">Set the video format of the channel</param>
    /// <returns></returns>
    [HttpPut("/servers/{serverId}/channels/{channelId}/video-mode/{videoMode}")]
    public async Task SetMode(Guid serverId, int channelId, VideoMode videoMode)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.SetModeAsync(videoMode);
    }

    /// <summary>
    /// Set the video mode for the specified channel <paramref name="channelId"/>
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="layout">Set the audio layout</param>
    /// <returns></returns>
    [HttpPut("/servers/{serverId}/channels/{channelId}/channel-layout/{layout}")]
    public async Task SetChannelLayout(Guid serverId, int channelId, ChannelLayout layout)
    {
        var channel = await GetChannel(serverId, channelId);
        await channel.SetChannelLayoutAsync(layout);
    }

    /// <summary>
    /// Allows for exclusive access to a channel.
    /// </summary>
    /// <param name="serverId">The server connection Id to use</param>
    /// <param name="channelId">The channel Id on the CasparCG Server</param>
    /// <param name="lockAction">what type of lock action to run</param>
    /// <param name="lockPhrase">secret phrase for lock</param>
    /// <returns></returns>
    [HttpPut("/servers/{serverId}/channels/{channelId}/lock/{lockAction}")]
    public async Task Lock(Guid serverId, int channelId, LockAction lockAction, [FromQuery] string? lockPhrase = null)
    {
        var channel = await GetChannel(serverId, channelId);
        if (lockPhrase == null)
            await channel.LockAsync(lockAction);
        else
            await channel.LockAsync(lockAction, lockPhrase);
    }   
}
