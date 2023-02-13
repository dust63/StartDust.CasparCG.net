using MediatR;

namespace StarDust.CasparCG.net.RestApi.Applications.Events;

public record CasparCGServerCreated(Guid serverId) : INotification;
