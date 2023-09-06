using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Controllers
{
    public class MixerController : BaseCasparCGController
    {
        public MixerController(IMediator mediator, CasparCGConnectionManager serverConnectionManager) : base(mediator, serverConnectionManager)
        {
        }

        /// <summary>
        /// Removes the template from the specified layer.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId">The video layer Id to interact with the Character Generator</param>
        /// <param name="volume">volume to set on the layer</param>
        /// <param name="deffer"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/mixer/volume/{volume}")]
        public async Task Clear(
            [FromRoute] Guid serverId,
            [FromRoute] int channelId,
            [FromRoute] int videoLayerId,
            [FromRoute] float volume,
            [FromQuery] bool deffer = false){
            var channel = await GetChannel(serverId, channelId);
            channel.MixerManager.MasterVolume(videoLayerId, volume, deffer);
        }
    }
}