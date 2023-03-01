using IOGameServer.Application.Models;

namespace IOGameServer.Application.Models.Components.Spawn
{
    public abstract class SpawnComponent : Component<IGameObject>
    {
        protected SpawnComponent(IGameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            Spawn();
        }

        public override void Update(double _) { }

        public abstract void Spawn();
    }
}
