using IOGameServer.Application.Helpers;
using IOGameServer.Application.Models;
using IOGameServer.Application.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace IOGameServer.Application.Services
{
    public sealed class GameService
    {
        private readonly GameSettings GameSettings;

        public ConcurrentDictionary<string, Game> Games { get; init; } = new ConcurrentDictionary<string, Game>(3, 10);

        public GameService(IOptions<GameSettings> gameSettings)
        {
            GameSettings = gameSettings.Value;
        }

        public (Game, Player) AddPlayer(string username, string userIdentifier)
        {
            var game = Games
                .Where(g => g.Value.PlayersDictionary.Count < GameSettings.TotalPlayersPerGame)
                .OrderByDescending(g => g.Value.PlayersDictionary.Count)
                .FirstOrDefault().Value;

            game ??= CreateGame();

            var player = game.AddPlayer(username, userIdentifier);

            return (game, player);
        }

        public void RemovePlayer(string gameId, string playerId)
        {
            var game = GetGame(gameId);

            if (game == null)
            {
                return;
            }

            game.RemovePlayer(playerId);

            if (game.PlayersDictionary.IsEmpty)
            {
                Games.TryRemove(game.Id, out _);
            }
        }

        private Game CreateGame()
        {
            Game game = new(GameSettings)
            {
                Id = IdFactory.GenerateUniqueId()
            };

            Games.TryAdd(game.Id, game);

            return game;
        }

        public Game GetGame(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return null;
            }

            Games.TryGetValue(groupId, out Game game);

            return game;
        }
    }
}
