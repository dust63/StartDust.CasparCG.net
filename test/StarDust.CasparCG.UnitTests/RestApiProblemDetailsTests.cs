using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiProblemDetailsTests
{
    [Fact]
    public void Caspar_transport_errors_map_to_bad_gateway()
    {
        var problem = CasparProblemDetailsFactory.FromException(new IOException("boom"));

        Assert.Equal(StatusCodes.Status502BadGateway, problem.Status);
        Assert.Equal("CasparCG upstream failure", problem.Title);
        Assert.Equal("boom", problem.Detail);
    }
}
