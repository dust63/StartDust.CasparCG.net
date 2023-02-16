using StarDust.CasparCG.net.Microsoft.DependencyInjections;
using StarDust.CasparCG.net.RestApi.Services;
using MediatR;
using StarDust.CasparCG.net.RestApi.Data;
using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Startdust CasparCG Api v1 Documentation",
        Version = "v1",
        License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/license/mit/") },
        Description = "Provide rest api endpoint for CasparCG Server. More info on http://casparcg.com/",
    });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Application configuration
builder.Services.AddCasparCG();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddData(builder.Configuration);
builder.Services.AddSingleton<CasparCGConnectionManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Startdust CasparCG Api v1 Documentation");
        options.RoutePrefix = "swagger";
    });
    app.UseReDoc(options =>
  {
      options.DocumentTitle = "Startdust CasparCG Api v1 Documentation";
      options.SpecUrl = "/swagger/v1/swagger.json";
      options.RoutePrefix = "redoc";
  });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
