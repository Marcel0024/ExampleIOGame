using IOGameServer.Application.Services;
using IOGameServer.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace IOGameServer.Hubs
{
    public partial class GameHub : Hub<IGameHub>
    {
        private const string _gameIdKey = "groupId";
        private const string _playerIdKey = "playerId";
        private const string _usernameKey = "username";

        private readonly GameService GameService;

        public GameHub(GameService gameService)
        {
            GameService = gameService;
        }

        public async Task JoinGame(string username)
        {
            username = await UsernameValidator.Validate(username);

            var (game, player) = GameService
                .AddPlayer(username, Context.ConnectionId);

            await SetContextItems(username, game.Id, player.Id);
        }

        public void ChangeDirection(double direction)
        {
            GameService
                .GetGame(GetGameId())?
                .GetPlayer(GetPlayerId())?
                .SetDirection(direction);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            GameService
                .RemovePlayer(GetGameId(), GetPlayerId());

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SetContextItems(string username, string gameId, string playerId)
        {
            Context.Items[_usernameKey] = username;
            Context.Items[_gameIdKey] = gameId;
            Context.Items[_playerIdKey] = playerId;

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        private string GetPlayerId()
        {
            return (string)Context.Items[_playerIdKey];
        }

        private string GetGameId()
        {
            return (string)Context.Items[_gameIdKey];

        }
    }
}
