using StarDust.CasparCG;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi(options =>
{
    options.MapAdminEndpoints = true;
});

var app = builder.Build();
await app.Services.GetRequiredService<CasparClient>().ConnectAsync();
app.MapCasparCGApi();

await app.RunAsync();
