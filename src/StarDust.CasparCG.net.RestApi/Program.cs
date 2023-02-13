using StarDust.CasparCG.net.Microsoft.DependencyInjections;
using StarDust.CasparCG.net.RestApi.Services;
using MediatR;
using StarDust.CasparCG.net.RestApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application configuration
builder.Services.AddCasparCG();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddData(builder.Configuration);
builder.Services.AddSingleton<ServerConnectionManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
