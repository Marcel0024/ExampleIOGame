using IOGameServer.Application.Helpers;
using IOGameServer.Application.Models.Components.Movement;
using IOGameServer.Application.Models.GameObjects;

namespace IOGameServer.Application.Models.Components.Shoot
{
    public sealed class ShootPerSecond : Component
    {
        double CurrentFireCoolDown { get; set; }
        public required double FireCoolDown { get; set; }

        public ShootPerSecond(IGameObject gameObject) : base(gameObject) { }

        public override void Start() { }

        public override void Update(double distance)
        {
            CoolDown(distance);

            if (CanFire())
            {
                var direction = GameObject.GetComponent<MovementNormal>().Direction;

                var newBullet = new Bullet(GameObject.Game, GameObject, GameObject.X, GameObject.Y, direction)
                {
                    Id = IdFactory.GenerateUniqueId(),
                };

                newBullet.Start();

                GameObject.AddItemToGame(newBullet);
            }
        }

        private bool CanFire()
        {
            if (CurrentFireCoolDown <= 0)
            {
                CurrentFireCoolDown = FireCoolDown;
                return true;
            }

            return false;
        }

        private void CoolDown(double distance)
        {
            CurrentFireCoolDown -= distance;
        }
    }
}
