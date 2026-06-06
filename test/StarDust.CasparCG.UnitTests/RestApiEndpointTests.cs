using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiEndpointTests
{
    [Fact]
    public void MapCasparCGApi_maps_server_and_data_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/version", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /data/{key}", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void MapCasparCGApi_includes_cg_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/add", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/play", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/stop", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/next", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/remove", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/clear", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/update", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/cg/invoke", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void MapCasparCGApi_includes_thumbnail_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /thumbnails", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /thumbnails/{fileName}", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /thumbnails/{fileName}/generate", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /thumbnails/generate-all", StringComparison.Ordinal) == true);
    }

    private static Endpoint[] GetEndpoints()
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();

        app.MapCasparCGApi();

        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .ToArray();
    }
}
