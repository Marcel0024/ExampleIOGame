using IOGameServer.Application.Helpers;
using IOGameServer.Application.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application.Services;

public sealed class GameService(IOptions<GameSettings> gameSettings)
{
    private readonly GameSettings _gameSettings = gameSettings.Value;

    public ConcurrentDictionary<string, Game> Games { get; init; } = new (3, 10);

    public (Game, Player) AddPlayer(string username, string connectionId)
    {
        var game = Games.Values
            .Where(game => game.TotalPlayers < _gameSettings.TotalPlayersPerGame)
            .FirstOrDefault();

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

        if (game.TotalPlayers <= 0)
        {
            Games.TryRemove(game.Id, out _);
        }
    }

    private Game CreateGame()
    {
        var game = new Game()
        {
            Id = IdFactory.GenerateUniqueId(),
            Settings = _gameSettings
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
