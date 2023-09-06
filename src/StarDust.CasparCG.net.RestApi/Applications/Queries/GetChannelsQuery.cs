using MediatR;
using StarDust.CasparCG.net.RestApi.Contracts;

namespace StarDust.CasparCG.net.RestApi.Applications.Queries;

public record GetChannelsQuery(Guid ServerId, int PageIndex, int PageSize) : IRequest<IList<ChannelDto>>;
