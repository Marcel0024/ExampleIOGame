using IOGameServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace IOGameServer.Application.Services
{
    public sealed class ClientUpdaterHostedService : IHostedService, IDisposable
    {
        private readonly GameService _gameService;
        private readonly IHubContext<GameHub, IGameHub> _hubContext;
        private readonly CancellationTokenSource _stoppingCts = new();

        private PeriodicTimer Timer { get; init; }
        private Task ExecutingTask { get; set; }


        public ClientUpdaterHostedService(IHubContext<GameHub, IGameHub> hubContext, GameService gameService)
        {
            _hubContext = hubContext;
            _gameService = gameService;

            Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / 24));
        }

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

                await HandleDeadPlayers(game);
                await UpdateAllPlayersOfGame(game);
            }
        }

        private async Task UpdateAllPlayersOfGame(Game game)
        {
            foreach (var player in game.PlayersDictionary.Values)
            {
                var playerUpdate = game.CreateUpdateJson(player);

                await _hubContext.Clients
                    .Client(player.ConnectionId)
                    .GameUpdate(JsonSerializer.Serialize(playerUpdate));
            }
        }

        private async Task HandleDeadPlayers(Game game)
        {
            var deadPlayers = game.HandleDeadPlayers();

            foreach (var deadPlayer in deadPlayers)
            {
                await _hubContext.Clients
                    .Client(deadPlayer.ConnectionId)
                    .GameOver();
            }
        }

        public void Dispose() => _stoppingCts.Cancel();
    }
}
