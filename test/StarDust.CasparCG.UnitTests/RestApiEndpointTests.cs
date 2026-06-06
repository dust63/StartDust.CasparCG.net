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
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/info", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/info/config", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/info/paths", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /server/gl/info", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /server/gl/gc", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /media/files/{fileName}", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /fonts", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /templates", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /data", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /data/{key}", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("DELETE /data/{key}", StringComparison.Ordinal) == true);
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
    public void MapCasparCGApi_includes_basic_control_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/load", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/clear", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/swap", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/add", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/remove", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/apply", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/print", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/set", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/clear-all", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/clear", StringComparison.Ordinal) == true);
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

    [Fact]
    public void MapCasparCGApi_includes_first_mixer_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/keyer", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/invert", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/blend", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/opacity", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/brightness", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/saturation", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/contrast", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/volume", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/commit", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/clear", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/grid", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /mixer/master-volume", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void MapCasparCGApi_includes_free_form_mixer_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/chroma", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/levels", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/fill", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/clip", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/anchor", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/crop", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/rotation", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/perspective", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /channels/{channel}/layers/{layer}/mixer/grid", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void MapCasparCGApi_includes_admin_runtime_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /server/diag", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /admin/bye", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /admin/kill", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /admin/log-level", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("PUT /admin/log-level", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /admin/locks/{channel}/acquire", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /admin/locks/{channel}/release", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /admin/locks/{channel}/clear", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void MapCasparCGApi_includes_server_scoped_routes()
    {
        var endpoints = GetEndpoints();

        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /servers/{name}/server/version", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /servers/{name}/channels/{channel}/add", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("POST /servers/{name}/channels/{channel}/layers/{layer}/play", StringComparison.Ordinal) == true);
        Assert.Contains(endpoints, endpoint => endpoint.DisplayName?.Contains("GET /servers/{name}/events", StringComparison.Ordinal) == true);
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
