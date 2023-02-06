using MediatR;

namespace StarDust.CasparCG.net.RestApi.Requests;

public class CreateCasparCGServerRequest : IRequest<Guid>
{
    public CreateCasparCGServerRequest(Guid id, string hostname, string? name)
    {
        Id = id;
        Hostname = hostname;
        Name = name;
    }
    public Guid Id { get; }

    public string Hostname { get; }

    public string? Name { get; }
}