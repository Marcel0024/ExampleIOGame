using IOGameServer.Application.Helpers;
using IOGameServer.Application.Settings;
using IOGameServer.Application.Models;
using IOGameServer.Hubs.ClientModels;
using System.Collections.Concurrent;

namespace IOGameServer.Application
{
    public sealed class Game
    {
        private readonly GameSettings _gameSettings;
        private double _timeDifference;
        private DateTime _lastDateTimeUpdated = DateTime.UtcNow;
        public required string Id { get; init; }
        public ConcurrentDictionary<string, Player> PlayersDictionary { get; init; } = new(3, 10);
        public ConcurrentDictionary<string, Bullet> BulletsDictionary { get; init; } = new(3, 200);

        public Game(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public void Update()
        {
            CalculateTimeSinceLastUpdate();

            UpdateBullets();
            UpdatePlayers();

            HandleCollisions();
        }

        private void CalculateTimeSinceLastUpdate()
        {
            var now = DateTime.UtcNow;
            _timeDifference = now.Subtract(_lastDateTimeUpdated).TotalSeconds;
            _lastDateTimeUpdated = now;
        }

        private void UpdateBullets()
        {
            foreach (var bullet in BulletsDictionary.Values)
            {
                bullet.Update(_timeDifference);

                if (bullet.ReachedBorder(_gameSettings.MapSize))
                {
                    BulletsDictionary.TryRemove(bullet.Id, out _);
                }
            }
        }

        private void UpdatePlayers()
        {
            foreach (var player in PlayersDictionary.Values)
            {
                player.Update(_timeDifference);

                if (player.CanFireBullet())
                {
                    var id = IdFactory.GenerateUniqueId();

                    BulletsDictionary.TryAdd(id, new Bullet
                    {
                        Id = id,
                        PlayerId = player.Id,
                        Direction = player.Direction,
                        Speed = _gameSettings.BulletSpeed,
                        X = player.X,
                        Y = player.Y,
                    });
                }
            }
        }

        private void HandleCollisions()
        {
            foreach (var bullet in BulletsDictionary.Values)
            {
                foreach (var player in PlayersDictionary.Values)
                {
                    if (player.Id == bullet.PlayerId)
                    {
                        continue;
                    }

                    if (player.DistanceTo(bullet) <= _gameSettings.PlayerRadius + _gameSettings.BulletRadius)
                    {
                        player.TakeBulletDamage();

                        PlayersDictionary.TryGetValue(bullet.PlayerId, out var shooter);
                        shooter?.ScoreBulletHit();

                        BulletsDictionary.TryRemove(bullet.Id, out _);
                        break;
                    }
                }
            }
        }

        public Player GetPlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            PlayersDictionary.TryGetValue(id, out var player);

            return player;
        }

        public Player AddPlayer(string username, string connectionId)
        {
            // Generate a position to start this player at.
            var x = _gameSettings.MapSize * (0.25 + Random.Shared.NextDouble() * 0.5);
            var y = _gameSettings.MapSize * (0.25 + Random.Shared.NextDouble() * 0.5);

            var newPlayer = new Player(_gameSettings)
            {
                Direction = 0,
                Id = IdFactory.GenerateUniqueId(),
                ConnectionId = connectionId,
                Username = username,
                X = (int)x,
                Y = (int)y,
            };

            PlayersDictionary[newPlayer.Id] = newPlayer;

            return newPlayer;
        }

        public UpdateModel CreateUpdateJson(Player player)
        {
            var halfAMapSize = _gameSettings.MapSize / 2;
            var players = PlayersDictionary.Values.ToArray();

            var nearbyPlayers = players
                .Where(p => p.Id != player.Id && p.DistanceTo(player) <= halfAMapSize)
                .Select(p => p.GetClientModel());

            var nearbyBullets = BulletsDictionary.Values
                .Where(b => b.DistanceTo(player) <= halfAMapSize);

            return new UpdateModel
            {
                T = _timeDifference,
                Me = player.GetClientModel(),
                P = nearbyPlayers,
                B = nearbyBullets.Select(b => b.GetClientModel()),
                L = GetLeaderBoard(players)
            };
        }

        private static IEnumerable<UpdateModel.LeaderBoard> GetLeaderBoard(IEnumerable<Player> players)
        {
            return players
                .OrderByDescending(p => p.Score)
                .Take(5)
                .Select(p => new UpdateModel.LeaderBoard
                {
                    Name = p.Username,
                    Score = (int)p.Score,
                });
        }

        public void RemovePlayer(string id)
        {
            PlayersDictionary.TryRemove(id, out _);
        }

        public void ChangePlayerDirection(string id, int direction)
        {

            PlayersDictionary.TryGetValue(id, out var player);
            player?.SetDirection(direction);
        }

        public IEnumerable<Player> HandleDeadPlayers()
        {
            var deadPlayers = PlayersDictionary.Values
                .Where(p => p.HP <= 0)
                .ToArray();

            foreach (var player in deadPlayers)
            {
                PlayersDictionary.TryRemove(player.Id, out _);
            }

            return deadPlayers;
        }
    }
}
