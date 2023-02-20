using IOGameServer.Application.Helpers;
using IOGameServer.Application.Settings;
using IOGameServer.Application.Models;
using IOGameServer.Hubs.ClientModels;
using System.Collections.Concurrent;

namespace IOGameServer.Application
{
    public class Game
    {
        private double _timeDifference;
        private DateTime _lastDateTimeUpdated = DateTime.UtcNow;
        private readonly GameSettings GameSettings;

        public ConcurrentDictionary<string, Player> PlayersDictionary { get; init; } = new ConcurrentDictionary<string, Player>(3, 10);
        public ConcurrentDictionary<string, Bullet> BulletsDictionary { get; init; } = new ConcurrentDictionary<string, Bullet>(3, 10);
        public required string Id { get; init; }

        public Game(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
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

                if (bullet.ReachedBorder(GameSettings.MapSize))
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
                        Speed = GameSettings.BulletSpeed,
                        X = player.X,
                        Y = player.Y,
                    });
                }
            }
        }

        private void HandleCollisions()
        {
            foreach (var bullet in BulletsDictionary.Values.ToArray())
            {
                foreach (var player in PlayersDictionary.Values)
                {
                    if (player.Id == bullet.PlayerId)
                    {
                        continue;
                    }

                    if (player.DistanceTo(bullet) <= GameSettings.PlayerRadius + GameSettings.BulletRadius)
                    {
                        player.TakeBulletDamage();

                        PlayersDictionary.TryGetValue(bullet.PlayerId, out var shooter);
                        shooter?.ScoreBulletHit();

                        BulletsDictionary.Remove(bullet.Id, out _);
                        break;
                    }
                }
            }
        }

        public Player GetPlayer(string id)
        {
            return PlayersDictionary[id];
        }

        public Player AddPlayer(string username, string connectionId)
        {
            // Generate a position to start this player at.
            var x = GameSettings.MapSize * (double)(0.25 + Random.Shared.NextDouble() * 0.5);
            var y = GameSettings.MapSize * (double)(0.25 + Random.Shared.NextDouble() * 0.5);

            var newPlayer = new Player(GameSettings)
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
            var players = PlayersDictionary.Values.ToArray();

            var nearbyPlayers = players
                .Where(p => p.Id != player.Id && p.DistanceTo(player) <= GameSettings.MapSize / 2);

            var nearbyBullets = BulletsDictionary.Values
                .Where(b => b.DistanceTo(player) <= GameSettings.MapSize / 2);

            return new UpdateModel
            {
                T = _timeDifference,
                Me = player.ToJson(),
                P = nearbyPlayers.Select(p => p.ToJson()),
                B = nearbyBullets.Select(b => b.ToJson()),
                L = GetLeaderBoard(players)
            };
        }

        public IEnumerable<UpdateModel.LeaderBoard> GetLeaderBoard(Player[] players)
        {
            return players
                .OrderByDescending(p => p.Score)
                .Take(5)
                .Select(p => new UpdateModel.LeaderBoard
                {
                    Username = p.Username,
                    Score = (int)p.Score,
                });
        }

        public void RemovePlayer(string id)
        {
            PlayersDictionary.TryRemove(id, out _);
        }

        public void ChangePlayerDirection(string id, int direction)
        {

            PlayersDictionary.TryGetValue(id, out Player player);
            player?.SetDirection(direction);
        }

        public Player[] HandleDeadPlayers()
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
