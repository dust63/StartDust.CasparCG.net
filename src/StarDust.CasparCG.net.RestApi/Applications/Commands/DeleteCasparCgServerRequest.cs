using MediatR;

namespace StarDust.CasparCG.net.RestApi.Applications.Commands;

public record DeleteCasparCgServerRequest(Guid ServerId) : IRequest<Unit>;
