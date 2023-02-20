using IOGameServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace IOGameServer.Application.Services
{
    public class ClientUpdaterHostedService : IHostedService, IDisposable
    {
        private Timer Timer { get; init; }

        public IHubContext<GameHub, IGameHub> HubContext { get; }
        public GameService GameService { get; }

        public ClientUpdaterHostedService(IHubContext<GameHub, IGameHub> hubContext, GameService gameService)
        {
            HubContext = hubContext;
            GameService = gameService;

            Timer = new Timer(
                    async (s) => { await UpdateAllGames(); },
                    null,
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromMilliseconds(1000 / 30));
        }

        private async Task UpdateAllGames()
        {
            foreach (var game in GameService.Games.Values.ToArray())
            {
                game.Update();

                await HandleDeadPlayers(game);
                await UpdateAllPlayersOfGame(game);
            }
        }

        private async Task UpdateAllPlayersOfGame(Game game)
        {
            foreach (var player in game.PlayersDictionary.Values.ToArray())
            {
                var playerUpdate = game.CreateUpdateJson(player);

                await HubContext.Clients
                    .Client(player.ConnectionId)
                    .GameUpdate(JsonSerializer.Serialize(playerUpdate));
            }
        }

        private async Task HandleDeadPlayers(Game game)
        {
            var deadPlayers = game.HandleDeadPlayers();

            foreach (var deadPlayer in deadPlayers)
            {
                await HubContext.Clients
                    .Client(deadPlayer.ConnectionId)
                    .GameOver();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose() => Timer?.Dispose();
    }
}
