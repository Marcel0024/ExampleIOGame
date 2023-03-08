using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application.Models.Components.Damageable
{
    public sealed class Damageable : Component
    {
        public Damageable(Bullet gameObject) : base(gameObject) { }

        public required IGameObject ShotByPlayer { get; set; }

        public override void Start() { }

        public override void Update(double _) { }

        public void HandleDamage(IGameObject gameObject)
        {
            if (GameObject?.GetComponent<Damageable>()?.ShotByPlayer?.Id == gameObject.Id)
            {
                return;
            }

            gameObject
                .GetComponent<Health.Health>()?
                .TakeDamage(GameObject.Game.Settings.BulletDamage);

            GameObject
                .GetComponent<Damageable>()?
                .ShotByPlayer?
                .GetComponent<Score.ScoreIncrementPerSecond>()?
                .IncreaseScore(GameObject.Game.Settings.ScoreBulletHit);

            GameObject.RemoveMe();
        }
    }
}
