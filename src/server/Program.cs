using IOGameServer.Application.Services;
using IOGameServer.Application.Settings;
using IOGameServer.Hubs;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddSignalR(options =>
    {
        options.MaximumReceiveMessageSize = 1024;
    })
    .AddMessagePackProtocol();

builder.Configuration.AddJsonFile("gamesettings.json");
builder.Services
    .AddOptions<GameSettings>()
    .Bind(builder.Configuration.GetRequiredSection(nameof(GameSettings)))
    .ValidateOnStart();

builder.Services.AddHostedService<ClientUpdaterHostedService>();
builder.Services.AddSingleton<GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();

app.MapHub<GameHub>("/gameHub", options =>
{
    options.TransportMaxBufferSize = 1024;
    options.ApplicationMaxBufferSize = 1024;
    options.Transports = HttpTransportType.WebSockets;
});

app.Run();
