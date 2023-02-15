namespace StarDust.CasparCG.net.RestApi.Contracts;

public class CasparCGServerStatusDto
{
    public Guid Id{get;set;}

    public string? Name{get;set;}

    public string? Hostname{get;set;}   

    public bool IsListening{get;set;}

    public bool IsInstantiated { get; set; }

    public bool IsConnected{get;set;}    
}