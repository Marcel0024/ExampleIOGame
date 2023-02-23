using IOGameServer.Application.Settings;
using IOGameServer.Hubs.ClientModels;

namespace IOGameServer.Application.Models
{
    public sealed class Player : GameObject
    {
        private readonly GameSettings _gameSettings;

        public required string Username { get; init; }
        public required string ConnectionId { get; init; }
        public double HP { get; private set; }
        public double Score { get; private set; }

        private double FireCoolDown { get; set; }      


        public Player(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;

            Speed = gameSettings.PlayerSpeed;
            HP = gameSettings.PlayerMaxHP;
            FireCoolDown = gameSettings.PlayerFireCooldown;
        }

        public override void Update(double distance)
        {
            base.Update(distance);

            // Update score
            Score += distance * _gameSettings.ScorePerSecond;

            // Make sure the player stays in bounds
            X = Math.Max(5, Math.Min(_gameSettings.MapSize, X));
            Y = Math.Max(5, Math.Min(_gameSettings.MapSize, Y));

            FireCoolDown -= distance;
        }

        public void TakeBulletDamage()
        {
            HP -= _gameSettings.BulletDamage;
        }

        public void ScoreBulletHit()
        {
            Score += _gameSettings.ScoreBulletHit;
        }

        public bool CanFireBullet()
        {
            if (FireCoolDown <= 0)
            {
                FireCoolDown += _gameSettings.PlayerFireCooldown;
                return true;
            }

            return false;
        }

        public UpdateModel.Player GetClientModel() => new()
        {
            Id = Id,
            X = X,
            Y = Y,
            Dir = Direction,
            Hp = HP
        };
    }
}
