using MediatR;

namespace StarDust.CasparCG.net.RestApi.Applications.Events;

public record CasparCGServerDeleted(Guid serverId) : INotification;
