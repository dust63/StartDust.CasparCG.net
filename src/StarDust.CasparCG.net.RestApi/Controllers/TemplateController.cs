using MediatR;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplateController : BaseCasparCGController
    {
        public TemplateController(IMediator mediator, CasparCGConnectionManager serverConnectionManager) : base(mediator, serverConnectionManager)
        {
        }

        /// <summary>
        /// Prepares a template for displaying. It won't show until you call CG PLAY (unless you supply the play-on-load flag, 1 for true).
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <param name="cglayerId"></param>
        /// <param name="templateName"></param>
        /// <param name="autoPlay"></param>
        /// <param name="dataStoreName"></param> 
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/{cglayerId}/add/{templateName}")]
        public async Task Add(
            Guid serverId,
            int channelId,
            int videoLayerId,
            uint cglayerId,
            string templateName,
            [FromQuery] bool autoPlay = false,
            [FromQuery] string? dataStoreName = null
            )
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Add(videoLayerId, cglayerId, templateName, autoPlay, dataStoreName);
        }

        /// <summary>
        /// Plays and displays the template in the specified layer.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <param name="cglayerId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/{cglayerId}/play")]
        public async Task Play(
            Guid serverId,
            int channelId,
            int videoLayerId,
            uint cglayerId)
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Play(videoLayerId, cglayerId);
        }

        /// <summary>
        /// Stops and removes the template from the specified layer. This is different from REMOVE in that the template gets a chance to animate out when it is stopped.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <param name="cglayerId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/{cglayerId}/stop")]
        public async Task Stop(
            Guid serverId,
            int channelId,
            int videoLayerId,
            uint cglayerId)
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Stop(videoLayerId, cglayerId);
        }

        /// <summary>
        /// Stops and removes the template from the specified layer. This is different from REMOVE in that the template gets a chance to animate out when it is stopped.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <param name="cglayerId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/{cglayerId}/next")]
        public async Task Next(
            Guid serverId,
            int channelId,
            int videoLayerId,
            uint cglayerId)
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Next(videoLayerId, cglayerId);
        }

        /// <summary>
        /// Removes the template from the specified layer.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <param name="cglayerId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/{cglayerId}/remove")]
        public async Task Remove(
            Guid serverId,
            int channelId,
            int videoLayerId,
            uint cglayerId)
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Remove(videoLayerId, cglayerId);
        }

         /// <summary>
        /// Removes the template from the specified layer.
        /// </summary>
        /// <param name="serverId">The server connection Id to use</param>
        /// <param name="channelId">The channel Id on the CasparCG Server</param>
        /// <param name="videoLayerId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{serverId}/channels/{channelId}/layers/{videoLayerId}/cg/clear")]
        public async Task Clear(
            Guid serverId,
            int channelId,
            int videoLayerId)
        {
            var channel = await GetChannel(serverId, channelId);
            channel.CG.Clear(videoLayerId);
        }
    }
}