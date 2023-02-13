using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Applications.Commands;
using StarDust.CasparCG.net.RestApi.Applications.Queries;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Services;
using CasparModels = StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Controllers
{
    [ApiController]
    [Route("api/servers")]
    public class ServersController : ControllerBase
    {
        private readonly ServerConnectionManager _serverConnectionManager;
        private readonly IMediator _mediator;

        public ServersController(ServerConnectionManager serverConnectionManager, IMediator mediator)
        {
            _mediator = mediator;
            _serverConnectionManager = serverConnectionManager;
        }

        [HttpGet]
        public async Task<IEnumerable<CasparCGServerDto>> ListServers([FromQuery] int pageIndex = 0,[FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var servers = await _mediator.Send(new GetServersQuery(pageIndex, pageSize), cancellationToken);
            return servers.Select(e=> new CasparCGServerDto{Hostname = e.Hostname, Id = e.Id, Name = e.Name});
        }

        [HttpGet("status")]
        public async Task<IEnumerable<CasparCGServerStatusDto>> GetServersStatus()
        {
           return await _mediator.Send(new GetServersStatusQuery());
        }

        [HttpPost]
        public async Task CreateServer(CreateCasparCGServerRequestDto request, CancellationToken cancellationToken){
            await _mediator.Send(new CreateCasparCGServerRequest(request.Id, request.Hostname, request.Name), cancellationToken);
        }

        [HttpDelete("{serverId}")]
        public async Task DeleteServer(Guid serverId, CancellationToken token)
        {
            await _mediator.Send(new DeleteCasparCgServerRequest(serverId));
        }

        [HttpPost("{serverId}/channels/{channelId}/layers/{layerId}/load-background")]
        public async Task LoadBg(Guid serverId, int channelId, uint layerId,[FromQuery]string clipName,[FromQuery] bool auto = false, CancellationToken token = default)
        {
            var casparCg = await _serverConnectionManager[serverId];
            await Task.Run(()=> {
                casparCg.Channels[channelId-1].LoadBG(new CasparModels.Media.CasparPlayingInfoItem(layerId, clipName), auto);             
            }, token);
        }

        [HttpPost("{serverId}/channels/{channelId}/layers/{layerId}/play")]
        public async Task Play(Guid serverId, int channelId, uint layerId, CancellationToken token)
        {
            var casparCg = await _serverConnectionManager[serverId];
            await Task.Run(()=> {
                casparCg.Channels[channelId-1].Play(1);              
            }, token);
        }
    }
}