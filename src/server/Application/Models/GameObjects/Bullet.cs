using IOGameServer.Hubs.ClientModels;
using IOGameServer.Application.Models.Components.Collision;
using IOGameServer.Application.Models.Components.Damageable;
using IOGameServer.Application.Models.Components.Movement;
using IOGameServer.Application.Models.Components.Spawn;
using IOGameServer.Application.Models.Inputs;

namespace IOGameServer.Application.Models.GameObjects
{
    public sealed class Bullet : GameObject
    {
        public Bullet(Game game, Player shotByPlayer, int x, int y, double direction) : base(game)
        {
            AddComponent(new CollisionBorderRemoveOnTouch(this));
            AddComponent(new CollisionObject(this) { Radius = Game.Settings.BulletRadius });
            AddComponent(new SpawnFixed(this, x, y));
            AddComponent(new Damageable(this) { ShotByPlayer = shotByPlayer, });
            AddComponent(new MovementNormal(this)
            {
                Speed = Game.Settings.BulletSpeed,
                Direction = direction
            });
        }

        public override void HandleInput(IInput input) { } // bullets take no input

        public UpdateModel.Bullet GetClientModel() => new()
        {
            Id = Id,
            X = X,
            Y = Y
        };
    }
}
