using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.Protocol.Amcp;
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

    [Theory]
    [InlineData(400, StatusCodes.Status400BadRequest)]
    [InlineData(401, StatusCodes.Status400BadRequest)]
    [InlineData(402, StatusCodes.Status400BadRequest)]
    [InlineData(403, StatusCodes.Status400BadRequest)]
    [InlineData(404, StatusCodes.Status404NotFound)]
    [InlineData(500, StatusCodes.Status502BadGateway)]
    [InlineData(501, StatusCodes.Status502BadGateway)]
    [InlineData(502, StatusCodes.Status502BadGateway)]
    [InlineData(503, StatusCodes.Status403Forbidden)]
    [InlineData(504, StatusCodes.Status429TooManyRequests)]
    [InlineData(600, StatusCodes.Status501NotImplemented)]
    public void Amcp_command_failures_map_to_http_status_and_extensions(int amcpStatusCode, int expectedHttpStatus)
    {
        var response = new AmcpResponse
        {
            StatusCode = amcpStatusCode,
            Category = amcpStatusCode >= 500 ? AmcpStatusCategory.ServerError : AmcpStatusCategory.ClientError,
            CommandText = "PLAY",
            StatusLine = $"{amcpStatusCode} PLAY FAILED",
            Lines = [],
            Raw = $"{amcpStatusCode} PLAY FAILED\r\n"
        };

        var problem = CasparProblemDetailsFactory.FromException(new AmcpCommandException(response));

        Assert.Equal(expectedHttpStatus, problem.Status);
        Assert.Equal(amcpStatusCode, problem.Extensions["amcpStatusCode"]);
        Assert.Equal($"{amcpStatusCode} PLAY FAILED", problem.Extensions["amcpStatusLine"]);
        Assert.Equal("PLAY", problem.Extensions["amcpCommandText"]);
        Assert.Equal(response.Category.ToString(), problem.Extensions["amcpCategory"]);
    }
}
