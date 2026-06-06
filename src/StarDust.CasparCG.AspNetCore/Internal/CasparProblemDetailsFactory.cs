using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.AspNetCore.Internal;

/// <summary>
/// Creates <see cref="ProblemDetails"/> payloads for REST API failures.
/// </summary>
public static class CasparProblemDetailsFactory
{
    /// <summary>
    /// Maps an exception into a <see cref="ProblemDetails"/> payload.
    /// </summary>
    /// <param name="exception">The source exception.</param>
    /// <returns>The mapped problem details.</returns>
    public static ProblemDetails FromException(Exception exception) =>
        exception switch
        {
            AmcpCommandException amcpException => FromAmcpResponse(amcpException.Response, amcpException.Message),
            IOException ioException => new ProblemDetails
            {
                Title = "CasparCG upstream failure",
                Status = StatusCodes.Status502BadGateway,
                Detail = ioException.Message
            },
            InvalidOperationException invalidOperationException => new ProblemDetails
            {
                Title = "CasparCG client is unavailable",
                Status = StatusCodes.Status503ServiceUnavailable,
                Detail = invalidOperationException.Message
            },
            _ => new ProblemDetails
            {
                Title = "CasparCG request failed",
                Status = StatusCodes.Status503ServiceUnavailable,
                Detail = exception.Message
            }
        };

    private static ProblemDetails FromAmcpResponse(AmcpResponse response, string detail)
    {
        var problem = new ProblemDetails
        {
            Title = "CasparCG upstream failure",
            Status = MapHttpStatus(response.StatusCode),
            Detail = detail
        };

        problem.Extensions["amcpStatusCode"] = response.StatusCode;
        problem.Extensions["amcpStatusLine"] = response.StatusLine;
        problem.Extensions["amcpCommandText"] = response.CommandText;
        problem.Extensions["amcpCategory"] = response.Category.ToString();
        return problem;
    }

    private static int MapHttpStatus(int amcpStatusCode) =>
        amcpStatusCode switch
        {
            400 or 401 or 402 or 403 => StatusCodes.Status400BadRequest,
            404 => StatusCodes.Status404NotFound,
            500 or 501 or 502 => StatusCodes.Status502BadGateway,
            503 => StatusCodes.Status403Forbidden,
            504 => StatusCodes.Status429TooManyRequests,
            600 => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status502BadGateway
        };
}
