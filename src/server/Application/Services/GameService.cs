using IOGameServer.Application.Helpers;
using IOGameServer.Application.Models;
using IOGameServer.Application.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace IOGameServer.Application.Services
{
    public sealed class GameService
    {
        private readonly GameSettings _gameSettings;

        public ConcurrentDictionary<string, Game> Games { get; init; } = new (3, 10);

        public GameService(IOptions<GameSettings> gameSettings)
        {
            _gameSettings = gameSettings.Value;
        }

        public (Game, Player) AddPlayer(string username, string connectionId)
        {
            var game = Games
                .Where(g => g.Value.PlayersDictionary.Count < _gameSettings.TotalPlayersPerGame)
                .OrderByDescending(g => g.Value.PlayersDictionary.Count)
                .FirstOrDefault().Value;

            game ??= CreateGame();

            var player = game.AddPlayer(username, connectionId);

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
            var game = new Game(_gameSettings)
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

            Games.TryGetValue(groupId, out var game);

            return game;
        }
    }
}
