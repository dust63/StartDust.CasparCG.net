using MediatR;

namespace StarDust.CasparCG.net.RestApi.Requests;

public class DeleteCasparCgServerRequest : IRequest<Unit>
{
    public DeleteCasparCgServerRequest(Guid id)
    {
        this.Id = id;
    }
    
    public Guid Id{get;}
}