using IOGameServer.Application.Services;
using IOGameServer.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace IOGameServer.Hubs
{
    public sealed class GameHub : Hub<IGameHub>
    {
        private const string GameIdKey = "groupId";
        private const string PlayerIdKey = "playerId";

        private readonly GameService _gameService;

        public GameHub(GameService gameService)
        {
            _gameService = gameService;
        }

        public async Task JoinGame(string username)
        {
            username = await UsernameValidator.Validate(username);

            var (game, player) = _gameService
                .AddPlayer(username, Context.ConnectionId);

            Context.Items[GameIdKey] = game.Id;
            Context.Items[PlayerIdKey] = player.Id;
        }

        public void ChangeDirection(double direction)
        {
            _gameService
                .GetGame(GetGameId())?
                .GetPlayer(GetPlayerId())?
                .SetDirection(direction);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _gameService
                .RemovePlayer(GetGameId(), GetPlayerId());

            await base.OnDisconnectedAsync(exception);
        }
        
        private string GetPlayerId()
        {
            return (string)Context.Items[PlayerIdKey];
        }

        private string GetGameId()
        {
            return (string)Context.Items[GameIdKey];

        }
    }
}
