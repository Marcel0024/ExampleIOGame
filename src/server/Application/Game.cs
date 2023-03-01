using IOGameServer.Application.Helpers;
using IOGameServer.Application.Settings;
using IOGameServer.Application.Models;
using IOGameServer.Hubs.ClientModels;
using System.Collections.Concurrent;
using IOGameServer.Application.Models.Components.Score;
using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application
{
    public sealed class Game
    {
        private double _timeDifference;
        private DateTime _lastDateTimeUpdated = DateTime.UtcNow;

        public required string Id { get; init; }
        public required GameSettings Settings { get; init; }
        public int TotalPlayers { get; private set; } = 0;

        public ConcurrentDictionary<string, IGameObject> GameObjects { private get; init; } = new(3, 2000);

        public ConcurrentQueue<IGameObject> QueueToRemoveGameObjects { get; init; } = new();
        public ConcurrentQueue<IGameObject> QueueToAddGameObjects { get; init; } = new();
        public ConcurrentQueue<Player> QueueToNotifyDeadPlayers { get; init; } = new();

        public void Update()
        {
            CalculateTimeSinceLastUpdate();

            UpdateObjects();

            HandleRemovedObjects();
            HandleAddedObjects();
        }

        private void CalculateTimeSinceLastUpdate()
        {
            var now = DateTime.UtcNow;
            _timeDifference = now.Subtract(_lastDateTimeUpdated).TotalSeconds;
            _lastDateTimeUpdated = now;
        }

        private void UpdateObjects()
        {
            var gameObjects = GameObjects.Values.ToArray();

            for (int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject1 = gameObjects[i];

                gameObject1.Update(_timeDifference);

                for (int j = i + 1; j < gameObjects.Length; j++)
                {
                    var gameObject2 = gameObjects[j];

                    if (gameObject1.HasCollidedWith(gameObject2))
                    {
                        gameObject1.HandleCollisionImpact(gameObject2);
                        gameObject2.HandleCollisionImpact(gameObject1);
                    }
                }
            }
        }

        private void HandleRemovedObjects()
        {
            while (QueueToRemoveGameObjects.TryDequeue(out var gameObjectToRemove))
            {
                if (gameObjectToRemove is Player player)
                {
                    QueueToNotifyDeadPlayers.Enqueue(player);
                }

                GameObjects.TryRemove(gameObjectToRemove.Id, out _);
            }

            QueueToRemoveGameObjects.Clear();
        }

        private void HandleAddedObjects()
        {
            while (QueueToAddGameObjects.TryDequeue(out var gameObjectToAdd))
            {
                GameObjects.TryAdd(gameObjectToAdd.Id, gameObjectToAdd);
            }

            QueueToAddGameObjects.Clear();
        }

        public Player GetPlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }

            GameObjects.TryGetValue(id, out var player);

            return (Player)player;
        }

        public Player AddPlayer(string username, string connectionId)
        {
            var newPlayer = new Player(this)
            {
                Id = IdFactory.GenerateUniqueId(),
                ConnectionId = connectionId,
                Username = username,
            };

            newPlayer.Start();

            GameObjects[newPlayer.Id] = newPlayer;

            TotalPlayers++;

            return newPlayer;
        }

        public IEnumerable<Player> GetPlayers()
        {
            return GameObjects.Values.Where(go => go is Player).Cast<Player>();
        }

        public UpdateModel CreateUpdateJson(Player player)
        {
            var halfAMapSize = Settings.MapSize / 2;
            var allVisibleObjects = GameObjects.Values.Where(p => p.DistanceTo(player) <= halfAMapSize).ToArray();

            var nearbyPlayers = allVisibleObjects.Where(x => x is Player).Select(x => ((Player)x));
            var nearbyBullets = allVisibleObjects.Where(x => x is Bullet).Select(x => ((Bullet)x).GetClientModel());

            return new UpdateModel
            {
                T = _timeDifference,
                Me = player.GetClientModel(),
                P = nearbyPlayers.Where(p => p.Id != player.Id).Select(x => x.GetClientModel()),
                B = nearbyBullets,
                L = GetLeaderBoard(nearbyPlayers)
            };
        }

        private static IEnumerable<UpdateModel.LeaderBoard> GetLeaderBoard(IEnumerable<Player> players)
        {
            return players
                .OrderByDescending(p => p.GetComponent<ScoreIncrementPerSecond>().Score)
                .Take(5)
                .Select(p => new UpdateModel.LeaderBoard
                {
                    Name = p.Username,
                    Score = (int)p.GetComponent<ScoreIncrementPerSecond>().Score,
                });
        }

        public void RemovePlayer(string id)
        {
            GameObjects.TryRemove(id, out _);
            TotalPlayers--;
        }
    }
}
