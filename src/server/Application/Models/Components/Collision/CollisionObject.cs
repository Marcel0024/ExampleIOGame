namespace IOGameServer.Application.Models.Components.Collision
{
    public sealed class CollisionObject : Component<IGameObject>
    {
        public required int Radius { get; set; }
        Damageable.Damageable Damageable { get; set; }

        public CollisionObject(IGameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            Damageable = GameObject.GetComponent<Damageable.Damageable>();
        }

        public override void Update(double distance) { }

        public bool Collided(IGameObject gameObject)
        {
            return GameObject.DistanceTo(gameObject) <= Radius + gameObject.GetComponent<CollisionObject>().Radius;
        }

        public void HandleCollision(IGameObject gameObject)
        {
            Damageable?.HandleDamage(gameObject);
        }
    }
}
