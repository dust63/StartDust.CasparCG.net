using MediatR;
using StarDust.CasparCG.net.RestApi.Contracts;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery, IList<ChannelDto>>
{
    private readonly CasparCGConnectionManager _connectionManager;

    public GetChannelsQueryHandler(CasparCGConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task<IList<ChannelDto>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var server = await _connectionManager.GetorAddServerConnection(request.ServerId);
        return server!.Channels
            .Select(x=> new ChannelDto
            {
                Id = x.ID,
                Status = x.Status,
                VideoMode = x.VideoMode,
                ActiveClip = x.ActiveClip,
                Consumers = x.Output?.Consumers?.Select(c=> new ConsumerDto
                {
                    Type = c.Type,
                    Device = c.Device,
                    Windowed = c.Windowed,
                    Embeddedaudio = c.Embeddedaudio,
                    Lowlatency = c.Lowlatency,
                }).ToList()
            })
            .OrderBy(x => x.Id)            
            .Take(request.PageSize)
            .Skip(request.PageIndex * request.PageSize)
            .ToList();
    }
}