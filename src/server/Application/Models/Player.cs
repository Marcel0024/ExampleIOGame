using IOGameServer.Application.Settings;
using IOGameServer.Hubs.ClientModels;

namespace IOGameServer.Application.Models
{
    public sealed class Player : GameObject
    {
        private readonly GameSettings GameSettings;

        public required string Username { get; set; }
        public required string ConnectionId { get; init; }
        public double HP { get; set; }
        public double Score { get; private set; } = 0;
        public double FireCoolDown { get; set; }

        public Player(GameSettings gameSettings)
        {
            GameSettings = gameSettings;

            Speed = gameSettings.PlayerSpeed;
            HP = gameSettings.PlayerMaxHP;
            FireCoolDown = gameSettings.PlayerFireCooldown;
        }

        public override void Update(double distance)
        {
            base.Update(distance);

            // Update score
            Score += (double)distance * GameSettings.ScorePerSecond;

            // Make sure the player stays in bounds
            X = Math.Max(5, Math.Min(GameSettings.MapSize, X));
            Y = Math.Max(5, Math.Min(GameSettings.MapSize, Y));

            FireCoolDown -= distance;
        }

        public void TakeBulletDamage()
        {
            HP -= GameSettings.BulletDamage;
        }

        public void ScoreBulletHit()
        {
            Score += GameSettings.ScoreBulletHit;
        }

        public bool CanFireBullet()
        {
            if (FireCoolDown <= 0)
            {
                FireCoolDown += GameSettings.PlayerFireCooldown;
                return true;
            }

            return false;
        }

        public new UpdateModel.Player ToJson() => new()
        {
            Id = Id,
            X = X,
            Y = Y,
            Direction = Direction,
            Hp = HP
        };
    }
}
