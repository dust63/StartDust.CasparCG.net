using MediatR;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public record GetServerByIdQuery(Guid ServerId) : IRequest<CasparCGServer>;
