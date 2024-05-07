using IOGameServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application.Services;

public sealed class ClientUpdaterHostedService(IHubContext<GameHub, IGameHub> hubContext, GameService gameService) : IHostedService, IDisposable
{
    private readonly GameService _gameService = gameService;
    private readonly IHubContext<GameHub, IGameHub> _hubContext = hubContext;
    private readonly CancellationTokenSource _stoppingCts = new();

    private PeriodicTimer Timer { get; init; } = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / 24));
    private Task ExecutingTask { get; set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ExecutingTask = Task.Run(async () =>
        {
            while (await Timer.WaitForNextTickAsync(_stoppingCts.Token))
            {
                await UpdateAllGames();
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (ExecutingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            _stoppingCts.Cancel();
        }
        finally
        {
            await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    private async Task UpdateAllGames()
    {
        foreach (var game in _gameService.Games.Values)
        {
            game.Update();

            await SendToAllPlayersOfGame(game);
            await HandleDeadPlayers(game);
        }
    }

    private async Task SendToAllPlayersOfGame(Game game)
    {
        foreach (var player in game.GetPlayers())
        {
            var playerUpdate = game.CreateUpdateJson(player);

            await _hubContext.Clients
                .Client(player.ConnectionId)
                .GameUpdate(playerUpdate);
        }
    }

    private async Task HandleDeadPlayers(Game game)
    {
        while (game.QueueToNotifyDeadPlayers.TryDequeue(out Player player))
        {
            await _hubContext.Clients
                .Client(player.ConnectionId)
                .GameOver();
        }
    }

    public void Dispose() => _stoppingCts.Cancel();
}
