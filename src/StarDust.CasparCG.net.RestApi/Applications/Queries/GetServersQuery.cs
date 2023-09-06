using MediatR;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public record GetServersQuery(int pageIndex, int pageSize): IRequest<List<CasparCGServer>>;
