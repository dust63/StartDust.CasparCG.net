using MediatR;
using StarDust.CasparCG.net.RestApi.Contracts;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public record GetServersStatusQuery(): IRequest<IEnumerable<CasparCGServerStatusDto>>;