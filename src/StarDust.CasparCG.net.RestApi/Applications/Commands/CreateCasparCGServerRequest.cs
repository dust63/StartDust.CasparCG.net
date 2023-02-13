using MediatR;

namespace StarDust.CasparCG.net.RestApi.Applications.Commands;

public record CreateCasparCGServerRequest(Guid ServerId, string Hostname, string? Name) : IRequest<Guid>;
