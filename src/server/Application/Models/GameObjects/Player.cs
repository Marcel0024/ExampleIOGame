using IOGameServer.Hubs.ClientModels;
using IOGameServer.Application.Models.Components.Collision;
using IOGameServer.Application.Models.Components.Health;
using IOGameServer.Application.Models.Components.Movement;
using IOGameServer.Application.Models.Components.Score;
using IOGameServer.Application.Models.Components.Shoot;
using IOGameServer.Application.Models.Components.Spawn;
using IOGameServer.Application.Models.Inputs;
using IOGameServer.Application.Models.Inputs.Player;

namespace IOGameServer.Application.Models.GameObjects
{
    public sealed class Player : GameObject
    {
        public required string Username { get; init; }
        public required string ConnectionId { get; init; }

        public Player(Game game) : base(game)
        {
            AddComponent(new SpawnRandom(this));
            AddComponent(new MovementNormal(this)
            {
                Speed = Game.Settings.PlayerSpeed,
                Direction = 1
            });
            AddComponent(new ScoreIncrementPerSecond(this) { ScorePerSecond = Game.Settings.ScorePerSecond });
            AddComponent(new CollisionBorderStayInside(this));
            AddComponent(new CollisionObject(this) { Radius = Game.Settings.PlayerRadius });
            AddComponent(new ShootPerSecond(this) { FireCoolDown = Game.Settings.PlayerFireCooldown });
            AddComponent(new Health(this) { HP = Game.Settings.PlayerMaxHP });
        }

        public override void HandleInput(IInput input)
        {
            if (input is DirectionInput directionInput)
            {
                GetComponent<MovementNormal>().SetDirection(directionInput.Direction);
            }
        }

        public UpdateModel.Player GetClientModel() => new()
        {
            Id = Id,
            X = X,
            Y = Y,
            Dir = GetComponent<MovementNormal>().Direction,
            Hp = GetComponent<Health>().HP
        };
    }
}
