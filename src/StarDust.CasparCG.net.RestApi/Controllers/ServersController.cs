using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Requests;
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

        [HttpGet("")]
        public IEnumerable<CasparCGServerDto> ListServers(){
            return _serverConnectionManager.GetServerList().Select(s=> new CasparCGServerDto{ Id = s.Id, Hostname = s.Hostname});
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
            var casparCg = _serverConnectionManager[serverId];
            await Task.Run(()=> {
                casparCg.Channels[channelId-1].LoadBG(new CasparModels.Media.CasparPlayingInfoItem(layerId, clipName), auto);             
            }, token);
        }

        [HttpPost("{serverId}/channels/{channelId}/layers/{layerId}/play")]
        public async Task Play(Guid serverId, int channelId, uint layerId, CancellationToken token)
        {
            var casparCg = _serverConnectionManager[serverId];
            await Task.Run(()=> {
                casparCg.Channels[channelId-1].Play(1);              
            }, token);
        }
    }
}