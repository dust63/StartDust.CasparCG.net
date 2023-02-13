namespace StarDust.CasparCG.net.RestApi.Exceptions;

public class AmqpServerDidNotResponse : InvalidOperationException
{   
    public AmqpServerDidNotResponse(string hostname):base($"The CasparCG server is not running on host: {hostname}.")
    {        
    }
}