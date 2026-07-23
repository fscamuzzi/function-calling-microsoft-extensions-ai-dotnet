using ToolCallingDemo.Endpoints;
using ToolCallingDemo.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAssistant(builder.Configuration);

var app = builder.Build();

app.MapAssistantEndpoints();

app.Run();
