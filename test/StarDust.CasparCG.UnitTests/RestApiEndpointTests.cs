using Microsoft.AspNetCore.Builder;
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
        var builder = WebApplication.CreateSlimBuilder();

        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();

        app.MapCasparCGApi();

        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .ToArray();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/version", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /data/{key}", StringComparison.Ordinal) == true);
    }
}
