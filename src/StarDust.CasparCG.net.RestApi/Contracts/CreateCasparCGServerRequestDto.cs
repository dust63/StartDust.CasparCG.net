namespace StarDust.CasparCG.net.RestApi.Contracts;

public class CreateCasparCGServerRequestDto
{
    public required Guid Id{get;set;}

    public required string Hostname{get;set;}

    public string? Name{get;set;}
}